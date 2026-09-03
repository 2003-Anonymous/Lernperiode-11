using CommunityToolkit.Mvvm.Input;
using VokabelTrainer.Models;

namespace VokabelTrainer.ViewModels;

public partial class StartViewModel : ViewModelBase
{
    private readonly MainViewModel _main;

    public string StatusText => $"{WordList.UnknownCount} offen, {WordList.KnownCount} gewusst";

    public StartViewModel(MainViewModel main)
    {
        _main = main;
    }

    // Wird zu LearnUnknownCommand -> Binding in StartView.axaml
    [RelayCommand]
    private void LearnUnknown() => _main.ShowLearn(true);

    [RelayCommand]
    private void LearnAll() => _main.ShowLearn(false);

    [RelayCommand]
    private void ShowWordList() => _main.ShowWordList();
}
