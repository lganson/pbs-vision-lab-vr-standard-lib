using System;
using System.Collections;
#if UNITY_EDITOR
using Standard_Library.Editor;
#endif
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using Object = UnityEngine.Object;

namespace Standard_Library
{
    [System.Serializable]
    public class HandCheckTask : Task
    {
        #if UNITY_EDITOR
        [Scene]
        #endif
        [SerializeField] private string handCheckSceneName;
        [Header("Hand Check Scene Config")]
        [SerializeField, TextArea(3, 10)] private string handCheckTaskText = "SAMPLE TEXT";
        [SerializeField] private string handCheckTaskTextObjectName = "HandCheckTaskText";
        [SerializeField] private bool spawnPoles = true;
        [SerializeField] private bool requirePolesToBeTouched;
        [SerializeField, Tooltip("Name of the origin in the hand check scene")] private string originName = "XR Origin";
        [SerializeField, Tooltip("X axis offset will be flipped for the right pole")] private Vector3 poleToBeTouchedOffset = new Vector3(-.35f, 1.2f, -2f) - new Vector3(0f, 0f, -3f);
        [SerializeField] private XRSimpleInteractable poleToBeTouchedPrefab;
        private bool leftHandValid;
        private bool rightHandValid;

        protected override void PerformTask()
        {
            SceneManager.LoadScene(handCheckSceneName);
            SceneManager.sceneLoaded += OnSceneLoaded;
        }
        
        public virtual void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            SceneManager.sceneLoaded -= OnSceneLoaded;
            InputManager.EnableController?.Invoke(true);
            InputManager.OnBothTriggersPressed.AddListener(ValidateHandCheck);
            //Find objects in the scene
            GameObject playerRoot = GameObject.Find(originName);
            TextMeshProUGUI handCheckText = GameObject.Find(handCheckTaskTextObjectName).GetComponent<TextMeshProUGUI>();
            if(handCheckText != null) handCheckText.text = handCheckTaskText; //if the hand check text exists assign the text to whatever is defined in the task
            else Debug.Log("Failed to assign task text");
            if (playerRoot == null) throw new Exception("XR Origin not found. Please fix the scene and reload");
            //Set up the poles to
            if (!spawnPoles && !requirePolesToBeTouched) return;
            XRSimpleInteractable leftPole = Object.Instantiate(poleToBeTouchedPrefab);
            leftPole.transform.position = playerRoot.transform.position + poleToBeTouchedOffset;
            XRSimpleInteractable rightPole = Object.Instantiate(poleToBeTouchedPrefab);
            rightPole.transform.position = playerRoot.transform.position + new Vector3(-poleToBeTouchedOffset.x, poleToBeTouchedOffset.y, poleToBeTouchedOffset.z);
            ApplyEvents(leftPole, rightPole);
        }
        private void ApplyEvents(XRSimpleInteractable leftPole, XRSimpleInteractable rightPole)
        {
            leftPole.hoverEntered.AddListener(_ => leftHandValid = true);
            leftPole.hoverExited.AddListener(_ => leftHandValid = false);
            rightPole.hoverEntered.AddListener(_ => rightHandValid = true);
            rightPole.hoverExited.AddListener(_ => rightHandValid = false);
            onTaskComplete.AddOnce(() =>
            {
                leftPole.hoverEntered.RemoveAllListeners();
                rightPole.hoverEntered.RemoveAllListeners();
                leftPole.hoverExited.RemoveAllListeners();
                rightPole.hoverExited.RemoveAllListeners();
            });
        }

        private void ValidateHandCheck()
        {
            if (requirePolesToBeTouched)
            {
                Debug.Log("Checking hands");
                //Check if both poles are touched, return if not.
                if (!leftHandValid || !rightHandValid)
                {
                    Debug.Log("Hand check failed");
                    return;
                }
            }
            Debug.Log("Hand check passed");
            InputManager.OnBothTriggersPressed.RemoveListener(ValidateHandCheck);
            onTaskComplete?.Invoke();
        }
    }
}