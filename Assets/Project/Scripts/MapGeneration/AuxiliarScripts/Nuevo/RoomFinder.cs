using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class RoomFinder
{
    List<RoomWithConfiguration> roomsPrefabs;

    public RoomFinder(RoomsDataBase roomDataBase)
    {
        roomsPrefabs = roomDataBase.GetRoomsList();
    }

    public RoomWithConfiguration FindRoomPrefab(Dictionary<DirectionFlag, DirectionAvailability> entrances)
    {
        // Lista para almacenar posibles candidatos
        List<RoomWithConfiguration> candidates = new List<RoomWithConfiguration>();

        foreach(RoomWithConfiguration room in roomsPrefabs)
        {
            bool isValid = true;
            foreach (var entrance in entrances)
            {
                switch (entrance.Value)
                {
                    case DirectionAvailability.Open:
                        // Debe tener entrada en esta dirección
                        if (!room.openDirections.Contains(entrance.Key))
                            isValid = false;
                        break;

                    case DirectionAvailability.Closed:
                        // No debe tener entrada en esta dirección
                        if (room.openDirections.Contains(entrance.Key))
                            isValid = false;
                        break;

                    case DirectionAvailability.Free:
                        // Puede o no tener entrada, no afecta
                        break;
                }
            }
            if (isValid)
                candidates.Add(room);
        }

        // Seleccionar aleatoriamente un candidato válido si existe
        if (candidates.Count > 0)
        {
            System.Random rnd = new System.Random();
            RoomWithConfiguration selectedPrefab = candidates[rnd.Next(candidates.Count)];
            return selectedPrefab;
        }
        return null;
    }
}
