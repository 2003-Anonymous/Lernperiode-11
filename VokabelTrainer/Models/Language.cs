using CommunityToolkit.Mvvm.ComponentModel;

namespace VokabelTrainer.Models;

public partial class Language : ObservableObject
{
    public int Id { get; set; }

    [ObservableProperty]
    public partial string Name { get; set; }

    public Language(string name)
    {
        Name = name;
    }

    public Language(int id, string name)
        : this(name)
    {
        Id = id;
    }

    public override string ToString() => Name;
}
