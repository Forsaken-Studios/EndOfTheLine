using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ExpandedLoreView : MonoBehaviour
{

    [SerializeField] private TextMeshProUGUI titleText; 
    [SerializeField] private TextMeshProUGUI loreText; 
    [SerializeField] private TextMeshProUGUI signText;
    private int totalPages = 0;

    /// <summary>
    /// Page display controller < Left page     Right Page > 
    /// </summary>
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.LeftArrow) && this.loreText.pageToDisplay > 1)
            this.loreText.pageToDisplay--;
        else if (Input.GetKeyDown(KeyCode.RightArrow) && this.loreText.pageToDisplay < totalPages)
            this.loreText.pageToDisplay++;
    }

    /// <summary>
    /// Set up TextMeshProUGUI when showing the view
    /// </summary>
    /// <param name="lore"></param>
    public void SetUpProperties(LoreSO lore)
    {
        this.titleText.text = lore.loreTitle.ToString();
        this.loreText.text = lore.loreDescription;
        this.loreText.ForceMeshUpdate();
        this.totalPages = this.loreText.textInfo.pageCount;
        this.signText.text = lore.loreSign;
    }
}
