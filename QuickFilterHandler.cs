using Autodesk.Revit.UI;
using System;
using System.Windows.Forms;

namespace QApps
{
    public class QuickFilterHandler : IExternalEventHandler
    {
        public QuickFilterViewModel ViewModel;

        /// <summary>
        /// Thực hiện các lệnh khi được Raise() lên
        /// </summary>
        /// <param name="app"></param>
        public void Execute(UIApplication app)
        {
            try
            {
                ViewModel.GetElements();
            }
            catch (Exception e)
            {
                MessageBox.Show(e.ToString());
            }
        }

        public string GetName()
        {
            return "Q'Apps Solutions External Event";
        }
    }
}
