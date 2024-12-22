using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoreBarmanWagon : MonoBehaviour
{
    [SerializeField] private GameObject[] loreObject;
    [SerializeField] private GameObject[] barmanObject;

    private int auxLore = 0;
    private int auxBarman = 0;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetLoreObject(int lorePos)
    {
        loreObject[auxLore].SetActive(false);
        loreObject[lorePos].SetActive(true);
        auxLore = lorePos;
    }

    public void SetBarmanObject(int barmanPos)
    {
        barmanObject[auxBarman].SetActive(false);
        barmanObject[barmanPos].SetActive(true);
        auxBarman = barmanPos;
    }
}
