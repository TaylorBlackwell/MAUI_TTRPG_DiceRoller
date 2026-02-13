using MAUI_TTRPG_DiceRoller.PageModels;

namespace MAUI_TTRPG_DiceRoller.Pages
{
    public partial class MainPage : ContentPage
    {
        public MainPage(DiceRollerPageModel model)
        {
            InitializeComponent();
            BindingContext = model;
        }
    }
}