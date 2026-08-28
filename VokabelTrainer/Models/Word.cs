using CommunityToolkit.Mvvm.ComponentModel;

namespace VokabelTrainer.Models;

public partial class Word : ObservableObject
{
    public int Id { get; set; }

    [ObservableProperty]
    public partial string German { get; set; }

    [ObservableProperty]
    public partial string ForeignLanguage { get; set; }

    public Word(string german, string foreignLanguage)
    {
        German = german;
        ForeignLanguage = foreignLanguage;
    }

    public Word(int id, string german, string foreignLanguage)
        : this(german, foreignLanguage)
    {
        Id = id;
    }
}
