using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using UnityEngine;

public class Subsection
{
    private TypeSubsection _typeSubsection;
    private RoomWithConfiguration _currentRoom;
    private DirectionAvailability _northAvailability, _southAvailability, _eastAvailability, _westAvailability;
    private int _amountParentsRoom;
    private int _amountParentsCorridor;
    private Vector2Int _startingCell; // [row, col] Esquina inferior izquierda
    private int _subsectionRow;
    private int _subsectionCol;
    private List<DirectionAvailability> _directionRequirement;

    private RoomFinder _roomFinder;

    public Subsection(Vector2Int startingCell, int rowSection, int colSection, RoomFinder roomFinder)
    {
        this._startingCell = startingCell;
        _typeSubsection = TypeSubsection.Empty;
        _currentRoom = new RoomWithConfiguration();
        _amountParentsRoom = 0;
        _amountParentsCorridor = 0;
        this._subsectionRow = rowSection;
        this._subsectionCol = colSection;
        this._roomFinder = roomFinder;
        _directionRequirement = new List<DirectionAvailability>
        {
            DirectionAvailability.Free, // north
            DirectionAvailability.Free, // south
            DirectionAvailability.Free, // west
            DirectionAvailability.Free  // east
        };

        _northAvailability = DirectionAvailability.Closed;
        _southAvailability = DirectionAvailability.Closed;
        _eastAvailability = DirectionAvailability.Closed;
        _westAvailability = DirectionAvailability.Closed;
    }

    public Vector3 GetCenterPosition()
    {
        Vector3 positionCell = new Vector3((_startingCell.y + 1) * MapGenerator.Instance.cellSize + MapGenerator.Instance.cellSize / 2, (_startingCell.x + 1) * MapGenerator.Instance.cellSize + MapGenerator.Instance.cellSize / 2, 0);
        return positionCell;
    }

    public Vector2Int GetGlobalCell(int localRow, int localCol)
    {
        Vector2Int globalCell = new Vector2Int(_startingCell.x + localRow, _startingCell.y + localCol);
        return globalCell;
    }

    public int GetSubsectionRow()
    {
        return _subsectionRow;
    }

    public int GetSubsectionCol()
    {
        return _subsectionCol;
    }

    public TypeSubsection GetTypeSubsection()
    {
        return _typeSubsection;
    }

    public void IncrementAmountParentsRoom(int previousParentsRoom)
    {
        _amountParentsRoom = previousParentsRoom++;
    }
    public void IncrementAmountParentsCorridor(int previousParentsCorridor)
    {
        _amountParentsCorridor = previousParentsCorridor++;
    }

    public void ResetAmountParentsRoom()
    {
        _amountParentsRoom = 0;
    }

    public void ResetAmountParentsCorridor()
    {
        _amountParentsCorridor = 0;
    }

    public int GetParentsRooms()
    {
        return _amountParentsRoom;
    }
    public int GetParentsCorridor()
    {
        return _amountParentsCorridor;
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
        return _startingCell;
    }

    public RoomWithConfiguration GetCurrentRoom()
    {
        if (_currentRoom.roomPrefab == null || _currentRoom.configurationIndex == -1)
            return null;

        return _currentRoom;
    }

    public List<Subsection> SetCurrentRoom(Subsection[,] subsectionsGrid)
    {
        if(_typeSubsection != TypeSubsection.Empty)
        {
            return new List<Subsection>();
        }
        UpdateDirectionAvailability(subsectionsGrid);
        List<Subsection> nextSubsections = new List<Subsection>();

        Dictionary<DirectionFlag, DirectionAvailability> roomRequirements = new Dictionary<DirectionFlag, DirectionAvailability>();
        roomRequirements.Add(DirectionFlag.Up, _directionRequirement[0]);
        roomRequirements.Add(DirectionFlag.Down, _directionRequirement[1]);
        roomRequirements.Add(DirectionFlag.Right, _directionRequirement[2]);
        roomRequirements.Add(DirectionFlag.Left, _directionRequirement[3]);

        _currentRoom = _roomFinder.FindRoomPrefab(roomRequirements);
        if (_currentRoom == null)
        {
            bool result = SetCloseRoom(subsectionsGrid);
            if (result == true)
                return new List<Subsection>();
            else
                return null;
        }

        _northAvailability = DirectionAvailability.Closed;
        _southAvailability = DirectionAvailability.Closed;
        _eastAvailability = DirectionAvailability.Closed;
        _westAvailability = DirectionAvailability.Closed;

        foreach (DirectionFlag direction in _currentRoom.openDirections)
        {
            switch (direction)
            {
                case DirectionFlag.Up:
                    _northAvailability = DirectionAvailability.Open;
                    break;
                case DirectionFlag.Down:
                    _southAvailability = DirectionAvailability.Open;
                    break;
                case DirectionFlag.Right:
                    _eastAvailability = DirectionAvailability.Open;
                    break;
                case DirectionFlag.Left:
                    _westAvailability = DirectionAvailability.Open;
                    break;
            }
        }

        _typeSubsection = TypeSubsection.Room;
        nextSubsections = GetNextSubsections(subsectionsGrid);
        foreach (Subsection subsection in nextSubsections)
        {
            subsection.IncrementAmountParentsRoom(_amountParentsRoom);
            subsection.ResetAmountParentsCorridor();

        }

        return nextSubsections;
    }

    public List<Subsection> SetCurrentCorridor(Subsection[,] subsectionsGrid, bool isClosing = false)
    {
        if (_typeSubsection != TypeSubsection.Empty)
        {
            return new List<Subsection>();
        }
        if (!isClosing)
            UpdateDirectionAvailability(subsectionsGrid);
        List<Subsection> nextSubsections = new List<Subsection>();

        int amountOpen = 0;
        List<int> directionsFree = new List<int>();
        // Norte
        if (_directionRequirement[0] == DirectionAvailability.Open)
        {
            _northAvailability = DirectionAvailability.Open;
            amountOpen++;
        }
        else if (_directionRequirement[0] == DirectionAvailability.Closed)
        {
            _northAvailability = DirectionAvailability.Closed;
        }
        else if (_directionRequirement[0] == DirectionAvailability.Free && !isClosing)
        {
            int randomNumber = Random.Range(1, 3);
            if (randomNumber == 1)
            {
                _northAvailability = DirectionAvailability.Open;
                amountOpen++;
            }
            else if (randomNumber == 2)
            {
                _northAvailability = DirectionAvailability.Closed;
                directionsFree.Add(0);
            }
        }
        // Sur
        if (_directionRequirement[1] == DirectionAvailability.Open)
        {
            _southAvailability = DirectionAvailability.Open;
            amountOpen++;
        }
        else if (_directionRequirement[1] == DirectionAvailability.Closed)
        {
            _southAvailability = DirectionAvailability.Closed;
        }
        else if (_directionRequirement[1] == DirectionAvailability.Free && !isClosing)
        {
            int randomNumber = Random.Range(1, 3);
            if (randomNumber == 1)
            {
                _southAvailability = DirectionAvailability.Open;
                amountOpen++;
            }
            else if (randomNumber == 2)
            {
                _southAvailability = DirectionAvailability.Closed;
                directionsFree.Add(1);
            }
        }
        // Este
        if (_directionRequirement[2] == DirectionAvailability.Open)
        {
            _eastAvailability = DirectionAvailability.Open;
            amountOpen++;
        }
        else if (_directionRequirement[2] == DirectionAvailability.Closed)
        {
            _eastAvailability = DirectionAvailability.Closed;
        }
        else if (_directionRequirement[2] == DirectionAvailability.Free && !isClosing)
        {
            int randomNumber = Random.Range(1, 3);
            if (randomNumber == 1)
            {
                _eastAvailability = DirectionAvailability.Open;
                amountOpen++;
            }
            else if (randomNumber == 2)
            {
                _eastAvailability = DirectionAvailability.Closed;
                directionsFree.Add(2);
            }
        }
        // Oeste
        if (_directionRequirement[3] == DirectionAvailability.Open)
        {
            _westAvailability = DirectionAvailability.Open;
            amountOpen++;
        }
        else if (_directionRequirement[3] == DirectionAvailability.Closed)
        {
            _westAvailability = DirectionAvailability.Closed;
        }
        else if (_directionRequirement[3] == DirectionAvailability.Free && !isClosing)
        {
            int randomNumber = Random.Range(1, 3);
            if (randomNumber == 1)
            {
                _westAvailability = DirectionAvailability.Open;
                amountOpen++;
            }
            else if (randomNumber == 2)
            {
                _westAvailability = DirectionAvailability.Closed;
                directionsFree.Add(3);
            }
        }

        if (amountOpen == 1)
        {
            if (directionsFree.Count == 0)
            {
                bool result = SetCloseRoom(subsectionsGrid, false);
                if (result == true)
                {
                    return new List<Subsection>();
                }
                else
                {
                    return null;
                }
            }

            System.Random rnd = new System.Random();
            List<int> shuffledList = directionsFree.OrderBy(x => rnd.Next()).ToList();

            switch (shuffledList[0])
            {
                case 0:
                    _northAvailability = DirectionAvailability.Open;
                    break;
                case 1:
                    _southAvailability = DirectionAvailability.Open;
                    break;
                case 2:
                    _eastAvailability = DirectionAvailability.Open;
                    break;
                case 3:
                    _westAvailability = DirectionAvailability.Open;
                    break;
            }
        }

        _typeSubsection = TypeSubsection.Corridor;

        nextSubsections = GetNextSubsections(subsectionsGrid);
        foreach (Subsection subsection in nextSubsections)
        {
            subsection.ResetAmountParentsRoom();
            subsection.IncrementAmountParentsCorridor(_amountParentsCorridor);
        }
        _directionRequirement[0] = _northAvailability;
        _directionRequirement[1] = _southAvailability;
        _directionRequirement[2] = _eastAvailability;
        _directionRequirement[3] = _westAvailability;

        return nextSubsections;
    }

    private List<Subsection> GetNextSubsections(Subsection[,] subsectionsGrid)
    {
        List<Subsection> nextSubsections = new List<Subsection>();

        // Norte
        if (_northAvailability == DirectionAvailability.Open)
        {
            if (_subsectionRow + 1 < subsectionsGrid.GetLength(0))
                nextSubsections.Add(subsectionsGrid[_subsectionRow + 1, _subsectionCol]);
        }
        // Sur
        if (_southAvailability == DirectionAvailability.Open)
        {
            if (_subsectionRow - 1 >= 0)
                nextSubsections.Add(subsectionsGrid[_subsectionRow - 1, _subsectionCol]);
        }
        // Este
        if (_eastAvailability == DirectionAvailability.Open)
        {
            if (_subsectionCol + 1 < subsectionsGrid.GetLength(1))
                nextSubsections.Add(subsectionsGrid[_subsectionRow, _subsectionCol + 1]);
        }
        // Oeste
        if (_westAvailability == DirectionAvailability.Open)
        {
            if (_subsectionCol - 1 >= 0)
                nextSubsections.Add(subsectionsGrid[_subsectionRow, _subsectionCol - 1]);
        }

        return nextSubsections;
    }

    // Asigna una habitación de clausura.
    public bool SetCloseRoom(Subsection[,] subsectionsGrid, bool updateDirections = true)
    {
        if (_typeSubsection != TypeSubsection.Empty)
        {
            return true;
        }
        if (updateDirections)
            UpdateDirectionAvailability(subsectionsGrid);

        // NORTE
        if (_directionRequirement[0] == DirectionAvailability.Free || _directionRequirement[0] == DirectionAvailability.Closed)
        {
            _directionRequirement[0] = DirectionAvailability.Closed;
            _northAvailability = DirectionAvailability.Closed;
        }
        else
        {
            _northAvailability = _directionRequirement[0];
        }
        // SUR
        if (_directionRequirement[1] == DirectionAvailability.Free || _directionRequirement[1] == DirectionAvailability.Closed)
        {
            _directionRequirement[1] = DirectionAvailability.Closed;
            _southAvailability = DirectionAvailability.Closed;
        }
        else
        {
            _southAvailability = _directionRequirement[1];
        }
        // ESTE
        if (_directionRequirement[2] == DirectionAvailability.Free || _directionRequirement[2] == DirectionAvailability.Closed)
        {
            _directionRequirement[2] = DirectionAvailability.Closed;
            _eastAvailability = DirectionAvailability.Closed;
        }
        else
        {
            _eastAvailability = _directionRequirement[2];
        }
        // OESTE
        if (_directionRequirement[3] == DirectionAvailability.Free || _directionRequirement[3] == DirectionAvailability.Closed)
        {
            _directionRequirement[3] = DirectionAvailability.Closed;
            _westAvailability = DirectionAvailability.Closed;
        }
        else
        {
            _westAvailability = _directionRequirement[3];
        }

        Dictionary<DirectionFlag, DirectionAvailability> roomRequirements = new Dictionary<DirectionFlag, DirectionAvailability>();
        roomRequirements.Add(DirectionFlag.Up, _directionRequirement[0]);
        roomRequirements.Add(DirectionFlag.Down, _directionRequirement[1]);
        roomRequirements.Add(DirectionFlag.Right, _directionRequirement[2]);
        roomRequirements.Add(DirectionFlag.Left, _directionRequirement[3]);
        _currentRoom = _roomFinder.FindRoomPrefab(roomRequirements);
        if (_currentRoom == null)
        {
            SetCurrentCorridor(subsectionsGrid, true);
            return true;
        }

        _typeSubsection = TypeSubsection.Room;

        return true;
    }

    public void SetAsStart(DirectionAvailability north, DirectionAvailability east, DirectionAvailability west)
    {
        _typeSubsection = TypeSubsection.Start;

        _northAvailability = north;
        _southAvailability = DirectionAvailability.Closed;
        _eastAvailability = east;
        _westAvailability = west;

    }

    public void SetAsEnd()
    {
        _typeSubsection = TypeSubsection.End;
        _northAvailability = _directionRequirement[0];
        _southAvailability = _directionRequirement[1];
        _eastAvailability = _directionRequirement[2];
        _westAvailability = _directionRequirement[3];
    }

    // Actualiza el estado de DirectionAvailability según las subsecciones adyacentes.
    public void UpdateDirectionAvailability(Subsection[,] subsectionsGrid)
    {
        // Norte
        int northRow = _subsectionRow + 1;
        int northCol = _subsectionCol;
        if (IsInGrid(subsectionsGrid, northRow, northCol))
        {
            Subsection northSubsection = subsectionsGrid[northRow, northCol];
            _directionRequirement[0] = CheckDirectionAvailability(northSubsection, DirectionFlag.Up);
        }
        else
        {
            _directionRequirement[0] = DirectionAvailability.Closed;
        }

        // Sur
        int southRow = _subsectionRow - 1;
        int southCol = _subsectionCol;
        if (IsInGrid(subsectionsGrid, southRow, southCol))
        {
            Subsection southSubsection = subsectionsGrid[southRow, southCol];
            _directionRequirement[1] = CheckDirectionAvailability(southSubsection, DirectionFlag.Down);
        }
        else
        {
            _directionRequirement[1] = DirectionAvailability.Closed;
        }

        // Este
        int eastRow = _subsectionRow;
        int eastCol = _subsectionCol + 1;
        if (IsInGrid(subsectionsGrid, eastRow, eastCol))
        {
            Subsection eastSubsection = subsectionsGrid[eastRow, eastCol];
            _directionRequirement[2] = CheckDirectionAvailability(eastSubsection, DirectionFlag.Right);
        }
        else
        {
            _directionRequirement[2] = DirectionAvailability.Closed;
        }

        // Oeste
        int westRow = _subsectionRow;
        int westCol = _subsectionCol - 1;
        if (IsInGrid(subsectionsGrid, westRow, westCol))
        {
            Subsection westSubsection = subsectionsGrid[westRow, westCol];
            _directionRequirement[3] = CheckDirectionAvailability(westSubsection, DirectionFlag.Left);
        }
        else
        {
            _directionRequirement[3] = DirectionAvailability.Closed;
        }
    }

    // Verifica si una celda está dentro del grid
    bool IsInGrid(Subsection[,] subsectionsGrid, int row, int col)
    {
        int rows = subsectionsGrid.GetLength(0);
        int cols = subsectionsGrid.GetLength(1);

        return row >= 0 && row < rows && col >= 0 && col < cols;
    }

    // Determina el tipo de la subsection adyacente
    private DirectionAvailability CheckDirectionAvailability(Subsection subsection, DirectionFlag direction)
    {
        switch (subsection.GetTypeSubsection())
        {
            case TypeSubsection.Room:
                return HasDirectionTowards(subsection, direction) ? DirectionAvailability.Open : DirectionAvailability.Closed;
            case TypeSubsection.Corridor:
                return HasDirectionTowards(subsection, direction) ? DirectionAvailability.Open : DirectionAvailability.Closed;
            case TypeSubsection.Start:
                return HasDirectionTowards(subsection, direction) ? DirectionAvailability.Open : DirectionAvailability.Closed;
            case TypeSubsection.End:
                return HasDirectionTowards(subsection, direction) ? DirectionAvailability.Open : DirectionAvailability.Closed;
            case TypeSubsection.Empty:
                return DirectionAvailability.Free;
            default:
                return DirectionAvailability.Closed;
        }
    }

    // Verifica si la dirección de la subsection apunta hacia esta
    private bool HasDirectionTowards(Subsection subsection, DirectionFlag direction)
    {
        switch (direction)
        {
            case DirectionFlag.Down:
                return subsection.GetNorthAvailability() == DirectionAvailability.Open;
            case DirectionFlag.Up:
                return subsection.GetSouthAvailability() == DirectionAvailability.Open;
            case DirectionFlag.Right:
                return subsection.GetWestAvailability() == DirectionAvailability.Open;
            case DirectionFlag.Left:
                return subsection.GetEastAvailability() == DirectionAvailability.Open;
            default:
                return false;
        }
    }
}