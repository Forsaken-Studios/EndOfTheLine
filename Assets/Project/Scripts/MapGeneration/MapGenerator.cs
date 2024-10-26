using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class MapGenerator : MonoBehaviour
{
    #region Properties
    [Header("Ajustes generales")]
    [Tooltip("X = columns; Y = rows.")]
    [SerializeField] private Vector2Int _gridSize;
    public float cellSize = 5;
    [Tooltip("Tamaño, en celdas, de la entrada al mapa.")]
    [SerializeField] private int _startSize = 2;
    [Tooltip("Tamaño, en celdas, de la salida del mapa.")]
    [SerializeField] private int _exitSize = 2;

    [Header("Condiciones generación")]
    [Tooltip("Número de habitaciones 3x3 que habrá en el mapa.")]
    [SerializeField] private int _amountRooms3x3 = 4;
    [Tooltip("Porcentaje del grid que se quiere rellenar con habitaciones.")]
    [Range(1, 100)]
    [SerializeField] private int _mapPercentageOcuppied = 25;
    [Tooltip("Representa el radio de distancia (en celdas) en el que una habitación con más de una entrada va a buscar unir esas entradas extra con un pasillo.")]
    [SerializeField] private int _roomRadius = 5;

    [Header("Opciones visuales")]
    [SerializeField] private bool _isDrawingGizmos = true;

    [Header("Extra")]
    [SerializeField] private CorridorsDataBase corridorsDB;

    [HideInInspector] public Vector3 startingGridPosition = Vector3.zero;
    private Cell[,] _grid; // [row, col]
    private System.Random _rnd;
    private List<GameObject> _corridorsInserted;
    private Dictionary<Vector3, GameObject> _roomsInserted;

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
        // Inicialización de listas.
        _roomsInserted = new Dictionary<Vector3, GameObject>();
        _corridorsInserted = new List<GameObject>();

        // Inicialización de prefabs de pasillos y de habitaciones.
        if (corridorsDB != null)
        {
            corridorsDB.LoadCorridors();
            ParseAllCorridors();
        }
        else
        {
            Debug.LogError("Error cargando los prefabs de los pasillos.");
        }
        RoomLoader.Load();

        BuildMap();
    }

    private void ParseAllCorridors()
    {
        foreach (GameObject corridor in corridorsDB.corridorPrefabs)
        {
            corridor.GetComponent<Corridor>().ParseCorridorData();
        }
    }

    private void BuildMap()
    {
        // Borrado y creación de la estructura de carpetas en la jerarquia de los pasillos y habitaciones.
        GameObject mapObject = GameObject.Find("Map");
        if (mapObject == null)
        {
            mapObject = new GameObject("Map");
        }

        GameObject roomsObject = GameObject.Find("Rooms");
        if (roomsObject == null)
        {
            roomsObject = new GameObject("Rooms");
            roomsObject.transform.SetParent(mapObject.transform, false);
        }
        else
        {
            foreach (Transform room in roomsObject.transform)
            {
                Destroy(room.gameObject);
            }
            _roomsInserted.Clear();
        }

        GameObject corridorsObject = GameObject.Find("Corridors");
        if (corridorsObject == null)
        {
            corridorsObject = new GameObject("Corridors");
            corridorsObject.transform.SetParent(mapObject.transform, false);
        }
        else
        {
            foreach (Transform corridor in corridorsObject.transform)
            {
                Destroy(corridor.gameObject);
            }
            _corridorsInserted.Clear();
        }

        // Se inicializa el grid general.
        _rnd = new System.Random();
        InitializeGrid();

        // Se coloca la entrada y salida del mapa.
        PlacingStart();
        PlacingExit();

        // Se crea el mapa.
        bool mapCreated = false;
        int tries = 1;
        int auxMapPercentageOcuppied = _mapPercentageOcuppied;
        while (mapCreated == false)
        {
            if(tries > 0)
            {
                mapCreated = PlacingRooms();
                if(mapCreated == true)
                {
                    Debug.Log($"Map percentage: {_mapPercentageOcuppied}");
                    _mapPercentageOcuppied = auxMapPercentageOcuppied;
                    break;
                }
                else
                {
                    tries--;
                }
            }
            else
            {
                _mapPercentageOcuppied -= 1;
                tries = 1;

                if (_mapPercentageOcuppied <= 1)
                {
                    _mapPercentageOcuppied = auxMapPercentageOcuppied;
                    Debug.LogWarning("FINAL MAP GENERATION - No se pudo crear el mapa con los parámetros dados.");
                    break;
                }
            }
        }

        // Se instancian los pasillos.
        InstantiateCorridorsPrefabs();

        // Construir navmesh.
        NavmeshManager.Instance.GenerateNavmesh();
    }

    private void InstantiateCorridorsPrefabs()
    {
        // Recorre todas las celdas cogiendo las que son pasillos.
        foreach (Cell cell in _grid)
        {
            if (cell.State == CellState.Corridor)
            {
                // Configura las aperturas y puertas de la celda.
                Cell aboveNeighbour = AuxiliarMapGenerator.GetAboveNeighbour(cell, _grid);
                Cell belowNeighbour = AuxiliarMapGenerator.GetBelowNeighbour(cell, _grid);
                Cell rightNeighbour = AuxiliarMapGenerator.GetRightNeighbour(cell, _grid);
                Cell leftNeighbour = AuxiliarMapGenerator.GetLeftNeighbour(cell, _grid);
                cell.SetCorridorConfiguration(aboveNeighbour, belowNeighbour, rightNeighbour, leftNeighbour);

                // Instancia el prefab de pasillo que equivalga a la configuración de la celda.
                foreach (GameObject corridorPrefab in corridorsDB.corridorPrefabs)
                {
                    Corridor corridor = corridorPrefab.GetComponent<Corridor>();

                    // Si el prefab encaja con la configuración lo instancia en la posición de la celda.
                    if (corridor.MatchesConfig(cell.IsOpenUp, cell.IsOpenDown, cell.IsOpenRight, cell.IsOpenLeft, cell.HasDoorUp, cell.HasDoorDown, cell.HasDoorRight, cell.HasDoorLeft))
                    {
                        // Instanciar el prefab del pasillo
                        Vector3 centeredPosition = new Vector3(cell.Position3D.x + 4, cell.Position3D.y + 4, cell.Position3D.z);
                        GameObject corridorInstance = Instantiate(corridorPrefab, centeredPosition, corridorPrefab.transform.rotation);
                        corridorInstance.transform.SetParent(GameObject.Find("Corridors").transform);
                        _corridorsInserted.Add(corridorInstance);
                    }
                }
            }
        }
    }

    #region Gizmos Initialization

    private void OnDrawGizmos()
    {
        if (_gridSize == Vector2Int.zero || cellSize == 0 || _grid == null || !_isDrawingGizmos)
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
            case CellState.FillingRoom:
                Gizmos.color = UnityEngine.Color.magenta;
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
                PlacerStartExit(0, false, _gridSize.y / 4 * 3, _gridSize.y - 1, CellState.End);
                break;
            case 1:
                PlacerStartExit(_gridSize.y - 1, true, 0, _gridSize.x - 1, CellState.End);
                break;
            case 2:
                PlacerStartExit(_gridSize.x - 1, false, _gridSize.y / 4 * 3, _gridSize.y - 1, CellState.End);
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
            copiedGrid = AuxiliarMapGenerator.CopyGrid(modifiedGrid); ;

            // Intentar llevar el resto de salidas al final.
            modifiedGrid = TryToReachFinal(copiedGrid);

            if (modifiedGrid == null)
            {
                Debug.LogWarning("FINAL MAP GENERATION -Retrying to create map-.");
                foreach (KeyValuePair<Vector3, GameObject> entry in _roomsInserted)
                {
                    Destroy(entry.Value);
                }
                _roomsInserted.Clear();
                return false;
            }
            else
            {
                _roomsInserted.Clear();
                _grid = modifiedGrid;
                return true;
            }
        }

    }

    private Cell[,] TryToReachFinal(Cell[,] grid)
    {
        List<Cell> restEntrancesList = GetRestEntrances(grid); ;
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

        List<Cell> adjacentCellsList = GetAdjacentCells(startCell, grid, _roomRadius);
        Debug.Log($"Number AdjacentCells for [{startCell.Row}, {startCell.Col}]: {adjacentCellsList.Count}");

        foreach (Cell adjacentCell in adjacentCellsList)
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

        return null;
    }

    private List<Cell> GetAdjacentCells(Cell cell, Cell[,] grid, int radius)
    {
        List<Cell> adjacentCells = new List<Cell>();

        for (int row = 0; row < grid.GetLength(0); row++)
        {
            for (int col = 0; col < grid.GetLength(1); col++)
            {
                if (row == cell.Row && col == cell.Col)
                {
                    continue;
                }
                if ((row > cell.Row - radius && row < cell.Row + radius) || (col > cell.Col - radius && col < cell.Col + radius))
                {
                    continue;
                }
                if (grid[row, col].State == CellState.Corridor)
                {
                    adjacentCells.Add(grid[row, col]);
                }
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
        foreach (Cell destinationCell in AuxiliarMapGenerator.GetPossibleEndCells(grid))
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
        int currentAmountRooms3x3 = 0;
        int tries = 5;

        while (tries > 0)
        {
            bool isInserted = false;

            GameObject roomPrefab;
            if (currentAmountRooms3x3 < _amountRooms3x3)
            {
                roomPrefab = RoomLoader.GetRandomRoom(true);
            }
            else
            {
                roomPrefab = RoomLoader.GetRandomRoom(false);
            }
            
            GameObject roomObject = Instantiate(roomPrefab, Vector3.zero, roomPrefab.transform.rotation);
            roomObject.transform.SetParent(GameObject.Find("Rooms").transform, false);
            Room room = roomObject.GetComponent<Room>();
            room.InitializeRandomRoom();

            int insertionTry = 250;

            while (insertionTry > 0)
            {
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
                }

                // Intentar insertar la habitación y obtener el grid modificado
                Vector3 positonToInstantiate = grid[initialRow, initialCol].Position3D + room.centerPosition;
                Cell[,] modifiedGrid = AuxiliarMapGenerator.InsertRoom(grid, room.selfGrid, initialRow, initialCol, endCellList);

                if (modifiedGrid != null)
                {
                    room.MoveRoomPosition(positonToInstantiate);
                    _roomsInserted.Add(positonToInstantiate, roomObject);
                    grid = modifiedGrid; // Actualizar grid con el grid modificado
                    isInserted = true;
                    break;
                }
                else
                {
                    insertionTry--;
                }

            }

            if (isInserted == true)
            {
                if(currentAmountRooms3x3 < _amountRooms3x3)
                {
                    currentAmountRooms3x3++;
                }
            }
            else
            {
                tries--;
                Destroy(roomObject);
            }

            if(GetOccupiedMapPercentage(grid) >= _mapPercentageOcuppied && currentAmountRooms3x3 == _amountRooms3x3)
            {
                break;
            }
        }

        if (GetOccupiedMapPercentage(grid) >= _mapPercentageOcuppied && currentAmountRooms3x3 == _amountRooms3x3)
        {
            return grid;
        }
        else
        {
            foreach (KeyValuePair<Vector3, GameObject> entry in _roomsInserted)
            {
                Destroy(entry.Value);
            }
            _roomsInserted.Clear();
            return null;
        }
    }

    private int GetOccupiedMapPercentage(Cell[,] currentGrid)
    {
        int totalCells = currentGrid.GetLength(0) * currentGrid.GetLength(1);
        int currentCellsOcuppied = 0;

        foreach(Cell cell in currentGrid)
        {
            if(cell.State == CellState.Room || cell.State == CellState.EntranceRoom || cell.State == CellState.CorridorRoom || cell.State == CellState.FillingRoom)
            {
                currentCellsOcuppied++;
            }
        }

        return (int)(((float)currentCellsOcuppied/totalCells) * 100);
    }
    #endregion
}