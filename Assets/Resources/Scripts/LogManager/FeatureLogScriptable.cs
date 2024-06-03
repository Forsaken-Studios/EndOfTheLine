using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Utils.CustomLogs
{
    [CreateAssetMenu(fileName = "LogData", menuName = "Logs")]
    public class FeatureLogScriptable : ScriptableObject
    {
        [SerializeField] private List<FeatureLog> _features;

        public FeatureLog GetLogData(FeatureType feature)
        {
            if (_features == null)
            {
                return null;
            }

            var featureData = _features.Find((element) => element.Feature == feature);
            return featureData;
        }
    }
}