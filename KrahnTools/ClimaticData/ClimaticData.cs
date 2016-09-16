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


namespace KrahnTools.ClimaticData
{
    [TransactionAttribute(TransactionMode.Manual)]
    [RegenerationAttribute(RegenerationOption.Manual)]
    public class ClimaticData : IExternalCommand
    {
        static AddInId appId = new AddInId(new Guid("1264EF6D-D1EB-4D06-A08C-94FECE39A2A8"));

        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            //Get application and document objects
            UIApplication uiapp = commandData.Application;
            UIDocument uidoc = uiapp.ActiveUIDocument;
            Application app = uiapp.Application;
            Document doc = uidoc.Document;

            using (ClimaticData_form inputForm = new ClimaticData_form())
            {
                inputForm.ShowDialog();
            }
            return Result.Succeeded;
        }
    }//end class SpecialCahracters
}//end namespace
