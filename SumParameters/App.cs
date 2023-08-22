using Autodesk.Revit.UI;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Reflection;
using System.IO;
using Autodesk.Windows;

namespace SumParameters
{
    public class App : IExternalApplication
    {
       
        public Result OnStartup(UIControlledApplication application)
        {
            var SumPanelStream = application.CreateRibbonPanel("SumParameters");

            PushButtonData buttonDataStream = Icons.GetButtonIconsStream();

            SumPanelStream.AddItem(buttonDataStream);

            //для добавления панели на вкладку изменить
            var tab = ComponentManager.Ribbon.FindTab("Modify");
            if (tab != null)
            {
                var adwPanel = new Autodesk.Windows.RibbonPanel();
                adwPanel.CopyFrom(GetRibbonPanel(SumPanelStream));
                tab.Panels.Add(adwPanel);
            }

            return Result.Succeeded;
        }
        private static readonly FieldInfo RibbonPanelField = typeof(Autodesk.Revit.UI.RibbonPanel).GetField("m_RibbonPanel", BindingFlags.Instance | BindingFlags.NonPublic);
        public static Autodesk.Windows.RibbonPanel GetRibbonPanel(Autodesk.Revit.UI.RibbonPanel panel)
        {
            return RibbonPanelField.GetValue(panel) as Autodesk.Windows.RibbonPanel;
        }
        public Result OnShutdown(UIControlledApplication application)
        {
            return Result.Succeeded;
        }
    }

}



class Icons
{
    public static PushButtonData GetButtonIconsStream()
    {
        string assemblyLocation = Assembly.GetExecutingAssembly().Location;
        string executableLocation = Path.GetDirectoryName(assemblyLocation);
        string dllLocation = Path.Combine(executableLocation, "SumParameters.dll");
        PushButtonData sumParametersButton = new PushButtonData("sumParametersButton", "Sum", dllLocation, "SumParameters.Command");
        sumParametersButton.ToolTip = "Cумма параметров";
        sumParametersButton.LargeImage = RetriveImage("SumParameters.Resources.LargeIcon.ico");
        sumParametersButton.Image = RetriveImage("SumParameters.Resources.SmallIcon.ico");
        return sumParametersButton;
    }

    /// <summary>
    /// Класс конвертации картинки
    /// </summary>
    private static ImageSource RetriveImage(string imagePath)
    {

        Stream stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(imagePath);

        switch (imagePath.Substring(imagePath.Length - 3))
        {
            case "jpg":
                var jpgDecoder = new System.Windows.Media.Imaging.JpegBitmapDecoder(stream, BitmapCreateOptions.PreservePixelFormat, BitmapCacheOption.Default);
                return jpgDecoder.Frames[0];
            case "bmp":
                var bmpDecoder = new System.Windows.Media.Imaging.BmpBitmapDecoder(stream, BitmapCreateOptions.PreservePixelFormat, BitmapCacheOption.Default);
                return bmpDecoder.Frames[0];
            case "png":
                var pngDecoder = new System.Windows.Media.Imaging.PngBitmapDecoder(stream, BitmapCreateOptions.PreservePixelFormat, BitmapCacheOption.Default);
                return pngDecoder.Frames[0];
            case "ico":
                var icoDecoder = new System.Windows.Media.Imaging.IconBitmapDecoder(stream, BitmapCreateOptions.PreservePixelFormat, BitmapCacheOption.Default);
                return icoDecoder.Frames[0];
            default:
                return null;
        }
    }
}