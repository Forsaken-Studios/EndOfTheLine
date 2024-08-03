using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ExpandedLoreView : MonoBehaviour
{

    [SerializeField] private TextMeshProUGUI titleText; 
    [SerializeField] private TextMeshProUGUI loreText; 
    [SerializeField] private TextMeshProUGUI signText;

    [SerializeField] private int totalPages = 0;
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.LeftArrow) && this.loreText.pageToDisplay > 1)
        {
            this.loreText.pageToDisplay--;
        }else if (Input.GetKeyDown(KeyCode.RightArrow) && this.loreText.pageToDisplay < totalPages)
        {
            this.loreText.pageToDisplay++;
        }
    }


    public void SetUpProperties(LoreSO lore)
    {
        this.titleText.text = lore.loreTitle.ToString();
        this.loreText.text = lore.loreDescription;
        this.loreText.ForceMeshUpdate();
        Debug.Log(this.loreText.textInfo.pageCount);
        this.totalPages = this.loreText.textInfo.pageCount;
        this.signText.text = lore.loreSign;
    }
}
