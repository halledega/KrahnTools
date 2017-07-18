#region Header
//
// HelperClass.cs - Dylan's Revit API helper methods
//
// Copyright (C) 20015-2017 by Dylan James,
// All rights reserved.
//
#endregion // Header
#region Namespaces
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
using BuildingCoder;
#endregion

namespace KrahnTools{

    class XyzEqualityComparer : IEqualityComparer<XYZ>
    {
        public bool Equals(XYZ a, XYZ b)
        {
            return Util.IsEqual(a, b);
        }

        public int GetHashCode(XYZ a)
        {
            return Util.PointString(a).GetHashCode();
        }
    }

    class WallOpening2d
    {
        public ElementId Id { get; set; }
        public XYZ Start { get; set; }
        public XYZ End { get; set; }
        override public string ToString()
        {
            //TaskDialog.Show("ID",Id.ToString());
            return "("
              //+ Id.ToString() + "@"
              + Util.PointString(Start) + "-"
              + Util.PointString(End) + ")";
        }
    }

    class MySelectionFilter : ISelectionFilter
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

    class ScottWilsonVoodooMagic
    {

        public enum SpecialReferenceType
        {
            Left = 0,
            CenterLR = 1,
            Right = 2,
            Front = 3,
            CenterFB = 4,
            Back = 5,
            Bottom = 6,
            CenterElevation = 7,
            Top = 8
        }

        public static Reference GetSpecialFamilyReference(FamilyInstance inst, SpecialReferenceType refType, Document doc)
        {
            Reference indexRef = null;
            int idx = (int)refType;
            if (inst != null)
            {
                Document dbDoc = inst.Document;

                Options geomOptions = dbDoc.Application.Create.NewGeometryOptions();

                if (geomOptions != null)
                {
                    geomOptions.ComputeReferences = true;
                    geomOptions.DetailLevel = ViewDetailLevel.Fine; //used to be undefined.
                                                                    //has to be false, otherwise if it has a void it will crash the program
                    geomOptions.IncludeNonVisibleObjects = false;
                }

                GeometryElement gElement = inst.get_Geometry(geomOptions);
                GeometryInstance gInst = gElement.First() as GeometryInstance;

                String sampleStableRef = null;

                if (gInst != null)
                {
                    GeometryElement gSymbol = gInst.GetSymbolGeometry();
                    if (gSymbol != null)
                    {
                        foreach (GeometryObject geomObj in gSymbol)
                        {
                            if (geomObj is Solid)
                            {
                                Solid solid = geomObj as Solid;

                                if (solid.Faces.Size > 0)
                                {
                                    Face face = solid.Faces.get_Item(0);

                                    sampleStableRef = face.Reference.ConvertToStableRepresentation(dbDoc);

                                    break;
                                }
                            }
                            else if (geomObj is Curve)
                            {
                                Curve curve = geomObj as Curve;

                                sampleStableRef = curve.Reference.ConvertToStableRepresentation(dbDoc);

                                break;
                            }
                            else if (geomObj is Point)
                            {
                                Point point = geomObj as Point;

                                sampleStableRef = point.Reference.ConvertToStableRepresentation(dbDoc);

                                break;
                            }
                        }
                    }




                    if (sampleStableRef != null)
                    {
                        String[] refTokens = sampleStableRef.Split(
                          new char[] { ':' });

                        String customStableRef = refTokens[0] + ":"
                          + refTokens[1] + ":" + refTokens[2] + ":"
                          + refTokens[3] + ":" + idx.ToString();


                        indexRef = Reference.ParseFromStableRepresentation(dbDoc, customStableRef);

                        GeometryObject geoObj = inst.GetGeometryObjectFromReference(indexRef);

                        if (geoObj != null)
                        {
                            String finalToken = "";

                            if (geoObj is Edge)
                            {
                                finalToken = ":LINEAR";
                            }

                            if (geoObj is Face)
                            {
                                finalToken = ":SURFACE";
                            }

                            customStableRef += finalToken;

                            indexRef = Reference.ParseFromStableRepresentation(dbDoc, customStableRef);
                        }
                        else
                        {
                            indexRef = null;
                        }
                    }
                }
                else
                {
                    throw new Exception("No Symbol Geometry found...");
                }
            }

            return indexRef;
        }
    }

    /// <summary>
    /// This class creates a bounding box around a wall, in the direction of the wall.
    /// Min = Wall start point - wall width, 
    /// Max = Wall end point + wall width.
    /// </summary>
    class CustomWallBoundingBox
    {
        //member variables
        private const double _buffer = 1;
        public Wall wall;
        public Outline outline { get { return checkOutline(Min, Max); } }
        public Outline outlineTop { get { return checkOutline(startThreeQuarterPanelHeight, Max); } }
        public Outline outlineBottom { get { return checkOutline(Min, EndQuarterPanelHeight); } }
        public Outline outlineStart { get { return checkOutline(Min, EndQuarterPanelWidth); } }
        public Outline outlineEnd { get { return checkOutline(startThreeQuarterPanelWidth, Max); } }
        public Element element;
        public XYZ wallStartPoint;
        public XYZ wallEndPoint;
        public Document document;
        public IList<Reference> sideFaces;
        public double wallWidth;
        public Curve wallCurve;
        public XYZ wallDirection;
        

        public XYZ Max;
        public XYZ Min;
        public double wallHeight;
        public double wallLength;
  

        private XYZ startQuarterPanelHeight;
        private XYZ EndQuarterPanelHeight;
        private XYZ startHalfPanelHeight;
        private XYZ EndHalfPanelHeight;
        private XYZ startThreeQuarterPanelHeight;
        private XYZ EndThreeQuarterPanelHeight;

        private XYZ startQuarterPanelWidth;
        private XYZ EndQuarterPanelWidth;
        private XYZ startHalfPanelWidth;
        private XYZ EndHalfPanelWidth;
        private XYZ startThreeQuarterPanelWidth;
        private XYZ EndThreeQuarterPanelWidth;

        private XYZ widthOffset;
        private XYZ startOffset;
        private XYZ endOffset;

        

        //constructors
        public CustomWallBoundingBox(Wall wa)
        {
            wall = wa;
            
            element = wa as Element;
            document = wa.Document;

            wallWidth = wa.Width;
            
            
            wallCurve = (wall.Location as LocationCurve).Curve;
            wallStartPoint = wallCurve.GetEndPoint(0);
            wallEndPoint = wallCurve.GetEndPoint(1);
            wallHeight = element.get_Parameter(BuiltInParameter.WALL_USER_HEIGHT_PARAM).AsDouble();
            wallDirection = (wallEndPoint - wallStartPoint).Normalize();
            wallLength = (wallEndPoint - wallStartPoint).GetLength();
           

            calcOutline();

        }

        //methods
        private void calcOutline()
        {
            

            sideFaces = HostObjectUtils.GetSideFaces(wall, ShellLayerType.Interior);
            Face face = document.GetElement(sideFaces[0]).GetGeometryObjectFromReference(sideFaces[0]) as Face;
            Reference reference = face.Reference;
            UV uv = new UV(0, 0);

            XYZ wallfacedir = face.ComputeNormal(uv) as XYZ;

            widthOffset = wallfacedir.Multiply(wallWidth/2+_buffer);
            startOffset = wallDirection.Multiply(_buffer);
            endOffset = wallDirection.Multiply(_buffer);

            

            Min = new XYZ(wallStartPoint.X + widthOffset.X - startOffset.X, wallStartPoint.Y + widthOffset.Y - startOffset.Y, wallStartPoint.Z - _buffer);
            Max = new XYZ(wallEndPoint.X - widthOffset.X + endOffset.X, wallEndPoint.Y - widthOffset.Y + endOffset.Y, wallEndPoint.Z + wallHeight + _buffer);

            

            startQuarterPanelHeight = Min + new XYZ(0,0,wallHeight * 1 / 4);
            EndQuarterPanelHeight = Max - new XYZ(0, 0, wallHeight * 3 / 4);
            startHalfPanelHeight = Min + new XYZ(0, 0, wallHeight * 1 / 2);
            EndHalfPanelHeight = Min - new XYZ(0, 0, wallHeight * 1 / 2);
            startThreeQuarterPanelHeight = Min + new XYZ(0, 0, wallHeight * 3 / 4);
            EndThreeQuarterPanelHeight = Max - new XYZ(0, 0, wallHeight * 1 / 4);
            
            startQuarterPanelWidth = Min + wallDirection.Multiply(wallLength* 1 / 4);
            EndQuarterPanelWidth = Max - wallDirection.Multiply(wallLength * 3 / 4);
            startHalfPanelWidth = Min + wallDirection.Multiply(wallLength * 1 / 2);
            EndHalfPanelWidth = Max - wallDirection.Multiply(wallLength * 1 / 2);
            startThreeQuarterPanelWidth = Min + wallDirection.Multiply(wallLength * 3 / 4);
            EndThreeQuarterPanelWidth = Max - wallDirection.Multiply(wallLength * 1 / 4);
        } 

        private Outline checkOutline(XYZ minpoint, XYZ maxpoint)
        {
            XYZ poswidthoffset = 2 * new XYZ(widthOffset.X + startOffset.X / 2, widthOffset.Y + startOffset.Y / 2, 0);
            XYZ negwidthoffset = 2 * new XYZ(- widthOffset.X - endOffset.X / 2, - widthOffset.Y - endOffset.Y / 2, 0);

            List<Outline> outlinelist = new List<Outline>();

            outlinelist.Add(new Outline(minpoint, maxpoint)); 
            outlinelist.Add(new Outline(maxpoint, minpoint));
            outlinelist.Add(new Outline(minpoint + negwidthoffset, maxpoint + poswidthoffset));//max too high and min too low, switch sine and no off.
            outlinelist.Add(new Outline(maxpoint + poswidthoffset, minpoint + negwidthoffset));

            XYZ newMin = new XYZ(maxpoint.X, maxpoint.Y, minpoint.Z);
            XYZ newMax = new XYZ(minpoint.X, minpoint.Y, maxpoint.Z);

            outlinelist.Add(new Outline(newMin, newMax)); 
            outlinelist.Add(new Outline(newMax, newMin));
            outlinelist.Add(new Outline(newMin + poswidthoffset, newMax + negwidthoffset)); //min too high and max too low, switch sine and double off.
            outlinelist.Add(new Outline(newMax + negwidthoffset, newMin + poswidthoffset));

            Outline tempoutline = null;
            foreach (Outline outl in outlinelist)
            {
                if (outl.IsEmpty == false)
                {
                    tempoutline = outl;
                }
            }

            return tempoutline;


        }
    }

    class HelperClass
    {

        #region GeometricTransforms

        public static Transform getLocalCoordinates(Element element, Boolean isStruct, UIDocument uidoc)
            {
                Transform transform = null;

                int isstruct = 1;


                if (isStruct == true)
                {
                    isstruct = -1;
                }

                if (element is Wall)
                {
                    Wall wall = element as Wall;

                    LocationCurve lc = wall.Location as LocationCurve;
                    Line line = lc.Curve as Line;

                    IList<Reference> sideFaces = HostObjectUtils.GetSideFaces(wall, ShellLayerType.Interior);

                    Face face = uidoc.Document.GetElement(sideFaces[0]).GetGeometryObjectFromReference(sideFaces[0]) as Face;

                    Reference reference = face.Reference;

                    UV uv = new UV(0, 0);

                    XYZ wallfacedir = face.ComputeNormal(uv) as XYZ;

                    XYZ p = line.GetEndPoint(0);
                    XYZ q = line.GetEndPoint(1);
                    XYZ v = q - p;

                    BoundingBoxXYZ bb = wall.get_BoundingBox(null);
                    double minZ = bb.Min.Z;
                    double maxZ = bb.Max.Z;

                    double w = v.GetLength();
                    double h = maxZ - minZ;

                    XYZ midpoint = p + 0.5 * v;

                    XYZ up = XYZ.BasisZ;

                    XYZ walldir = wallfacedir.CrossProduct(up);

                    transform = Transform.Identity;
                    transform.Origin = new XYZ(midpoint.X, midpoint.Y, 0);
                    transform.BasisX = walldir.Normalize() * isstruct * -1;
                    transform.BasisY = up;
                    transform.BasisZ = wallfacedir * isstruct;
                }
                return transform;
            }

        public static Transform getLocalCoordinatesPoint(Element element, UIDocument uidoc)
            {
                Transform transform = null;

                if (element is Wall)
                {
                    Wall wall = element as Wall;

                    LocationCurve lc = wall.Location as LocationCurve;
                    Line line = lc.Curve as Line;

                    IList<Reference> sideFaces = HostObjectUtils.GetSideFaces(wall, ShellLayerType.Interior);

                    Face face = uidoc.Document.GetElement(sideFaces[0]).GetGeometryObjectFromReference(sideFaces[0]) as Face;

                    Reference reference = face.Reference;

                    UV uv = new UV(0, 0);

                    XYZ wallfacedir = face.ComputeNormal(uv) as XYZ;

                    XYZ p = line.GetEndPoint(0);
                    XYZ q = line.GetEndPoint(1);
                    XYZ v = q - p;

                    BoundingBoxXYZ bb = wall.get_BoundingBox(null);
                    double minZ = bb.Min.Z;
                    double maxZ = bb.Max.Z;

                    double w = v.GetLength();
                    double h = maxZ - minZ;

                    XYZ midpoint = p + 0.5 * v;

                    XYZ up = XYZ.BasisZ;

                    XYZ walldir = wallfacedir.CrossProduct(up);

                    transform = Transform.Identity;
                    transform.Origin = new XYZ(midpoint.X, midpoint.Y, 0);
                    transform.BasisX = walldir.Normalize();
                    transform.BasisY = up;
                    transform.BasisZ = wallfacedir * -1;
                }
                return transform;
            }

        #endregion

        #region Methods

        public static bool IsSurface(Reference r)
            {
                return ElementReferenceType.REFERENCE_TYPE_SURFACE == r.ElementReferenceType;
            }
        public static List<Reference> GetWallOpenings(Wall wall, View3D view, bool vertical, double offset, bool start)
            {
                
                Document doc = wall.Document;
                Level level = doc.GetElement(wall.LevelId) as Level;
                double elevation = level.Elevation;
                Curve c = (wall.Location as LocationCurve).Curve;
                XYZ wallOrigin = c.GetEndPoint(0);
                XYZ wallEndPoint = c.GetEndPoint(1);
                XYZ wallDirectionHorizontal = wallEndPoint - wallOrigin;
                double wallLength = wallDirectionHorizontal.GetLength();
                wallDirectionHorizontal = wallDirectionHorizontal.Normalize();



                XYZ wallDirectionVertical = new XYZ(0d, 0d, 1d);

                UV offsetOutHorizontal = new UV();
                UV offsetOutVertical1 = new UV();
                UV offsetOutVertical2 = new UV();

                IList<ReferenceWithContext> refs = null;
                ReferenceIntersector intersector = new ReferenceIntersector(wall.Id, FindReferenceTarget.Face, view);

                if (vertical == false)
                {
                    offsetOutHorizontal = offset * new UV(wallDirectionHorizontal.X, wallDirectionHorizontal.Y);
                    XYZ rayStartHorizontal = new XYZ(wallOrigin.X - offsetOutHorizontal.U, wallOrigin.Y - offsetOutHorizontal.V, elevation + offset);
                    refs = intersector.Find(rayStartHorizontal, wallDirectionHorizontal);
                }
                else if(vertical == true & start == true)
                {
                    offsetOutVertical1 = offset * new UV(wallDirectionHorizontal.X, wallDirectionHorizontal.Y);
                    XYZ rayStartVertical1 = new XYZ(wallOrigin.X + offsetOutVertical1.U, wallOrigin.Y + offsetOutVertical1.V, elevation - offset);
                    refs = intersector.Find(rayStartVertical1, wallDirectionVertical);
                }
                else if (vertical == true & start == false)
                {
                    offsetOutVertical2 = offset * new UV(wallDirectionHorizontal.X, wallDirectionHorizontal.Y);
                    XYZ rayStartVertical2 = new XYZ(wallEndPoint.X - offsetOutVertical2.U, wallEndPoint.Y - offsetOutVertical2.V, elevation - offset);
                    refs = intersector.Find(rayStartVertical2, wallDirectionVertical);
                }

            List<Reference> faceReferenceList = new List<Reference>(refs
                      .Where<ReferenceWithContext>(r => IsSurface(
                       r.GetReference()))
                      .Where<ReferenceWithContext>(r => r.Proximity
                       < wallLength + offset + offset)
                      .Select<ReferenceWithContext, Reference>(r
                       => r.GetReference()));

                    return faceReferenceList;
                }

        #endregion

        public static View3D Get3dView(Document doc)
        {
            FilteredElementCollector collector
              = new FilteredElementCollector(doc)
                .OfClass(typeof(View3D));

            foreach (View3D v in collector)
            {


                if (!v.IsTemplate)
                {
                    return v;
                }
            }
            TaskDialog.Show("error",
                "never expected a null view to be returned"
                + " from filtered element collector");

            // Skip view template here because view 
            // templates are invisible in project 
            // browser
            return null;
        }

        public static BoundingBoxIsInsideFilter GetSelection(UIDocument uidoc, Reference reference, bool inside, View view)
        {
            Document doc = uidoc.Document;
            Element e = doc.GetElement(reference);
            Wall wall = e as Wall;
            ICollection<ElementId> selSet = new List<ElementId>();
            BoundingBoxXYZ bb = e.get_BoundingBox(view);
            XYZ buffer = new XYZ(0.083, 0.083, 0.083);
            Outline outline = new Outline(bb.Min - buffer, bb.Max + buffer);
            BoundingBoxIsInsideFilter bbfilter = new BoundingBoxIsInsideFilter(outline, true);

            return bbfilter;

        }

        public static View CreateViewAndSectionMark(UIDocument uidoc, Element e, bool StructSelected)
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

    }


    
}
