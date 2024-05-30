using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

/// <summary>
/// DEPRECATED - NOT USING
/// </summary>
public class TakeItemText : MonoBehaviour
{
    [SerializeField] private GameObject floatingTextPrefab;
    [SerializeField] private TextMeshProUGUI newItemText;
    private bool alreadyShowingText = false;
    private Dictionary<Item, int> itemsInQueue; //TESTING 
    private Dictionary<Item, int> itemsShowing;//TESTING 
    private int currentIndex = 0; //TESTING 
    private bool isCorroutineRunning = false; //TESTING
    void Start()
    {
        itemsInQueue = new Dictionary<Item, int>();
        itemsShowing = new Dictionary<Item, int>();
        this.newItemText.gameObject.SetActive(false);
    }
    void Update()
    {
    }

    public void NewItemAddedToInventory(Item item, int amount)
    {
        this.newItemText.gameObject.SetActive(true);
        if (alreadyShowingText)
            StopAllCoroutines();
        
        alreadyShowingText = true;
        StartCoroutine(AddedNewItemTextCoroutine(item, amount));
    }

    public IEnumerator AddedNewItemTextCoroutine(Item item, int amount)
    {
        while (true)
        {
            newItemText.text = "x" + amount + " "+ item.itemName.ToString();
            yield return new WaitForSeconds(1f);
            newItemText.text = "";
            this.newItemText.gameObject.SetActive(false);
            alreadyShowingText = false;
            StopAllCoroutines();
            yield return null; 
        }
    }
    
    #region Testing to add several items at the same time
    /// <summary>
    /// Testing
    /// </summary>
    /// <param name="items"></param>
    /// <returns></returns>
    private IEnumerator WriteNewSeveralItemsCorroutine(Dictionary<Item, int> items)
    {
        newItemText.gameObject.SetActive(true);
        isCorroutineRunning = true;
        bool aux = false;
        while (true)
        {
           foreach (var item in itemsShowing)
           {
                newItemText.text = "x" + item.Value.ToString() + " "+ item.Key.name;
                //items.Remove(item.Key);
                //yield return new WaitForSeconds(1f);
                Debug.Log("CURRENT ITEM: " + item.Key.name);
                if (itemsInQueue.Count == 0)
                {
                    aux = true;
                }
           }
           if (itemsInQueue.Count == 0 && aux)
           {
                Debug.Log("GOING TO CHECK");
                newItemText.gameObject.SetActive(false);
                itemsShowing.Clear();
                isCorroutineRunning = false;
                CheckDictionaryStatus();
                yield return null;
           }
           else
           {
                 yield return null;
           }
        }
    }
    private void CheckDictionaryStatus()
    {
        itemsShowing.Clear();
        if (itemsInQueue.Count == 0)
        {
            //Si no hay en cola, apagamos la corroutina
            StopAllCoroutines();
        }
        else
        {
            //En caso contrario, pasamos una lista a la otra
            itemsShowing = itemsInQueue;
            itemsInQueue.Clear();
            StopAllCoroutines();
            Debug.Log("HOLA");
        }
    }

    public void WriteNewItemInText(Item itemToWrite, int amount)
    {
        if (itemsShowing.Count == 0)
        {
            itemsShowing.Add(itemToWrite, amount);
            //StartCoroutine(WriteNewItemsCorroutine(itemsShowing));
        }
        else
        {
            if (itemsInQueue.ContainsKey(itemToWrite))
            {
                itemsInQueue[itemToWrite] += amount;
            }
            else
            {
                itemsInQueue.Add(itemToWrite, amount);
            }
        }
    }
    #endregion
    private void OnDisable()
    {
        StopAllCoroutines();
    }
}
