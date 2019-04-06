using System.ComponentModel;
using System.Windows;
using ProcessManager.View;

namespace ProcessManager
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        
        private ProcessView _processView;

        public MainWindow()
        {
            InitializeComponent();
            ShowProcessesListView();
        }

        private void ShowProcessesListView()
        {
            MainGrid.Children.Clear();
            if (_processView == null)
            {
                _processView = new ProcessView();
            }
                
            MainGrid.Children.Add(_processView);
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            _processView?.Close();
            Tools.ProcessManager.Close();
            base.OnClosing(e);
        }
        
    }
}
