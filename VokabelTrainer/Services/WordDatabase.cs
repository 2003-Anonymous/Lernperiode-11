using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.Data.Sqlite;
using VokabelTrainer.Models;

namespace VokabelTrainer.Services;

public static class WordDatabase
{
    private const string DefaultLanguageName = "Polnisch";

    private static readonly string ConnectionString = BuildConnectionString();

    private static string BuildConnectionString()
    {
        var folder = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
            "VokabelTrainer");

        Directory.CreateDirectory(folder);

        return new SqliteConnectionStringBuilder
        {
            DataSource = Path.Combine(folder, "vokabeln.db")
        }.ToString();
    }

    private static SqliteConnection OpenConnection()
    {
        var connection = new SqliteConnection(ConnectionString);
        connection.Open();
        return connection;
    }

    public static void Initialize()
    {
        using var connection = OpenConnection();

        using var create = connection.CreateCommand();
        create.CommandText =
            """
            CREATE TABLE IF NOT EXISTS Languages (
                Id   INTEGER PRIMARY KEY AUTOINCREMENT,
                Name TEXT NOT NULL
            );

            CREATE TABLE IF NOT EXISTS Words (
                Id              INTEGER PRIMARY KEY AUTOINCREMENT,
                German          TEXT NOT NULL,
                ForeignLanguage TEXT NOT NULL,
                IsKnown         INTEGER NOT NULL DEFAULT 0,
                LanguageId      INTEGER NOT NULL DEFAULT 1
            );

            CREATE TABLE IF NOT EXISTS Settings (
                Key   TEXT PRIMARY KEY,
                Value TEXT NOT NULL
            );
            """;
        create.ExecuteNonQuery();

        AddColumnIfMissing(connection, "Words", "IsKnown", "INTEGER NOT NULL DEFAULT 0");
        AddColumnIfMissing(connection, "Words", "LanguageId", "INTEGER NOT NULL DEFAULT 1");

        var defaultLanguageId = EnsureDefaultLanguage(connection);

        using var orphans = connection.CreateCommand();
        orphans.CommandText =
            "UPDATE Words SET LanguageId = $id WHERE LanguageId NOT IN (SELECT Id FROM Languages);";
        orphans.Parameters.AddWithValue("$id", defaultLanguageId);
        orphans.ExecuteNonQuery();
    }

    private static void AddColumnIfMissing(SqliteConnection connection, string table, string column, string definition)
    {
        using var columns = connection.CreateCommand();
        columns.CommandText = $"SELECT COUNT(*) FROM pragma_table_info('{table}') WHERE name = $column;";
        columns.Parameters.AddWithValue("$column", column);

        if (Convert.ToInt64(columns.ExecuteScalar()) > 0)
        {
            return;
        }

        using var alter = connection.CreateCommand();
        alter.CommandText = $"ALTER TABLE {table} ADD COLUMN {column} {definition};";
        alter.ExecuteNonQuery();
    }

    private static int EnsureDefaultLanguage(SqliteConnection connection)
    {
        using var existing = connection.CreateCommand();
        existing.CommandText = "SELECT Id FROM Languages ORDER BY Id LIMIT 1;";

        var found = existing.ExecuteScalar();

        if (found is not null and not DBNull)
        {
            return Convert.ToInt32(found);
        }

        var languageId = InsertLanguage(connection, DefaultLanguageName);
        SeedDefaultWords(connection, languageId);
        return languageId;
    }

    private static int InsertLanguage(SqliteConnection connection, string name)
    {
        using var insert = connection.CreateCommand();
        insert.CommandText = "INSERT INTO Languages (Name) VALUES ($name); SELECT last_insert_rowid();";
        insert.Parameters.AddWithValue("$name", name);
        return Convert.ToInt32(insert.ExecuteScalar());
    }

    private static void SeedDefaultWords(SqliteConnection connection, int languageId)
    {
        using var count = connection.CreateCommand();
        count.CommandText = "SELECT COUNT(*) FROM Words;";

        if (Convert.ToInt64(count.ExecuteScalar()) > 0)
        {
            return;
        }

        var defaults = new[]
        {
            ("Apfel", "jablko"),
            ("Brot",  "chleb"),
            ("Katze", "kot"),
            ("Tiger", "tygrys"),
            ("Biber", "bober"),
        };

        using var transaction = connection.BeginTransaction();

        foreach (var (german, foreignLanguage) in defaults)
        {
            using var insert = connection.CreateCommand();
            insert.Transaction = transaction;
            insert.CommandText =
                "INSERT INTO Words (German, ForeignLanguage, LanguageId) VALUES ($german, $foreign, $language);";
            insert.Parameters.AddWithValue("$german", german);
            insert.Parameters.AddWithValue("$foreign", foreignLanguage);
            insert.Parameters.AddWithValue("$language", languageId);
            insert.ExecuteNonQuery();
        }

        transaction.Commit();
    }

    public static List<Language> LoadLanguages()
    {
        using var connection = OpenConnection();

        using var select = connection.CreateCommand();
        select.CommandText = "SELECT Id, Name FROM Languages ORDER BY Name;";

        var languages = new List<Language>();

        using var reader = select.ExecuteReader();
        while (reader.Read())
        {
            languages.Add(new Language(reader.GetInt32(0), reader.GetString(1)));
        }

        return languages;
    }

    public static int InsertLanguage(string name)
    {
        using var connection = OpenConnection();
        return InsertLanguage(connection, name);
    }

    public static void UpdateLanguage(Language language)
    {
        using var connection = OpenConnection();

        using var update = connection.CreateCommand();
        update.CommandText = "UPDATE Languages SET Name = $name WHERE Id = $id;";
        update.Parameters.AddWithValue("$name", language.Name);
        update.Parameters.AddWithValue("$id", language.Id);
        update.ExecuteNonQuery();
    }

    public static void DeleteLanguage(Language language)
    {
        using var connection = OpenConnection();
        using var transaction = connection.BeginTransaction();

        using var deleteWords = connection.CreateCommand();
        deleteWords.Transaction = transaction;
        deleteWords.CommandText = "DELETE FROM Words WHERE LanguageId = $id;";
        deleteWords.Parameters.AddWithValue("$id", language.Id);
        deleteWords.ExecuteNonQuery();

        using var deleteLanguage = connection.CreateCommand();
        deleteLanguage.Transaction = transaction;
        deleteLanguage.CommandText = "DELETE FROM Languages WHERE Id = $id;";
        deleteLanguage.Parameters.AddWithValue("$id", language.Id);
        deleteLanguage.ExecuteNonQuery();

        transaction.Commit();
    }

    public static List<Word> LoadWords(int languageId)
    {
        using var connection = OpenConnection();

        using var select = connection.CreateCommand();
        select.CommandText =
            "SELECT Id, German, ForeignLanguage, IsKnown FROM Words WHERE LanguageId = $language ORDER BY Id;";
        select.Parameters.AddWithValue("$language", languageId);

        var words = new List<Word>();

        using var reader = select.ExecuteReader();
        while (reader.Read())
        {
            words.Add(new Word(
                reader.GetInt32(0),
                reader.GetString(1),
                reader.GetString(2),
                reader.GetBoolean(3)));
        }

        return words;
    }

    public static int CountWords(int languageId)
    {
        using var connection = OpenConnection();

        using var count = connection.CreateCommand();
        count.CommandText = "SELECT COUNT(*) FROM Words WHERE LanguageId = $language;";
        count.Parameters.AddWithValue("$language", languageId);

        return Convert.ToInt32(count.ExecuteScalar());
    }

    public static int Insert(string german, string foreignLanguage, int languageId)
    {
        using var connection = OpenConnection();

        using var insert = connection.CreateCommand();
        insert.CommandText =
            """
            INSERT INTO Words (German, ForeignLanguage, IsKnown, LanguageId)
            VALUES ($german, $foreign, 0, $language);
            SELECT last_insert_rowid();
            """;
        insert.Parameters.AddWithValue("$german", german);
        insert.Parameters.AddWithValue("$foreign", foreignLanguage);
        insert.Parameters.AddWithValue("$language", languageId);

        return Convert.ToInt32(insert.ExecuteScalar());
    }

    public static void Update(Word word)
    {
        using var connection = OpenConnection();

        using var update = connection.CreateCommand();
        update.CommandText =
            "UPDATE Words SET German = $german, ForeignLanguage = $foreign, IsKnown = $known WHERE Id = $id;";
        update.Parameters.AddWithValue("$german", word.German);
        update.Parameters.AddWithValue("$foreign", word.ForeignLanguage);
        update.Parameters.AddWithValue("$known", word.IsKnown);
        update.Parameters.AddWithValue("$id", word.Id);
        update.ExecuteNonQuery();
    }

    public static void UpdateIsKnown(Word word)
    {
        using var connection = OpenConnection();

        using var update = connection.CreateCommand();
        update.CommandText = "UPDATE Words SET IsKnown = $known WHERE Id = $id;";
        update.Parameters.AddWithValue("$known", word.IsKnown);
        update.Parameters.AddWithValue("$id", word.Id);
        update.ExecuteNonQuery();
    }

    public static void Delete(Word word)
    {
        using var connection = OpenConnection();

        using var delete = connection.CreateCommand();
        delete.CommandText = "DELETE FROM Words WHERE Id = $id;";
        delete.Parameters.AddWithValue("$id", word.Id);
        delete.ExecuteNonQuery();
    }

    public static int? LoadSelectedLanguageId()
    {
        using var connection = OpenConnection();

        using var select = connection.CreateCommand();
        select.CommandText = "SELECT Value FROM Settings WHERE Key = 'SelectedLanguageId';";

        return select.ExecuteScalar() is string value && int.TryParse(value, out var id) ? id : null;
    }

    public static void SaveSelectedLanguageId(int languageId)
    {
        using var connection = OpenConnection();

        using var save = connection.CreateCommand();
        save.CommandText =
            """
            INSERT INTO Settings (Key, Value) VALUES ('SelectedLanguageId', $value)
            ON CONFLICT(Key) DO UPDATE SET Value = $value;
            """;
        save.Parameters.AddWithValue("$value", languageId.ToString());
        save.ExecuteNonQuery();
    }
}
