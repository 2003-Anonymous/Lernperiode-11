using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;

namespace VokabelTrainer.ViewModels;

public partial class MainViewModel : ViewModelBase
{
    [ObservableProperty]
    public partial string Greeting { get; set; } = "Welcome to Avalonia!";

    public ObservableCollection<string> Woerter { get; } =
    [
        "Apfel: jablko",
        "Brot: chleb",
        "Katze: kot",
        "Tiger: tygrys",
        "Bieber: bober"
    ];
}
