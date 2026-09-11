using System.Collections.ObjectModel;
using System.Linq;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using VokabelTrainer.Models;

namespace VokabelTrainer.ViewModels;

public partial class CollectionViewModel : ViewModelBase
{
    private readonly MainViewModel _main;

    public ObservableCollection<Collection> Collections => WordList.Collections;

    public string LanguageName => WordList.CurrentLanguage?.Name ?? "Keine Sprache";

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(AddCommand))]
    [NotifyCanExecuteChangedFor(nameof(RenameCommand))]
    public partial string NameInput { get; set; } = "";

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(RenameCommand))]
    [NotifyCanExecuteChangedFor(nameof(DeleteCommand))]
    [NotifyCanExecuteChangedFor(nameof(UseCommand))]
    [NotifyPropertyChangedFor(nameof(SelectionInfo))]
    public partial Collection? SelectedCollection { get; set; }

    public string SelectionInfo => SelectedCollection is { } collection
        ? $"{collection.Name}: {WordList.CountWords(collection)} Wörter"
        : "Keine Sammlung ausgewählt";

    public CollectionViewModel(MainViewModel main)
    {
        _main = main;
        SelectedCollection = WordList.CurrentCollection ?? Collections.FirstOrDefault();
    }

    partial void OnSelectedCollectionChanged(Collection? value) => NameInput = IsReal(value) ? value!.Name : "";

    private static bool IsReal(Collection? collection) => collection is not null && collection.Id != 0;

    private bool CanAdd() => WordList.CurrentLanguage is not null && !string.IsNullOrWhiteSpace(NameInput);

    private bool CanRename() => IsReal(SelectedCollection) && !string.IsNullOrWhiteSpace(NameInput);

    private bool CanDelete() => IsReal(SelectedCollection);

    private bool CanUse() => SelectedCollection is not null;

    [RelayCommand(CanExecute = nameof(CanAdd))]
    private void Add()
    {
        var name = NameInput.Trim();

        if (Collections.Any(collection => collection.Name == name))
        {
            return;
        }

        SelectedCollection = WordList.AddCollection(name);
        NameInput = "";
    }

    [RelayCommand(CanExecute = nameof(CanRename))]
    private void Rename()
    {
        if (SelectedCollection is { } collection)
        {
            WordList.RenameCollection(collection, NameInput.Trim());
            OnPropertyChanged(nameof(SelectionInfo));
        }
    }

    [RelayCommand(CanExecute = nameof(CanDelete))]
    private void Delete()
    {
        if (SelectedCollection is { } collection)
        {
            WordList.RemoveCollection(collection);
            SelectedCollection = Collections.FirstOrDefault();
        }
    }

    [RelayCommand(CanExecute = nameof(CanUse))]
    private void Use()
    {
        WordList.SelectCollection(SelectedCollection);
        _main.ShowStart();
    }

    [RelayCommand]
    private void ReturnHome() => _main.ShowStart();
}
