using System.Collections.Generic;
using CommunityToolkit.Mvvm.Input;
using VokabelTrainer.Models;

namespace VokabelTrainer.ViewModels;

public partial class ResultViewModel : ViewModelBase
{
    private readonly MainViewModel _main;

    // Kommen fertig von der Lernseite herein - dieses ViewModel rechnet nur noch aus.
    public List<Word> KnownWords { get; }
    public List<Word> UnknownWords { get; }

    public int KnownCount => KnownWords.Count;
    public int UnknownCount => UnknownWords.Count;
    public int TotalCount => KnownWords.Count + UnknownWords.Count;

    public string Summary => $"{KnownCount} von {TotalCount} gewusst";

    public ResultViewModel(MainViewModel main, List<Word> known, List<Word> unknown)
    {
        _main = main;
        KnownWords = known;
        UnknownWords = unknown;
    }

    [RelayCommand]
    private void ReturnHome() => _main.ShowStart();
}
