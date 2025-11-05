using Spectre.Console;
using System.Globalization;
using System.Text;

namespace HabitLogger;

internal class UserInterface
{
    private readonly LogViewManager _logViewManager = new();

    internal UserInterface()
    {
        Console.OutputEncoding = System.Text.Encoding.UTF8;
        Console.CursorVisible = false;
    }

    internal void MainMenu()
    {
        bool _exitHabitLogger = false;
        OptionsManager optionsManager = new OptionsManager();

        do
        {
            bool _actionSelected = false;
            string _action = "";
            GenerateUserInterface();
            optionsManager.ShowOptions();
            _logViewManager.Update();
            do
            {
                ConsoleKeyInfo keyInfo = Console.ReadKey(true);
                switch (keyInfo.Key)
                {
                    case ConsoleKey.LeftArrow:
                        optionsManager.MoveLeft();
                        break;
                    case ConsoleKey.RightArrow:
                        optionsManager.MoveRight();
                        break;
                    case ConsoleKey.UpArrow:
                        _logViewManager.MoveUp();
                        break;
                    case ConsoleKey.DownArrow:
                        _logViewManager.MoveDown();
                        break;
                    case ConsoleKey.Escape:
                        _action = "exit";
                        _actionSelected = true;
                        break;
                    case ConsoleKey.Enter:
                    case ConsoleKey.Spacebar:
                        _action = optionsManager.Accion();
                        _actionSelected = true;
                        break;
                    default:
                        break;
                }

            } while (!_actionSelected);

            switch (_action)
            {
                case "Add":
                    AddLogEntry();
                    break;
                case "Modify":
                    ModifyLogEntry(_logViewManager.CurrentRegister());
                    break;
                case "Delete":
                    DeleteLogEntry(_logViewManager.CurrentRegister());
                    _action = "";
                    break;

                case "Exit":
                    _exitHabitLogger = true;
                    break;
                default:
                    break;
            }

        } while (!_exitHabitLogger);
        ShowInfo("[yellow]   E X I T E D   L O G G E R[/]", delay: false);
        return;
    }

    private void ModifyLogEntry(HabitRecord record)
    {
        if (record.Id != "")
        {
            ShowActionTitle("Modify Register", show: true);
            HabitRecord UpDateRecord = new HabitRecord();
            UpDateRecord.Date = ReadDate(record.Date);
            UpDateRecord.Beers = ReadBeers(record.Beers);
            UpDateRecord.Location = ReadLocation(record.Location);
            int.TryParse(record.Id, out int UpdateId);
            int.TryParse(UpDateRecord.Beers, out int UpDateBeers);

            using (DatabaseManager databaseManager = new())
            {

                if (databaseManager.ModifyHabit(UpdateId, UpDateRecord.Date, UpDateBeers, UpDateRecord.Location))
                {
                    _logViewManager.Update();

                    ShowInfo($"[Turquoise4]Record Nº[/] [yellow]{record.Id}[/][Turquoise4] Modifyed corectlty[/]", delay: true);
                }
                else
                {
                    ShowInfo($"[red]Record Nº[/] [yellow]{record.Id}[/][red] NOT FOUND[/]", delay: false);

                }
            }
        }
        ShowActionTitle("Modify Register", show: false);
    }

    private void AddLogEntry()
    {
        HabitRecord record = new HabitRecord();
        ShowActionTitle("Add New Entry", show: true);
        record.Date = ReadDate();
        record.Beers = ReadBeers();
        record.Location = ReadLocation();
        int.TryParse(record.Id, out int id);
        int.TryParse(record.Beers, out int beers);
        using (DatabaseManager databaseManager = new())
        {

            if (databaseManager.AddHabit(record.Date, beers, record.Location))
            {
                _logViewManager.Update();

                ShowInfo("Record Added corectlty", delay: true);
            }
            else
            {
                ShowInfo($"[red]ERROR RECORD NOT ADDED [/]", delay: true);

            }
        }
        ShowActionTitle("Add New Entry", show: true);

    }

    private void DeleteLogEntry(HabitRecord record)
    {
        if (record.Id != "")
        {
            ShowActionTitle("Delete Entry", show: true);
            Console.SetCursorPosition(75, 12);
            AnsiConsole.Markup($"[Plum1]Id:[/] [yellow]{record.Id}[/]");
            Console.SetCursorPosition(75, 13);
            AnsiConsole.Markup($"[Plum1]Date:[/] [yellow]{record.Date}[/]");
            Console.SetCursorPosition(75, 14);
            AnsiConsole.Markup($"[Plum1]Beers:[/] [yellow]{record.Beers}[/]");
            Console.SetCursorPosition(75, 15);
            AnsiConsole.Markup($"[Plum1]Location:[/] [yellow]{record.Location}[/]");
            int.TryParse(record.Id, out int id);

            if (ShowInfo("[red]      RECORD WILL BE DELETED[/]", yes: true))
            {
                using (DatabaseManager databaseManager = new())
                {
                    if (databaseManager.DeleteHabit(id))
                    {
                        _logViewManager.Update();

                        ShowInfo($"[Turquoise4]Record Nº[/] [yellow]{record.Id}[/][Turquoise4] deleted corectlty[/]", delay: true);
                    }
                    else
                    {
                        ShowInfo($"[red]Record Nº[/] [yellow]{record.Id}[/][red] NOT FOUND[/]", true);

                    }
                }
            }
            ShowActionTitle("Delete Entry", show: true);

        }
    }

    private void ShowActionTitle(string title, bool show)
    {
        Console.SetCursorPosition(80, 10);
        if (show)
            AnsiConsole.Markup($"[Turquoise4]{title}[/]");
        else
            Console.Write(new string(' ', title.Length));
    }

    private static bool ShowInfo(string text, bool delay = false, bool yes = false)
    {
        Console.CursorVisible = false;
        Console.SetCursorPosition(73, 17);
        AnsiConsole.Markup($"[Turquoise4]{text}[/]");

        if (delay && !yes)
        {
            Thread.Sleep(2000);
        }
        else if (!delay && !yes)
        {
            Console.SetCursorPosition(73, 18);
            AnsiConsole.Markup("[Turquoise4]   press any [/][Wheat1]KEY[/] [Turquoise4]to contiune[/]");
            Console.ReadKey();
        }
        if (yes)
        {
            string answer = "";
            do
            {
                Console.SetCursorPosition(73, 18);
                AnsiConsole.Markup("[Turquoise4]Press[/] [yellow]'y'[/][Turquoise4] to confirm or[/] [yellow]'n'[/][Turquoise4] to exit[/]");
                ConsoleKeyInfo keyInfo = Console.ReadKey(true);
                answer = keyInfo.KeyChar.ToString();
                if (answer.ToLower() == "y") yes = true;
                if (answer.ToLower() == "n") yes = false;

            }
            while (answer.ToLower() != "y" && answer.ToLower() != "n");
        }
        Console.SetCursorPosition(73, 17);
        Console.Write(new string(' ', text.Length));
        Console.SetCursorPosition(73, 18);
        AnsiConsole.Markup(new string(' ', 35));
        return yes;
    }

    private static void GenerateUserInterface()
    {
        Console.Title = "Habit Logger VELCEJEN";
        Drawgrid();
        Console.SetCursorPosition(Enums.leftPos + 28, Enums.topPos);
        AnsiConsole.Markup($"[Plum1]H a b i t    L o g g e r[/]");
    }

    private static void Drawgrid()
    {
        DrawBox<Enums.Perimeter>(Enums.BoxType.normalBox);
        DrawBox<Enums.IdColumn>(Enums.BoxType.normalBox);
        DrawBox<Enums.DateColumn>(Enums.BoxType.middleBox);
        DrawBox<Enums.LocationColumn>(Enums.BoxType.normalBox);
        DrawBox<Enums.BeersColumn>(Enums.BoxType.middleBox);

        DrawBox<Enums.IdHeader>(Enums.BoxType.crossBox);
        DrawBox<Enums.DateHeader>(Enums.BoxType.middleCrossBox);
        DrawBox<Enums.LocationHeader>(Enums.BoxType.crossBox);
        DrawBox<Enums.BeersHeader>(Enums.BoxType.middleCrossBox);
        var text = new List<(string Name, int Value1, int Value2)>();

        text.Add(("Id", (int)Enums.IdHeader.top + 1, (int)Enums.IdHeader.left + 2));
        text.Add(("date", (int)Enums.DateHeader.top + 1, (int)Enums.DateHeader.left + 4));
        text.Add(("beers", (int)Enums.BeersHeader.top + 1, (int)Enums.BeersHeader.left + 1));
        text.Add(("location", (int)Enums.LocationHeader.top + 1, (int)Enums.LocationHeader.left + 6));
        foreach (var item in text)
        {
            Console.SetCursorPosition(item.Value2, item.Value1);
            AnsiConsole.Markup($"[Plum1]{item.Name}[/]");
        }
    }

    private static void DrawBox<T>(Enums.BoxType box) where T : Enum
    {
        int left = (int)Enum.Parse(typeof(T), "left");
        int top = (int)Enum.Parse(typeof(T), "top");
        int width = (int)Enum.Parse(typeof(T), "width");
        int height = (int)Enum.Parse(typeof(T), "height");
        string[] corner = { "┌", "└", "┐", "┘" };
        string[] middle = { "┬", "┴", "┬", "┴" };
        string[] cross = { "┌", "├", "┐", "┤" };
        string[] middleCross = { "┬", "┼", "┬", "┼" };
        switch (box)
        {
            case Enums.BoxType.normalBox:
                break;
            case Enums.BoxType.middleBox:
                corner = middle;
                break;
            case Enums.BoxType.crossBox:
                corner = cross;
                break;
            case Enums.BoxType.middleCrossBox:
                corner = middleCross;
                break;
        }
        // top border
        Console.SetCursorPosition(left, top);
        Console.Write($"{corner[0]}" + new string('─', width - 2) + $"{corner[2]}");

        // sides
        for (int i = 1; i < height - 1; i++)
        {
            Console.SetCursorPosition(left, top + i);
            Console.Write("│" + new string(' ', width - 2) + "│");
        }

        // bottom border
        Console.SetCursorPosition(left, top + height - 1);
        Console.Write($"{corner[1]}" + new string('─', width - 2) + $"{corner[3]}");
    }

    internal static string ReadDate(String? modifyText = null)
    {
        var input = new StringBuilder();

        Console.SetCursorPosition(75, 12);
        AnsiConsole.Markup("[Plum1]Date:[/] ");
        if (modifyText != null)
        {
            input.Append(modifyText);
            AnsiConsole.Markup($"[yellow]{modifyText}[/]");
        }
        else
        {

            input.Append(DateTime.Today.ToString("dd-MM-yyyy"));
            AnsiConsole.Markup($"[yellow]{DateTime.Today.ToString("dd-MM-yyyy")}[/]");
        }
        while (true)
        {
            Console.CursorVisible = true;
            var key = Console.ReadKey(intercept: true);
            if (key.Key == ConsoleKey.Enter)
            {
                Console.WriteLine();
                string dateText = input.ToString();

                if (DateTime.TryParseExact(dateText, "dd-MM-yyyy",
                        CultureInfo.InvariantCulture,
                        DateTimeStyles.None,
                        out _))
                {
                    Console.CursorVisible = false;
                    return dateText;
                }
                else
                {
                    ShowInfo("[red]        Invalid Date[/]", true);
                    Console.SetCursorPosition(75, 12);
                    AnsiConsole.Markup("                ");
                    Console.SetCursorPosition(75, 12);
                    AnsiConsole.Markup("[plum1]Date:[/] ");
                    input.Clear();
                    continue;
                }
            }
            if (key.Key == ConsoleKey.Backspace && input.Length > 0)
            {
                input.Remove(input.Length - 1, 1);
                Console.Write("\b \b");
                continue;
            }
            if (char.IsDigit(key.KeyChar))
            {
                if (input.Length < 10)
                {
                    input.Append(key.KeyChar);
                    AnsiConsole.Markup($"[yellow]{key.KeyChar}[/]");
                }
                if (input.Length == 3 || input.Length == 6)
                {
                    char lastChar = input[input.Length - 1];
                    if (char.IsDigit(lastChar))
                    {
                        input.Remove(input.Length - 1, 1);
                        input.Append('-' + lastChar.ToString());
                        Console.Write("\b \b");
                        AnsiConsole.Markup($"[yellow]-{lastChar.ToString()}[/]");
                    }
                }
                if (input.Length == 2 || input.Length == 5)
                {
                    input.Append('-');
                    AnsiConsole.Markup($"[yellow]-[/]");
                }
            }
        }
    }

    private static string ReadBeers(String? modifyText = null)
    {
        Console.CursorVisible = true;
        var input = new StringBuilder();
        Console.SetCursorPosition(75, 13);
        AnsiConsole.Markup("[Plum1]Beers:[/] ");
        if (modifyText != null)
        {
            input.Append(modifyText);
            AnsiConsole.Markup($"[yellow]{modifyText}[/]");
        }
        while (true)
        {
            Console.CursorVisible = true;
            var key = Console.ReadKey(intercept: true);
            if (key.Key == ConsoleKey.Enter)
            {
                Console.WriteLine();
                string beersText = input.ToString();

                if (Int32.TryParse(beersText, out int beers) && beers > 0)
                {

                    Console.CursorVisible = false;
                    return beersText;
                }
                else
                {
                    ShowInfo("[red]        Invalid Number.[/]", true);
                    Console.SetCursorPosition(75, 13);
                    AnsiConsole.Markup("                ");
                    Console.SetCursorPosition(75, 13);
                    AnsiConsole.Markup("[plum1]Beers: [/]");
                    input.Clear();
                    continue;
                }
            }
            if (key.Key == ConsoleKey.Backspace && input.Length > 0)
            {
                input.Remove(input.Length - 1, 1);
                Console.Write("\b \b");
                continue;
            }
            if (char.IsDigit(key.KeyChar))
            {
                if (input.Length < 3)
                {
                    input.Append(key.KeyChar);
                    AnsiConsole.Markup($"[yellow]{key.KeyChar}[/]");
                }
            }
        }
    }

    private static string ReadLocation(String? modifyText = null)
    {
        var input = new StringBuilder();
        Console.CursorVisible = true;
        Console.SetCursorPosition(75, 14);
        AnsiConsole.Markup("[Plum1]Location:[/] ");
        if (modifyText != null)
        {
            input.Append(modifyText);
            AnsiConsole.Markup($"[yellow]{modifyText}[/]");
        }
        while (true)
        {
            Console.CursorVisible = true;
            var key = Console.ReadKey(intercept: true);
            if (key.Key == ConsoleKey.Enter && input.ToString().Trim().Length > 0)
            {

                Console.WriteLine();
                string locationText = input.ToString().Trim();
                Console.CursorVisible = false;
                return locationText;
            }
            else if (key.Key == ConsoleKey.Enter)
            {
                input.Clear();
                ShowInfo("[red]      Invalid Entry[/]", delay: true);
                Console.SetCursorPosition(75, 14);
                AnsiConsole.Markup("[Plum1]Location:[/] ");
            }
            if (key.Key == ConsoleKey.Backspace && input.Length > 0)
            {
                input.Remove(input.Length - 1, 1);
                Console.Write("\b \b");
                continue;
            }

            if (input.Length < 18 && !char.IsControl(key.KeyChar))
            {
                char character = key.KeyChar;
                if (char.IsLetter(character) || char.IsPunctuation(character) || char.IsWhiteSpace(character))
                {
                    input.Append(character);
                    AnsiConsole.Markup($"[yellow]{Markup.Escape(character.ToString())}[/]");
                }
            }
        }
    }
}