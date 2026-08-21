using System.Collections.Generic;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using VokabelTrainer.Models;

namespace VokabelTrainer.ViewModels;

public partial class LearnViewModel : ViewModelBase
{
    private readonly MainViewModel _main;

    private readonly List<Word> _words =
    [
        new Word("Apfel", "jablko"),
        new Word("Brot",  "chleb"),
        new Word("Katze", "kot"),
        new Word("Tiger", "tygrys"),
        new Word("Biber", "bober"),
    ];

    private int _index;

    [ObservableProperty]
    public partial string Question { get; set; } = "";

    [ObservableProperty]
    public partial string Answer { get; set; } = "";

    [ObservableProperty]
    public partial bool AnswerVisible { get; set; }

    public List<Word> KnownWords { get; } = [];
    public List<Word> UnknownWords { get; } = [];

    public LearnViewModel(MainViewModel main)
    {
        _main = main;
        ShowWord();
    }

    private void ShowWord()
    {
        if (_index >= _words.Count)
        {
            // Liste durch -> Ergebnisse weiterreichen und Ergebnisseite zeigen
            _main.ShowResult(KnownWords, UnknownWords);
            return;
        }

        Question = _words[_index].German;
        Answer = _words[_index].ForeignLanguage;
        AnswerVisible = false;
    }

    [RelayCommand]
    private void ShowAnswer() => AnswerVisible = true;

    [RelayCommand]
    private void Next()
    {
        _index++;
        ShowWord();
    }

    [RelayCommand]
    private void Known()
    {
        KnownWords.Add(_words[_index]);
        Next();
    }

    [RelayCommand]
    private void NotKnown()
    {
        UnknownWords.Add(_words[_index]);
        Next();
    }
}
