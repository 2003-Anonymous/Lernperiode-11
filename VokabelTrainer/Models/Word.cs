namespace VokabelTrainer.Models;

public class Word
{
    public string German { get; set; } = "";
    public string ForeignLanguage { get; set; } = "";

    public Word(string german, string foreignLanguage)
    {
        German = german;
        ForeignLanguage = foreignLanguage;
    }
}