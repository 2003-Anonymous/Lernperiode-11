using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.Data.Sqlite;
using VokabelTrainer.Models;

namespace VokabelTrainer.Services;

public static class WordDatabase
{
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
            CREATE TABLE IF NOT EXISTS Words (
                Id              INTEGER PRIMARY KEY AUTOINCREMENT,
                German          TEXT NOT NULL,
                ForeignLanguage TEXT NOT NULL,
                IsKnown         INTEGER NOT NULL DEFAULT 0
            );
            """;
        create.ExecuteNonQuery();

        AddIsKnownColumnIfMissing(connection);

        using var count = connection.CreateCommand();
        count.CommandText = "SELECT COUNT(*) FROM Words;";

        if (Convert.ToInt64(count.ExecuteScalar()) == 0)
        {
            SeedDefaultWords(connection);
        }
    }

    private static void AddIsKnownColumnIfMissing(SqliteConnection connection)
    {
        using var columns = connection.CreateCommand();
        columns.CommandText = "SELECT COUNT(*) FROM pragma_table_info('Words') WHERE name = 'IsKnown';";

        if (Convert.ToInt64(columns.ExecuteScalar()) > 0)
        {
            return;
        }

        using var alter = connection.CreateCommand();
        alter.CommandText = "ALTER TABLE Words ADD COLUMN IsKnown INTEGER NOT NULL DEFAULT 0;";
        alter.ExecuteNonQuery();
    }

    private static void SeedDefaultWords(SqliteConnection connection)
    {
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
            insert.CommandText = "INSERT INTO Words (German, ForeignLanguage) VALUES ($german, $foreign);";
            insert.Parameters.AddWithValue("$german", german);
            insert.Parameters.AddWithValue("$foreign", foreignLanguage);
            insert.ExecuteNonQuery();
        }

        transaction.Commit();
    }

    public static List<Word> LoadAll()
    {
        using var connection = OpenConnection();

        using var select = connection.CreateCommand();
        select.CommandText = "SELECT Id, German, ForeignLanguage, IsKnown FROM Words ORDER BY Id;";

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

    public static int Insert(string german, string foreignLanguage)
    {
        using var connection = OpenConnection();

        using var insert = connection.CreateCommand();
        insert.CommandText =
            "INSERT INTO Words (German, ForeignLanguage, IsKnown) VALUES ($german, $foreign, 0); SELECT last_insert_rowid();";
        insert.Parameters.AddWithValue("$german", german);
        insert.Parameters.AddWithValue("$foreign", foreignLanguage);

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
}
