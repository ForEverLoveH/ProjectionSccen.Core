using ProjectionSccen.Core.ViewModels;
using System.Windows.Controls;

namespace ProjectionSccen.Core.ViewPage{

    public partial class TenPeople : UserControl
    {
        private TenPeopleViewModel viewModel;

        /// <summary>
        ///
        /// </summary>
        public TenPeople()
        {
            InitializeComponent();
            viewModel = new TenPeopleViewModel();
            this.DataContext = viewModel;
        }
    }
}