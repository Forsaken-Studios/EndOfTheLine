using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class WallPlacers : MonoBehaviour
{
    [SerializeField] private GameObject firstPlacer;
    [SerializeField] private GameObject secondPlacer;

    [SerializeField] private GameObject wall;
    public void SetLocations(Vector2 location1, Vector2 location2, out GameObject wallSquare)
    {
        firstPlacer.transform.position = location1;
        secondPlacer.transform.position = location2;
        
        wall.transform.position = location1;
        float distance = Vector2.Distance(location1, location2);
        wall.transform.localScale = new Vector3(1, distance, 1);
        //float angle = Vector2.Angle(location1, location2);
        float angle = Mathf.Atan2(location2.y - location1.y, location2.x - location1.x) * Mathf.Rad2Deg;
        angle -= 90;
        wall.transform.rotation = Quaternion.Euler(0, 0, angle);
        wall.SetActive(false);
        wallSquare = wall;
    }
}
