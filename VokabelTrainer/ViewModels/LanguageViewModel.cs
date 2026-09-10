using System.Collections.ObjectModel;
using System.Linq;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using VokabelTrainer.Models;

namespace VokabelTrainer.ViewModels;

public partial class LanguageViewModel : ViewModelBase
{
    private readonly MainViewModel _main;

    public ObservableCollection<Language> Languages => WordList.Languages;

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(AddCommand))]
    [NotifyCanExecuteChangedFor(nameof(RenameCommand))]
    public partial string NameInput { get; set; } = "";

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(RenameCommand))]
    [NotifyCanExecuteChangedFor(nameof(DeleteCommand))]
    [NotifyCanExecuteChangedFor(nameof(UseCommand))]
    [NotifyPropertyChangedFor(nameof(SelectionInfo))]
    public partial Language? SelectedLanguage { get; set; }

    public string SelectionInfo => SelectedLanguage is { } language
        ? $"{language.Name}: {WordList.CountWords(language)} Wörter"
        : "Keine Sprache ausgewählt";

    public LanguageViewModel(MainViewModel main)
    {
        _main = main;
        SelectedLanguage = WordList.CurrentLanguage;
    }

    partial void OnSelectedLanguageChanged(Language? value) => NameInput = value?.Name ?? "";

    private bool HasName() => !string.IsNullOrWhiteSpace(NameInput);

    private bool CanRename() => SelectedLanguage is not null && HasName();

    private bool HasSelection() => SelectedLanguage is not null;

    [RelayCommand(CanExecute = nameof(HasName))]
    private void Add()
    {
        var name = NameInput.Trim();

        if (Languages.Any(language => language.Name == name))
        {
            return;
        }

        SelectedLanguage = WordList.AddLanguage(name);
        NameInput = "";
    }

    [RelayCommand(CanExecute = nameof(CanRename))]
    private void Rename()
    {
        if (SelectedLanguage is { } language)
        {
            WordList.RenameLanguage(language, NameInput.Trim());
            OnPropertyChanged(nameof(SelectionInfo));
        }
    }

    [RelayCommand(CanExecute = nameof(HasSelection))]
    private void Delete()
    {
        if (SelectedLanguage is { } language)
        {
            WordList.RemoveLanguage(language);
            SelectedLanguage = WordList.CurrentLanguage;
        }
    }

    [RelayCommand(CanExecute = nameof(HasSelection))]
    private void Use()
    {
        WordList.SelectLanguage(SelectedLanguage);
        _main.ShowStart();
    }

    [RelayCommand]
    private void ReturnHome() => _main.ShowStart();
}
