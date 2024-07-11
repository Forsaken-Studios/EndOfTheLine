using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrainPanels : MonoBehaviour
{

    [SerializeField] private List<GameObject> trainPanels;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    /// <summary>
    /// index 0 - mission selector
    /// index 1 - Control room
    /// index 2 - extra
    /// </summary>
    /// <param name="index"></param>
    public void HideTrainRoom(int index )
    {
        GameObject blackPanel = trainPanels[index].gameObject;
        blackPanel.GetComponent<Animator>().SetBool("hideRoom", true); 
        blackPanel.GetComponent<Animator>().SetBool("showRoom", false); 
    }
    
    public void ShowTrainRoom(int index )
    {
        GameObject blackPanel = trainPanels[index].gameObject;
        blackPanel.GetComponent<Animator>().SetBool("showRoom", true); 
        blackPanel.GetComponent<Animator>().SetBool("hideRoom", false); 
    }
    
}
