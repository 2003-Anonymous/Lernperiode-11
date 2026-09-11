using System.Collections.ObjectModel;
using System.Linq;
using VokabelTrainer.Services;

namespace VokabelTrainer.Models;

public static class WordList
{
    public static ObservableCollection<Language> Languages { get; } = [];
    public static ObservableCollection<Collection> Collections { get; } = [];
    public static Collection? CurrentCollection { get; private set; }
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

    private static void ReloadWords()
    {
        Words.Clear();

        if (CurrentLanguage is null)
        {
            return;
        }

        foreach (var word in WordDatabase.LoadWords(CurrentLanguage.Id, CurrentCollection?.Id))
        {
            Words.Add(word);
        }
    }

    public static void SelectLanguage(Language? language)
    {
        CurrentLanguage = language;
        CurrentCollection = null;

        Collections.Clear();
        Words.Clear();

        if (language is null)
        {
            return;
        }

        WordDatabase.SaveSelectedLanguageId(language.Id);

        Collections.Add(new Collection(0, language.Id, "Alle Wörter"));

        foreach (var collection in WordDatabase.LoadCollections(language.Id))
        {
            Collections.Add(collection);
        }

        ReloadWords();
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

    public static void SelectCollection(Collection? collection)
    {
        CurrentCollection = collection is null || collection.Id == 0 ? null : collection;
        ReloadWords();
    }

    public static Collection AddCollection(string name)
    {
        var id = WordDatabase.InsertCollection(name, CurrentLanguage!.Id);
        var collection = new Collection(id, CurrentLanguage.Id, name);
        Collections.Add(collection);
        return collection;
    }

    public static void RenameCollection(Collection collection, string name)
    {
        collection.Name = name;
        WordDatabase.UpdateCollection(collection);
    }

    public static void RemoveCollection(Collection collection)
    {
        WordDatabase.DeleteCollection(collection);
        Collections.Remove(collection);

        foreach (var word in Words.Where(word => word.CollectionId == collection.Id))
        {
            word.CollectionId = null;
        }

        if (CurrentCollection?.Id == collection.Id)
        {
            SelectCollection(null);
        }
    }

    public static int CountWords(Language language) => WordDatabase.CountWords(language.Id);

    public static int CountWords(Collection collection) => CurrentLanguage is null
        ? 0
        : WordDatabase.CountWords(CurrentLanguage.Id, collection.Id == 0 ? null : collection.Id);

    public static Word Add(string german, string foreignLanguage, int? collectionId)
    {
        var id = WordDatabase.Insert(german, foreignLanguage, CurrentLanguage!.Id, collectionId);
        var word = new Word(id, german, foreignLanguage, false) { CollectionId = collectionId };

        if (CurrentCollection is null || CurrentCollection.Id == collectionId)
        {
            Words.Add(word);
        }

        return word;
    }

    public static void Update(Word word, string german, string foreignLanguage, int? collectionId)
    {
        word.German = german;
        word.ForeignLanguage = foreignLanguage;
        word.CollectionId = collectionId;
        WordDatabase.Update(word);

        if (CurrentCollection is not null && CurrentCollection.Id != collectionId)
        {
            Words.Remove(word);
        }
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
