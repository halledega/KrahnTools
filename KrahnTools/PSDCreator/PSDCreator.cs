using System;
using System.Collections.Generic;
using System.Linq;

using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;
using Autodesk.Revit.ApplicationServices;
using Autodesk.Revit.Attributes;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace KrahnTools.PSDCreator
{

    [TransactionAttribute(TransactionMode.Manual)]
    [RegenerationAttribute(RegenerationOption.Manual)]
    public class CreatePSD : IExternalCommand
    {
        public bool StructSelected;
        string titleBlock = "Titleblock - 11 x 17_Krahn Engineering - PSD.rfa";
        string geometryViewTemplate = "PSD Geometry";
        string rebarViewTemplate = "PSD Reinforcement";
        string panelNumber;

        static AddInId appId = new AddInId(new Guid("E5E1F080-0B67-4702-BE0D-1A847BE73A98"));

        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            //Get application and document objects
            UIApplication uiapp = commandData.Application;
            UIDocument uidoc = uiapp.ActiveUIDocument;
            Application app = uiapp.Application;
            Document doc = uidoc.Document;

            using (PSDOptions_form inputForm = new PSDOptions_form())
            {
                inputForm.ShowDialog();

                if (inputForm.DialogResult == System.Windows.Forms.DialogResult.Cancel)
                {
                    return Result.Cancelled;
                }

                if (inputForm.DialogResult == System.Windows.Forms.DialogResult.OK)
                {
                    if (inputForm.isStructural == true)
                    {
                        StructSelected = true;

                    }
                    else
                    {
                        StructSelected = false;
                    }
                }
            }


            //check to see if the PSD titleblock is in the current document.
            FilteredElementCollector titleblockCollector = new FilteredElementCollector(doc).OfClass(typeof(Family));
            Family family = titleblockCollector.FirstOrDefault<Element>(e => e.Name.Equals(titleBlock)) as Family;
            string FamilyPath = @"C:\ProgramData\Autodesk\Revit\Addins\2016\KrahnTools\PSDCreator\" + titleBlock;

            

            //if the titleblock doesn't exist in the current document, it is imported.
            if (null == family)
            {
                // It is not present, so check for the file to load it from:
                if (!File.Exists(FamilyPath))
                {
                    TaskDialog.Show("Error", "The PSD Template is missing from the addins folder.");
                    return Result.Failed;
                }

                using (Transaction t = new Transaction(doc))
                {
                    t.Start("Load Titleblock");
                    doc.LoadFamily(FamilyPath, out family);
                    t.Commit();
                }
            }

            //makes sure the correct view templates have been created in the project.
            FilteredElementCollector viewTemplateCollector = new FilteredElementCollector(doc).OfClass(typeof(View));
            View embedtitleblock = viewTemplateCollector.FirstOrDefault<Element>(e => e.Name.Equals(geometryViewTemplate)) as View;
            View rebartitleblock = viewTemplateCollector.FirstOrDefault<Element>(e => e.Name.Equals(rebarViewTemplate)) as View;

            //if they don't exist in the project, warning is shown and application is terminated
            if ((embedtitleblock == null || rebartitleblock == null))
            {
                TaskDialog.Show("Error", "You are missing the required PSD view templates, please add them to continue");
                return Result.Failed;
            }

            while (true)
            {

                Reference myRef = null;

                try
                {
                    //Prompts the user to select a wall (and only a wall) using the MySelectionFilter Class created below
                    myRef = uidoc.Selection.PickObject(ObjectType.Element, new MySelectionFilter("Walls"), "Select a wall");
                }
                catch (Autodesk.Revit.Exceptions.OperationCanceledException)
                {
                    //this exception is called when the user presses the escape key. It is handled, ignored and the addin is terminated.
                    return Result.Succeeded;
                }
                catch (Autodesk.Revit.Exceptions.ArgumentOutOfRangeException)
                {

                }

                if (myRef == null)
                    return Result.Succeeded;

                //Creates an element e from the selected object reference -- this will be the wall element
                Element e = doc.GetElement(myRef);

                View sectionView = CreateViewAndSectionMark(uidoc, e);

                //get the mark parameter and set it to the panel number variable
                if(e.get_Parameter(BuiltInParameter.ALL_MODEL_MARK).AsString() == null)
                {
                    TaskDialog.Show("No Panel Number", "Selected Panel does not have a Type Mark (Panel Number).\nPlease ensure all panels are numbered prior to creating PSD views.");
                    return Result.Succeeded;
                }
                else
                {
                    panelNumber = e.get_Parameter(BuiltInParameter.ALL_MODEL_MARK).AsString();
                }

                //duplicate the active view using the the DuplicateViewEmbeds Function created below
                View Embedview = SetupViewEmbeds(doc, panelNumber, sectionView);

                //Creates a selection filter to dump objects in for later selection
                ICollection<ElementId> selSet = new List<ElementId>();

                //Gets the bounding box of the selected wall element picked above
                BoundingBoxXYZ bb = e.get_BoundingBox(Embedview);

                //adds a buffer to the bounding box to ensure all elements are contained within the box
                XYZ buffer = new XYZ(0.1, 0.1, 0.1);

                //creates an ouline based on the boundingbox corners of the panel and adds the buffer
                Outline outline = new Outline(bb.Min - buffer, bb.Max + buffer);

                //filters the selection by the bounding box of the selected object
                //the "true" statement inverts the selection and selects all other objects
                BoundingBoxIsInsideFilter bbfilter = new BoundingBoxIsInsideFilter(outline, true);

                //creates a new filtered element collector that filters by the active view settings
                FilteredElementCollector collector = new FilteredElementCollector(doc, Embedview.Id);

                //collects all objects that pass through the requirements of the bbfilter
                collector.WherePasses(bbfilter);

                //adds each element that passed the bbfilter to the current selection collector
                //also checks to see if the element can be hidden. if it can't, it appents it to the failedtohide string
                foreach (Element el in collector)
                {
                    if (el.CanBeHidden(Embedview) == true)
                        selSet.Add(el.Id);
                }

                ICollection<BuiltInCategory> bcat = new List<BuiltInCategory>();

                //add all levels and grids to filter -- these are filtered out by the viewtemplate, but are nice to have
                bcat.Add(BuiltInCategory.OST_Levels);
                bcat.Add(BuiltInCategory.OST_Grids);
                //this removes the dimesnsions from the current selection, currently turned off because 
                //dimensions changes the size of the viewport which makes it difficult to place on sheet.
                //bcat.Add(BuiltInCategory.OST_Dimensions);

                //create new multi category filter
                ElementMulticategoryFilter multiCatFilter = new ElementMulticategoryFilter(bcat);

                //create new filtered element collector, add the passing levels and grids, then remove them from the selection
                foreach (Element el in new FilteredElementCollector(doc)
                         .WherePasses(multiCatFilter))
                {
                    selSet.Remove(el.Id);
                }

                //makes the selection of the current selection collector, doesn't need to be done as we can hide directly from the creates list
                //uidoc.Selection.SetElementIds(selSet);

                using (Transaction t = new Transaction(doc, "Hide Elements"))
                {
                    t.Start();
                    Embedview.HideElements(selSet);

                    //DOESN'T WORK BECAUSE THESE SET NUMBERS ARE BASED ON WHERE THE ELEVATION IS PLACED 
                    //IN PLAN VIEW....CAN'T RELATE THIS LOCATION TO THE PANEL LOCATION.
                    //MAYBE PLACE NEW ELEVATION VIEW AT CENTER OF PANEL?
                    //view.get_Parameter(BuiltInParameter.VIEWER_BOUND_OFFSET_LEFT).Set(-15); //DISTANCE FROM EL TAG IN PLAN
                    //view.get_Parameter(BuiltInParameter.VIEWER_BOUND_OFFSET_RIGHT).Set(15);
                    t.Commit();
                }

                //try
                //{
                TagElement(uidoc, myRef, Embedview);
                //creates a new PSD sheet
                ViewSheet PSDsheet = CreatePSDSheet(doc, panelNumber);
                //place view on PSDsheet
                PlaceOnSheet(doc, PSDsheet, Embedview);
                //creates rebar view
                View rebarView = DuplicateViewRebar(doc, panelNumber, Embedview);
                //place rebar view on PSDsheet
                PlaceOnSheet(doc, PSDsheet, rebarView);
                //}
                //catch
                //{
                //    TaskDialog.Show("Error", "Revit Encountered an Error. Sheet probably already exists.");
                //    return Result.Failed;
                //}
            }


        }

        private View SetupViewEmbeds(Document doc, string pnum, View view)
        {

            using (Transaction t2 = new Transaction(doc, "Change Sheet Properties"))
            {
                t2.Start();

                view.get_Parameter(BuiltInParameter.VIEW_SCALE_PULLDOWN_IMPERIAL).Set(64);
                view.get_Parameter(BuiltInParameter.VIEW_NAME).Set("PANEL_" + pnum + "_EMBEDS");

                View viewTemplate = (from v in new FilteredElementCollector(doc)
                                     .OfClass(typeof(View))
                                     .Cast<View>()
                                     where v.IsTemplate == true && v.Name == geometryViewTemplate
                                     select v)
                                     .First();
                view.ViewTemplateId = viewTemplate.Id;
                t2.Commit();
            }
            //returns the embed view
            return view;

        }

        private View DuplicateViewRebar(Document doc, string pnum, View view)
        {


            using (Transaction t = new Transaction(doc, "Duplicate Embed View"))
            {
                t.Start();
                //the crop box isactive has to be changed to false so that when hidding elements
                //it hides all the elements (some of which may be outside the crop view
                view = doc.GetElement(view.Duplicate(ViewDuplicateOption.WithDetailing)) as View;
                t.Commit();

                //switches to the created view -- not used
                //uidoc.ActiveView = view;
            }

            using (Transaction t2 = new Transaction(doc, "Change Sheet Properties"))
            {
                t2.Start();

                view.get_Parameter(BuiltInParameter.VIEW_SCALE_PULLDOWN_IMPERIAL).Set(64);
                view.get_Parameter(BuiltInParameter.VIEW_NAME).Set("PANEL_" + pnum + "_REBAR");
                //view.get_Parameter( BuiltInParameter.VIEW_TEMPLATE).Set("PSD_EMBEDS");
                View viewTemplate = (from v in new FilteredElementCollector(doc)
                                     .OfClass(typeof(View))
                                     .Cast<View>()
                                     where v.IsTemplate == true && v.Name == rebarViewTemplate
                                     select v)
                                     .First();
                view.ViewTemplateId = viewTemplate.Id;

                t2.Commit();
            }
            return view;

        }

        private ViewSheet CreatePSDSheet(Document doc, string panelNumber)
        {

            IEnumerable<FamilySymbol> familyList = from elem in new FilteredElementCollector(doc)
                                               .OfClass(typeof(FamilySymbol))
                                               .OfCategory(BuiltInCategory.OST_TitleBlocks)
                                                   let type = elem as FamilySymbol
                                                   where type.Name.Contains("11 x 17")
                                                   select type;

            ViewSheet viewSheet = null;
            //Create sheet view
            using (Transaction t = new Transaction(doc, "Place View on Sheet"))
            {
                t.Start();
                viewSheet = ViewSheet.Create(doc, familyList.First().Id);
                if (null == viewSheet)
                {
                    throw new Exception("Failed to create new ViewSheet.");
                }

                //set the sheet information
                viewSheet.Name = "PANEL";
                viewSheet.SheetNumber = panelNumber;
                //cant change disciplin from api....   

                t.Commit();
            }
            return viewSheet;

        }

        private void PlaceOnSheet(Document doc, ViewSheet sheet, Element viewelement)
        {

            XYZ insertionPoint = null;

            if (viewelement.Name.Contains("EMBED"))
            {
                //location (in feet) from the 0,0,0 coordinate on the page to place center of view
                insertionPoint = new XYZ(0.183, 0.375, 0);
            }
            else if (viewelement.Name.Contains("REBAR"))
            {
                //location (in feet) from the 0,0,0 coordinate on the page to place center of view
                insertionPoint = new XYZ(0.808, 0.375, 0);
            }

            using (Transaction t = new Transaction(doc, "Place View on Sheet"))
            {
                t.Start();
                Viewport.Create(doc, sheet.Id, viewelement.Id, insertionPoint);
                t.Commit();
            }
        }

        private View CreateViewAndSectionMark(UIDocument uidoc, Element e)
        {

            Document doc = uidoc.Document;

            Wall wall = e as Wall;



            // Ensure wall is straight

            LocationCurve lc = wall.Location as LocationCurve;

            Line line = lc.Curve as Line;



            if (null == line)
            {
                TaskDialog.Show("Error", "Unable to retrieve wall location line.");

                //return;
            }

            // Determine view family type to use

            ViewFamilyType vft
              = new FilteredElementCollector(doc)
                .OfClass(typeof(ViewFamilyType))
                .Cast<ViewFamilyType>()
                .FirstOrDefault<ViewFamilyType>(x =>
                 ViewFamily.Section == x.ViewFamily);

            XYZ p = line.GetEndPoint(0);
            XYZ q = line.GetEndPoint(1);
            XYZ v = q - p;

            //Gets the bounding box of the wall
            BoundingBoxXYZ bb = wall.get_BoundingBox(null);
            double minZ = bb.Min.Z;
            double maxZ = bb.Max.Z;

            //gets the width and height of the wall
            double w = v.GetLength();
            double h = maxZ - minZ;

            //this is the bottom left and upper right coordinate of the viewport window. the Z value also sets the depth of the view
            XYZ max = new XYZ(0.5 * w, maxZ, 1);
            XYZ min = new XYZ(0.5 * -w, minZ, -1);

            Transform localcoordinates = HelperClass.getLocalCoordinates(e, StructSelected, uidoc);

            //Create a new bounding box. this box will define the limits of the section mark 
            BoundingBoxXYZ sectionBox = new BoundingBoxXYZ();
            sectionBox.Transform = localcoordinates;
            sectionBox.Min = min;
            sectionBox.Max = max;

            // Create wall section view
            View view = null;

            using (Transaction tx = new Transaction(doc))
            {
                tx.Start("Create Wall Section View");

                view = ViewSection.CreateSection(doc, vft.Id, sectionBox) as View;

                //cropbox is turned off so all elements get hidden when passing the active view filter
                view.CropBoxActive = false;

                tx.Commit();
            }

            return view;
        }

        public void TagElement(UIDocument uidoc, Reference myRef, View view)
        {
            Document doc = uidoc.Document;
            if (myRef == null)
                return;

            Element e = doc.GetElement(myRef);

            Wall wall = e as Wall;

            //Creates a selection filter to dump objects in for later selection
            ICollection<ElementId> selec = new List<ElementId>();

            //Gets the bounding box of the selected wall element picked above
            BoundingBoxXYZ bb = e.get_BoundingBox(view);

            //adds a buffer to the bounding box to ensure all elements are contained within the box
            XYZ buffer = new XYZ(0.1, 0.1, 0.1);

            //creates an ouline based on the boundingbox corners of the panel and adds the buffer
            Outline outline = new Outline(bb.Min - buffer, bb.Max + buffer);

            //filters the selection by the bounding box of the selected object
            //the "true" statement inverts the selection and selects all other objects
            BoundingBoxIsInsideFilter bbfilter = new BoundingBoxIsInsideFilter(outline, false);

            //creates a new filtered element collector that filters by the active view settings
            FilteredElementCollector collector = new FilteredElementCollector(doc, view.Id);

            //collects all objects that pass through the requirements of the bbfilter
            collector.WherePasses(bbfilter);

            //adds each element that passed the bbfilter to the current selection collector
            //also checks to see if the element can be hidden

            ICollection<Element> Embeds = new List<Element>();

            foreach (Element el in collector)
            {
                if (el.Name.Contains("EM"))
                {
                    selec.Add(el.Id);
                    Embeds.Add(el);
                }
            }

            // define tag mode and tag orientation for new tag
            TagMode tagMode = TagMode.TM_ADDBY_CATEGORY;
            TagOrientation tagorn = TagOrientation.Horizontal;

            //Tag the wall and place it in the middle of the wall
            LocationCurve wallLoc = wall.Location as LocationCurve;

            XYZ wallStart = wallLoc.Curve.GetEndPoint(0);
            XYZ wallEnd = wallLoc.Curve.GetEndPoint(1);
            XYZ wallMid = wallLoc.Curve.Evaluate(0.5, true);

            using (Transaction t = new Transaction(doc, "Create Structural connection tag."))
            {
                t.Start();

                IndependentTag newTag = doc.Create.NewTag(view, wall, false, tagMode, tagorn, wallMid);
                if (newTag == null)
                    throw new Exception("Create IndependentTag Failed.");

                foreach (Element el in Embeds)
                {
                    LocationPoint embedpoint = el.Location as LocationPoint;
                    XYZ embedXyz = embedpoint.Point;
                    IndependentTag embedTag = doc.Create.NewTag(view, el, false, tagMode, tagorn, embedXyz);

                    if (embedTag == null)
                    {
                        throw new Exception("Create IndependentTag Failed.");
                    }
                }

                //move panel tag 15' up from bottom of panel
                newTag.Location.Move(new XYZ(0, 0, 15));

                // newTag.TagText is read-only, so we change the Type Mark type parameter to 
                // set the tag text.  The label parameter for the tag family determines
                // what type parameter is used for the tag text.

                //WallType type = wall.WallType;

                //Parameter foundParameter = type.LookupParameter("Type Mark");
                //bool result = foundParameter.Set("Hello");

                // set leader mode free
                // otherwise leader end point move with elbow point

                //newTag.LeaderEndCondition = LeaderEndCondition.Free;
                //XYZ elbowPnt = wallMid + new XYZ(5.0, 5.0, 0.0);
                //newTag.LeaderElbow = elbowPnt;
                //headerPnt = wallMid + new XYZ(10.0, 10.0, 0.0);
                //newTag.TagHeadPosition = headerPnt;

                t.Commit();
            }
        }

        private class MySelectionFilter : ISelectionFilter
        {
            static string CategoryName = "";

            public MySelectionFilter(string name)
            {
                CategoryName = name;
            }
            public bool AllowElement(Element e)
            {
                if (e.Category.Name == CategoryName)
                    return true;

                return false;
            }
            //not used, but needed for the ISelectionFilter
            public bool AllowReference(Reference r, XYZ point)
            {
                return true;
            }
        }
    }//end class
}//end namespace
