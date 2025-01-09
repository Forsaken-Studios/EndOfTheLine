using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class RoomFinder
{
    Dictionary<int, List<RoomWithConfiguration>> roomsPrefabs;

    public RoomFinder(RoomsDataBase roomDataBase)
    {
        roomsPrefabs = roomDataBase.GetRoomDB();
    }

    public RoomWithConfiguration FindRoomPrefab(Dictionary<DirectionFlag, DirectionAvailability> entrances)
    {
        List<DirectionFlag> enetrancesSeeked = new List<DirectionFlag>();

        List<List<DirectionFlag>> completeList = new List<List<DirectionFlag>>();
        completeList.Add(GetOpenDirections(entrances));
        completeList.AddRange(GetOpenAndFreeCombinations(entrances));

        List<RoomWithConfiguration> possibleRooms = new List<RoomWithConfiguration>();

        foreach (var combination in completeList)
        {
            if (IsListEqual(combination, new List<DirectionFlag> { DirectionFlag.Down }))
            {
                if (roomsPrefabs.ContainsKey(1))
                    possibleRooms.AddRange(roomsPrefabs[1]);
            }
            else if (IsListEqual(combination, new List<DirectionFlag> { DirectionFlag.Left }))
            {
                if (roomsPrefabs.ContainsKey(2))
                    possibleRooms.AddRange(roomsPrefabs[2]);
            }
            else if (IsListEqual(combination, new List<DirectionFlag> { DirectionFlag.Up }))
            {
                if (roomsPrefabs.ContainsKey(3))
                    possibleRooms.AddRange(roomsPrefabs[3]);
            }
            else if (IsListEqual(combination, new List<DirectionFlag> { DirectionFlag.Right }))
            {
                if (roomsPrefabs.ContainsKey(4))
                    possibleRooms.AddRange(roomsPrefabs[4]);
            }
            else if (IsListEqual(combination, new List<DirectionFlag> { DirectionFlag.Up, DirectionFlag.Down }))
            {
                if (roomsPrefabs.ContainsKey(5))
                    possibleRooms.AddRange(roomsPrefabs[5]);
            }
            else if (IsListEqual(combination, new List<DirectionFlag> { DirectionFlag.Left, DirectionFlag.Right }))
            {
                if (roomsPrefabs.ContainsKey(6))
                    possibleRooms.AddRange(roomsPrefabs[6]);
            }
            else if (IsListEqual(combination, new List<DirectionFlag> { DirectionFlag.Left, DirectionFlag.Down }))
            {
                if (roomsPrefabs.ContainsKey(7))
                    possibleRooms.AddRange(roomsPrefabs[7]);
            }
            else if (IsListEqual(combination, new List<DirectionFlag> { DirectionFlag.Left, DirectionFlag.Up }))
            {
                if (roomsPrefabs.ContainsKey(8))
                    possibleRooms.AddRange(roomsPrefabs[8]);
            }
            else if (IsListEqual(combination, new List<DirectionFlag> { DirectionFlag.Up, DirectionFlag.Right }))
            {
                if (roomsPrefabs.ContainsKey(9))
                    possibleRooms.AddRange(roomsPrefabs[9]);
            }
            else if (IsListEqual(combination, new List<DirectionFlag> { DirectionFlag.Right, DirectionFlag.Down }))
            {
                if (roomsPrefabs.ContainsKey(10))
                    possibleRooms.AddRange(roomsPrefabs[10]);
            }
            else if (IsListEqual(combination, new List<DirectionFlag> { DirectionFlag.Down, DirectionFlag.Left, DirectionFlag.Up }))
            {
                if (roomsPrefabs.ContainsKey(11))
                    possibleRooms.AddRange(roomsPrefabs[11]);
            }
            else if (IsListEqual(combination, new List<DirectionFlag> { DirectionFlag.Left, DirectionFlag.Up, DirectionFlag.Right }))
            {
                if (roomsPrefabs.ContainsKey(12))
                    possibleRooms.AddRange(roomsPrefabs[12]);
            }
            else if (IsListEqual(combination, new List<DirectionFlag> { DirectionFlag.Up, DirectionFlag.Right, DirectionFlag.Down }))
            {
                if (roomsPrefabs.ContainsKey(13))
                    possibleRooms.AddRange(roomsPrefabs[13]);
            }
            else if (IsListEqual(combination, new List<DirectionFlag> { DirectionFlag.Right, DirectionFlag.Down, DirectionFlag.Left }))
            {
                if (roomsPrefabs.ContainsKey(14))
                    possibleRooms.AddRange(roomsPrefabs[14]);
            }
            else if (IsListEqual(combination, new List<DirectionFlag> { DirectionFlag.Right, DirectionFlag.Down, DirectionFlag.Left, DirectionFlag.Up }))
            {
                if(roomsPrefabs.ContainsKey(15))
                    possibleRooms.AddRange(roomsPrefabs[15]);
            }
        }

        if(possibleRooms.Count == 0)
        {
            return null;
        }
        else
        {
            System.Random random = new System.Random();
            return possibleRooms[random.Next(0, possibleRooms.Count)];
        }
    }

    bool IsListEqual(List<DirectionFlag> list1, List<DirectionFlag> list2)
    {
        // Se comparan las listas para asegurarse de que coincidan exactamente
        return list1.Count == list2.Count &&
               list1.All(list2.Contains) &&
               list2.All(list1.Contains);
    }

    List<DirectionFlag> GetOpenDirections(Dictionary<DirectionFlag, DirectionAvailability> directions)
    {
        // Filtrar las que están en estado Open
        return directions.Where(pair => pair.Value == DirectionAvailability.Open)
                         .Select(pair => pair.Key)
                         .ToList();
    }

    List<List<DirectionFlag>> GetOpenAndFreeCombinations(Dictionary<DirectionFlag, DirectionAvailability> directions)
    {
        // Obtener las listas iniciales
        List<DirectionFlag> openDirections = GetOpenDirections(directions);
        List<DirectionFlag> freeDirections = directions.Where(pair => pair.Value == DirectionAvailability.Free)
                                                       .Select(pair => pair.Key)
                                                       .ToList();

        // Generar combinaciones
        List<List<DirectionFlag>> combinations = new List<List<DirectionFlag>>
        {
            openDirections // Incluye la lista inicial con solo las abiertas
        };

        // Generar combinaciones con las libres
        int freeCount = freeDirections.Count;
        for (int i = 1; i <= freeCount; i++)
        {
            var subsets = GetCombinations(freeDirections, i);
            foreach (var subset in subsets)
            {
                var combination = new List<DirectionFlag>(openDirections);
                combination.AddRange(subset);
                combinations.Add(combination);
            }
        }

        return combinations;
    }

    List<List<T>> GetCombinations<T>(List<T> list, int length)
    {
        // Generar todas las combinaciones posibles de tamaño "length"
        if (length == 0) return new List<List<T>> { new List<T>() };
        if (list.Count == 0) return new List<List<T>>();

        var head = list[0];
        var tail = list.Skip(1).ToList();

        var includeHead = GetCombinations(tail, length - 1).Select(subset => new List<T> { head }.Concat(subset).ToList()).ToList();
        var excludeHead = GetCombinations(tail, length);

        return includeHead.Concat(excludeHead).ToList();
    }
}
