using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LooteableObjectUI : MonoBehaviour
{
    [SerializeField] private GameObject looteableItemsPanel;
    [SerializeField] private GameObject player;


    private float distanceNeededToClosePanel = 2f;
    /// <summary>
    /// WIP - Being able to select what items do we want to take
    /// </summary>
    [SerializeField] private 
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (looteableItemsPanel.activeSelf)
        {
             if (Vector2.Distance(player.transform.position, this.gameObject.transform.position) >
                    distanceNeededToClosePanel)
                {
                    DesactivateLooteablePanel();
                }
        }
           
    }


    public void ActivateLooteablePanel()
    {
        this.looteableItemsPanel.SetActive(true);
    }

    public void DesactivateLooteablePanel()
    {
        this.looteableItemsPanel.SetActive(false);
    }
}
