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

namespace KrahnTools.SpecialCharacters
{
    [TransactionAttribute(TransactionMode.Manual)]
    [RegenerationAttribute(RegenerationOption.Manual)]
    public class SpecialCharacters : IExternalCommand
    {
        static AddInId appId = new AddInId(new Guid("D934A71B-5705-4954-A9A6-195C4EEB7E23"));

        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            //Get application and document objects
            UIApplication uiapp = commandData.Application;
            UIDocument uidoc = uiapp.ActiveUIDocument;
            Application app = uiapp.Application;
            Document doc = uidoc.Document;

            using (SpecialCharacters_form inputForm = new SpecialCharacters_form())
            {
                inputForm.ShowDialog();
            }
            return Result.Succeeded;
        }
    }//end class SpecialCahracters
}//end namespace
