using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum CellState
{
    Empty,
    Corridor,
    InternalCorridor,

    Start,
    End,

    Room,
    EntranceRoom,
    CorridorRoom,
    FillingRoom
}
