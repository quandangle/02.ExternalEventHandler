#region Namespaces

using System.IO;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Application = Autodesk.Revit.ApplicationServices.Application;
#endregion
namespace QApps
{
    [Transaction(TransactionMode.Manual)]
    public class QuickFilterCmd : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, 
            ref string message, ElementSet elements)
        {
            UIApplication uiapp = commandData.Application;
            UIDocument uidoc = uiapp.ActiveUIDocument;
            Application app = uiapp.Application;
            Document doc = uidoc.Document;

            new QuickFilterViewModel(commandData);
            return Result.Cancelled;

        }
    }
}
