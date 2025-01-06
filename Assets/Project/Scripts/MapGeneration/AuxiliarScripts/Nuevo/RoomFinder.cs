using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class RoomFinder
{
    Dictionary<DirectionFlag, List<RoomWithConfiguration>> roomsPrefabs;

    public RoomFinder(RoomsDataBase roomDataBase)
    {
        roomsPrefabs = roomDataBase.GetRoomsPrefabs();
    }

    public RoomWithConfiguration FindRoomPrefab(Dictionary<DirectionFlag, DirectionAvailability> entrances) 
    {
        // Lista para almacenar posibles candidatos
        List<RoomWithConfiguration> candidates = new List<RoomWithConfiguration>();

        // Filtrar por condiciones `Open`
        foreach (KeyValuePair<DirectionFlag, DirectionAvailability> entry in entrances)
        {
            if (entry.Value == DirectionAvailability.Open)
            {
                if (roomsPrefabs.ContainsKey(entry.Key))
                {
                    // Si está abierta, los prefabs de esta dirección son candidatos iniciales
                    candidates = candidates.Count == 0
                        ? new List<RoomWithConfiguration>(roomsPrefabs[entry.Key])
                        : new List<RoomWithConfiguration>(candidates.Intersect(roomsPrefabs[entry.Key]));
                }
                else
                {
                    // Si no hay prefabs en esta dirección, no hay candidatos posibles
                    return null;
                }
            }
        }

        // Excluir candidatos que estén en `Closed`
        foreach (KeyValuePair<DirectionFlag, DirectionAvailability> entry in entrances)
        {
            if (entry.Value == DirectionAvailability.Closed)
            {
                if (roomsPrefabs.ContainsKey(entry.Key))
                {
                    candidates.RemoveAll(room => roomsPrefabs[entry.Key].Contains(room));
                }
            }
        }

        // Retornar aleatoriamente un candidato válido si existe
        if (candidates.Count > 0)
        {
            System.Random rnd = new System.Random();

            // Ordenar aleatoriamente
            List<RoomWithConfiguration> shuffledRooms = candidates.OrderBy(x => rnd.Next()).ToList();

            RoomWithConfiguration selectedPrefab = candidates[0];
            return selectedPrefab;
        }

        return null;
    }
}
