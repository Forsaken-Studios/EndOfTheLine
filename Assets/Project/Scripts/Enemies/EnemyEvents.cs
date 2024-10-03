using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public static class EnemyEvents
{
    public static Action<GameObject, Vector3, Vector3> OnSeenPlayer; // First self position, second player position.
    public static Action OnForgetPlayer;
    public static Action<GameObject, Vector3, bool> OnIsAtPlayerLastSeenPosition;
}
