using CommunityToolkit.Mvvm.Input;
using MAUI_TTRPG_DiceRoller.Models;

namespace MAUI_TTRPG_DiceRoller.PageModels
{
    public interface IProjectTaskPageModel
    {
        IAsyncRelayCommand<ProjectTask> NavigateToTaskCommand { get; }
        bool IsBusy { get; }
    }
}