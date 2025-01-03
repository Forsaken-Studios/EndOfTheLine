using System.Collections.Generic;
using UnityEngine;

public class Subsection
{
    private TypeSubsection _typeSubsection;
    private RoomWithConfiguration _currentRoom;
    private DirectionAvailability _northAvailability, _southAvailability, _eastAvailability, _westAvailability;
    private int _amountParentsRoom;
    private Vector2Int _subsectionCell;

    public Subsection(Vector2Int subsectionCell)
    {
        this._subsectionCell = subsectionCell;
        _typeSubsection = TypeSubsection.Empty;
        _currentRoom = new RoomWithConfiguration();
        _amountParentsRoom = 0;
    }

    public void IncrementAmountParentsRoom(int previousParentsRoom)
    {
        _amountParentsRoom = previousParentsRoom++;
    }

    public void ResetAmountParentsRoom()
    {
        _amountParentsRoom = 0;
    }
    
    public int GetParentsRooms()
    {
        return _amountParentsRoom;
    }

    public void SetNorthAvailability(DirectionAvailability newState)
    {
        _northAvailability = newState;
    }
    public void SetSouthAvailability(DirectionAvailability newState)
    {
        _southAvailability = newState;
    }
    public void SetEastAvailability(DirectionAvailability newState)
    {
        _eastAvailability = newState;
    }
    public void SetWestAvailability(DirectionAvailability newState)
    {
        _westAvailability = newState;
    }

    public DirectionAvailability GetNorthAvailability()
    {
        return _northAvailability;
    }
    public DirectionAvailability GetSouthAvailability()
    {
        return _southAvailability;
    }
    public DirectionAvailability GetEastAvailability()
    {
        return _eastAvailability;
    }
    public DirectionAvailability GetWestAvailability()
    {
        return _westAvailability;
    }

    public Vector2Int GetSubsectionCell()
    {
        return _subsectionCell;
    }

    public RoomWithConfiguration GetCurrentRoom()
    {
        if(_currentRoom.roomPrefab == null || _currentRoom.configurationIndex == -1)
            return null;

        return _currentRoom;
    }

    // TODO
    public List<Subsection> SetCurrentRoom(Subsection[,] subsectionsGrid)
    {
        return null;
    }

    // TODO
    public List<Subsection> SetCurrentCorridor(Subsection[,] subsectionsGrid)
    {
        return null;
    }

    // TODO
    public void SetCloseRoom(Subsection[,] subsectionsGrid)
    {
        _typeSubsection = TypeSubsection.Room;
    }
}