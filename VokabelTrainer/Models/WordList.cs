using System.Collections.ObjectModel;
using System.Linq;
using VokabelTrainer.Services;

namespace VokabelTrainer.Models;

public static class WordList
{
    public static ObservableCollection<Language> Languages { get; } = [];

    public static ObservableCollection<Word> Words { get; } = [];

    public static Language? CurrentLanguage { get; private set; }

    public static int KnownCount => Words.Count(word => word.IsKnown);

    public static int UnknownCount => Words.Count(word => !word.IsKnown);

    public static void Load()
    {
        WordDatabase.Initialize();
        LoadLanguages();

        var savedId = WordDatabase.LoadSelectedLanguageId();
        var language = Languages.FirstOrDefault(item => item.Id == savedId) ?? Languages.FirstOrDefault();

        SelectLanguage(language);
    }

    private static void LoadLanguages()
    {
        Languages.Clear();
        foreach (var language in WordDatabase.LoadLanguages())
        {
            Languages.Add(language);
        }
    }

    public static void SelectLanguage(Language? language)
    {
        CurrentLanguage = language;

        Words.Clear();

        if (language is null)
        {
            return;
        }

        WordDatabase.SaveSelectedLanguageId(language.Id);

        foreach (var word in WordDatabase.LoadWords(language.Id))
        {
            Words.Add(word);
        }
    }

    public static Language AddLanguage(string name)
    {
        var id = WordDatabase.InsertLanguage(name);
        var language = new Language(id, name);

        LoadLanguages();

        return Languages.First(item => item.Id == language.Id);
    }

    public static void RenameLanguage(Language language, string name)
    {
        language.Name = name;
        WordDatabase.UpdateLanguage(language);
    }

    public static void RemoveLanguage(Language language)
    {
        WordDatabase.DeleteLanguage(language);
        LoadLanguages();

        if (CurrentLanguage?.Id == language.Id)
        {
            SelectLanguage(Languages.FirstOrDefault());
        }
    }

    public static int CountWords(Language language) => WordDatabase.CountWords(language.Id);

    public static Word Add(string german, string foreignLanguage)
    {
        var id = WordDatabase.Insert(german, foreignLanguage, CurrentLanguage!.Id);
        var word = new Word(id, german, foreignLanguage, false);
        Words.Add(word);
        return word;
    }

    public static void Update(Word word, string german, string foreignLanguage)
    {
        word.German = german;
        word.ForeignLanguage = foreignLanguage;
        WordDatabase.Update(word);
    }

    public static void SetKnown(Word word, bool isKnown)
    {
        word.IsKnown = isKnown;
        WordDatabase.UpdateIsKnown(word);
    }

    public static void Remove(Word word)
    {
        WordDatabase.Delete(word);
        Words.Remove(word);
    }
}
