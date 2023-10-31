using ProjectionSccen.Core.ViewModels;
using System.Windows.Controls;

namespace ProjectionSccen.Core.ViewPage;

public partial class FourPeople : UserControl
{
    private FourPeopleViewModel viewModel;

    public FourPeople()
    {
        InitializeComponent();
        viewModel = new FourPeopleViewModel();
        this.DataContext = viewModel;
    }
}