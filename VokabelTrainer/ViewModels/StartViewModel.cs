using CommunityToolkit.Mvvm.Input;

namespace VokabelTrainer.ViewModels;

public partial class StartViewModel : ViewModelBase
{
    private readonly MainViewModel _main;

    public StartViewModel(MainViewModel main)
    {
        _main = main;
    }

    // Wird zu StartCommand -> Binding in StartView.axaml
    [RelayCommand]
    private void Start() => _main.ShowLearn();
}
