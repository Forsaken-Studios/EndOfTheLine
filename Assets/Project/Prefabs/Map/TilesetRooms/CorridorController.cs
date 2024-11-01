using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CorridorController : MonoBehaviour
{
    public List<GameObject> WallConfigurationNorth;
    public List<GameObject> WallConfigurationSouth;
    public List<GameObject> WallConfigurationEast ;
    public List<GameObject> WallConfigurationWest ;
    
    public List<GameObject> CornerConfigurationNE;
    public List<GameObject> CornerConfigurationES;
    public List<GameObject> CornerConfigurationSW;
    public List<GameObject> CornerConfigurationWN;
    
    
    [Header("Tester")]
    public int N;
    public int S;
    public int E;
    public int W;

    public bool runWallSelector = false;
    
    public bool runResetWallSelector = false;

    public void WallSelector(int North, int South, int East, int West)
    {
        bool checkInputCorrect = true;
        if ((North > 2 || North < 0)||(South > 2 || South < 0)||(East > 2 || East < 0)||(West > 2 || West < 0))
        {
            checkInputCorrect = false;
            Debug.Log("[CorridorController.cs] Invalid Parameter. Range:[0,2]");
        }
        if (checkInputCorrect)
        {
            WallConfigurationNorth[North].SetActive(true);
            WallConfigurationSouth[South].SetActive(true);
            WallConfigurationEast [East ].SetActive(true);
            WallConfigurationWest [West ].SetActive(true);
            if (North < 2 && East < 2) { CornerConfigurationNE[0].SetActive(true);
            }else if (North != 2) {      CornerConfigurationNE[1].SetActive(true);
            }else if (East  != 2) {      CornerConfigurationNE[2].SetActive(true);
            }else {                      CornerConfigurationNE[3].SetActive(true);
            }
            if (East < 2 && South < 2) { CornerConfigurationES[0].SetActive(true);
            }else if (East  != 2) {      CornerConfigurationES[1].SetActive(true);
            }else if (South != 2) {      CornerConfigurationES[2].SetActive(true);
            }else {                      CornerConfigurationES[3].SetActive(true);
            }
            if (South < 2 && West < 2) { CornerConfigurationSW[0].SetActive(true);
            }else if (South != 2) {      CornerConfigurationSW[1].SetActive(true);
            }else if (West  != 2) {      CornerConfigurationSW[2].SetActive(true);
            }else {                      CornerConfigurationSW[3].SetActive(true);
            }
            if (West < 2 && North < 2) { CornerConfigurationWN[0].SetActive(true);
            }else if (West  != 2) {      CornerConfigurationWN[1].SetActive(true);
            }else if (North != 2) {      CornerConfigurationWN[2].SetActive(true);
            }else {                      CornerConfigurationWN[3].SetActive(true);
            }
        }

    }
    
    public void ResetWallSelection()
    {
        foreach (var x in WallConfigurationNorth) { x.SetActive(false); }
        foreach (var x in WallConfigurationSouth) { x.SetActive(false); }
        foreach (var x in WallConfigurationEast ) { x.SetActive(false); }
        foreach (var x in WallConfigurationWest ) { x.SetActive(false); }
        
        foreach (var x in CornerConfigurationNE ) { x.SetActive(false); }
        foreach (var x in CornerConfigurationES ) { x.SetActive(false); }
        foreach (var x in CornerConfigurationSW ) { x.SetActive(false); }
        foreach (var x in CornerConfigurationWN ) { x.SetActive(false); }
    }
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (runWallSelector)
        {
            WallSelector(N,S,E,W);
            runWallSelector = false;
        }

        if (runResetWallSelector)
        {
            ResetWallSelection();
            runResetWallSelector = false;
        }
    }
}
