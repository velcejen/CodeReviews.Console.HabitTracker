using Microsoft.Data.Sqlite;
using System.Configuration;

namespace HabitLogger;

internal class DatabaseManager : IDisposable
{
    private readonly string _connectionString;

    internal DatabaseManager()
    {
        _connectionString = ConfigurationManager.AppSettings.Get("ConnectionString")
        ?? throw new InvalidOperationException("ConnectionString not found in App.config");
    }

    internal void CreateTable()
    {
        using var connection = new SqliteConnection(_connectionString);
        connection.Open();
        using (var tableCmd = connection.CreateCommand())
        {
            tableCmd.CommandText =
                @"CREATE TABLE IF NOT EXISTS Habits (
                            Id INTEGER PRIMARY KEY AUTOINCREMENT,
                            Date TEXT,
                            Beers INTEGER,
                            location TEXT
                         )";
            tableCmd.ExecuteNonQuery();
        }
    }

    internal bool AddHabit(string date, int beers, string location)
    {
        bool succeeded = false;
        using var connection = new SqliteConnection(_connectionString);
        connection.Open();
        using (var addCmd = connection.CreateCommand())
        {
            addCmd.CommandText =
                @"INSERT INTO Habits (Date, Beers,Location) 
                VALUES ($date, $beers,$location)";
            addCmd.Parameters.AddWithValue("$date", date);
            addCmd.Parameters.AddWithValue("$beers", beers);
            addCmd.Parameters.AddWithValue("$location", location);
            int rowsAffected = addCmd.ExecuteNonQuery();

            if (rowsAffected == 0) succeeded = false;
            else succeeded = true;
        }
        return succeeded;
    }

    internal bool ModifyHabit(int id, string date, int beers, string location)
    {
        bool succeeded = false;

        using var connection = new SqliteConnection(_connectionString);
        connection.Open();
        using (var updateCmd = connection.CreateCommand())
        {
            updateCmd.CommandText =
                 @"UPDATE Habits
                 SET Date = $date, Beers = $beers, Location = $location
                 WHERE Id = $id";
            updateCmd.Parameters.AddWithValue("$date", date);
            updateCmd.Parameters.AddWithValue("$beers", beers);
            updateCmd.Parameters.AddWithValue("$location", location);
            updateCmd.Parameters.AddWithValue("$id", id);
            int rowsAffected = updateCmd.ExecuteNonQuery();

            if (rowsAffected == 0) succeeded = false;
            else succeeded = true;
        }
        return succeeded;
    }

    internal bool DeleteHabit(int id)
    {
        using var connection = new SqliteConnection(_connectionString);
        connection.Open();
        bool succeeded = false;
        using (var deleteCmd = connection.CreateCommand())
        {
            deleteCmd.CommandText = @"DELETE FROM Habits WHERE Id = $id";
            deleteCmd.Parameters.AddWithValue("$id", id);

            int rowsAffected = deleteCmd.ExecuteNonQuery();

            if (rowsAffected == 0) succeeded = false;
            else succeeded = true;
        }
        return succeeded;
    }

    internal List<HabitRecord> GetAllHabits()
    {
        using var connection = new SqliteConnection(_connectionString);
        connection.Open();
        var habits = new List<HabitRecord>();
        using (var selectCmd = connection.CreateCommand())
        {
            selectCmd.CommandText = "SELECT Id, Date, Beers, Location FROM Habits";
            using (var reader = selectCmd.ExecuteReader())
            {
                while (reader.Read())
                {
                    var habit = new HabitRecord
                    {
                        Id = reader.GetInt32(0).ToString(),
                        Date = reader.GetString(1),
                        Beers = reader.GetInt32(2).ToString(),
                        Location = reader.GetString(3)

                    };
                    habits.Add(habit);
                }
            }
        }
        return habits;
    }

    public void Dispose()
    {
        using var connection = new SqliteConnection(_connectionString);

        connection?.Close();
        connection?.Dispose();

    }

    internal void SeedRandomData(int count)
    {
        var rnd = new Random();
        using var connection = new SqliteConnection(_connectionString);
        connection.Open();
        string[] locations = { "Madrid", "Barcelona", "Valencia", "Seville", "Bilbao", "Granada", "Toledo", "Benetusser", "Sedavi" };

        for (int i = 0; i < count; i++)
        {
            string date = DateTime.Now
                .AddDays(-rnd.Next(0, 365))
                .ToString("dd-MM-yyyy");

            int beers = rnd.Next(0, 10); // 0–9 beers
            string location = locations[rnd.Next(locations.Length)];

            using (var insertCmd = connection.CreateCommand())
            {
                insertCmd.CommandText =
                    @"INSERT INTO Habits (Date, Beers, Location)
                  VALUES (@date, @beers, @location)";

                insertCmd.Parameters.AddWithValue("@date", date);
                insertCmd.Parameters.AddWithValue("@beers", beers);
                insertCmd.Parameters.AddWithValue("@location", location);

                insertCmd.ExecuteNonQuery();
            }
        }
    }


}

