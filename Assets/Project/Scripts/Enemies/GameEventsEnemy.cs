using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public static class GameEventsEnemy
{
    public static Action<Transform, Transform> OnSeeingPlayer;
    public static Action OnForgetPlayer;
}
