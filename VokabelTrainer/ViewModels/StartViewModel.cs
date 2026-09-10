using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using VokabelTrainer.Models;

namespace VokabelTrainer.ViewModels;

public partial class StartViewModel : ViewModelBase
{
    private readonly MainViewModel _main;

    public ObservableCollection<Language> Languages => WordList.Languages;

    [ObservableProperty]
    public partial Language? SelectedLanguage { get; set; }

    public string StatusText => WordList.CurrentLanguage is null
        ? "Noch keine Sprache angelegt"
        : $"{WordList.UnknownCount} offen, {WordList.KnownCount} gewusst";

    public StartViewModel(MainViewModel main)
    {
        _main = main;
        SelectedLanguage = WordList.CurrentLanguage;
    }

    partial void OnSelectedLanguageChanged(Language? value)
    {
        if (value is null || value.Id == WordList.CurrentLanguage?.Id)
        {
            return;
        }

        WordList.SelectLanguage(value);
        OnPropertyChanged(nameof(StatusText));
    }

    // Wird zu LearnUnknownCommand -> Binding in StartView.axaml
    [RelayCommand]
    private void LearnUnknown() => _main.ShowLearn(true);

    [RelayCommand]
    private void LearnAll() => _main.ShowLearn(false);

    [RelayCommand]
    private void ShowWordList() => _main.ShowWordList();

    [RelayCommand]
    private void ShowLanguages() => _main.ShowLanguages();
}
