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





namespace KrahnTools
{
    class ribbonUI : IExternalApplication
    {
        public Autodesk.Revit.UI.Result OnStartup(UIControlledApplication application)
        {
            string folderPath = @"C:\ProgramData\Autodesk\Revit\Addins\2016";
            string dll = Path.Combine(folderPath, "PSDcreator.dll");

            string myRibbon = "Krahn Tools";
            application.CreateRibbonTab(myRibbon);

            RibbonPanel panelA = application.CreateRibbonPanel(myRibbon, "PSD Tools");

            // Standard buttons

            PushButton btnOne = (PushButton)panelA.AddItem(new PushButtonData("Create PSD", "CreatePSD", dll, "PSDcreator.CreatePSD"));
            // need reference to PresentationCore to get access to the System.Windows.Media.Imaging namespace which includes BitmapImage
            btnOne.LargeImage = new BitmapImage(new Uri(Path.Combine(folderPath, "PSDcreator/KrahnLogo32.png"), UriKind.Absolute));
            btnOne.Image = new BitmapImage(new Uri(Path.Combine(folderPath, "PSDcreator/KrahnLogo16.png"), UriKind.Absolute));
            btnOne.ToolTipImage = new BitmapImage(new Uri(Path.Combine(folderPath, "PSDcreator/Panel.png"), UriKind.Absolute));
            btnOne.ToolTip = "Click this button to create panel shop drawings";
            //btnOne.LongDescription = "";

            return Result.Succeeded;
        }

        public Result OnShutdown(UIControlledApplication application)
        {
            return Result.Succeeded;
        }
    }
}



