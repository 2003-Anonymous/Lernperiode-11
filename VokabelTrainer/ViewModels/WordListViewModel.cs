using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using VokabelTrainer.Models;

namespace VokabelTrainer.ViewModels;

public partial class WordListViewModel : ViewModelBase
{
    private readonly MainViewModel _main;

    public ObservableCollection<Word> FilteredWords { get; } = [];

    public string CountText =>
        $"{FilteredWords.Count} von {WordList.Words.Count} Wörtern - {WordList.UnknownCount} offen, {WordList.KnownCount} gewusst";

    [ObservableProperty]
    public partial string SearchText { get; set; } = "";

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(SaveCommand))]
    public partial string GermanInput { get; set; } = "";

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(SaveCommand))]
    public partial string ForeignInput { get; set; } = "";

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(DeleteCommand))]
    [NotifyPropertyChangedFor(nameof(EditorTitle))]
    [NotifyPropertyChangedFor(nameof(IsEditing))]
    public partial Word? SelectedWord { get; set; }

    public bool IsEditing => SelectedWord is not null;

    public string EditorTitle => IsEditing ? "Wort bearbeiten" : "Neues Wort";

    public WordListViewModel(MainViewModel main)
    {
        _main = main;
        WordList.Words.CollectionChanged += OnWordsChanged;
        ApplyFilter();
    }

    private void OnWordsChanged(object? sender, NotifyCollectionChangedEventArgs e) => ApplyFilter();

    partial void OnSearchTextChanged(string value) => ApplyFilter();

    private void ApplyFilter()
    {
        var search = SearchText.Trim();

        var matches = string.IsNullOrEmpty(search)
            ? WordList.Words
            : WordList.Words.Where(word =>
                word.German.Contains(search, StringComparison.CurrentCultureIgnoreCase) ||
                word.ForeignLanguage.Contains(search, StringComparison.CurrentCultureIgnoreCase));

        FilteredWords.Clear();
        foreach (var word in matches)
        {
            FilteredWords.Add(word);
        }

        OnPropertyChanged(nameof(CountText));
    }

    private bool CanSave()
        => !string.IsNullOrWhiteSpace(GermanInput) && !string.IsNullOrWhiteSpace(ForeignInput);

    partial void OnSelectedWordChanged(Word? value)
    {
        GermanInput = value?.German ?? "";
        ForeignInput = value?.ForeignLanguage ?? "";
    }

    [RelayCommand(CanExecute = nameof(CanSave))]
    private void Save()
    {
        var german = GermanInput.Trim();
        var foreignLanguage = ForeignInput.Trim();

        if (SelectedWord is { } word)
        {
            WordList.Update(word, german, foreignLanguage);
            ApplyFilter();
        }
        else
        {
            WordList.Add(german, foreignLanguage);
        }

        NewWord();
    }

    private bool CanDelete() => SelectedWord is not null;

    [RelayCommand(CanExecute = nameof(CanDelete))]
    private void Delete()
    {
        if (SelectedWord is { } word)
        {
            WordList.Remove(word);
            NewWord();
        }
    }

    [RelayCommand]
    private void NewWord()
    {
        SelectedWord = null;
        GermanInput = "";
        ForeignInput = "";
    }

    [RelayCommand]
    private void ClearSearch() => SearchText = "";

    [RelayCommand]
    private void Start() => _main.ShowLearn(false);

    [RelayCommand]
    private void ReturnHome() => _main.ShowStart();
}
