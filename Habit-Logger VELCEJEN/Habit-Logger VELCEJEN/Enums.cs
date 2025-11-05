namespace HabitLogger;

internal class Enums
{
    internal const int topPos = 3;
    internal const int leftPos = 10;

    internal enum Perimeter
    {
        top = 1,
        left = 10,
        width = 100,
        height = 28
    }
    internal enum IdColumn
    {
        top = topPos + 4,
        left = leftPos + 20,
        width = 6,
        height = 15
    }
    internal enum IdHeader
    {
        top = topPos + 2,
        left = leftPos + 20,
        width = 6,
        height = 3
    }
    internal enum DateColumn
    {
        top = IdColumn.top,
        left = IdColumn.left + IdColumn.width - 1,
        width = 12,
        height = 15
    }
    internal enum DateHeader
    {
        top = IdHeader.top,
        left = IdHeader.left + IdColumn.width - 1,
        width = 12,
        height = 3
    }
    internal enum BeersColumn
    {
        top = DateColumn.top,
        left = DateColumn.left + DateColumn.width - 1,
        width = 7,
        height = 15
    }
    internal enum BeersHeader
    {
        top = DateHeader.top,
        left = DateHeader.left + DateHeader.width - 1,
        width = 7,
        height = 3
    }
    internal enum LocationColumn
    {
        top = BeersColumn.top,
        left = BeersColumn.left + BeersColumn.width - 1,
        width = 20,
        height = 15
    }
    internal enum LocationHeader
    {
        top = BeersHeader.top,
        left = BeersHeader.left + BeersHeader.width - 1,
        width = 20,
        height = 3
    }
    internal enum BoxType
    {
        normalBox = 1,
        middleBox = 2,
        crossBox = 3,
        middleCrossBox = 4
    }
    internal enum View
    {
        top = topPos + 5,
        idLeft = IdColumn.left + 1,
        dateLeft = DateColumn.left + 1,
        beersLeft = BeersColumn.left + 1,
        locationLeft = LocationColumn.left + 1,
    }

    internal enum OptionsAdd
    {
        x = IdColumn.left + 5,
        y = Enums.IdColumn.height + 8
    }

    internal enum OptionsModify
    {
        x = IdColumn.left + 12,
        y = IdColumn.height + 8
    }

    internal enum OptionsDelete
    {
        x = IdColumn.left + 22,
        y = IdColumn.height + 8
    }

    internal enum OptionsExit
    {
        x = IdColumn.left + 32,
        y = IdColumn.height + 8
    }


}