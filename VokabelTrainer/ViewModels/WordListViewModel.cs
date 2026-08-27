using System.Collections.Generic;
using CommunityToolkit.Mvvm.Input;
using VokabelTrainer.Models;

namespace VokabelTrainer.ViewModels;

public partial class WordListViewModel : ViewModelBase
{
    private readonly MainViewModel _main;

    public List<Word> Words => WordList.Words;

    public string CountText => $"{Words.Count} Wörter";

    public WordListViewModel(MainViewModel main)
    {
        _main = main;
    }

    [RelayCommand]
    private void Start() => _main.ShowLearn();

    [RelayCommand]
    private void ReturnHome() => _main.ShowStart();
}
