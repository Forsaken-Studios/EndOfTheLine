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
    [SerializeField] private RoomsDataBase _roomsDataBase;

    [HideInInspector] public Vector3 startingGridPosition = Vector3.zero;
    [HideInInspector] public int endWall;
    private Cell[,] _cellsGrid; // [row, col] GetLength(0), GetLength(1)
    private System.Random _rnd;
    private Subsection[,] _subsectionsGrid; // [row, col] GetLength(0), GetLength(1)
    private List<Subsection> _nextSubsections;
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

        _subsectionMapPercentage = 100 / (_SubsectionsGridSize.x * _SubsectionsGridSize.y);

        InitializeGrid();
        InitializeSubsections();

        StartCoroutine(GenerateMap());
    }

    #region Generation
    private IEnumerator GenerateMap()
    {
        // GameManager.Instance.sceneIsLoading = true;  // TODO
        yield return null;

        PlaceStart();

        while (_currentMapPercentageOccupied < _maxMapPercentageOcuppied)
        {
            ConfigureNextSubsections();

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
        int randomColumn = _rnd.Next(0, _SubsectionsGridSize.x);
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
        else if (randomColumn == _SubsectionsGridSize.x - 1)
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

        // Se marca la subsection como Start.
        randomSubsection.SetAsStart(north, east, west);
        AddPercentage();
        if(north == DirectionAvailability.Open)
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

            // Opción 1: Si hay muchas habitaciones seguidas.
            if (currentSubsection.GetParentsRooms() >= 4)
            {
                // Se decide entre pasillo o habitación de clausura.
                if (_rnd.Next(0, 2) == 0)
                {
                    List<Subsection> generatedSubsections = currentSubsection.SetCurrentCorridor(_subsectionsGrid);
                    foreach (var subsection in generatedSubsections)
                    {
                        nextSubsectionsAux.Add(subsection);
                        AddPercentage();
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
                    nextSubsectionsAux.Add(subsection);
                    AddPercentage();
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
                        nextSubsectionsAux.Add(subsection);
                        AddPercentage();
                    }
                }
            }
        }

        _nextSubsections = nextSubsectionsAux;
    }
    #endregion

    private void CompleteMap()
    {
        while (_nextSubsections.Count > 0)
        {
            Subsection nextSubsection = DequeueRandomSubsection();
            nextSubsection.SetCloseRoom(_subsectionsGrid);
        }

        PlaceExit();
    }

    private Subsection DequeueRandomSubsection()
    {
        _nextSubsections = _nextSubsections.OrderBy(x => _rnd.Next()).ToList();
        Subsection subsectionToReturn = _nextSubsections[0];
        _nextSubsections.RemoveAt(0);
        return subsectionToReturn;
    }

    private void PlaceExit()
    {
        // Recorrer las filas desde la última hacia abajo.
        for (int row = _SubsectionsGridSize.y - 1; row >= 0; row--)
        {
            // Se crea una lista de subsecciones de la fila actual.
            List<Subsection> roomSubsections = new List<Subsection>();

            for (int col = 0; col < _SubsectionsGridSize.x; col++)
            {
                Subsection subsection = _subsectionsGrid[row, col];
                if (subsection.GetTypeSubsection() == TypeSubsection.Room)
                {
                    roomSubsections.Add(subsection);
                }
            }

            // Si se encuentran subsecciones marcadas como Room se selecciona una aleatoriamente.
            if (roomSubsections.Count > 0)
            {
                roomSubsections = roomSubsections.OrderBy(x => _rnd.Next()).ToList();
                Subsection selectedSubsection = roomSubsections[0];
                selectedSubsection.SetAsEnd();

                return;
            }
        }
    }

    private void InstantiateAll()
    {
        foreach (Subsection subsection in _subsectionsGrid)
        {
            switch (subsection.GetTypeSubsection())
            {
                case TypeSubsection.Room:
                    InstantiateRoom();
                    break;
                case TypeSubsection.Corridor:
                    InstantiateCorridor();
                    break;
            }
        }
    }

    private void InstantiateRoom()
    {

    }

    private void InstantiateCorridor()
    {

    }

    #region Grids Initialization
    private void InitializeGrid()
    {
        int totalRows = _SubsectionsGridSize.y * 3 + (_SubsectionsGridSize.y - 1);
        int totalCols = _SubsectionsGridSize.x * 3 + (_SubsectionsGridSize.x - 1);

        _cellsGrid = new Cell[totalRows, totalCols];

        for (int row = 0; row < totalRows; row++)
        {
            for (int column = 0; column < totalCols; column++)
            {
                _cellsGrid[row, column] = new Cell(row, column);
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
                Vector2Int startingPosition = new Vector2Int(
                    row * (3 + 1),
                    col * (3 + 1)
                );

                _subsectionsGrid[row, col] = new Subsection(
                    startingPosition,
                    row,
                    col,
                    _roomsDataBase
                    );
            }
        }
    }
    #endregion
}