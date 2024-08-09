using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ExpandedLoreView : MonoBehaviour
{

    [SerializeField] private TextMeshProUGUI titleText; 
    [SerializeField] private TextMeshProUGUI loreText; 
    [SerializeField] private TextMeshProUGUI signText;

    [SerializeField] private Button increasePageButton;
    [SerializeField] private Button decreasePageButton;
    private int totalPages = 0;


    private void Start()
    {
        increasePageButton.onClick.AddListener(() => IncreasePage());
        decreasePageButton.onClick.AddListener(() => DecreasePage());
    }

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
        this.decreasePageButton.interactable = false;
        this.titleText.text = lore.loreTitle.ToString();
        this.loreText.text = lore.loreDescription;
        this.loreText.ForceMeshUpdate();
        this.totalPages = this.loreText.textInfo.pageCount;
        this.signText.text = lore.loreSign;
    }

    private void IncreasePage()
    {
        if (this.loreText.pageToDisplay < totalPages)
        {
            if (this.loreText.pageToDisplay == 1)
                this.decreasePageButton.interactable = true;
            this.loreText.pageToDisplay++;
            if (this.loreText.pageToDisplay == totalPages)
                this.increasePageButton.interactable = false;
            else
                this.increasePageButton.interactable = true;
        }
    }   
    private void DecreasePage()
    {
        if (this.loreText.pageToDisplay > 1)
        { 
            if (this.loreText.pageToDisplay == totalPages)
                this.increasePageButton.interactable = true;
            
            this.loreText.pageToDisplay--;
            if (this.loreText.pageToDisplay == 1)
                this.decreasePageButton.interactable = false;
            else
                this.decreasePageButton.interactable = true;
        }
    }
}
