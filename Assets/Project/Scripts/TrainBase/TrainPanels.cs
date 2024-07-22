using System.Collections;
using System.Collections.Generic;
using System.Net.Mime;
using UnityEngine;
using UnityEngine.UI;

public class TrainPanels : MonoBehaviour
{

    [SerializeField] private List<GameObject> trainPanels;
    
    /// <summary>
    /// index 0 - mission selector
    /// index 1 - Control room
    /// index 2 - extra
    /// index 3 - extra 2
    /// </summary>
    /// <param name="index"></param>
    public void HideTrainRoom(int index)
    {
        GameObject blackPanel = trainPanels[index].gameObject;
        blackPanel.GetComponent<Animator>().SetBool("hideRoom", true); 
        blackPanel.GetComponent<Animator>().SetBool("showRoom", false); 
    }
    
    public void ShowTrainRoom(int index, bool isUnlocked)
    {
        GameObject blackPanel = trainPanels[index].gameObject;
        Image trainImage = blackPanel.transform.parent.GetComponent<Image>();
        SetTrainColor(trainImage, isUnlocked);
        blackPanel.GetComponent<Animator>().SetBool("showRoom", true); 
        blackPanel.GetComponent<Animator>().SetBool("hideRoom", false); 
    }

    private void SetTrainColor(Image trainImage, bool isUnlocked)
    {
        if (isUnlocked)
        {
            trainImage.color = Color.white;
        }
        else
        {
            //Locked
            trainImage.color = Color.grey;
        }
    }

}
