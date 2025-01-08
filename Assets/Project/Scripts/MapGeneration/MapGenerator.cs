using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

public class MapGenerator : MonoBehaviour
{
    #region Properties
    [Header("Ajustes generales")]
    [Tooltip("X = columns; Y = rows.")]
    [SerializeField] private Vector2Int _SubsectionsGridSize; // [col, row]
    [HideInInspector] public float cellSize = 8;
    private int _startSize = 3;
    private int _exitSize = 3;

    [Header("Condiciones generación")]
    [Tooltip("Porcentaje del grid que se quiere rellenar")]
    [Range(1, 100)]
    [SerializeField] private float _maxMapPercentageOcuppied = 25;
    [SerializeField] private int balancePoint = 5;
    private float _currentMapPercentageOccupied = 0;
    private float _subsectionMapPercentage;

    [Header("Opciones visuales")]
    [SerializeField] private bool _isDrawingGizmos = true;

    [Header("Prefabs")]
    [SerializeField] private GameObject _corridorBase;
    [SerializeField] private GameObject _stationsBase;
    [SerializeField] private RoomsDataBase _roomsDataBase;

    [HideInInspector] public Vector3 startingGridPosition = Vector3.zero;
    [HideInInspector] public int endWall;
    private Cell[,] _cellsGrid; // [row, col] GetLength(0), GetLength(1)
    private System.Random _rnd;
    private Subsection[,] _subsectionsGrid; // [row, col] GetLength(0), GetLength(1)
    private List<Subsection> _nextSubsections;
    private RoomFinder _roomFinder;
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
    }
    #endregion

    void Start()
    {
        _rnd = new System.Random();
        _nextSubsections = new List<Subsection>();
        _roomFinder = new RoomFinder(_roomsDataBase);

        _subsectionMapPercentage = 100 / (_SubsectionsGridSize.x * _SubsectionsGridSize.y);

        InitializeGrid();
        InitializeSubsections();

        StartCoroutine(GenerateMap());
        //Test();
    }


    #region Test
    public void Test()
    {
        // Simular un conjunto de entradas con direcciones y disponibilidades
        Dictionary<DirectionFlag, DirectionAvailability> testEntrances = new Dictionary<DirectionFlag, DirectionAvailability>
    {
        { DirectionFlag.Up, DirectionAvailability.Closed },
        { DirectionFlag.Down, DirectionAvailability.Free },
        { DirectionFlag.Left, DirectionAvailability.Free },
        { DirectionFlag.Right, DirectionAvailability.Open }
    };

        // Imprimir las entradas de prueba
        Debug.Log("-test- Testing FindRoomPrefab with the following entrances:");
        foreach (var entrance in testEntrances)
        {
            Debug.Log($"-test- Direction: {entrance.Key}, Availability: {entrance.Value}");
        }

        // Llamar a FindRoomPrefab
        RoomWithConfiguration room = _roomFinder.FindRoomPrefab(testEntrances);

        // Verificar el resultado
        if (room != null)
        {
            Debug.Log($"-test- Found Room Prefab: {room.roomPrefab.name}, Configuration Index: {room.configurationIndex}");

            // Instanciar la habitación encontrada
            GameObject roomObject = Instantiate(room.roomPrefab, Vector3.zero, room.roomPrefab.transform.rotation);
            Room roomComponent = roomObject.GetComponent<Room>();

            if (roomComponent != null)
            {
                // Configurar la habitación utilizando su índice de configuración
                roomComponent.InitializeRoom(room.configurationIndex);
                roomComponent.MoveRoomPosition(Vector3.zero); // Cambiar la posición si es necesario

                // Mostrar nombre del archivo y configuración
                string prefabPath = UnityEditor.AssetDatabase.GetAssetPath(room.roomPrefab);
                Debug.Log($"-test- Instantiated Room Prefab File: {prefabPath}, Configuration Index: {room.configurationIndex}");
            }
            else
            {
                Debug.LogWarning("-test- The instantiated prefab does not have a Room component.");
            }
        }
        else
        {
            Debug.Log("-test- No suitable room prefab found.");
        }
    }




    #endregion


    #region Generation
    private IEnumerator GenerateMap()
    {
        // GameManager.Instance.sceneIsLoading = true;  // TODO
        yield return null;
        // TODO: hacer que se guarde en subcarpetas como en la anterior MapGenerator

        PlaceStart();

        while (_currentMapPercentageOccupied < _maxMapPercentageOcuppied)
        {
            ConfigureNextSubsections();
            if(_nextSubsections.Count == 0)
            {
                Debug.LogWarning("-test- No hay más nextSubsections");
            }

            yield return null;
        }

        CompleteMap();
        InstantiateAll();
    }

    private void AddPercentage()
    {
        _currentMapPercentageOccupied += _subsectionMapPercentage;
    }

    private void PlaceStart()
    {
        // Se obtiene una columna aleatoria y se selecciona la subsección aleatoria en la fila 0.
        int randomColumn = _rnd.Next(0, _SubsectionsGridSize.y);
        Subsection randomSubsection = _subsectionsGrid[0, randomColumn];

        // Se escogen las direcciones al azar.
        DirectionAvailability north = DirectionAvailability.Closed;
        DirectionAvailability east = DirectionAvailability.Closed;
        DirectionAvailability west = DirectionAvailability.Closed;

        // Si está en la esquina inferior izquierda.
        if (randomColumn == 0)
        {
            north = DirectionAvailability.Open;
            east = DirectionAvailability.Open;
        }
        // Si está en la esquina inferior derecha.
        else if (randomColumn == _SubsectionsGridSize.y - 1)
        {
            north = DirectionAvailability.Open;
            west = DirectionAvailability.Open;
        }
        // Si está dentro.
        else
        {
            // Se eligen dos direcciones aleatorias entre norte, este y oeste.
            List<string> directions = new List<string> { "north", "east", "west" };
            directions = directions.OrderBy(x => _rnd.Next()).ToList();

            for (int i = 0; i < 2; i++)
            {
                switch (directions[i])
                {
                    case "north":
                        north = DirectionAvailability.Open;
                        break;
                    case "east":
                        east = DirectionAvailability.Open;
                        break;
                    case "west":
                        west = DirectionAvailability.Open;
                        break;
                }
            }
        }

        Debug.Log($"subsection start = [{0}, {randomColumn}]; {north}, {east}, {west}");

        // Se marca la subsection como Start.
        randomSubsection.SetAsStart(north, east, west);
        AddPercentage();
        if (north == DirectionAvailability.Open)
        {
            Subsection newSubsection = _subsectionsGrid[randomSubsection.GetSubsectionRow() + 1, randomSubsection.GetSubsectionCol()];
            _nextSubsections.Add(newSubsection);
            AddPercentage();
        }
        if (east == DirectionAvailability.Open)
        {
            Subsection newSubsection = _subsectionsGrid[randomSubsection.GetSubsectionRow(), randomSubsection.GetSubsectionCol() + 1];
            _nextSubsections.Add(newSubsection);
            AddPercentage();
        }
        if (west == DirectionAvailability.Open)
        {
            Subsection newSubsection = _subsectionsGrid[randomSubsection.GetSubsectionRow(), randomSubsection.GetSubsectionCol() - 1];
            _nextSubsections.Add(newSubsection);
            AddPercentage();
        }
    }

    private void ConfigureNextSubsections()
    {
        List<Subsection> nextSubsectionsAux = new List<Subsection>();

        // Se itera sobre cada subsección en `_nextSubsections`.
        while (_nextSubsections.Count > 0)
        {
            Subsection currentSubsection = DequeueRandomSubsection();
            if(currentSubsection.GetTypeSubsection() == TypeSubsection.Start)
            {
                continue;
            }

            // Opción 1: Si hay muchas habitaciones seguidas.
            if (currentSubsection.GetParentsRooms() >= 4)
            {
                // Se decide entre pasillo o habitación de clausura.
                if (_rnd.Next(0, 2) == 0)
                {
                    List<Subsection> generatedSubsections = currentSubsection.SetCurrentCorridor(_subsectionsGrid);
                    foreach (var subsection in generatedSubsections)
                    {
                        if (subsection.GetTypeSubsection() == TypeSubsection.Empty)
                        {
                            nextSubsectionsAux.Add(subsection);
                            AddPercentage();
                        }
                    }
                }
                else
                {
                    currentSubsection.SetCloseRoom(_subsectionsGrid);
                }
                continue;
            }

            // Opción 2: Se elige en base a probabilidades.

            // Se calculan las probabilidades para habitación, pasillo o clausura.
            int queueSize = _nextSubsections.Count;
            float closeRoomProbability;
            float corridorProbability;
            float roomProbability;

            if (queueSize == balancePoint)
            {
                closeRoomProbability = 1f / 3f;
                corridorProbability = 1f / 3f;
                roomProbability = 1f / 3f;
            }
            else
            {
                closeRoomProbability = Mathf.Clamp01((float)(queueSize - balancePoint) / balancePoint);
                corridorProbability = Mathf.Clamp01((float)(balancePoint - queueSize) / balancePoint);
                roomProbability = 1f - closeRoomProbability - corridorProbability;
            }

            // Se elige aleatoriamente en función de las probabilidades.
            float randomValue = (float)_rnd.NextDouble();
            // Habitación de clausura.
            if (randomValue <= closeRoomProbability)
            {
                currentSubsection.SetCloseRoom(_subsectionsGrid);
            }
            // Pasillo.
            else if (randomValue <= closeRoomProbability + corridorProbability)
            {
                List<Subsection> generatedSubsections = currentSubsection.SetCurrentCorridor(_subsectionsGrid);
                foreach (var subsection in generatedSubsections)
                {
                    if (subsection.GetTypeSubsection() == TypeSubsection.Empty)
                    {
                        nextSubsectionsAux.Add(subsection);
                        AddPercentage();
                    }
                }
            }
            // Habitación.
            else
            {
                List<Subsection> generatedSubsections = currentSubsection.SetCurrentRoom(_subsectionsGrid);
                if (generatedSubsections != null)
                {
                    foreach (var subsection in generatedSubsections)
                    {
                        if (subsection.GetTypeSubsection() == TypeSubsection.Empty)
                        {
                            nextSubsectionsAux.Add(subsection);
                            AddPercentage();
                        }
                    }
                }
            }
        }

        _nextSubsections = nextSubsectionsAux;
    }

    private void CompleteMap()
    {
        while (_nextSubsections.Count > 0)
        {
            Subsection nextSubsection = DequeueRandomSubsection();
            if(nextSubsection.GetTypeSubsection() == TypeSubsection.Empty)
            {
                nextSubsection.SetCloseRoom(_subsectionsGrid);
            }
        }

        PlaceExit();
    }

    private Subsection DequeueRandomSubsection()
    {
        int index = _rnd.Next(_nextSubsections.Count);
        Subsection subsectionToReturn = _nextSubsections[index];
        _nextSubsections.RemoveAt(index);
        return subsectionToReturn;
    }

    // TODO
    private void PlaceExit()
    {
        // Recorrer las filas desde la última hacia abajo.
        for (int row = _SubsectionsGridSize.x - 1; row >= 0; row--)
        {
            // Se crea una lista de subsecciones de la fila actual.
            List<Subsection> roomSubsections = new List<Subsection>();

            for (int col = 0; col < _SubsectionsGridSize.y; col++)
            {
                Subsection subsection = _subsectionsGrid[row, col];
                if (subsection.GetTypeSubsection() == TypeSubsection.Room || subsection.GetTypeSubsection() == TypeSubsection.Corridor)
                {
                    roomSubsections.Add(subsection);
                }
            }

            // Si se encuentran subsecciones marcadas como Room o Corridor se selecciona una aleatoriamente.
            if (roomSubsections.Count > 0)
            {
                roomSubsections = roomSubsections.OrderBy(x => _rnd.Next()).ToList();
                Subsection selectedSubsection = roomSubsections[0];
                selectedSubsection.SetAsEnd();

                return;
            }
        }
    }
    #endregion

    #region Instantiation
    private void InstantiateAll()
    {
        foreach (Subsection subsection in _subsectionsGrid)
        {
            switch (subsection.GetTypeSubsection())
            {
                case TypeSubsection.Room:
                    InstantiateRoom(subsection);
                    break;
                case TypeSubsection.Corridor:
                    ConfigureCorridor(subsection);
                    break;
                case TypeSubsection.Start:
                    ConfigureStart(subsection);
                    break;
                case TypeSubsection.End:
                    ConfigureEnd(subsection);
                    break;
            }
        }

        ConfigureInternalCorridors();

        foreach (Cell cell in _cellsGrid)
        {
            if (cell.State == CellState.Start)
            {
                // Configura las aperturas y puertas de la celda.
                Cell aboveNeighbour = AuxiliarMapGenerator.GetAboveNeighbour(cell, _cellsGrid);
                Cell belowNeighbour = AuxiliarMapGenerator.GetBelowNeighbour(cell, _cellsGrid);
                Cell rightNeighbour = AuxiliarMapGenerator.GetRightNeighbour(cell, _cellsGrid);
                Cell leftNeighbour = AuxiliarMapGenerator.GetLeftNeighbour(cell, _cellsGrid);
                cell.SetCorridorConfiguration(aboveNeighbour, belowNeighbour, rightNeighbour, leftNeighbour, _cellsGrid);

                // Instancia el prefab de estación que equivalga a la configuración de la celda.
                Vector3 centeredPosition = new Vector3(cell.Position3D.x + 4, cell.Position3D.y + 4, cell.Position3D.z);
                GameObject stationInstance = Instantiate(_stationsBase, centeredPosition, _stationsBase.transform.rotation);
                stationInstance.name = $"StationStart_{cell.Row}x{cell.Col}_{cell.north}{cell.south}{cell.east}{cell.west}";
                // stationInstance.transform.SetParent(GameObject.Find("Stations").transform);
                StationController stationController = stationInstance.GetComponentInChildren<StationController>();
                stationController.WallSelector(cell.north, cell.south, cell.east, cell.west);
            }

            if (cell.State == CellState.End)
            {
                // Configura las aperturas y puertas de la celda.
                Cell aboveNeighbour = AuxiliarMapGenerator.GetAboveNeighbour(cell, _cellsGrid);
                Cell belowNeighbour = AuxiliarMapGenerator.GetBelowNeighbour(cell, _cellsGrid);
                Cell rightNeighbour = AuxiliarMapGenerator.GetRightNeighbour(cell, _cellsGrid);
                Cell leftNeighbour = AuxiliarMapGenerator.GetLeftNeighbour(cell, _cellsGrid);
                cell.SetCorridorConfiguration(aboveNeighbour, belowNeighbour, rightNeighbour, leftNeighbour, _cellsGrid);

                // Instancia el prefab de estación que equivalga a la configuración de la celda.
                Vector3 centeredPosition = new Vector3(cell.Position3D.x + 4, cell.Position3D.y + 4, cell.Position3D.z);
                GameObject stationInstance = Instantiate(_stationsBase, centeredPosition, _stationsBase.transform.rotation);
                stationInstance.name = $"StationEnd_{cell.Row}x{cell.Col}_{cell.north}{cell.south}{cell.east}{cell.west}";
                // stationInstance.transform.SetParent(GameObject.Find("Stations").transform);
                StationController stationController = stationInstance.GetComponentInChildren<StationController>();
                stationController.WallSelector(cell.north, cell.south, cell.east, cell.west);
            }

            if (cell.State == CellState.Corridor)
            {
                // Configura las aperturas y puertas de la celda.
                Cell aboveNeighbour = AuxiliarMapGenerator.GetAboveNeighbour(cell, _cellsGrid);
                Cell belowNeighbour = AuxiliarMapGenerator.GetBelowNeighbour(cell, _cellsGrid);
                Cell rightNeighbour = AuxiliarMapGenerator.GetRightNeighbour(cell, _cellsGrid);
                Cell leftNeighbour = AuxiliarMapGenerator.GetLeftNeighbour(cell, _cellsGrid);
                cell.SetCorridorConfiguration(aboveNeighbour, belowNeighbour, rightNeighbour, leftNeighbour, _cellsGrid);

                // Instancia el prefab de pasillo que equivalga a la configuración de la celda.
                Vector3 centeredPosition = new Vector3(cell.Position3D.x + 4, cell.Position3D.y + 4, cell.Position3D.z);
                GameObject corridorInstance = Instantiate(_corridorBase, centeredPosition, _corridorBase.transform.rotation);
                corridorInstance.name = $"Corridor_{cell.Row}x{cell.Col}_{cell.north}{cell.south}{cell.east}{cell.west}";
                //corridorInstance.transform.SetParent(GameObject.Find("Corridors").transform);
                CorridorController corridorController = corridorInstance.GetComponentInChildren<CorridorController>();
                corridorController.WallSelector(cell.north, cell.south, cell.east, cell.west);
            }
        }
    }

    // TODO: hacer que se genere una celda de pasillo en frente de la entrada.
    private void InstantiateRoom(Subsection subsection)
    {
        Debug.Log($"-test- Subsection[{subsection.GetSubsectionRow()}, {subsection.GetSubsectionCol()}]");
        RoomWithConfiguration currentRoomWithConfiguration = subsection.GetCurrentRoom();
        GameObject roomPrefab = currentRoomWithConfiguration.roomPrefab;

        GameObject roomObject = Instantiate(roomPrefab, Vector3.zero, roomPrefab.transform.rotation);
        // roomObject.transform.SetParent(GameObject.Find("Rooms").transform, false);  
        Room room = roomObject.GetComponent<Room>();
        room.InitializeRoom(currentRoomWithConfiguration.configurationIndex);
        room.MoveRoomPosition(subsection.GetCenterPosition());

        Debug.Log($"-test- Room name = {roomObject.name} [{subsection.GetSubsectionRow()}, {subsection.GetSubsectionCol()}]");
    }

    private void ConfigureCorridor(Subsection subsection)
    {
        Vector2Int centerCell = subsection.GetGlobalCell(1, 1);
        _cellsGrid[centerCell.x, centerCell.y].SetCellState(CellState.Corridor);

        if (subsection.GetNorthAvailability() == DirectionAvailability.Open)
        {
            _cellsGrid[centerCell.x + 1, centerCell.y].SetCellState(CellState.Corridor);
            _cellsGrid[centerCell.x + 2, centerCell.y].SetCellState(CellState.Corridor);
        }
        if (subsection.GetSouthAvailability() == DirectionAvailability.Open)
        {
            Debug.Log($"[{centerCell.x}, {centerCell.y}]");
            _cellsGrid[centerCell.x - 1, centerCell.y].SetCellState(CellState.Corridor);
            _cellsGrid[centerCell.x - 2, centerCell.y].SetCellState(CellState.Corridor);
        }
        if (subsection.GetEastAvailability() == DirectionAvailability.Open)
        {
            _cellsGrid[centerCell.x, centerCell.y + 1].SetCellState(CellState.Corridor);
            _cellsGrid[centerCell.x, centerCell.y + 2].SetCellState(CellState.Corridor);
        }
        if (subsection.GetWestAvailability() == DirectionAvailability.Open)
        {
            _cellsGrid[centerCell.x, centerCell.y - 1].SetCellState(CellState.Corridor);
            _cellsGrid[centerCell.x, centerCell.y - 2].SetCellState(CellState.Corridor);
        }
    }

    private void ConfigureInternalCorridors()
    {
        // Filas
        for (int row = 0; row < _subsectionsGrid.GetLength(0) - 1; row++)
        {
            for (int col = 0; col < _subsectionsGrid.GetLength(1); col++)
            {
                Subsection currentSubsection = _subsectionsGrid[row, col];
                Vector2Int startingCell = currentSubsection.GetGlobalCell(3, 0);
                List<Cell> cellsToConnect = new List<Cell>();

                for (int cellCol = startingCell.y; cellCol < startingCell.y + 3; cellCol++)
                {
                    Cell cell = _cellsGrid[startingCell.x, cellCol];
                    if (cell.State == CellState.Corridor)
                    {
                        cellsToConnect.Add(cell);
                    }
                }

                ConnectCells(cellsToConnect);
                cellsToConnect.Clear();
            }
        }

        // Columnas
        for (int col = 0; col < _subsectionsGrid.GetLength(1) - 1; col++)
        {
            for (int row = 0; row < _subsectionsGrid.GetLength(0); row++)
            {
                Subsection currentSubsection = _subsectionsGrid[row, col];
                Vector2Int startingCell = currentSubsection.GetGlobalCell(0, 3);
                List<Cell> cellsToConnect = new List<Cell>();

                for (int cellRow = startingCell.x; cellRow < startingCell.x + 3; cellRow++)
                {
                    Cell cell = _cellsGrid[cellRow, startingCell.y];
                    if (cell.State == CellState.Corridor)
                    {
                        cellsToConnect.Add(cell);
                    }
                }

                ConnectCells(cellsToConnect);
                cellsToConnect.Clear();
            }
        }

    }

    private void ConnectCells(List<Cell> cellsToConnect)
    {
        if (cellsToConnect.Count < 2)
        {
            return;
        }

        bool isSameCol = false;
        int prevCol = -1;

        foreach (Cell cell in cellsToConnect)
        {
            if (prevCol == -1)
            {
                prevCol = cell.Col;
            }
            else if (prevCol == cell.Col)
            {
                isSameCol = true;
            }
            else
            {
                isSameCol = false;
            }
        }

        if (isSameCol)
        {
            for (int row = cellsToConnect[0].Row; row < cellsToConnect[cellsToConnect.Count() - 1].Row; row++)
            {
                _cellsGrid[row, cellsToConnect[0].Col].SetCellState(CellState.Corridor);
            }
        }
        else
        {
            for (int col = cellsToConnect[0].Col; col < cellsToConnect[cellsToConnect.Count() - 1].Col; col++)
            {
                _cellsGrid[cellsToConnect[0].Row, col].SetCellState(CellState.Corridor);
            }
        }
    }

    private void ConfigureStart(Subsection subsection)
    {
        Vector2Int baseCell = new Vector2Int(0, 0);

        if (subsection.GetNorthAvailability() == DirectionAvailability.Open)
        {
            baseCell = subsection.GetGlobalCell(1, 1);
            _cellsGrid[baseCell.x, baseCell.y].SetCellState(CellState.Corridor);
            _cellsGrid[baseCell.x + 1, baseCell.y].SetCellState(CellState.Corridor);
            _cellsGrid[baseCell.x + 2, baseCell.y].SetCellState(CellState.Corridor);
        }
        if (subsection.GetEastAvailability() == DirectionAvailability.Open)
        {
            baseCell = subsection.GetGlobalCell(0, 2);
            _cellsGrid[baseCell.x, baseCell.y + 1].SetCellState(CellState.Corridor);
        }
        if (subsection.GetWestAvailability() == DirectionAvailability.Open)
        {
            baseCell = subsection.GetGlobalCell(0, 0);
            _cellsGrid[baseCell.x, baseCell.y - 1].SetCellState(CellState.Corridor);
        }

        baseCell = subsection.GetGlobalCell(0, 1);
        _cellsGrid[baseCell.x, baseCell.y].SetCellState(CellState.Start);
        _cellsGrid[baseCell.x, baseCell.y - 1].SetCellState(CellState.Start);
        _cellsGrid[baseCell.x, baseCell.y + 1].SetCellState(CellState.Start);
    }

    // TODO: borrar estado de celdas de la habitacion previa.
    private void ConfigureEnd(Subsection subsection)
    {
        Vector2Int baseCell = new Vector2Int(0, 0);

        if (subsection.GetSouthAvailability() == DirectionAvailability.Open)
        {
            baseCell = subsection.GetGlobalCell(1, 1);
            Debug.Log($"-test- end [{baseCell.x}, {baseCell.y}]");
            _cellsGrid[baseCell.x, baseCell.y].SetCellState(CellState.Corridor);
            _cellsGrid[baseCell.x - 1, baseCell.y].SetCellState(CellState.Corridor);
            _cellsGrid[baseCell.x - 2, baseCell.y].SetCellState(CellState.Corridor);
        }
        else
        {
            Debug.Log($"-test- end [{subsection.GetSubsectionRow()}, {subsection.GetSubsectionCol()}] tipo {subsection.GetSouthAvailability()}");
        }
        if (subsection.GetEastAvailability() == DirectionAvailability.Open)
        {
            baseCell = subsection.GetGlobalCell(2, 2);
            Debug.Log($"-test- end [{baseCell.x}, {baseCell.y}]");
            _cellsGrid[baseCell.x, baseCell.y + 1].SetCellState(CellState.Corridor);
        }
        else
        {
            Debug.Log($"-test- end tipo {subsection.GetEastAvailability()}");
        }
        if (subsection.GetWestAvailability() == DirectionAvailability.Open)
        {
            baseCell = subsection.GetGlobalCell(2, 0);
            Debug.Log($"-test- end [{baseCell.x}, {baseCell.y}]");
            _cellsGrid[baseCell.x, baseCell.y - 1].SetCellState(CellState.Corridor);
        }
        else
        {
            Debug.Log($"-test- end tipo {subsection.GetWestAvailability()}");
        }

        baseCell = subsection.GetGlobalCell(2, 1);
        _cellsGrid[baseCell.x, baseCell.y].SetCellState(CellState.End);
        _cellsGrid[baseCell.x, baseCell.y - 1].SetCellState(CellState.End);
        _cellsGrid[baseCell.x, baseCell.y + 1].SetCellState(CellState.End);
    }
    #endregion

    #region Grids Initialization
    private void InitializeGrid()
    {
        int totalRows = _SubsectionsGridSize.y * 3 + (_SubsectionsGridSize.y - 1);
        int totalCols = _SubsectionsGridSize.x * 3 + (_SubsectionsGridSize.x - 1);

        _cellsGrid = new Cell[totalRows, totalCols];

        for (int row = 0; row < totalRows; row++)
        {
            for (int col = 0; col < totalCols; col++)
            {
                _cellsGrid[row, col] = new Cell(row, col);
                if ((row + 1) % 4 == 0 && (col + 1) % 4 == 0)
                {
                    _cellsGrid[row, col].SetCellState(CellState.InternalCorridor);
                }
            }
        }
    }

    private void InitializeSubsections()
    {
        _subsectionsGrid = new Subsection[_SubsectionsGridSize.y, _SubsectionsGridSize.x];

        for (int row = 0; row < _SubsectionsGridSize.y; row++)
        {
            for (int col = 0; col < _SubsectionsGridSize.x; col++)
            {
                // Se calcula la posición inicial de cada subsección, considerando el perímetro compartido.
                Vector2Int startingCell = new Vector2Int(
                    row * (3 + 1),
                    col * (3 + 1)
                );

                _subsectionsGrid[row, col] = new Subsection(
                    startingCell,
                    row,
                    col,
                    _roomFinder
                    );
            }
        }

        // Filas
        for (int row = 0; row < _subsectionsGrid.GetLength(0) - 1; row++)
        {
            for (int col = 0; col < _subsectionsGrid.GetLength(1); col++)
            {
                Subsection currentSubsection = _subsectionsGrid[row, col];
                Vector2Int startingCell = currentSubsection.GetGlobalCell(3, 0);

                for (int cellCol = startingCell.y; cellCol < startingCell.y + 3; cellCol++)
                {
                    Cell cell = _cellsGrid[startingCell.x, cellCol];
                    cell.SetCellState(CellState.InternalCorridor);
                }
            }
        }

        // Columnas
        for (int col = 0; col < _subsectionsGrid.GetLength(1) - 1; col++)
        {
            for (int row = 0; row < _subsectionsGrid.GetLength(0); row++)
            {
                Subsection currentSubsection = _subsectionsGrid[row, col];
                Vector2Int startingCell = currentSubsection.GetGlobalCell(0, 3);

                for (int cellRow = startingCell.x; cellRow < startingCell.x + 3; cellRow++)
                {
                    Cell cell = _cellsGrid[cellRow, startingCell.y];
                    cell.SetCellState(CellState.InternalCorridor);
                }
            }
        }
    }
    #endregion

    #region Gizmos

    private void OnDrawGizmos()
    {
        if (_SubsectionsGridSize == Vector2Int.zero || cellSize == 0 || _cellsGrid == null || !_isDrawingGizmos)
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
        for (int row = 0; row < _cellsGrid.GetLength(0); row++)
        {
            for (int col = 0; col < _cellsGrid.GetLength(1); col++)
            {
                yield return _cellsGrid[row, col];
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
                Gizmos.color = UnityEngine.Color.cyan;
                break;
            case CellState.FillingRoom:
                Gizmos.color = UnityEngine.Color.magenta;
                break;
            case CellState.Corridor:
                Gizmos.color = UnityEngine.Color.blue;
                break;
            case CellState.InternalCorridor:
                Gizmos.color = UnityEngine.Color.black;
                break;
            case CellState.Start:
                Gizmos.color = UnityEngine.Color.white;
                break;
            case CellState.End:
                Gizmos.color = UnityEngine.Color.white;
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
}