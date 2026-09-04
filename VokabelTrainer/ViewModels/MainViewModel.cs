using System;
using System.Collections.Generic;
using System.Linq;
using CommunityToolkit.Mvvm.ComponentModel;
using VokabelTrainer.Models;

namespace VokabelTrainer.ViewModels;

public partial class MainViewModel : ViewModelBase
{
    // Die gerade angezeigte Seite. Typ ViewModelBase -> kann jedes ViewModel aufnehmen.
    [ObservableProperty]
    public partial ViewModelBase CurrentPage { get; set; }

    public MainViewModel()
    {
        CurrentPage = new StartViewModel(this);   // Startseite
    }

    public void ShowStart() => CurrentPage = new StartViewModel(this);
    public void ShowWordList() => CurrentPage = new WordListViewModel(this);

    public void ShowLearn(bool onlyUnknown)
    {
        List<Word> words = onlyUnknown
            ? [.. WordList.Words.Where(word => !word.IsKnown)]
            : [.. WordList.Words];

        if (words.Count == 0)
        {
            ShowResult([], []);
            return;
        }

        Shuffle(words);

        CurrentPage = new LearnViewModel(this, words);
    }

    private static void Shuffle(List<Word> words)
    {
        for (var i = words.Count - 1; i > 0; i--)
        {
            var j = Random.Shared.Next(i + 1);
            (words[i], words[j]) = (words[j], words[i]);
        }
    }

    // Die Lernseite reicht ihre Ergebnisse hier durch an die Ergebnisseite.
    public void ShowResult(List<Word> known, List<Word> unknown)
        => CurrentPage = new ResultViewModel(this, known, unknown);
}
