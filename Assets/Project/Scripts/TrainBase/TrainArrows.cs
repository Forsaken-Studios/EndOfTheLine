using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrainArrows : MonoBehaviour
{
    
    
    [SerializeField] private GameObject leftArrow;
    [SerializeField] private GameObject rightArrow;
    public static TrainArrows Instance;
    private void Awake()
    {
        if (Instance != null)
        {
            Debug.LogWarning("[TrainArrows.cs] : There is already a TrainArrows Instance");
            Destroy(this);
        }

        Instance = this;
    }
    public void ShowLeftArrow() => leftArrow.SetActive(true);
    public void HideLeftArrow() => leftArrow.SetActive(false);
    public void ShowRightArrow() => rightArrow.SetActive(true);
    public void HideRightArrow() => rightArrow.SetActive(false);
}
