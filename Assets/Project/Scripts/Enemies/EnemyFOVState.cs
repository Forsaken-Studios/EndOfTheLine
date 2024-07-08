using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Utils.CustomLogs;

namespace FieldOfView
{
    public class EnemyFOVState : MonoBehaviour
    {

        private FOVState _fovState;

        public FOVState FOVState
        {
            get { return _fovState; }
            set { _fovState = value; }
        }

        private void Update()
        {
            LogManager.Log("CURRENT STATUS: " + _fovState.ToString(), FeatureType.EnemyAI);
        }
    }

    public enum FOVState
    {
        isWatching,
        isDetecting,
        isSeeing,
        isDetected,
    }
}