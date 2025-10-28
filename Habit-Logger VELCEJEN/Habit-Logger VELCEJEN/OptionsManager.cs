using Spectre.Console;

namespace HabitLogger.VELCEJEN;

internal class OptionsManager
{
    private int _selectedOption;

    private readonly List<(string name, int leftPos, int topPos)> menuOption = new()
    {
        ("Add",(int)Enums.IdColumn.left+5, (int)Enums.IdColumn.height + 8 ),
        ("Modify",(int)Enums.IdColumn.left+12, (int)Enums.IdColumn.height + 8),
        ("Delete",(int)Enums.IdColumn.left+22,(int)Enums.IdColumn.height + 8),
        ("Exit",(int)Enums.IdColumn.left+32, (int)Enums.IdColumn.height + 8)
    };
    internal OptionsManager()
    {
        _selectedOption = 0;
        ShowOptions();
    }

    internal void ShowOptions()
    {
        for (int option = 0; option < menuOption.Count; option++)
        {
            if (option == _selectedOption)
            {
                Console.SetCursorPosition(menuOption[option].leftPos, menuOption[option].topPos);
                AnsiConsole.MarkupLine($"[bold yellow]{menuOption[option].name}[/]");
            }
            else
            {
                Console.SetCursorPosition(menuOption[option].leftPos, menuOption[option].topPos);
                AnsiConsole.Markup($"[Turquoise4]{menuOption[option].name}[/]");
            }
        }
    }

    internal void MoveRight()
    {
        _selectedOption++;
        if (_selectedOption < 0) _selectedOption = 3;
        if (_selectedOption > 3) _selectedOption = 0;
        ShowOptions();
    }

    internal void MoveLeft()
    {
        _selectedOption--;
        if (_selectedOption < 0) _selectedOption = 3;
        if (_selectedOption > 3) _selectedOption = 0;

        ShowOptions();
    }

    internal string Accion()
    {
        return menuOption[_selectedOption].name;
    }
}
