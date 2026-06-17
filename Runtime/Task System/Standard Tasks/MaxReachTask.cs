using System;
using TMPro;
#if UNITY_EDITOR
using Standard_Library.Editor;
#endif
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Standard_Library
{
    [Serializable]
    public class MaxReachTask : Task
    {
        #if UNITY_EDITOR
        [Scene]
        #endif
        [SerializeField] private string maxReachSceneName;
        [Header("Max Reach Scene Config")]
        [SerializeField] private string xrOriginName = "XR Origin";
        private GameObject xrOrigin;
        protected override void PerformTask()
        {
            SceneManager.LoadScene(maxReachSceneName);
            SceneManager.sceneLoaded += OnSceneLoaded;
        }

        private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            InputManager.EnableController?.Invoke(true);
            InputManager.OnBothTriggersPressed.AddOnce(CollectDevicePositions);
            InputManager.OnBothTriggersPressed.AddOnce(()=>onTaskComplete?.Invoke());
            xrOrigin = GameObject.Find(xrOriginName);
        }

        private void CollectDevicePositions()
        {
            MaxReachData reachData = new MaxReachData();
            var posData = new InputDevicePositionData(InputManager.GetInstance().GetDeviceCollection());
            reachData.inputDevicePositionData = posData;
            if (!xrOrigin)
            {
                Debug.LogWarning("XR Origin is missing");
                DataTracker.GetInstance().AppendData(reachData);
                return;
            }
            reachData.xrOriginPosition = xrOrigin.transform.position;
            reachData.maxReachPositionDeltaLeft = posData.leftPosition - reachData.xrOriginPosition;
            reachData.maxReachPositionDeltaRight = posData.rightPosition - reachData.xrOriginPosition;
            reachData.maxReachPositionDeltaAverage = (posData.leftPosition + posData.rightPosition)/2 - reachData.xrOriginPosition;
            DataTracker.GetInstance().AppendData(reachData);
        }
        [Serializable]
        private class MaxReachData : DataSerializer
        {
            public InputDevicePositionData inputDevicePositionData;
            public Vector3 xrOriginPosition;
            public Vector3 maxReachPositionDeltaLeft;
            public Vector3 maxReachPositionDeltaRight;
            public Vector3 maxReachPositionDeltaAverage;
        }
    }
    
}