using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;

public static class AuxiliarMapGenerator
{
    #region A Estrella

    [Header("Parameters")]
    private static List<Cell> OpenNodes = new List<Cell>();
    private static List<Cell> ClosedNodes = new List<Cell>();
    private static List<Cell> FinalPath = new List<Cell>();

    // Given a cell and a grid it tries to create the shortest path for the cell with a A* algorithm.
    public static Cell[,] FindPath(List<Cell> destinationCellList, Cell[,] grid, bool isPointToPoint = false, List<Cell> possibleOriginCells = null)
    {
        Cell[,] copiedGrid = CopyGrid(grid);
        if(isPointToPoint == false)
        {
            possibleOriginCells = GetPossibleStartCells(copiedGrid);
        }

        List<Cell> destinationCellListCopied = new List<Cell>();
        foreach (Cell cell in destinationCellList)
        {
            destinationCellListCopied.Add(copiedGrid[cell.Row, cell.Col]);
        }

        List<Cell> possibleOriginCellsCopied = new List<Cell>();
        foreach (Cell cell in possibleOriginCells)
        {
            possibleOriginCellsCopied.Add(copiedGrid[cell.Row, cell.Col]);
        }


        foreach (Cell startCell in possibleOriginCellsCopied)
        {
            OpenNodes.Clear();
            ClosedNodes.Clear();
            FinalPath.Clear();

            foreach (Cell cell in copiedGrid)
            {
                cell.ClearTemporalParent();
            }

            OpenNodes.Add(startCell);

            while (OpenNodes.Count > 0)
            {
                Cell currentCell = MostPromisingNode();

                // Si llegamos a la celda final
                foreach (Cell endCell in destinationCellListCopied)
                {
                    if (currentCell.Row == endCell.Row && currentCell.Col == endCell.Col)
                    {
                        CreateFinalPath(startCell, currentCell);
                        Debug.Log($"Camino encontrado a [{currentCell.Row}, {currentCell.Col}]");
                        return copiedGrid;
                    }
                }

                // Obtener los vecinos de la celda actual
                List<Cell> neighbours = GetNeighbours(currentCell, copiedGrid, destinationCellListCopied);

                foreach (Cell cell in neighbours)
                {
                    if (ClosedNodes.Contains(cell) || cell.State == CellState.Room || cell.State == CellState.Start || cell.State == CellState.End)
                        continue;

                    int stateCost = 0;
                    if(isPointToPoint == false)
                    {
                        stateCost = GetUsualStateCost(cell);
                    }
                    else
                    {
                        stateCost = GetFinalStateCost(cell);
                    }
                    

                    if (!OpenNodes.Contains(cell))
                    {
                        cell.ig = currentCell.ig + stateCost;
                        cell.ih = GetManhattanDistance(cell, destinationCellListCopied[0]);
                        cell.TemporalParent = currentCell;

                        OpenNodes.Add(cell);
                    }
                    else
                    {
                        if (cell.ig > currentCell.ig + stateCost)
                        {
                            cell.TemporalParent = currentCell;
                            cell.ig = currentCell.ig + stateCost;
                        }
                    }
                }

                ClosedNodes.Add(currentCell);
                OpenNodes.Remove(currentCell);
            }
        }

        return null;
    }

    private static int GetUsualStateCost(Cell cell)
    {
        int cost = 10;

        if (cell.State == CellState.Corridor || cell.State == CellState.CorridorRoom || cell.State == CellState.EntranceRoom)
        {
            cost = 1;
        }
        else if (cell.State == CellState.Empty)
        {
            cost = 10;
        }

        return cost;
    }

    private static int GetFinalStateCost(Cell cell)
    {
        int cost = 10;

        if (cell.State == CellState.Corridor)
        {
            cost = 1;
        }
        else if (cell.State == CellState.Empty)
        {
            cost = 10;
        }else if (cell.State == CellState.CorridorRoom || cell.State == CellState.EntranceRoom)
        {
            cost = 50;
        }

        return cost;
    }

    private static void CreateFinalPath(Cell start, Cell end)
    {
        Cell currentCell = end;

        start.SetInFinalPath();

        Debug.Log($"FinalPath ----------------------");
        while (currentCell != start)
        {
            if (currentCell.TemporalParent == null)
            {
                break; // Salir del bucle si no hay TemporalParent asignado
            }

            Debug.Log($"FinalPath: [{currentCell.Row}, {currentCell.Col}]");
            currentCell.SetInFinalPath();
            currentCell = currentCell.TemporalParent;


            if (!currentCell.DestinationCells.Contains(end))
            {
                currentCell.AddDestinationCell(end);
            }
        }
    }


    private static int GetManhattanDistance(Cell node, Cell endNode)
    {
        int distanciaX = Mathf.Abs(node.Col - endNode.Col);
        int distanciaY = Mathf.Abs(node.Row - endNode.Row);

        return distanciaX + distanciaY;
    }

    private static Cell MostPromisingNode()
    {
        if (OpenNodes.Count <= 0)
        {
            return null;
        }

        Cell bestNode = OpenNodes[0];

        foreach (Cell auxNode in OpenNodes)
        {
            if (auxNode.iF < bestNode.iF) //|| (auxNode.iF == bestNode.iF && auxNode.ih < bestNode.ih))
                bestNode = auxNode;
        }

        return bestNode;

    }
    #endregion

    #region Auxiliar functions

    // Given a cell and a list it deletes all the paths of the cell.
    public static void DeletePaths(List<Cell> roomCells, List<Cell> endCells)
    {
        foreach (Cell currentCell in roomCells)
        {
            foreach (Cell destinationCell in currentCell.DestinationCells)
            {
                if (!endCells.Contains(destinationCell))
                {
                    endCells.Add(destinationCell);
                }
            }
        }

        List<Cell> visitedParents = new List<Cell>();
        foreach (Cell endCell in endCells)
        {
            DeleteParents(endCell, visitedParents, roomCells);
        }
    }

    // Given a cell and a list it deletes all the parents of the cell.
    private static void DeleteParents(Cell currentCell, List<Cell> visitedParents, List<Cell> roomCells)
    {
        // Verificar si la celda ya ha sido visitada
        if (visitedParents.Contains(currentCell))
        {
            return;
        }

        visitedParents.Add(currentCell);

        // Si la celda no tiene padres, marcarla como vacía
        if (currentCell.Parents.Count == 0 && !roomCells.Contains(currentCell))
        {
            currentCell.SetCellState(CellState.Empty);
        }
        else
        {
            // Recorrer los padres de la celda
            foreach (var parent in currentCell.Parents)
            {
                DeleteParents(parent, visitedParents, roomCells);
            }
        }

        // Si no está en roomCells, marcarla como vacía
        if (!roomCells.Contains(currentCell))
        {
            currentCell.SetCellState(CellState.Empty);
        }

        // Limpiar referencias
        currentCell.Parents.Clear();
        currentCell.ClearTemporalParent();
        currentCell.DestinationCells.Clear();
    }

    private static List<Cell> GetVerticalNeighbours(Cell cell, Cell[,] grid)
    {
        List<Cell> neighbors = new List<Cell>();
        int row = cell.Row;
        int col = cell.Col;
        //Checking to the above
        if ((row + 1 < grid.GetLength(0)))
            neighbors.Add(grid[row + 1, col]);
        //Checking to the below
        if ((row - 1 >= 0))
            neighbors.Add(grid[row - 1, col]);
        return neighbors;
    }

    private static List<Cell> GetHorizontalNeighbours(Cell cell, Cell[,] grid)
    {
        List<Cell> neighbors = new List<Cell>();
        int row = cell.Row;
        int col = cell.Col;
        //Checking right
        if ((col + 1 < grid.GetLength(1)))
            neighbors.Add(grid[row, col + 1]);
        //Checking left
        if ((col - 1 >= 0))
            neighbors.Add(grid[row, col - 1]);
        return neighbors;
    }

    public static List<Cell> GetNeighbours(Cell cell, Cell[,] grid, List<Cell> endCells = null)
    {
        List<Cell> neighbors = new List<Cell>();
        int row = cell.Row;
        int col = cell.Col;

        foreach (Cell neighbourCell in GetHorizontalNeighbours(cell, grid))
        {
            if (cell.CanConnectTo(neighbourCell, endCells))
            {
                neighbors.Add(neighbourCell);
            }
        }

        foreach (Cell neighbourCell in GetVerticalNeighbours(cell, grid))
        {
            if (cell.CanConnectTo(neighbourCell, endCells))
            {
                neighbors.Add(neighbourCell);
            }
        }

        return neighbors;
    }

    // Given the grid it seek for possible start cells.
    private static List<Cell> GetPossibleStartCells(Cell[,] grid)
    {
        List<Cell> possibleStartCells = new List<Cell>();

        for (int col = 0; col < grid.GetLength(1); col++)
        {
            if (grid[0, col].State == CellState.Start)
            {
                List<Cell> neighboursList = GetNeighbours(grid[0, col], grid);
                foreach (Cell neighbour in neighboursList)
                {
                    possibleStartCells.Add(neighbour);
                }
            }
        }

        // List is randomly sorted.
        System.Random rng = new System.Random();
        possibleStartCells = possibleStartCells.OrderBy(a => rng.Next()).ToList();

        return possibleStartCells;
    }

    // Given the grid it seek for possible end cells.
    public static List<Cell> GetPossibleEndCells(Cell[,] grid)
    {
        List<Cell> possibleEndCells = new List<Cell>();

        for(int row = 0; row < grid.GetLength(0); row++)
        {
            for (int col = 0; col < grid.GetLength(1); col++)
            {
                if (grid[row, col].State == CellState.End)
                {
                    List<Cell> neighboursList = GetNeighbours(grid[row, col], grid);
                    foreach (Cell neighbour in neighboursList)
                    {
                        possibleEndCells.Add(neighbour);
                    }
                }
            }
        }

        // List is randomly sorted.
        System.Random rng = new System.Random();
        possibleEndCells = possibleEndCells.OrderBy(a => rng.Next()).ToList();

        return possibleEndCells;
    }

    // Given a list of cells and a grid it tries to re-calculate a new path to the same destination for those cells.
    public static Cell[,] ReFindPath(List<Cell> cellsToRecalculate, Cell[,] grid)
    {
        Cell[,] copiedGrid = CopyGrid(grid);
        bool isReCalculated = false;
        List<Cell> endCells = new List<Cell>();

        List<Cell> cellsToRecalculateCopied = new List<Cell>();
        foreach(Cell cell in cellsToRecalculate)
        {
            cellsToRecalculateCopied.Add(copiedGrid[cell.Row, cell.Col]);
        }

        // Delete paths and get endCells
        DeletePaths(cellsToRecalculateCopied, endCells);

        // Recalculate paths from each endcell
        foreach (Cell endCell in endCells)
        {
            List<Cell> endCellList = new List<Cell>();
            endCellList.Add(endCell);

            if(endCellList.Count == 0)
            {
                isReCalculated = false;
                break;
            }
            Cell[,] modifiedGrid = FindPath(endCellList, copiedGrid);
            if (modifiedGrid != null)
            {
                copiedGrid = modifiedGrid;
                isReCalculated = true;
            }
            else
            {
                isReCalculated = false;
                break;
            }
        }

        if(endCells.Count == 0 || isReCalculated == true)
        {
            return copiedGrid;
        }
        else
        {
            return null;
        }
    }


    // Given a grid it creates a copy which returns.
    public static Cell[,] CopyGrid(Cell[,] originalGrid)
    {
        int rows = originalGrid.GetLength(0);
        int cols = originalGrid.GetLength(1);

        Cell[,] gridCopy = new Cell[rows, cols];

        // Create cells for the new grid.
        for (int row = 0; row < rows; row++)
        {
            for (int col = 0; col < cols; col++)
            {
                Cell originalCell = originalGrid[row, col];

                gridCopy[row, col] = new Cell(
                    originalCell.Row,
                    originalCell.Col,
                    originalCell.State,
                    originalCell.IsRoomPlaceable,
                    originalCell.ig,
                    originalCell.ih,
                    originalCell.EntranceDirection
                );
            }
        }

        // Assign parents, temporal parents and destinations from the copied grid.
        for (int row = 0; row < rows; row++)
        {
            for (int col = 0; col < cols; col++)
            {
                Cell originalCell = originalGrid[row, col];
                Cell newCell = gridCopy[row, col];

                // Copy of the parents
                if (originalCell.Parents.Count > 0)
                {
                    foreach (Cell parentCell in originalCell.Parents)
                    {
                        Cell newParent = gridCopy[parentCell.Row, parentCell.Col];
                        newCell.Parents.Add(newParent);
                    }
                }

                // Copy of the TemporalParent
                if (originalCell.TemporalParent != null)
                {
                    newCell.TemporalParent = gridCopy[originalCell.TemporalParent.Row, originalCell.TemporalParent.Col];
                }

                // Copy of the DestinationCells
                if (originalCell.DestinationCells.Count > 0)
                {
                    foreach (Cell destinationCell in originalCell.DestinationCells)
                    {
                        Cell newDestination = gridCopy[destinationCell.Row, destinationCell.Col];
                        newCell.DestinationCells.Add(newDestination);
                    }
                }
            }
        }

        return gridCopy;
    }

    //public static bool IsPossibleInsertRoom(Cell[,] originalGrid, Cell[,] roomGrid, int initialRow, int initialCol)
    //{
    //    bool isPossible = true;

    //    int roomRows = roomGrid.GetLength(0);
    //    int roomCols = roomGrid.GetLength(1);
    //    int gridRows = originalGrid.GetLength(0);
    //    int gridCols = originalGrid.GetLength(1);

    //    // Verificar si la habitación puede insertarse en la cuadrícula.
    //    for (int row = initialRow; row < initialRow + roomRows; row++)
    //    {
    //        for (int col = initialCol; col < initialCol + roomCols; col++)
    //        {
    //            // Verificar si estamos dentro de los límites del grid.
    //            if (row >= 0 && row < gridRows && col >= 0 && col < gridCols)
    //            {
    //                // Si la celda en roomGrid no está vacía y no es colocable en originalGrid.
    //                if (roomGrid[row - initialRow, col - initialCol].State != CellState.Empty && !originalGrid[row, col].IsRoomPlaceable)
    //                {
    //                    isPossible = false;
    //                    Debug.Log($"-MAP GENERATION- Not possible insert room in cell: [{row}, {col}]  ([row, col])");
    //                    return false; // Salir al primer fallo.
    //                }
    //            }
    //            else
    //            {
    //                // Estamos fuera de los límites del originalGrid.
    //                Debug.Log($"-MAP GENERATION- Out of bounds: Not possible insert room in cell: [{row}, {col}]  ([row, col])");
    //                return false;
    //            }
    //        }
    //    }

    //    // Verificar las celdas alrededor del área de la habitación.
    //    for (int row = initialRow - 1; row <= initialRow + roomRows; row++)
    //    {
    //        for (int col = initialCol - 1; col <= initialCol + roomCols; col++)
    //        {
    //            // Verificar si estamos dentro de los límites.
    //            if ((row >= 1 && row < gridRows - 1) && (col >= 1 && col < gridCols - 1))
    //            {
    //                // Verificar si la celda está en el perímetro (no dentro del roomGrid).
    //                if (row == initialRow - 1 || row == initialRow + roomRows || col == initialCol - 1 || col == initialCol + roomCols)
    //                {
    //                    // Comprobar si el perímetro tiene celdas vacías o pasillos
    //                    if (originalGrid[row, col].State == CellState.Empty || originalGrid[row, col].State == CellState.Corridor)
    //                    {
    //                        isPossible = true; // Puede ser colocada si el perímetro es válido.
    //                    }
    //                    else
    //                    {
    //                        isPossible = false;
    //                        Debug.Log($"-MAP GENERATION- Perimeter issue: Not possible insert room near cell: [{row}, {col}]");
    //                        return false;
    //                    }
    //                }
    //            }
    //        }
    //    }

    //    return isPossible;
    //}

    public static bool IsPossibleInsertRoom(Cell[,] originalGrid, Cell[,] roomGrid, int initialRow, int initialCol)
    {
        int roomRows = roomGrid.GetLength(0);
        int roomCols = roomGrid.GetLength(1);
        int gridRows = originalGrid.GetLength(0);
        int gridCols = originalGrid.GetLength(1);

        // Verificar que la habitación no esté en el perímetro del grid
        if (initialRow <= 0 || initialRow + roomRows >= gridRows ||
            initialCol <= 0 || initialCol + roomCols >= gridCols)
        {
            Debug.Log($"-MAP GENERATION- Cannot place room at the perimeter. Position: [{initialRow}, {initialCol}]");
            return false;
        }

        // Verificar si la habitación puede insertarse en la cuadrícula.
        for (int row = initialRow; row < initialRow + roomRows; row++)
        {
            for (int col = initialCol; col < initialCol + roomCols; col++)
            {
                // Verificar si estamos dentro de los límites del grid.
                if (row >= 0 && row < gridRows && col >= 0 && col < gridCols)
                {
                    // Si la celda en roomGrid no está vacía y no es colocable en originalGrid.
                    if (roomGrid[row - initialRow, col - initialCol].State != CellState.Empty && !originalGrid[row, col].IsRoomPlaceable)
                    {
                        Debug.Log($"-MAP GENERATION- Not possible to insert room in cell: [{row}, {col}]");
                        return false; // Salir al primer fallo.
                    }
                }
                else
                {
                    // Estamos fuera de los límites del originalGrid.
                    Debug.Log($"-MAP GENERATION- Out of bounds: Not possible to insert room in cell: [{row}, {col}]");
                    return false;
                }
            }
        }

        // Verificar las celdas alrededor del área de la habitación.
        for (int row = initialRow - 1; row <= initialRow + roomRows; row++)
        {
            for (int col = initialCol - 1; col <= initialCol + roomCols; col++)
            {
                // Verificar si estamos dentro de los límites.
                if (row >= 0 && row < gridRows && col >= 0 && col < gridCols)
                {
                    // Verificar si la celda está en el perímetro (no dentro del roomGrid).
                    if (row == initialRow - 1 || row == initialRow + roomRows || col == initialCol - 1 || col == initialCol + roomCols)
                    {
                        // Comprobar si el perímetro tiene celdas vacías o pasillos
                        if (originalGrid[row, col].State != CellState.Empty && originalGrid[row, col].State != CellState.Corridor)
                        {
                            Debug.Log($"-MAP GENERATION- Perimeter issue: Not possible to insert room near cell: [{row}, {col}]");
                            return false;
                        }
                    }
                }
                else
                {
                    // Estamos fuera de los límites del originalGrid.
                    Debug.Log($"-MAP GENERATION- Out of bounds: Not possible to check perimeter at cell: [{row}, {col}]");
                    return false;
                }
            }
        }

        return true;
    }

    public static Cell[,] InsertRoom(Cell[,] grid, Cell[,] roomGrid, int initialRow, int initialCol, List<Cell> endCellList)
    {
        Cell[,] copiedGrid = CopyGrid(grid);

        List<Cell> endCellListCopied = new List<Cell>();
        foreach (Cell cell in endCellList)
        {
            endCellListCopied.Add(copiedGrid[cell.Row, cell.Col]);
        }

        // Comprobar si es posible insertar la habitación
        if (!IsPossibleInsertRoom(copiedGrid, roomGrid, initialRow, initialCol))
        {
            Debug.Log("MAP GENERATION - Room not possible to insert at given location.");
            return null;
        }

        Debug.Log("MAP GENERATION - Room possible to insert at given location.");

        List<Cell> roomCellsList = new List<Cell>();

        int rows = roomGrid.GetLength(0);
        int cols = roomGrid.GetLength(1);

        Debug.Log($"MAP GENERATION - Updating copied grid. Room size: {rows}x{cols}");

        // Actualizar celdas en el grid copiado
        for (int row = initialRow; row < initialRow + rows; row++)
        {
            for (int col = initialCol; col < initialCol + cols; col++)
            {
                Cell copiedCell = copiedGrid[row, col];
                Cell roomCell = roomGrid[row - initialRow, col - initialCol];

                // Actualizar las propiedades de la celda existente
                copiedCell.SetCellState(roomCell.State);
                copiedCell.SetEntranceDirection(roomCell.EntranceDirection);

                Debug.Log($"MAP GENERATION - Setting cell [{row}, {col}] to state: {copiedCell.State}");

                if (copiedCell.State != CellState.Empty)
                {
                    roomCellsList.Add(copiedCell);
                    Debug.Log($"MAP GENERATION - Added non-empty cell [{row}, {col}] to auxRoomCells.");
                }
            }
        }
        Debug.Log("MAP GENERATION - Finished updating cells.");

        // Intentar recalcular los caminos
        Debug.Log("MAP GENERATION - Recalculating paths (ReFindPath).");
        Cell[,] modifiedGrid = ReFindPath(roomCellsList, copiedGrid);
        if (modifiedGrid != null)
        {
            copiedGrid = modifiedGrid;
            Debug.Log("MAP GENERATION - Path recalculated successfully (ReFindPath).");
        }
        else
        {
            Debug.Log("MAP GENERATION - Path recalculation failed (ReFindPath).");
            return null;
        }

        // Intentar encontrar un camino hacia la celda final
        Debug.Log("MAP GENERATION - Finding path to endCell.");
        modifiedGrid = FindPath(endCellListCopied, copiedGrid);
        if (modifiedGrid != null)
        {
            Debug.Log("MAP GENERATION - Path to endCell found successfully.");
            copiedGrid = modifiedGrid;
            return copiedGrid;
        }
        else
        {
            Debug.Log("MAP GENERATION - Path to endCell not found.");
            return null;
        }
    }

    #endregion
}