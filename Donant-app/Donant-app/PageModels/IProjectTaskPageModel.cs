using CommunityToolkit.Mvvm.Input;
using Donant_app.Models;

namespace Donant_app.PageModels
{
    public interface IProjectTaskPageModel
    {
        IAsyncRelayCommand<ProjectTask> NavigateToTaskCommand { get; }
        bool IsBusy { get; }
    }
}