using ProjectionSccen.Core.ViewModels;
using System.Windows.Controls;

namespace ProjectionSccen.Core.ViewPage{

    public partial class TwentyPeople : UserControl
    {

        private TwentyPeopleViewModel ViewModel { get; set; }

        public TwentyPeople()
        {
            InitializeComponent();
            ViewModel = new TwentyPeopleViewModel();
            this.DataContext = ViewModel;
        }
    }
}