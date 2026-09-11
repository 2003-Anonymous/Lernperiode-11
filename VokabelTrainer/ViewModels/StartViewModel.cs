using System.Collections.ObjectModel;
using System.Linq;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using VokabelTrainer.Models;

namespace VokabelTrainer.ViewModels;

public partial class StartViewModel : ViewModelBase
{
    private readonly MainViewModel _main;

    public ObservableCollection<Language> Languages => WordList.Languages;

    public ObservableCollection<Collection> Collections => WordList.Collections;

    [ObservableProperty]
    public partial Language? SelectedLanguage { get; set; }

    [ObservableProperty]
    public partial Collection? SelectedCollection { get; set; }

    public int KnownCount => WordList.KnownCount;

    public int UnknownCount => WordList.UnknownCount;

    public int TotalCount => WordList.Words.Count;

    public bool HasWords => TotalCount > 0;

    public string StatusText => WordList.CurrentLanguage is null
        ? "Noch keine Sprache angelegt"
        : $"{TotalCount} Wörter in dieser Auswahl";

    public StartViewModel(MainViewModel main)
    {
        _main = main;
        SelectedLanguage = WordList.CurrentLanguage;
        SelectedCollection = WordList.CurrentCollection ?? WordList.Collections.FirstOrDefault();
    }

    private void RefreshStats()
    {
        OnPropertyChanged(nameof(KnownCount));
        OnPropertyChanged(nameof(UnknownCount));
        OnPropertyChanged(nameof(TotalCount));
        OnPropertyChanged(nameof(HasWords));
        OnPropertyChanged(nameof(StatusText));
    }

    partial void OnSelectedLanguageChanged(Language? value)
    {
        if (value is null || value.Id == WordList.CurrentLanguage?.Id)
        {
            return;
        }

        WordList.SelectLanguage(value);
        SelectedCollection = WordList.Collections.FirstOrDefault();
        RefreshStats();
    }

    partial void OnSelectedCollectionChanged(Collection? value)
    {
        if (value is null)
        {
            return;
        }

        WordList.SelectCollection(value);
        RefreshStats();
    }

    [RelayCommand]
    private void LearnUnknown() => _main.ShowLearn(true);

    [RelayCommand]
    private void LearnAll() => _main.ShowLearn(false);

    [RelayCommand]
    private void ShowWordList() => _main.ShowWordList();

    [RelayCommand]
    private void ShowLanguages() => _main.ShowLanguages();

    [RelayCommand]
    private void ShowCollections() => _main.ShowCollections();
}
