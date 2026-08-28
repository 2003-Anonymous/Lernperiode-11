using System.Collections.ObjectModel;
using VokabelTrainer.Services;

namespace VokabelTrainer.Models;

public static class WordList
{
    public static ObservableCollection<Word> Words { get; } = [];

    public static void Load()
    {
        WordDatabase.Initialize();

        Words.Clear();
        foreach (var word in WordDatabase.LoadAll())
        {
            Words.Add(word);
        }
    }

    public static Word Add(string german, string foreignLanguage)
    {
        var id = WordDatabase.Insert(german, foreignLanguage);
        var word = new Word(id, german, foreignLanguage);
        Words.Add(word);
        return word;
    }

    public static void Update(Word word, string german, string foreignLanguage)
    {
        word.German = german;
        word.ForeignLanguage = foreignLanguage;
        WordDatabase.Update(word);
    }

    public static void Remove(Word word)
    {
        WordDatabase.Delete(word);
        Words.Remove(word);
    }
}
