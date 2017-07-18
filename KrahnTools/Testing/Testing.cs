
#region Includes
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

using KrahnTools;
using BuildingCoder;
#endregion

namespace KrahnTools
{
    namespace Testing
    {
        [TransactionAttribute(TransactionMode.Manual)]
        [RegenerationAttribute(RegenerationOption.Manual)]
        public class Testing : IExternalCommand
        {
            static AddInId appId = new AddInId(new Guid("1FA40AA6-B5B5-4A25-AA20-D6D9109CA4FD"));

            public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
            {
                //Get application and document objects
                UIApplication uiapp = commandData.Application;
                UIDocument uidoc = uiapp.ActiveUIDocument;
                Application app = uiapp.Application;
                Document doc = uidoc.Document;


                Reference myRef = null;

                try
                {
                     myRef = uidoc.Selection.PickObject(ObjectType.Element, new MySelectionFilter("Walls"), "Select a wall");
                }
                catch (Autodesk.Revit.Exceptions.OperationCanceledException)
                {
                    return Result.Succeeded;
                }
                catch (Autodesk.Revit.Exceptions.ArgumentOutOfRangeException){}

                if (myRef == null)
                    return Result.Succeeded;


                Wall wall = doc.GetElement(myRef) as Wall;
                Element e = wall as Element;
                BoundingBoxXYZ bb = e.get_BoundingBox(doc.ActiveView);

                Level level = doc.GetElement(wall.LevelId) as Level;
                double elevation = level.Elevation;

                Curve c = (wall.Location as LocationCurve).Curve;
                XYZ wallStartPoint = c.GetEndPoint(0);
                XYZ wallEndPoint = c.GetEndPoint(1);
                XYZ wallDirectionHorizontal = wallEndPoint - wallStartPoint;
                double wallLength = wallDirectionHorizontal.GetLength();
                wallDirectionHorizontal = wallDirectionHorizontal.Normalize();

                double wallheight = e.get_Parameter(BuiltInParameter.WALL_USER_HEIGHT_PARAM).AsDouble();
                XYZ wallDirectionVertical = new XYZ(0d, 0d, 1d);

                XYZ topcorner = bb.Max;
                XYZ bottomcorner = bb.Min;

                UV offsetOutHorizontal = new UV();
                UV offsetOutVertical1 = new UV();
                UV offsetOutVertical2 = new UV();
                UV HorizontalDimOffset = new UV();

                // This is how far out from the start of the wall, the ray will start
                double offset = 1;

                //this locates the ray just outside the panel from the startpoint and shoots horizontally
                offsetOutHorizontal = offset * new UV(wallDirectionHorizontal.X, wallDirectionHorizontal.Y);
                XYZ rayStartHorizontal = new XYZ(wallStartPoint.X - offsetOutHorizontal.U, wallStartPoint.Y - offsetOutHorizontal.V, elevation + offset);

                //this locates the ray just outside the panel from the startpoint and shoots vertically up
                offsetOutVertical1 = offset * new UV(wallDirectionHorizontal.X, wallDirectionHorizontal.Y);
                XYZ rayStartVertical1 = new XYZ(wallStartPoint.X + offsetOutVertical1.U, wallStartPoint.Y + offsetOutVertical1.V, elevation - offset);

                //this locates the ray just outside the panel from the endpoint and shoots vertically up
                offsetOutVertical2 = offset * new UV(wallDirectionHorizontal.X, wallDirectionHorizontal.Y);
                XYZ rayStartVertical2 = new XYZ(wallEndPoint.X - offsetOutVertical2.U, wallEndPoint.Y - offsetOutVertical2.V, elevation - offset);

                HorizontalDimOffset = (wallLength / 2) * new UV(wallDirectionHorizontal.X,wallDirectionHorizontal.Y);


                CustomWallBoundingBox box = new CustomWallBoundingBox(wall);
                

                Outline outline = box.outline;
                Outline outlinetop = box.outlineTop;
                Outline outlinebottom = box.outlineBottom;
                Outline outlinestart = box.outlineStart;
                Outline outlineend = box.outlineEnd;


                using (Transaction t = new Transaction(doc, "Draw Model Line Along Wall"))
                {
                    t.Start();
                    Creator.CreateModelLine(doc, rayStartVertical1, rayStartVertical1+(wallDirectionVertical * 50));
                    Creator.CreateModelLine(doc, rayStartVertical2, rayStartVertical2 + (wallDirectionVertical * 50));
                    Creator.CreateModelLine(doc, rayStartHorizontal, rayStartHorizontal + (wallDirectionHorizontal * 10));
                    //Creator.CreateModelLine(doc, bottomcorner, topcorner);
                    Creator.CreateModelLine(doc, outline.MaximumPoint, outline.MinimumPoint);
                    //Creator.CreateModelLine(doc, bb.Max, bb.Min);
                    Creator.CreateModelLine(doc, outlinetop.MaximumPoint, outlinetop.MinimumPoint);
                    Creator.CreateModelLine(doc, outlinestart.MaximumPoint, outlinestart.MinimumPoint);
                    Creator.CreateModelLine(doc, outlineend.MaximumPoint, outlineend.MinimumPoint);
                    Creator.CreateModelLine(doc, outlinebottom.MaximumPoint, outlinebottom.MinimumPoint);
                    t.Commit();
                }

               
                //why is the outline empty?????

                

                return Result.Succeeded;
            }
        }
    }//end class SpecialCahracters
}//end namespace
