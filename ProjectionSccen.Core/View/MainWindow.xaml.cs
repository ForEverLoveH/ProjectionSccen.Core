using System.Windows;
using ProjectionSccen.Core.ViewModels;

namespace ProjectionSccen.Core.View{

    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            _viewModel = new MainWindowViewModel();
            this.DataContext = _viewModel;
        }

        private MainWindowViewModel _viewModel;
        private static MainWindow Instance;

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public static MainWindow GetInstance()
        {
            if (Instance == null)
            {
                Instance = new MainWindow();
            }

            return Instance;
        }
    }
}