using MAUI_TTRPG_DiceRoller.Models;

namespace MAUI_TTRPG_DiceRoller.Pages
{
    public partial class ProjectDetailPage : ContentPage
    {
        public ProjectDetailPage(ProjectDetailPageModel model)
        {
            InitializeComponent();

            BindingContext = model;
        }
    }
}
