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

    public ObservableCollection<Collection> EditorCollections { get; } = [];

    public string LanguageName => WordList.CurrentLanguage?.Name ?? "Keine Sprache";

    public string CountText =>
        $"{FilteredWords.Count} von {WordList.Words.Count} Wörtern · {WordList.UnknownCount} offen";

    [ObservableProperty]
    public partial string SearchText { get; set; } = "";

    [ObservableProperty]
    public partial bool EditorVisible { get; set; }

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(SaveCommand))]
    public partial string GermanInput { get; set; } = "";

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(SaveCommand))]
    public partial string ForeignInput { get; set; } = "";

    [ObservableProperty]
    public partial Collection? SelectedEditorCollection { get; set; }

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

        LoadEditorCollections();
        ApplyFilter();
        ResetEditor();
    }

    private void LoadEditorCollections()
    {
        var languageId = WordList.CurrentLanguage?.Id ?? 0;

        EditorCollections.Clear();
        EditorCollections.Add(new Collection(0, languageId, "Keine Sammlung"));

        foreach (var collection in WordList.Collections.Where(item => item.Id != 0))
        {
            EditorCollections.Add(collection);
        }
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
        => WordList.CurrentLanguage is not null
           && !string.IsNullOrWhiteSpace(GermanInput)
           && !string.IsNullOrWhiteSpace(ForeignInput);

    partial void OnSelectedWordChanged(Word? value)
    {
        GermanInput = value?.German ?? "";
        ForeignInput = value?.ForeignLanguage ?? "";
        SelectedEditorCollection = FindEditorCollection(value?.CollectionId);

        if (value is not null)
        {
            EditorVisible = true;
        }
    }

    private Collection? FindEditorCollection(int? collectionId)
        => EditorCollections.FirstOrDefault(collection => collection.Id == (collectionId ?? 0))
           ?? EditorCollections.FirstOrDefault();

    private int? ChosenCollectionId()
        => SelectedEditorCollection is { Id: > 0 } collection ? collection.Id : null;

    private void ResetEditor()
    {
        SelectedWord = null;
        GermanInput = "";
        ForeignInput = "";
        SelectedEditorCollection = EditorCollections.FirstOrDefault();
    }

    [RelayCommand(CanExecute = nameof(CanSave))]
    private void Save()
    {
        var german = GermanInput.Trim();
        var foreignLanguage = ForeignInput.Trim();

        if (SelectedWord is { } word)
        {
            WordList.Update(word, german, foreignLanguage, ChosenCollectionId());
            ApplyFilter();
        }
        else
        {
            WordList.Add(german, foreignLanguage, ChosenCollectionId());
        }

        ResetEditor();
        EditorVisible = false;
    }

    private bool CanDelete() => SelectedWord is not null;

    [RelayCommand(CanExecute = nameof(CanDelete))]
    private void Delete()
    {
        if (SelectedWord is { } word)
        {
            WordList.Remove(word);
            ResetEditor();
            EditorVisible = false;
        }
    }

    [RelayCommand]
    private void OpenEditor()
    {
        ResetEditor();
        EditorVisible = true;
    }

    [RelayCommand]
    private void CloseEditor()
    {
        ResetEditor();
        EditorVisible = false;
    }

    [RelayCommand]
    private void ClearSearch() => SearchText = "";

    [RelayCommand]
    private void Start() => _main.ShowLearn(false);

    [RelayCommand]
    private void ReturnHome() => _main.ShowStart();
}
