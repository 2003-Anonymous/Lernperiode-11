using CommunityToolkit.Mvvm.ComponentModel;

namespace VokabelTrainer.Models;

public partial class Collection : ObservableObject
{
    public int Id { get; set; }
    public int LanguageId { get; set; }

    [ObservableProperty]
    public partial string Name { get; set; }

    public Collection(int id, int languageId, string name)
    { 
        Id = id;
        LanguageId = languageId;
        Name = name;
    }
}