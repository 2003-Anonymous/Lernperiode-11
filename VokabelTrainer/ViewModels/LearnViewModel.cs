using System.Collections.Generic;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using VokabelTrainer.Models;

namespace VokabelTrainer.ViewModels;

public partial class LearnViewModel : ViewModelBase
{
    private readonly MainViewModel _main;

    private readonly List<Word> _words;

    private int _index;

    [ObservableProperty]
    public partial string Question { get; set; } = "";

    [ObservableProperty]
    public partial string Answer { get; set; } = "";

    [ObservableProperty]
    public partial bool AnswerVisible { get; set; }

    [ObservableProperty]
    public partial string ProgressText { get; set; } = "";

    public List<Word> KnownWords { get; } = [];
    public List<Word> UnknownWords { get; } = [];

    public LearnViewModel(MainViewModel main, List<Word> words)
    {
        _main = main;
        _words = words;
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
        ProgressText = $"{_index + 1} von {_words.Count}";
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
        var word = _words[_index];
        WordList.SetKnown(word, true);
        KnownWords.Add(word);
        Next();
    }

    [RelayCommand]
    private void NotKnown()
    {
        var word = _words[_index];
        WordList.SetKnown(word, false);
        UnknownWords.Add(word);
        Next();
    }
}
