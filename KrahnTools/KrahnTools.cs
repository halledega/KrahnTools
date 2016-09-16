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
            string folderPath = @"C:\ProgramData\Autodesk\Revit\Addins\2016\KrahnTools";
            string dll = Path.Combine(folderPath, "KrahnTools.dll");

            string myRibbon = "Krahn Tools";
            application.CreateRibbonTab(myRibbon);

            RibbonPanel panelA = application.CreateRibbonPanel(myRibbon, "PSD Tools");
            RibbonPanel panelB = application.CreateRibbonPanel(myRibbon, "Text Tools");
            RibbonPanel panelC = application.CreateRibbonPanel(myRibbon, "Text Tools");

            // Standard buttons

            PushButton btnAOne = (PushButton)panelA.AddItem(new PushButtonData("Create PSD", "CreatePSD", dll, "KrahnTools.PSDCreator.CreatePSD"));
            // need reference to PresentationCore to get access to the System.Windows.Media.Imaging namespace which includes BitmapImage
            btnAOne.LargeImage = new BitmapImage(new Uri(Path.Combine(folderPath, "imgs/KrahnLogo32.png"), UriKind.Absolute));
            btnAOne.Image = new BitmapImage(new Uri(Path.Combine(folderPath, "imgs/KrahnLogo16.png"), UriKind.Absolute));
            btnAOne.ToolTipImage = new BitmapImage(new Uri(Path.Combine(folderPath, "imgs/Panel.png"), UriKind.Absolute));
            btnAOne.ToolTip = "Click this button to create panel shop drawings";
            //btnOne.LongDescription = "";

            PushButton btnBOne = (PushButton)panelB.AddItem(new PushButtonData("Get Special Characters", "SpecialCharacters", dll, "KrahnTools.SpecialCharacters.SpecialCharacters"));
            // need reference to PresentationCore to get access to the System.Windows.Media.Imaging namespace which includes BitmapImage
            btnBOne.LargeImage = new BitmapImage(new Uri(Path.Combine(folderPath, "imgs/diameter_16.png"), UriKind.Absolute));
            btnBOne.Image = new BitmapImage(new Uri(Path.Combine(folderPath, "imgs/diameter_16.png"), UriKind.Absolute));
            //btnBOne.ToolTipImage = new BitmapImage(new Uri(Path.Combine(folderPath, "imgs/diameter_32.png"), UriKind.Absolute));
            btnBOne.ToolTip = "Copies special charecters (Alt-Codes) to clip board to be pastes into text block";
            //btnOne.LongDescription = "";

            PushButton btnCOne = (PushButton)panelB.AddItem(new PushButtonData("Get Climatic Data", "ClimaticData", dll, "KrahnTools.ClimaticData.ClimaticData"));
            // need reference to PresentationCore to get access to the System.Windows.Media.Imaging namespace which includes BitmapImage
            btnCOne.LargeImage = new BitmapImage(new Uri(Path.Combine(folderPath, "imgs/diameter_16.png"), UriKind.Absolute));
            btnCOne.Image = new BitmapImage(new Uri(Path.Combine(folderPath, "imgs/diameter_16.png"), UriKind.Absolute));
            //btnBOne.ToolTipImage = new BitmapImage(new Uri(Path.Combine(folderPath, "imgs/diameter_32.png"), UriKind.Absolute));
            btnCOne.ToolTip = "Allows users to load climatic data from Engineers Calculations or select for projects location from a list";
            //btnOne.LongDescription = "";

            return Result.Succeeded;
        }

        public Result OnShutdown(UIControlledApplication application)
        {
            return Result.Succeeded;
        }
    }
}



