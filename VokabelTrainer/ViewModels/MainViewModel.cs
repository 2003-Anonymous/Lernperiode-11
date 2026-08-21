using System.Collections.Generic;
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
    public void ShowLearn() => CurrentPage = new LearnViewModel(this);

    // Die Lernseite reicht ihre Ergebnisse hier durch an die Ergebnisseite.
    public void ShowResult(List<Word> known, List<Word> unknown)
        => CurrentPage = new ResultViewModel(this, known, unknown);
}
