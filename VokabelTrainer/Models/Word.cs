using CommunityToolkit.Mvvm.ComponentModel;

namespace VokabelTrainer.Models;

public partial class Word : ObservableObject
{
    public int Id { get; set; }

    [ObservableProperty]
    public partial string German { get; set; }

    [ObservableProperty]
    public partial string ForeignLanguage { get; set; }

    [ObservableProperty]
    public partial bool IsKnown { get; set; }

    public Word(string german, string foreignLanguage)
    {
        German = german;
        ForeignLanguage = foreignLanguage;
    }

    public Word(int id, string german, string foreignLanguage, bool isKnown)
        : this(german, foreignLanguage)
    {
        Id = id;
        IsKnown = isKnown;
    }
}
