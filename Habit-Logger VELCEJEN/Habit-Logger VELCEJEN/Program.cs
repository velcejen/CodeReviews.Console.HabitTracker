namespace HabitLogger;

class Program
{
    static void Main(string[] args)
    {
        using DatabaseManager databaseManager = new();
        databaseManager.CreateTable();

        // Use this option to poulate the habita table for review purposes

        //databaseManager.SeedRandomData(10);

        UserInterface userInterface = new();
        userInterface.MainMenu();
        Environment.Exit(0);
    }
}
