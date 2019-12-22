using System.Collections.Generic;
using System.Windows;
using System.Windows.Input;

namespace QApps
{
    public partial class QuickFilterWindow
    {
        private QuickFilterViewModel _viewModel;
        SKRrYXKUnVmrC_L dlqConstraint = new SKRrYXKUnVmrC_L();
        List<string> allValueParameters = new List<string>();


        public QuickFilterWindow(QuickFilterViewModel viewModel)
        {
            InitializeComponent();
            _viewModel = viewModel;
            DataContext = _viewModel;
            Icon = dlqConstraint.IconWindow;
        }

        private void btnOk_Click(object sender, RoutedEventArgs e)
        {
            _viewModel.GetElements();
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            Height = MainGrid.ActualHeight + 40;
            MinHeight = MainGrid.ActualHeight + 40;
        }
    }
}
