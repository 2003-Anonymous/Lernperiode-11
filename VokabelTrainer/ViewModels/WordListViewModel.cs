using System.Collections.ObjectModel;
using System.Collections.Specialized;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using VokabelTrainer.Models;

namespace VokabelTrainer.ViewModels;

public partial class WordListViewModel : ViewModelBase
{
    private readonly MainViewModel _main;

    public ObservableCollection<Word> Words => WordList.Words;

    public string CountText => $"{Words.Count} Wörter - {WordList.UnknownCount} offen, {WordList.KnownCount} gewusst";

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
        Words.CollectionChanged += OnWordsChanged;
    }

    private void OnWordsChanged(object? sender, NotifyCollectionChangedEventArgs e)
        => OnPropertyChanged(nameof(CountText));

    partial void OnSelectedWordChanged(Word? value)
    {
        GermanInput = value?.German ?? "";
        ForeignInput = value?.ForeignLanguage ?? "";
    }

    private bool CanSave()
        => !string.IsNullOrWhiteSpace(GermanInput) && !string.IsNullOrWhiteSpace(ForeignInput);

    [RelayCommand(CanExecute = nameof(CanSave))]
    private void Save()
    {
        var german = GermanInput.Trim();
        var foreignLanguage = ForeignInput.Trim();

        if (SelectedWord is { } word)
        {
            WordList.Update(word, german, foreignLanguage);
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
    private void Edit(Word word) => SelectedWord = word;

    [RelayCommand]
    private void NewWord()
    {
        SelectedWord = null;
        GermanInput = "";
        ForeignInput = "";
    }

    [RelayCommand]
    private void Start() => _main.ShowLearn(false);

    [RelayCommand]
    private void ReturnHome() => _main.ShowStart();
}
