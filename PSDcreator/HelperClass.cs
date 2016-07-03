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

namespace PSDcreator
{
    class HelperClass
    {

        public static Transform getLocalCoordinates(Element element, UIDocument uidoc)
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

                UV uv = new UV(0,0);

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
    }
}
