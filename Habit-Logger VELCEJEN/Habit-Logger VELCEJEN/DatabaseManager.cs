using Microsoft.Data.Sqlite;
using System.Configuration;

namespace HabitLogger.VELCEJEN;

internal class DatabaseManager : IDisposable
{
    private readonly string _connectionString;
    private SqliteConnection _connection;

    internal DatabaseManager()
    {
        _connectionString = ConfigurationManager.AppSettings.Get("ConnectionString")
        ?? throw new InvalidOperationException("ConnectionString not found in App.config");
        _connection = new SqliteConnection(_connectionString);
    }

    private void OpenConnection()
    {
        if (_connection.State != System.Data.ConnectionState.Open)
        {
            _connection.Open();
        }
    }

    private void CloseConnection()
    {
        if (_connection.State != System.Data.ConnectionState.Closed)
        {
            _connection.Close();
        }
    }

    internal void CreateTable()
    {
        OpenConnection();
        using (var tableCmd = _connection.CreateCommand())
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
        CloseConnection();
    }

    internal bool AddHabit(string date, int beers, string location)
    {
        bool succeded = false;
        OpenConnection();
        using (var addCmd = _connection.CreateCommand())
        {
            addCmd.CommandText =
                @"INSERT INTO Habits (Date, Beers,Location) 
                VALUES ($date, $beers,$location)";
            addCmd.Parameters.AddWithValue("$date", date);
            addCmd.Parameters.AddWithValue("$beers", beers);
            addCmd.Parameters.AddWithValue("$location", location);
            int rowsAffected = addCmd.ExecuteNonQuery();

            if (rowsAffected == 0) succeded = false;
            else succeded = true;
        }
        CloseConnection();
        return succeded;
    }

    internal bool ModifyHabit(int id, string date, int beers, string location)
    {
        bool succeded = false;
        OpenConnection();
        using (var updateCmd = _connection.CreateCommand())
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

            if (rowsAffected == 0) succeded = false;
            else succeded = true;
        }
        CloseConnection();
        return succeded;
    }

    internal bool DeleteHabit(int id)
    {
        OpenConnection();
        bool succeded = false;
        using (var deleteCmd = _connection.CreateCommand())
        {
            deleteCmd.CommandText = @"DELETE FROM Habits WHERE Id = $id";
            deleteCmd.Parameters.AddWithValue("$id", id);

            int rowsAffected = deleteCmd.ExecuteNonQuery();

            if (rowsAffected == 0) succeded = false;
            else succeded = true;
        }
        CloseConnection();
        return succeded;
    }

    internal List<HabitRecord> GetAllHabits()
    {
        OpenConnection();
        var habits = new List<HabitRecord>();
        using (var selectCmd = _connection.CreateCommand())
        {
            selectCmd.CommandText = "SELECT Id, Date, Beers, Location FROM Habits";
            using (var reader = selectCmd.ExecuteReader())
            {
                while (reader.Read())
                {
                    var habit = new HabitRecord
                    {
                        id = reader.GetInt32(0).ToString(),
                        date = reader.GetString(1),
                        beers = reader.GetInt32(2).ToString(),
                        location = reader.GetString(3)

                    };
                    habits.Add(habit);
                }
            }
        }
        CloseConnection();
        return habits;
    }
   
    public void Dispose()
    {
        _connection?.Close();
        _connection?.Dispose();

    }

    internal void SeedRandomData(int count)
    {
        var rnd = new Random();
        OpenConnection();
        string[] locations = { "Madrid", "Barcelona", "Valencia", "Seville", "Bilbao", "Granada", "Toledo","Benetusser","Sedavi" };

        for (int i = 0; i < count; i++)
        {
            string date = DateTime.Now
                .AddDays(-rnd.Next(0, 365))
                .ToString("dd-MM-yyyy");

            int beers = rnd.Next(0, 10); // 0–9 beers
            string location = locations[rnd.Next(locations.Length)];

            using (var insertCmd = _connection.CreateCommand())
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
        CloseConnection();
    }
}

