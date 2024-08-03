using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickupLoreText : MonoBehaviour
{

    private bool canRead;

    private Animator _animator;
    private LoreSO loreSO;
    
    // Start is called before the first frame update
    void Start()
    {
        _animator = GetComponent<Animator>();
        StartCoroutine(StartCountDownToDestroyObject(4f));
    }

    // Update is called once per frame
    void Update()
    {
        if (canRead)
        {
            if (Input.GetKeyDown(KeyCode.E))
            {
                OpenExpandedView();
            }
        }
    }

    private void OpenExpandedView()
    {
        LoreManager.Instance.SetIfPlayerIsReadingLore(true);
        GameManager.Instance.GameState = GameState.onLoreView;
        GameObject expandedView = Instantiate(LoreManager.Instance.GetExpandedLoreViewPrefab(),
            new Vector2(0, 0), Quaternion.identity);
        LoreManager.Instance.SetCurrentLoreView(expandedView);
        expandedView.GetComponentInChildren<ExpandedLoreView>().SetUpProperties(loreSO);
        DestroyElement();
    }
    

    public void SetLoreSO(LoreSO loreSO)
    {
        this.loreSO = loreSO;
    }
    private IEnumerator StartCountDownToDestroyObject(float time)
    {
        while (true)
        {
            canRead = true;
            yield return new WaitForSeconds(time);

            canRead = false;
            _animator.SetTrigger("destroyPanel");
        }
    }

    private void DestroyElement()
    {
        Destroy(this.gameObject.transform.parent.gameObject);
        StopAllCoroutines();
    }
}
