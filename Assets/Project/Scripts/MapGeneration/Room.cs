using System.Collections.Generic;
using UnityEngine;

public class Room : MonoBehaviour
{
    [Header("Configuración de la habitación")]
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

    [Header("Configuración de enemigos")]
    [SerializeField] private bool _manualConfigurationEnemies = false;
    [SerializeField] private int _minAmountEnemiesToSpawn = 0;
    [SerializeField] private int _maxAmountEnemiesToSpawn = 0;
    [SerializeField] private int _minAmountCamerasToSpawn = 0;
    [SerializeField] private int _maxAmountCamerasToSpawn = 0;
    private GameObject _currentConfiguration;

    void Start()
    {
        rnd = new System.Random();
    }

    void OnValidate()
    {
        if (_roomDataList == null)
        {
            Debug.LogError("El ScriptableObject 'roomData' no ha sido asignado.");
        }
    }
    public int GetCountRoomData()
    {
        return _roomDataList.Count;
    }

    public void SetRoomData(int roomDataSelected)
    {
        _configurationSelected = roomDataSelected;

        _roomSize = _roomDataList[_configurationSelected].roomSize;
        _shape = _roomDataList[_configurationSelected].GetShape();
        _entrances = _roomDataList[_configurationSelected].GetEntrances();
        _entrancesDirections = _roomDataList[_configurationSelected].entrancesDirections;
    }

    public bool InitializeRoom(int configurationSelected)
    {
        SetRoomData(configurationSelected);

        CalculateCenterPosition();
        return CreateSelfGrid();
    }

    public void MoveRoomPosition(Vector3 positionSelected)
    {
        _positionSelected = positionSelected;
        transform.position = _positionSelected;

        _currentConfiguration = null;
        for (int i = 1; i <= _roomDataList.Count; i++)
        {
            _currentConfiguration = transform.Find($"Configuration_{i}").gameObject;

            if (i - 1 == _configurationSelected)
            {
                _currentConfiguration.SetActive(true);
            }
            else
            {
                _currentConfiguration.SetActive(false);
            }
        }

        Transform aux = transform.Find($"AUX_Configuration_base");
        if (aux != null)
        {
            aux.gameObject.SetActive(false);
        }
        Transform gas = transform.Find($"Gas");
        if (gas != null)
        {
            gas.gameObject.SetActive(true);
        }
        Transform gasHigh = transform.Find($"GasHigh");
        if (gasHigh != null)
        {
            gasHigh.gameObject.SetActive(true);
        }

        // Colocación enemigos.
        PlaceRandomEnemies("Enemies", _minAmountEnemiesToSpawn, _maxAmountEnemiesToSpawn);
        PlaceRandomEnemies("Cameras", _minAmountEnemiesToSpawn, _maxAmountEnemiesToSpawn);
    }

    private void PlaceRandomEnemies(string element, int minValue, int maxValue)
    {
        GameObject EnemiesParent = null;
        Transform elementTransform = gameObject.transform.Find($"{element}");
        if (elementTransform == null)
        {
            return;
        }
        EnemiesParent = elementTransform.gameObject;

        List<GameObject> allEnemiesList = new List<GameObject>();
        for (int enemyIndex = 0; enemyIndex < EnemiesParent.transform.childCount; enemyIndex++)
        {
            allEnemiesList.Add(EnemiesParent.transform.GetChild(enemyIndex).gameObject);
        }
        if (allEnemiesList.Count == 0)
        {
            return;
        }

        //allEnemiesList = allEnemiesList.OrderBy(x => UnityEngine.Random.value).ToList();
        //int definitiveAmountEnemies = 0;
        //if (_manualConfigurationEnemies == true)
        //{
        //    definitiveAmountEnemies = UnityEngine.Random.Range(minValue, maxValue + 1);
        //}
        //else
        //{
        //    definitiveAmountEnemies = UnityEngine.Random.Range(1, allEnemiesList.Count);
        //}


        foreach (GameObject enemy in allEnemiesList)
        {
            enemy.SetActive(true);
        }
    }

    private void CalculateCenterPosition()
    {
        float centerPosition_x = 0;
        float centerPosition_y = 0;

        // Columnas.
        if (_shape.Cols == 1)
        {
            centerPosition_x = MapGenerator.Instance.cellSize / 2;
        }
        else if (_shape.Cols % 2 == 0)
        {
            centerPosition_x = MapGenerator.Instance.cellSize * _shape.Cols / 2 - MapGenerator.Instance.cellSize / 2;
        }
        else
        {
            centerPosition_x = MapGenerator.Instance.cellSize * _shape.Cols / 2;
        }

        // Filas.
        if (_shape.Rows == 1)
        {
            centerPosition_y = MapGenerator.Instance.cellSize / 2;
        }
        else if (_shape.Rows % 2 == 0)
        {
            centerPosition_y = MapGenerator.Instance.cellSize * _shape.Rows / 2 - MapGenerator.Instance.cellSize / 2;
        }
        else
        {
            centerPosition_y = MapGenerator.Instance.cellSize * _shape.Rows / 2;
        }

        centerPosition = new Vector3(centerPosition_x, centerPosition_y, centerPosition.z);
    }

    private bool CreateSelfGrid()
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
                    if (numberEntrances > 1)
                    {
                        selfGrid[row, col].SetCellState(CellState.CorridorRoom);
                    }
                    else
                    {
                        selfGrid[row, col].SetCellState(CellState.Room);
                    }
                }
                else if (_shape.GetValue(col, row) == false)
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

        return true;
    }

    private int GetCols()
    {
        return _roomSize.x;
    }

    private int GetRows()
    {
        return _roomSize.y;
    }

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

    public Dictionary<Vector2Int, DirectionFlag> GetEntrancesDirections()
    {
        return _entrancesDirections;
    }
}
