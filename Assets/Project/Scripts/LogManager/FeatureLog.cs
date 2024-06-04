using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Utils.CustomLogs
{
    [Serializable]
    public class FeatureLog
    {
        public FeatureType Feature;
        public Color CustomColor;
        public bool Enabled;
    }
}