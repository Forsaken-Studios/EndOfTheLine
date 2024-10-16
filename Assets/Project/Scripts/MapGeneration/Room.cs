using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using static RoomData;

public class Room : MonoBehaviour
{
    public Vector2Int _roomSize; // Room size in cells. x = col, y = row
    private BoolMatrix _shape; // Occupation matrix (true = ocuppied, false = not ocuppied).
    [HideInInspector] public Vector3 centerPosition;
    private BoolMatrix _entrances; // Entrances matrix (true = entrance, false = not entrance).
    private Dictionary<Vector2Int, DirectionFlag> _entrancesDirections; // Stores directions of each entrance. [Col, Row]????

    public Cell[,] selfGrid;// [Row, Col]

    [SerializeField] private List<RoomData> _roomDataList;
    System.Random rnd;

    private int _configurationSelected = 1;
    private Vector3 _positionSelected = Vector3.zero;

    void Start()
    {
        rnd = new System.Random();
    }

    public int GetNumberOfConfigurations()
    {
        return _roomDataList.Count;
    }

    public void SetPositionSelected(Vector3 positionSelected)
    {
        _positionSelected = positionSelected;
    }

    public void InitializeRandomRoom()
    {
        _configurationSelected = rnd.Next(1, _roomDataList.Count);

        _roomSize = _roomDataList[_configurationSelected].roomSize;
        _shape = _roomDataList[_configurationSelected].GetShape();
        _entrances = _roomDataList[_configurationSelected].GetEntrances();
        _entrancesDirections = _roomDataList[_configurationSelected].entrancesDirections;

        CalculateCenterPosition();
        CreateSelfGrid();
    }

    public GameObject InstantiateRoom()
    {
        GameObject roomInstantiated = Instantiate(this.gameObject, _positionSelected, Quaternion.identity);

        for (int i = 1; i < _roomDataList.Count; i++)
        {
            GameObject currentConfiguration = roomInstantiated.transform.Find($"Configuration_{i}").gameObject;

            if (i == _configurationSelected)
            {
                currentConfiguration.SetActive(true);
            }
            else
            {
                currentConfiguration.SetActive(false);
            }
        }

        GameObject aux = roomInstantiated.transform.Find($"AUX_Configuration_base").gameObject;
        aux.SetActive(false);
        GameObject gas = roomInstantiated.transform.Find($"Gas").gameObject;
        aux.SetActive(true);
        GameObject gasHigh = roomInstantiated.transform.Find($"GasHigh").gameObject;
        aux.SetActive(true);

        return roomInstantiated;
    }

    private void CalculateCenterPosition()
    {
        float centerPosition_x = 0;
        float centerPosition_y = 0;

        if(_shape.Rows % 2 == 0)
        {
            centerPosition_x = MapGenerator.Instance.cellSize * _shape.Rows/2 - MapGenerator.Instance.cellSize / 2;
        }
        else
        {
            centerPosition_x = MapGenerator.Instance.cellSize * _shape.Rows / 2;
        }

        if (_shape.Cols % 2 == 0)
        {
            centerPosition_y = MapGenerator.Instance.cellSize * _shape.Cols / 2 - MapGenerator.Instance.cellSize / 2;
        }
        else
        {
            centerPosition_y = MapGenerator.Instance.cellSize * _shape.Cols / 2;
        }

        centerPosition = new Vector3(centerPosition_x, centerPosition_y, centerPosition.z);
    }

    private void CreateSelfGrid()
    {
        // Creación mini grid de la habitación.
        selfGrid = new Cell[GetRows(), GetCols()];

        // Se cuenta el número de entradas y se marca como 'EntranceRoom' las celdas correspondientes en selfGrid.
        int numberEntrances = 0;
        for (int row = 0; row < _entrances.Rows; row++)
        {
            for (int col = 0; col < _entrances.Cols; col++)
            {
                selfGrid[row, col] = new Cell(row, col);
                if (_entrances.GetValue(col, row))
                {
                    selfGrid[row, col].SetCellState(CellState.EntranceRoom);
                    numberEntrances++;
                }
            }
        }

        // Si la matriz _shape lo marca como ocupado entonces se toma una decisión. Si hay más de una entrada
        // las celdas marcadas como ocupadas se marcan como 'CorridorRoom', si no se marcan como 'Room'.
        for (int row = 0; row < _shape.Rows; row++)
        {
            for (int col = 0; col < _shape.Cols; col++)
            {
                if (_shape.GetValue(col, row) == true && selfGrid[row, col].State != CellState.EntranceRoom)
                {
                    if (numberEntrances > 0)
                    {
                        selfGrid[row, col].SetCellState(CellState.CorridorRoom);
                    }
                    else
                    {
                        selfGrid[row, col].SetCellState(CellState.Room);
                    }
                }else if (_shape.GetValue(col, row) == false)
                {
                    selfGrid[row, col].SetCellState(CellState.FillingRoom);
                }
            }
        }

        // Se recorre _entrancesDirections y se coloca la dirección de entrada en cada celda usando SetEntranceDirection. 
        foreach (var cell in _entrancesDirections)
        {
            if (selfGrid[cell.Key.y, cell.Key.x].State == CellState.EntranceRoom)
            {
                selfGrid[cell.Key.y, cell.Key.x].SetEntranceDirection(cell.Value);
            }
        }
    }

    private int GetCols()
    {
        return _roomSize.x;
    }

    private int GetRows()
    {
        return _roomSize.y;
    }

    //public List<Vector2Int> GetPosStartEnd() // [Col, Row]
    //{
    //    List<Vector2Int> positionsToReturn = new List<Vector2Int>();

    //    // Recorre _entranceDirections y calcula las posiciones de start/end del algoritmo de búsqueda de caminos.
    //    foreach (var cell in _entrancesDirections)
    //    {
    //        Cell cellGrid = selfGrid[cell.Key.y, cell.Key.x];

    //        Vector2Int newPostion = new Vector2Int();
    //        switch (cellGrid.EntranceDirection)
    //        {
    //            case DirectionFlag.Up:
    //                newPostion.y = cellGrid.Row + 1;
    //                newPostion.x = cellGrid.Col;
    //                break;
    //            case DirectionFlag.Down:
    //                newPostion.y = cellGrid.Row - 1;
    //                newPostion.x = cellGrid.Col;
    //                break;
    //            case DirectionFlag.Right:
    //                newPostion.y = cellGrid.Row;
    //                newPostion.x = cellGrid.Col + 1;
    //                break;
    //            case DirectionFlag.Left:
    //                newPostion.y = cellGrid.Row;
    //                newPostion.x = cellGrid.Col - 1;
    //                break;
    //        }
    //        positionsToReturn.Add(newPostion);
    //    }

    //    return positionsToReturn;
    //}

    public List<Vector2Int> GetPosStartEnd() // [Col, Row]
    {
        List<Vector2Int> positionsToReturn = new List<Vector2Int>();

        // Recorre _entranceDirections y calcula las posiciones de start/end del algoritmo de búsqueda de caminos.
        foreach (var cell in _entrancesDirections)
        {
            Cell cellGrid = selfGrid[cell.Key.y, cell.Key.x];

            Vector2Int newPosition = new Vector2Int();
            switch (cellGrid.EntranceDirection)
            {
                case DirectionFlag.Up:
                    newPosition.x = cellGrid.Col;
                    newPosition.y = cellGrid.Row + 1;
                    break;
                case DirectionFlag.Down:
                    newPosition.x = cellGrid.Col;
                    newPosition.y = cellGrid.Row - 1;
                    break;
                case DirectionFlag.Right:
                    newPosition.x = cellGrid.Col + 1;
                    newPosition.y = cellGrid.Row;
                    break;
                case DirectionFlag.Left:
                    newPosition.x = cellGrid.Col - 1;
                    newPosition.y = cellGrid.Row;
                    break;
            }

            positionsToReturn.Add(newPosition);
        }

        return positionsToReturn;
    }

}
