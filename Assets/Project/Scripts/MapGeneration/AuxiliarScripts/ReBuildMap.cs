using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ReBuildMap : MonoBehaviour
{
    private Button _reBuildMapButton;

    void Start()
    {
        _reBuildMapButton = GetComponent<Button>();
        _reBuildMapButton.onClick.AddListener(ReBuild);
    }

    private void ReBuild()
    {
        GameEventsMap.OnReBuildMap?.Invoke();
    }
}
