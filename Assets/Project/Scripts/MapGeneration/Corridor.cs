using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Corridor : MonoBehaviour
{
    private bool _isOpenUp, _isOpenDown, _isOpenRight, _isOpenLeft;
    private bool _hasDoorUp, _hasDoorDown, _hasDoorRight, _hasDoorLeft;

    public void ParseCorridorData()
    {
        string prefabName = gameObject.name;

        if(prefabName.StartsWith("Corridor_"))
        {
            string[] sectionsName = prefabName.Split('_');

            if(sectionsName.Length == 3 )
            {
                string openingsSection = sectionsName[1];
                string doorsSection = sectionsName[2];

                _isOpenUp = openingsSection[0] == 'U';
                _isOpenDown = openingsSection[1] == 'D';
                _isOpenRight = openingsSection[2] == 'R';
                _isOpenLeft = openingsSection[3] == 'L';

                _hasDoorUp = doorsSection[0] == 'U';
                _hasDoorDown = doorsSection[1] == 'D';
                _hasDoorRight = doorsSection[2] == 'R';
                _hasDoorLeft = doorsSection[3] == 'L';
            }
            else
            {
                Debug.LogError("Nombre del prefab no tiene el formato esperado.");
            }
        }
        else
        {
            Debug.LogError("Nombre del prefab no tiene el formato esperado.");
        }
    }

    public bool MatchesConfig(bool openUp, bool openDown, bool openRight, bool openLeft,
                              bool doorUp, bool doorDown, bool doorRight, bool doorLeft)
    {
        return _isOpenUp == openUp &&
               _isOpenDown == openDown &&
               _isOpenRight == openRight &&
               _isOpenLeft == openLeft &&
               _hasDoorUp == doorUp &&
               _hasDoorDown == doorDown &&
               _hasDoorRight == doorRight &&
               _hasDoorLeft == doorLeft;
    }
}
