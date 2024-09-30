using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Xml.Linq;
using System.Xml.Schema;
using Unity.VisualScripting;
using Unity.VisualScripting.FullSerializer;
using UnityEditor;
using UnityEngine;
using Utils.CustomLogs;

public class MapGenerator : MonoBehaviour
{
    #region Properties
    [SerializeField] private Vector2Int _gridSize;
    public float cellSize = 5;
    [SerializeField] private int _startSize = 2;
    [SerializeField] private int _exitSize = 2;

    [SerializeField] private List<Room> _testRooms;
    [SerializeField] private List<Room> _testRoomsPrefabs;
    private Dictionary<Vector3, GameObject> _roomsToInsert;
    private List<GameObject> _roomsInserted;


    [HideInInspector] public Vector3 startingGridPosition = Vector3.zero;
    private Cell[,] _grid; // [row, col]
    [SerializeField] private int _maxRoomsInserted = 4;
    [SerializeField] private int _radius = 5;
    [SerializeField] private bool _isTotallyGizmos = true;

    private System.Random _rnd;

    private void OnValidate()
    {
        // Make sure _startSize and _exitSize are less or equal to _gridSize.
        if (_startSize > _gridSize.x)
        {
            Debug.LogWarning($"Start Size ({_startSize}) no puede ser mayor que Grid Size.x ({_gridSize.x}). Se ajustará a {_gridSize.x}.");
            _startSize = _gridSize.x;
        }

        if (_exitSize > _gridSize.x || _exitSize > _gridSize.y - 1)
        {
            int adjust = _gridSize.x < _gridSize.y - 1 ? _gridSize.x : _gridSize.y - 1;
            Debug.LogWarning($"Exit Size ({_exitSize}) no puede ser mayor que GridSize.x ({_gridSize.x}) o GridSize.y ({_gridSize.y}). Se ajustará al menor entre GridSize.x y GridSize.y, es decir {adjust}.");
            _exitSize = adjust;
        }
    }
    #endregion

    #region Singleton and Events
    public static MapGenerator Instance { get; private set; }
    private void Awake()
    {
        if (Instance != null)
        {
            Debug.LogError("There's more than one MapGenerator! " + transform + " - " + Instance);
            Destroy(gameObject);
            return;
        }
        Instance = this;

        GameEventsMap.OnReBuildMap += BuildMap;
    }

    void OnDestroy()
    {
        GameEventsMap.OnReBuildMap -= BuildMap;
    }
    #endregion

    void Start()
    {
        _roomsInserted = new List<GameObject>();
        _roomsToInsert = new Dictionary<Vector3, GameObject>();
        BuildMap();
    }

    private void BuildMap()
    {
        if (_roomsInserted != null && _roomsInserted.Count > 0)
        {
            foreach (GameObject room in _roomsInserted)
            {
                Destroy(room);
            }
        }

        _rnd = new System.Random();
        InitializeGrid();

        PlacingStart();
        PlacingExit();

        if (_isTotallyGizmos == true)
        {
            bool mapCreated = false;
            int tries = 5;
            while (mapCreated == false && tries > 0)
            {
                mapCreated = PlacingRooms();
                tries--;
            }
        }
        else
        {
            bool mapCreated = false;
            int tries = 5;
            while (mapCreated == false && tries > 0)
            {
                mapCreated = PlacingRoomsPrefabs();
                tries--;
            }
        }
    }

    #region Gizmos Initialization

    private void OnDrawGizmos()
    {
        if (_gridSize == Vector2Int.zero || cellSize == 0 || _grid == null)
        {
            return;
        }

        foreach (var cell in GetGridPositions())
        {
            DrawCellGizmos(cell);
        }
    }

    private IEnumerable<Cell> GetGridPositions()
    {
        for (int row = 0; row < _grid.GetLength(0); row++)
        {
            for (int col = 0; col < _grid.GetLength(1); col++)
            {
                yield return _grid[row, col];
            }
        }
    }

    private void DrawCellGizmos(Cell cellToDraw)
    {
        Gizmos.color = UnityEngine.Color.green;

        switch (cellToDraw.State)
        {
            case CellState.Empty:
                Gizmos.color = UnityEngine.Color.green;
                break;
            case CellState.Room:
                Gizmos.color = UnityEngine.Color.yellow;
                break;
            case CellState.CorridorRoom:
                Gizmos.color = UnityEngine.Color.white;
                break;
            case CellState.Corridor:
                Gizmos.color = UnityEngine.Color.blue;
                break;
            case CellState.Start:
                Gizmos.color = UnityEngine.Color.black;
                break;
            case CellState.End:
                Gizmos.color = UnityEngine.Color.black;
                break;
            case CellState.EntranceRoom:
                Gizmos.color = UnityEngine.Color.red;
                break;
        }

        // Margen entre celda y celda.
        float margin = 1f;

        // Defining all 4 vertex. Empezando desde el centro de la celda
        //Vector3 topLeft = cellToDraw.Position3D + new Vector3(-(cellSize / 2 - margin), cellSize / 2 - margin, 0);
        //Vector3 topRight = cellToDraw.Position3D + new Vector3(cellSize / 2 - margin, cellSize / 2 - margin, 0);
        //Vector3 bottomRight = cellToDraw.Position3D + new Vector3(cellSize / 2 - margin, -cellSize / 2 + margin, 0);
        //Vector3 bottomLeft = cellToDraw.Position3D + new Vector3(-(cellSize / 2 - margin), -cellSize / 2 + margin, 0);

        // Defining all 4 vertex. Empezando desde la esquina inferior izquieda de la celda.
        Vector3 topLeft = cellToDraw.Position3D + new Vector3(+margin, cellSize - margin, 0);
        Vector3 topRight = cellToDraw.Position3D + new Vector3(cellSize - margin, cellSize - margin, 0);
        Vector3 bottomRight = cellToDraw.Position3D + new Vector3(cellSize - margin, +margin, 0);
        Vector3 bottomLeft = cellToDraw.Position3D + new Vector3(+margin, +margin, 0);



        // Draw square lines.
        Gizmos.DrawLine(topLeft, topRight); // Up
        Gizmos.DrawLine(topRight, bottomRight); // Right
        Gizmos.DrawLine(bottomRight, bottomLeft); // Bottom
        Gizmos.DrawLine(bottomLeft, topLeft); // Left
    }
    #endregion

    #region Grid Initialization
    private void InitializeGrid()
    {
        _grid = new Cell[_gridSize.y, _gridSize.x];

        for (int row = 0; row < _gridSize.y; row++)
        {
            for (int column = 0; column < _gridSize.x; column++)
            {
                _grid[row, column] = new Cell(row, column);
            }
        }
    }
    #endregion

    #region Placing Start and Exit
    private void PlacingStart() // TODO: Colocar gameobject
    {
        PlacerStartExit(0, true, 0, _gridSize.x - 1, CellState.Start);
    }

    private void PlacingExit() // TODO: Colocar gameobject
    {
        // 0 = Left Wall; 1 = Upper Wall; 2 = Right Wall
        int wall = _rnd.Next(0, 2 + 1);

        switch (wall)
        {
            case 0:
                PlacerStartExit(0, false, _gridSize.y / 4 * 4, _gridSize.y - 1, CellState.End);
                break;
            case 1:
                PlacerStartExit(_gridSize.y - 1, true, 0, _gridSize.x - 1, CellState.End);
                break;
            case 2:
                PlacerStartExit(_gridSize.x - 1, false, _gridSize.y / 4 * 4, _gridSize.y - 1, CellState.End);
                break;
        }
    }

    private void PlacerStartExit(int fixedValue, bool isRowFixed, int minFlexValue, int maxFlexValue, CellState newState)
    {
        bool isValid = false;
        int sizePlaced = newState == CellState.Start ? _startSize : _exitSize;

        while (isValid == false)
        {
            List<int> flexValues = new List<int>();
            flexValues.Add(_rnd.Next(minFlexValue, maxFlexValue + 1));

            for (int i = 0; i < sizePlaced - 1; i++)
            {
                bool isPlaced = false;

                flexValues.Sort();

                while (!isPlaced)
                {
                    // direction = 0 -> left; direction = 1 -> right;
                    int direction = _rnd.Next(0, 1 + 1);

                    int newValue = 0;

                    switch (direction)
                    {
                        case 0:
                            newValue = flexValues[0] - 1;
                            break;
                        case 1:
                            newValue = flexValues[flexValues.Count - 1] + 1;
                            break;
                    }

                    if (newValue >= 0 && newValue <= maxFlexValue)
                    {
                        flexValues.Add(newValue);
                        isPlaced = true;
                    }
                }
            }

            // Check and change of state being fixedValue the column.
            if (!isRowFixed)
            {
                isValid = true;

                for (int i = 0; i < sizePlaced; i++)
                {
                    if (_grid[flexValues[i], fixedValue].State != CellState.Empty)
                    {
                        isValid = false;
                    }
                }

                if (!isValid)
                {
                    continue;
                }

                for (int i = 0; i < sizePlaced; i++)
                {
                    _grid[flexValues[i], fixedValue].SetCellState(newState);
                }
            }

            // Check and change of state being fixedValue the row.
            if (isRowFixed)
            {
                isValid = true;

                for (int i = 0; i < sizePlaced; i++)
                {
                    if (_grid[fixedValue, flexValues[i]].State != CellState.Empty)
                    {
                        isValid = false;
                    }
                }

                if (!isValid)
                {
                    continue;
                }

                for (int i = 0; i < sizePlaced; i++)
                {
                    _grid[fixedValue, flexValues[i]].SetCellState(newState);
                }
            }
        }
    }
    #endregion

    #region Placing Rooms
    private bool PlacingRooms()
    {
        // Intentar crear caminos a todas las habitaciones requeridas.
        Cell[,] copiedGrid = AuxiliarMapGenerator.CopyGrid(_grid);
        Cell[,] modifiedGrid = TryToInsertRooms(copiedGrid);

        if (modifiedGrid == null)
        {
            Debug.LogWarning("FINAL MAP GENERATION -Retrying to create map-.");
            return false;
        }
        else
        {
            copiedGrid = modifiedGrid;

            // Intentar llevar el resto de salidas al final.
            modifiedGrid = TryToReachFinal(copiedGrid);
            if (modifiedGrid == null)
            {
                Debug.LogWarning("FINAL MAP GENERATION -Retrying to create map-.");
                return false;
            }
            else
            {
                _grid = modifiedGrid;
                return true;
            }
        }

    }

    private bool PlacingRoomsPrefabs()
    {
        // Intentar crear caminos a todas las habitaciones requeridas.
        Cell[,] copiedGrid = AuxiliarMapGenerator.CopyGrid(_grid);
        Cell[,] modifiedGrid = TryToInsertRoomsPrefabs(copiedGrid);

        if (modifiedGrid == null)
        {
            Debug.LogWarning("FINAL MAP GENERATION -Retrying to create map-.");
            return false;
        }
        else
        {
            copiedGrid = modifiedGrid;

            // Intentar llevar el resto de salidas al final.
            modifiedGrid = TryToReachFinal(copiedGrid);
            if (modifiedGrid == null)
            {
                Debug.LogWarning("FINAL MAP GENERATION -Retrying to create map-.");
                _roomsToInsert.Clear();
                return false;
            }
            else
            {
                foreach(KeyValuePair<Vector3, GameObject> entry in _roomsToInsert)
                {
                    _roomsInserted.Add(Instantiate(entry.Value, entry.Key, Quaternion.identity));
                }
                _roomsToInsert.Clear();
                _grid = modifiedGrid;
                return true;
            }
        }

    }

    private Cell[,] TryToReachFinal(Cell[,] grid)
    {
        List<Cell> restEntrancesList = GetRestEntrances(grid);
        int counterRestEntrances = restEntrancesList.Count;

        foreach (Cell entranceCell in restEntrancesList)
        {
            // Buscar vecinos que sean Empty.
            List<Cell> cellNeighboursList = new List<Cell>();
            cellNeighboursList = AuxiliarMapGenerator.GetNeighbours(entranceCell, grid);

            foreach (Cell neighbour in cellNeighboursList)
            {
                if (neighbour.State == CellState.Empty)
                {
                    // Si es la última entrada entonces hacer que llegue al final.
                    if (counterRestEntrances == 1)
                    {
                        Cell[,] modifiedGrid = ReachFinal(neighbour, grid);
                        if (modifiedGrid != null)
                        {
                            grid = modifiedGrid;
                            return grid;
                        }
                        else
                        {
                            return null;
                        }
                    }
                    else
                    {
                        // Buscar el pasillo más cercano y unirse a él.
                        Cell[,] modifiedGrid = SeekAndJoinNearCorridors(neighbour, grid);
                        if (modifiedGrid != null)
                        {
                            grid = modifiedGrid;
                            break;
                        }
                        else
                        {
                            return null;
                        }
                    }
                }
            }

            counterRestEntrances--;
        }

        return grid;
    }

    private Cell[,] SeekAndJoinNearCorridors(Cell startCell, Cell[,] grid)
    {

        List<Cell> adjacentCellsList = GetAdjacentCells(startCell, grid, _radius);

        foreach (Cell adjacentCell in adjacentCellsList)
        {
            if (adjacentCell.State == CellState.Corridor)
            {
                List<Cell> destinationCellList = new List<Cell>() { adjacentCell };
                List<Cell> possibleOrigins = new List<Cell>() { startCell };
                Cell[,] modifiedGrid = AuxiliarMapGenerator.FindPath(destinationCellList, grid, true, possibleOrigins);
                if (modifiedGrid != null)
                {
                    Debug.Log($"Camino encontrado desde [{startCell.Row}, {startCell.Col}] hasta [{adjacentCell.Row}, {adjacentCell.Col}]");
                    grid = modifiedGrid;
                    return grid;
                }
            }
        }

        return null;
    }

    private List<Cell> GetAdjacentCells(Cell cell, Cell[,] grid, int radius)
    {
        List<Cell> adjacentCells = new List<Cell>();

        for (int row = cell.Row - radius; row <= cell.Row + radius; row++)
        {
            for (int col = cell.Col - radius; col <= cell.Col + radius; col++)
            {
                if ((row == cell.Row && col == cell.Col) || row >= grid.GetLength(0) || row <= 0 || col >= grid.GetLength(1) || col <= 0)
                {
                    continue;
                }
                adjacentCells.Add(grid[row, col]);
            }
        }

        System.Random rng = new System.Random();
        adjacentCells = adjacentCells.OrderBy(a => rng.Next()).ToList();

        return adjacentCells;
    }

    private List<Cell> GetRestEntrances(Cell[,] grid)
    {
        List<Cell> countRestEntrances = new List<Cell>();

        foreach (Cell cell in grid)
        {
            // Si la celda actual es una entrada.
            if (cell.State == CellState.EntranceRoom)
            {
                // Buscar vecinos que sean Empty.
                List<Cell> cellNeighboursList = new List<Cell>();
                cellNeighboursList = AuxiliarMapGenerator.GetNeighbours(cell, grid);

                foreach (Cell neighbour in cellNeighboursList)
                {
                    if (neighbour.State == CellState.Empty)
                    {
                        if (!countRestEntrances.Contains(cell))
                        {
                            countRestEntrances.Add(cell);
                        }
                    }
                }
            }
        }

        System.Random rng = new System.Random();
        countRestEntrances = countRestEntrances.OrderBy(a => rng.Next()).ToList();

        return countRestEntrances;
    }

    private Cell[,] ReachFinal(Cell start, Cell[,] grid)
    {
        List<Cell> possibleOrigins = new List<Cell>() { start };
        foreach(Cell destinationCell in AuxiliarMapGenerator.GetPossibleEndCells(grid))
        {
            List<Cell> destinationCellList = new List<Cell>() { destinationCell };
            Cell[,] modifiedGrid = AuxiliarMapGenerator.FindPath(destinationCellList, grid, true, possibleOrigins);
            if (modifiedGrid != null)
            {
                grid = modifiedGrid;
                return grid;
            }
        }

        return null;
    }

    private Cell[,] TryToInsertRooms(Cell[,] grid)
    {
        int roomsInserted = 0;
        int numberRoom = 0;

        foreach (Room room in _testRooms)
        {

            room.InitializeRoom();

            int insertionTry = 15;

            while (insertionTry > 0)
            {
                Debug.Log($"GetPosStartEnd(): Intento número {insertionTry} en habitación {numberRoom}");
                List<Cell> endCellList = new List<Cell>();

                int initialRow = _rnd.Next(0, grid.GetLength(0));
                int initialCol = _rnd.Next(0, grid.GetLength(1));

                List<Vector2Int> possibleEndCell = room.GetPosStartEnd();
                foreach (Vector2Int element in possibleEndCell)
                {
                    if (element.y + initialRow < 0 || element.y + initialRow >= grid.GetLength(0) ||
                        element.x + initialCol < 0 || element.x + initialCol >= grid.GetLength(1))
                    {
                        break;
                    }

                    endCellList.Add(grid[element.y + initialRow, element.x + initialCol]);
                    Debug.Log($"GetPosStartEnd(): grid[{element.y + initialRow}, {element.x + initialCol}]");
                }

                // Intentar insertar la habitación y obtener el grid modificado
                Cell[,] modifiedGrid = AuxiliarMapGenerator.InsertRoom(grid, room.selfGrid, initialRow, initialCol, endCellList);

                if (modifiedGrid != null)
                {
                    grid = modifiedGrid; // Actualizar grid con el grid modificado
                    roomsInserted++;
                    break;
                }
                else
                {
                    insertionTry--;
                }

            }

            if (roomsInserted == _maxRoomsInserted)
            {
                break;
            }

            numberRoom++;
        }

        Debug.Log($"MAP GENERATION -Final grid updated- with {roomsInserted} rooms inserted.");

        if (roomsInserted == _maxRoomsInserted)
        {
            return grid;
        }
        else
        {
            return null;
        }
    }

    private Cell[,] TryToInsertRoomsPrefabs(Cell[,] grid)
    {
        int roomsInserted = 0;
        int numberRoom = 0;

        foreach (Room room in _testRoomsPrefabs)
        {

            room.InitializeRoom();

            int insertionTry = 15;

            while (insertionTry > 0)
            {
                Debug.Log($"GetPosStartEnd(): Intento número {insertionTry} en habitación {numberRoom}");
                List<Cell> endCellList = new List<Cell>();

                int initialRow = _rnd.Next(0, grid.GetLength(0));
                int initialCol = _rnd.Next(0, grid.GetLength(1));

                List<Vector2Int> possibleEndCell = room.GetPosStartEnd();
                foreach (Vector2Int element in possibleEndCell)
                {
                    if (element.y + initialRow < 0 || element.y + initialRow >= grid.GetLength(0) ||
                        element.x + initialCol < 0 || element.x + initialCol >= grid.GetLength(1))
                    {
                        break;
                    }

                    endCellList.Add(grid[element.y + initialRow, element.x + initialCol]);
                    Debug.Log($"GetPosStartEnd(): grid[{element.y + initialRow}, {element.x + initialCol}]");
                }

                // Intentar insertar la habitación y obtener el grid modificado
                Vector3 positonToInstantiate = grid[initialRow, initialCol].Position3D + room.centerPosition;
                Cell[,] modifiedGrid = AuxiliarMapGenerator.InsertRoom(grid, room.selfGrid, initialRow, initialCol, endCellList);

                if (modifiedGrid != null)
                {
                    Debug.Log($"Nombre: {room.gameObject.name}");
                    _roomsToInsert.Add(positonToInstantiate, room.gameObject);
                    grid = modifiedGrid; // Actualizar grid con el grid modificado
                    roomsInserted++;
                    break;
                }
                else
                {
                    insertionTry--;
                }

            }

            if (roomsInserted == _maxRoomsInserted)
            {
                break;
            }

            numberRoom++;
        }

        Debug.Log($"MAP GENERATION -Final grid updated- with {roomsInserted} rooms inserted.");

        if (roomsInserted == _maxRoomsInserted)
        {
            return grid;
        }
        else
        {
            _roomsToInsert.Clear();
            return null;
        }
    }

    IEnumerator Test1()
    {
        List<Cell> list = new List<Cell>();
        list.Add(_grid[8, 2]);
        AuxiliarMapGenerator.FindPath(list, _grid);

        // Espera 2 segundos
        yield return new WaitForSeconds(1f);
    }

    IEnumerator Test2()
    {
        List<Cell> list = new List<Cell>();
        list.Add(_grid[8, 2]);
        Cell[,] modifiedGrid = AuxiliarMapGenerator.FindPath(list, _grid);
        if (modifiedGrid != null)
        {
            _grid = modifiedGrid;
        }

        // Espera 2 segundos
        yield return new WaitForSeconds(3f);
        Cell[,] copiedGrid = AuxiliarMapGenerator.CopyGrid(_grid);

        copiedGrid[5, 2].SetCellState(CellState.Room);

        List<Cell> cellsToRecalculate = new List<Cell>();
        cellsToRecalculate.Add(copiedGrid[5, 2]);

        modifiedGrid = AuxiliarMapGenerator.ReFindPath(cellsToRecalculate, copiedGrid);
        if (modifiedGrid != null)
        {
            _grid = modifiedGrid;
        }
        else
        {
            Debug.LogWarning("No se ha podido regenerar el camino");
        }

        Debug.Log(_grid[5, 2].State);
    }


    IEnumerator Test3()
    {
        List<Cell> list1 = new List<Cell>();
        list1.Add(_grid[8, 8]);
        Cell[,] modifiedGrid = AuxiliarMapGenerator.FindPath(list1, _grid);
        if (modifiedGrid != null)
        {
            _grid = modifiedGrid;
        }

        List<Cell> list2 = new List<Cell>();
        list2.Add(_grid[2, 9]);
        modifiedGrid = AuxiliarMapGenerator.FindPath(list2, _grid);
        if (modifiedGrid != null)
        {
            _grid = modifiedGrid;
        }

        List<Cell> list3 = new List<Cell>();
        list3.Add(_grid[5, 5]);
        modifiedGrid = AuxiliarMapGenerator.FindPath(list3, _grid);
        if (modifiedGrid != null)
        {
            _grid = modifiedGrid;
        }

        // Espera 2 segundos
        yield return new WaitForSeconds(3f);

        Cell[,] copiedGrid = AuxiliarMapGenerator.CopyGrid(_grid);

        copiedGrid[2, 8].SetCellState(CellState.Room);
        copiedGrid[2, 7].SetCellState(CellState.Room);
        copiedGrid[2, 6].SetCellState(CellState.Room);
        copiedGrid[2, 5].SetCellState(CellState.Room);
        copiedGrid[2, 4].SetCellState(CellState.Room);
        copiedGrid[2, 3].SetCellState(CellState.Room);
        copiedGrid[2, 2].SetCellState(CellState.Room);
        copiedGrid[2, 1].SetCellState(CellState.Room);
        copiedGrid[2, 0].SetCellState(CellState.Room);

        List<Cell> cellsToRecalculate = new List<Cell>();

        cellsToRecalculate.Add(copiedGrid[2, 8]);
        cellsToRecalculate.Add(copiedGrid[2, 7]);
        cellsToRecalculate.Add(copiedGrid[2, 6]);
        cellsToRecalculate.Add(copiedGrid[2, 5]);
        cellsToRecalculate.Add(copiedGrid[2, 4]);
        cellsToRecalculate.Add(copiedGrid[2, 3]);
        cellsToRecalculate.Add(copiedGrid[2, 2]);
        cellsToRecalculate.Add(copiedGrid[2, 1]);
        cellsToRecalculate.Add(copiedGrid[2, 0]);

        modifiedGrid = AuxiliarMapGenerator.ReFindPath(cellsToRecalculate, copiedGrid);
        if (modifiedGrid != null)
        {
            _grid = modifiedGrid;
        }
        else
        {
            Debug.LogWarning("No se ha podido regenerar el camino");
        }
    }

    IEnumerator Test4()
    {
        List<Cell> list1 = new List<Cell>();
        list1.Add(_grid[8, 8]);
        Cell[,] modifiedGrid = AuxiliarMapGenerator.FindPath(list1, _grid);
        if (modifiedGrid != null)
        {
            _grid = modifiedGrid;
        }

        List<Cell> list2 = new List<Cell>();
        list2.Add(_grid[2, 9]);
        modifiedGrid = AuxiliarMapGenerator.FindPath(list2, _grid);
        if (modifiedGrid != null)
        {
            _grid = modifiedGrid;
        }

        List<Cell> list3 = new List<Cell>();
        list3.Add(_grid[5, 5]);
        modifiedGrid = AuxiliarMapGenerator.FindPath(list3, _grid);
        if (modifiedGrid != null)
        {
            _grid = modifiedGrid;
        }

        Cell cellToDebug = _grid[2, 8];

        // Espera 2 segundos
        yield return new WaitForSeconds(3f);

        Cell[,] copiedGrid = AuxiliarMapGenerator.CopyGrid(_grid);

        copiedGrid[2, 9].SetCellState(CellState.Room);
        copiedGrid[2, 8].SetCellState(CellState.Room);
        copiedGrid[2, 7].SetCellState(CellState.Room);
        copiedGrid[2, 6].SetCellState(CellState.Room);
        copiedGrid[2, 5].SetCellState(CellState.Room);
        copiedGrid[2, 4].SetCellState(CellState.Room);
        copiedGrid[2, 3].SetCellState(CellState.Room);
        copiedGrid[2, 2].SetCellState(CellState.Room);
        copiedGrid[2, 1].SetCellState(CellState.Room);
        copiedGrid[2, 0].SetCellState(CellState.Room);

        List<Cell> cellsToRecalculate = new List<Cell>();

        cellsToRecalculate.Add(copiedGrid[2, 9]);
        cellsToRecalculate.Add(copiedGrid[2, 8]);
        cellsToRecalculate.Add(copiedGrid[2, 7]);
        cellsToRecalculate.Add(copiedGrid[2, 6]);
        cellsToRecalculate.Add(copiedGrid[2, 5]);
        cellsToRecalculate.Add(copiedGrid[2, 4]);
        cellsToRecalculate.Add(copiedGrid[2, 3]);
        cellsToRecalculate.Add(copiedGrid[2, 2]);
        cellsToRecalculate.Add(copiedGrid[2, 1]);
        cellsToRecalculate.Add(copiedGrid[2, 0]);

        modifiedGrid = AuxiliarMapGenerator.ReFindPath(cellsToRecalculate, copiedGrid);
        if (modifiedGrid != null)
        {
            _grid = modifiedGrid;
        }
        else
        {
            Debug.LogWarning("No se ha podido regenerar el camino");
        }
    }
    #endregion
}