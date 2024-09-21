using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Cell
{
    private List<CellState> AllowedConnections;
    public DirectionFlag EntranceDirection { get; private set; }
    public int Row { get; private set; }
    public int Col { get; private set; }
    public Vector3 Position3D { get; private set; }
    public CellState State { get; private set; }
    public List<Cell> Parents { get; private set; }
    public Cell TemporalParent { get;  set; }
    public List<Cell> DestinationCells { get; private set; }
    public bool IsRoomPlaceable { get; private set; } // Cell is empty or corridor to place a room over it.

    public int ig; // Coste de llegada al punto
    public int ih; // Manhattan value hasta el destino
    public int iF
    {
        get
        {
            return ig + ih;
        }
    } // Suma de los dos anteriores


    public Cell(int row, int col)
    {
        AllowedConnections = new List<CellState>();
        this.Row = row;
        this.Col = col;
        CalculateCellPosition3D(this.Col, this.Row);
        SetCellState(CellState.Empty);
        this.Parents = new List<Cell>();
        this.TemporalParent = null;
        this.DestinationCells = new List<Cell>();
        this.EntranceDirection = DirectionFlag.None;
        
    }

    public Cell(int row, int col, CellState state, bool isRoomPlacable, int ig, int ih, DirectionFlag entranceDirection)
    {
        AllowedConnections = new List<CellState>();
        this.Row = row;
        this.Col = col;
        CalculateCellPosition3D(this.Col, this.Row);
        SetCellState(state);
        this.Parents = new List<Cell>();
        this.TemporalParent = null;
        this.DestinationCells = new List<Cell>();
        this.IsRoomPlaceable = isRoomPlacable;
        this.ig = ig;
        this.ih = ih;
        this.EntranceDirection = entranceDirection;
        
    }

    private void CalculateCellPosition3D(int column, int row)
    {
        float x = MapGenerator.Instance.startingGridPosition.x + MapGenerator.Instance.cellSize * column;
        float y = MapGenerator.Instance.startingGridPosition.y + MapGenerator.Instance.cellSize * row;
        SetPosition3D(x, y);
    }

    private void SetPosition3D(float x, float y)
    {
        Position3D = new Vector3(x, y, Position3D.z);
    }

    public bool CanConnectTo(Cell newCell, List<Cell> endCells)
    {
        if(endCells != null)
        {
            foreach (Cell endCell in endCells)
            {
                if(State == CellState.EntranceRoom && (newCell.Row == endCell.Row && newCell.Col == endCell.Col))
                {
                    Debug.Log($"finalpath -> CanConnectTo(): no puedo conectar [{Row}, {Col}] con [{newCell.Row}, {newCell.Col}]");
                    return false;
                }
            }
        }


        bool allowedConnection = AllowedConnections.Contains(newCell.State);
        if ((State == CellState.Corridor || State == CellState.Empty) && newCell.State == CellState.EntranceRoom)
        {
            bool allowedEntrance = newCell.IsDoorInDirection(Row, Col);
            if(allowedConnection && allowedEntrance)
            {
                Debug.Log($"finalpath -> CanConnectTo(): CONECTADAAAAAAAAAA [{Row}, {Col}] con [{newCell.Row}, {newCell.Col}]");
                return true;
            }
            else
            {
                Debug.Log($"finalpath -> CanConnectTo(): no puedo conectar [{Row}, {Col}] con [{newCell.Row}, {newCell.Col}]");
                return false;
            }
        }
        if (State == CellState.EntranceRoom && (newCell.State == CellState.Corridor || newCell.State == CellState.Empty))
        {
            bool allowedExit = IsDoorInDirection(newCell.Row, newCell.Col);
            if (allowedConnection && allowedExit)
            {
                Debug.Log($"finalpath -> CanConnectTo(): CONECTADAAAAAAAAAA [{Row}, {Col}] con [{newCell.Row}, {newCell.Col}]");
                return true;
            }
            else
            {
                Debug.Log($"finalpath -> CanConnectTo(): no puedo conectar [{Row}, {Col}] con [{newCell.Row}, {newCell.Col}]");
                return false;
            }
        }
        if (allowedConnection)
        {
            Debug.Log($"finalpath -> CanConnectTo(): CONECTADAAAAAAAAAA [{Row}, {Col}] con [{newCell.Row}, {newCell.Col}] con estado actual: {State} y vecino: {newCell.State}");
        }
        else
        {
            Debug.Log($"finalpath -> CanConnectTo(): no puedo conectar [{Row}, {Col}] con [{newCell.Row}, {newCell.Col}]");
        }
        
        return allowedConnection;
    }

    public void SetCellState(CellState state)
    {
        this.State = state;
        AllowedConnections.Clear();

        switch (state)
        {
            case CellState.Start:
                IsRoomPlaceable = false;
                AllowedConnections.Add(CellState.Empty);
                AllowedConnections.Add(CellState.Corridor);
                break;
            case CellState.End:
                AllowedConnections.Add(CellState.Empty);
                AllowedConnections.Add(CellState.Corridor);
                IsRoomPlaceable = false;
                break;
            case CellState.Empty:
                IsRoomPlaceable = true;
                AllowedConnections.Add(CellState.Empty);
                AllowedConnections.Add(CellState.Corridor);
                AllowedConnections.Add(CellState.EntranceRoom);
                break;
            case CellState.Corridor:
                IsRoomPlaceable = true;
                AllowedConnections.Add(CellState.Empty);
                AllowedConnections.Add(CellState.Corridor);
                AllowedConnections.Add(CellState.EntranceRoom);
                break;
            case CellState.Room:
                IsRoomPlaceable = false;
                break;
            case CellState.CorridorRoom:
                IsRoomPlaceable = false;
                AllowedConnections.Add(CellState.CorridorRoom);
                AllowedConnections.Add(CellState.EntranceRoom);
                break;
            case CellState.EntranceRoom:
                IsRoomPlaceable = false;
                AllowedConnections.Add(CellState.CorridorRoom);
                AllowedConnections.Add(CellState.Corridor);
                AllowedConnections.Add(CellState.Empty);
                break;
        }
    }

    public void AddParent(Cell newCell)
    {
        Parents.Add(newCell);
    }
    
    public void AddDestinationCell(Cell newCell)
    {
        DestinationCells.Add(newCell);
    }

    public void SetInFinalPath()
    {
        if (TemporalParent != null)
        {
            if (!Parents.Contains(TemporalParent))
            {
                Parents.Add(TemporalParent);
            }
        }

        if (State == CellState.Empty)
        {
            SetCellState(CellState.Corridor);
        }
    }

    public void ClearTemporalParent()
    {
        TemporalParent = null;
    }

    public void SetEntranceDirection(DirectionFlag direction)
    {
        EntranceDirection = direction;
    }

    public bool IsDoorInDirection(int previousRow, int previousCol)
    {
        switch (EntranceDirection)
        {
            case DirectionFlag.Up:
                if(Row + 1 == previousRow && Col == previousCol)
                {
                    return true;
                }
                break;
            case DirectionFlag.Down:
                if (Row - 1 == previousRow && Col == previousCol)
                {
                    return true;
                }
                break;
            case DirectionFlag.Right:
                if (Row == previousRow && Col + 1 == previousCol)
                {
                    return true;
                }
                break;
            case DirectionFlag.Left:
                if (Row == previousRow && Col - 1 == previousCol)
                {
                    return true;
                }
                break;
        }

        return false;
    }
}
