using ProjectionSccen.Core.ViewModels;
using System.Windows.Controls;

namespace ProjectionSccen.Core.ViewPage{

    public partial class FivetyPeople : UserControl
    {
        private FivetyPeopleViewModel viewModel;

        public FivetyPeople()
        {
            InitializeComponent();
            viewModel = new FivetyPeopleViewModel();
            this.DataContext = viewModel;
        }
    }
}