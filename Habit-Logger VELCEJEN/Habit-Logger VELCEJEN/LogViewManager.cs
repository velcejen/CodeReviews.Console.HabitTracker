using Spectre.Console;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.PortableExecutable;
using System.Text;
using System.Threading.Tasks;


namespace HabitLogger.VELCEJEN;

internal class LogViewManager
{
    private List<HabitRecord> _habitsTable = [];
    private List<HabitRecord> _view = [];
    private int _startShowingRow;
    private const int _maximumRows = 13;
    private int _selectedRow;
    private int _habitsTableLastRow;
    private HabitRecord _emptyRow = new HabitRecord();
    private int _currentHabitTableRegister;


    internal LogViewManager()
    {
        Update();
    }

    internal void Update()
    {
        _view = new List<HabitRecord>();
        _emptyRow = new HabitRecord();
        _view.Clear();
        using (DatabaseManager databaseManager = new())
        {
            _habitsTable = databaseManager.GetAllHabits();
        }
        _habitsTableLastRow = _habitsTable.Count - 1;
        _startShowingRow = 0;
        _selectedRow = 0;
        _currentHabitTableRegister = _startShowingRow + _selectedRow;
        _emptyRow = new HabitRecord();

        FillView();
        ShowView();

    }
    private void ShowView()
    {
        for (int row = 0; row < _maximumRows; row++)
        {
            if (row == _selectedRow)
            {
                Console.SetCursorPosition((int)Enums.View.idLeft, (int)Enums.View.top + row);
                AnsiConsole.Markup($"[bold LightGoldenrod2]{_view[row].id,4}[/]");
                Console.SetCursorPosition((int)Enums.View.dateLeft, (int)Enums.View.top + row);
                AnsiConsole.Markup($"[bold LightGoldenrod2]{_view[row].date,-10}[/]");
                Console.SetCursorPosition((int)Enums.View.beersLeft, (int)Enums.View.top + row);
                AnsiConsole.Markup($"[bold LightGoldenrod2]{_view[row].beers,5}[/]");
                Console.SetCursorPosition((int)Enums.View.locationLeft, (int)Enums.View.top + row);
                AnsiConsole.Markup($"[bold LightGoldenrod2]{_view[row].location,-18}[/]");

            }
            else
            {
                Console.SetCursorPosition((int)Enums.View.idLeft, (int)Enums.View.top + row);
                AnsiConsole.Markup($"[LightSkyBlue1]{_view[row].id,4}[/]");
                Console.SetCursorPosition((int)Enums.View.dateLeft, (int)Enums.View.top + row);
                AnsiConsole.Markup($"[LightSkyBlue1]{_view[row].date,-10}[/]");
                Console.SetCursorPosition((int)Enums.View.beersLeft, (int)Enums.View.top + row);
                AnsiConsole.Markup($"[LightSkyBlue1]{_view[row].beers,5}[/]");
                Console.SetCursorPosition((int)Enums.View.locationLeft, (int)Enums.View.top + row);
                AnsiConsole.Markup($"[LightSkyBlue1]{_view[row].location,-18}[/]");
            }
        }
        _currentHabitTableRegister = _startShowingRow + _selectedRow;

    }

    private void FillView()
    {
        _view.Clear();
        for (int row = _startShowingRow; row < _startShowingRow + _maximumRows; row++)
        {
            if (row <= _habitsTableLastRow)
            {
                var newRow = new HabitRecord
                {
                    id = _habitsTable[row].id,
                    date = _habitsTable[row].date,
                    beers = _habitsTable[row].beers,
                    location = _habitsTable[row].location
                };
                _view.Add(newRow);
            }
            else { _view.Add(_emptyRow); }
        }
    }

    internal void MoveDown()
    {
        _selectedRow++;
        if (_selectedRow > _habitsTableLastRow || _selectedRow > _maximumRows - 1) _selectedRow--;
        if (_startShowingRow + _maximumRows - 1 < _habitsTableLastRow && _selectedRow == _maximumRows - 1)
        {
            _startShowingRow++;

        }
        FillView();
        ShowView();

    }

    internal void MoveUp()
    {
        _selectedRow--;
        if (_selectedRow < 0) _selectedRow++;
        if (_startShowingRow > 0 && _selectedRow == 0)
        {
            _startShowingRow--;

        }
        FillView();
        ShowView();
    }

    internal HabitRecord CurrentRegister()
    {
        var currentRegister = new HabitRecord();
        if (_habitsTable.Count>0)
        {
                currentRegister = _habitsTable[_currentHabitTableRegister];
        }
        return currentRegister;
    }
}






