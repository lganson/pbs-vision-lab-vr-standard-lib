using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.XR;
using UnityEngine.XR;
using UnityEngine.XR.Interaction.Toolkit.Inputs;
using CommonUsages = UnityEngine.XR.CommonUsages;
using InputDevice = UnityEngine.XR.InputDevice;

namespace Standard_Library
{
    [RequireComponent(typeof(InputActionManager))]
    public class InputManager : Singleton<InputManager>
    {
        [Header("Input Manager Config")]
        private InputActionManager inputActionManager;
        private Dictionary<string, InputActionAsset> inputActions;
    
        private InputDevice headsetDevice;
        private InputDevice leftController;
        private InputDevice rightController;
        private bool searchingForDevices;
        [SerializeField] private float timeBetweenDeviceChecks = 0.5f;
        public static readonly UnityEvent OnBothTriggersPressed = new UnityEvent();
        public static readonly UnityEvent OnEitherTriggerPressed = new UnityEvent();
        [SerializeField] private float pressWindow = 0.5f;
        private float leftPressTime = -1f;
        private float rightPressTime = -1f;
        
        public static readonly UnityEvent<bool> EnableController = new UnityEvent<bool>();
        public override void Awake()
        {
            base.Awake();
            inputActionManager = GetComponent<InputActionManager>();
            inputActions = new Dictionary<string, InputActionAsset>();
            foreach(var actionAsset in inputActionManager.actionAssets)
            {
                inputActions.Add(actionAsset.name, actionAsset);
            }
        
            //Get actions for trigger presses
            InputAction leftTriggerPressed = GetAction("XRI Left Interaction/Activate");
            InputAction rightTriggerPressed = GetAction("XRI Right Interaction/Activate");
            //Set events for double trigger presses
            leftTriggerPressed.performed += OnLeftTriggerPressed;
            rightTriggerPressed.performed += OnRightTriggerPressed;
            EnableController.AddListener(OnEnableControllers);
            //EnableController?.Invoke(startWithInputEnabled);
        }
        
        private void OnEnableControllers(bool enable)
        {
            if (enable) EnableControllers();
            else DisableControllers();
        }

        private void OnDestroy()
        {
            //Get actions for trigger presses
            InputAction leftTriggerPressed = GetAction("XRI Left Interaction/Activate");
            InputAction rightTriggerPressed =GetAction("XRI Left Interaction/Activate");
            //Set events for double trigger presses
            leftTriggerPressed.performed -= OnLeftTriggerPressed;
            rightTriggerPressed.performed -= OnRightTriggerPressed;
            EnableController.RemoveListener(OnEnableControllers);
            OnBothTriggersPressed.RemoveAllListeners();
            OnEitherTriggerPressed.RemoveAllListeners();
        }
    
        public InputAction GetAction(string actionName)
        {
            foreach (InputActionAsset asset in inputActions.Values)
            {
                var action = asset.FindAction(actionName);
                if (action != null)
                {
                    return action;
                }
            }
            throw new KeyNotFoundException(actionName);
        }

        public InputAction GetAction(CommonActions actionName)
        {
            switch (actionName)
            {
                case(CommonActions.LeftTrigger):
                    return  GetAction("XRI Left Interaction/Activate");
                case(CommonActions.RightTrigger):
                    return  GetAction("XRI Right Interaction/Activate");
                default:
                    throw new Exception("Common action " + actionName + " not added yet");
            }
        }
        public InputActionAsset GetActionAsset(string assetName)
        {
            if (inputActions.TryGetValue(assetName, out InputActionAsset action))
            {
                return action;
            }
            throw new KeyNotFoundException(assetName);
        }
        #region DeviceConnectionFlow
        public void Update()
        {
            if (DevicesConnected()) return;
            if(!searchingForDevices) StartCoroutine(GetDevices(timeBetweenDeviceChecks));
        }


        private bool DevicesConnected()
        {
            bool controllersConnected = leftController.isValid && rightController.isValid;
            return headsetDevice.isValid && controllersConnected;
        }
        private IEnumerator GetDevices(float delayTime)
        {
            searchingForDevices = true;
            yield return new WaitForSeconds(delayTime);

            List<InputDevice> devices = new List<InputDevice>();
            InputDevices.GetDevices(devices);
            foreach (var device in devices)
            {
                //Debug.Log(device.name + " " + device.characteristics);

                if ((device.characteristics & InputDeviceCharacteristics.HeadMounted) != 0)
                    headsetDevice = device;

                if ((device.characteristics & InputDeviceCharacteristics.Left) != 0 &&
                    (device.characteristics & InputDeviceCharacteristics.Controller) != 0)
                    leftController = device;

                if ((device.characteristics & InputDeviceCharacteristics.Right) != 0 &&
                    (device.characteristics & InputDeviceCharacteristics.Controller) != 0)
                    rightController = device;
            }
            searchingForDevices = false;
        }

        public InputDeviceCollection GetDeviceCollection()
        {
            while (!DevicesConnected())
            {
                Debug.LogWarning("Devices not connected. Waiting for connection");
            } 
            return new InputDeviceCollection(headsetDevice, leftController, rightController);
        }
        #endregion
    
    
    
        private void OnLeftTriggerPressed(InputAction.CallbackContext obj) => OnTriggerPressed(true);

        private void OnRightTriggerPressed(InputAction.CallbackContext obj) => OnTriggerPressed(false);
        private void OnTriggerPressed(bool left)
        {
            OnEitherTriggerPressed?.Invoke();
            switch (left)
            {
                case true:
                    leftPressTime = Time.time;
                    break;
                case false:
                    rightPressTime = Time.time;
                    break;
            }
            if (leftPressTime > 0f && rightPressTime > 0f &&
                Mathf.Abs(leftPressTime - rightPressTime) <= pressWindow)
            {
                OnBothTriggersPressed?.Invoke();
            }

            //Allow slight delay in getting both trigger presses to advance the scene
            if (leftPressTime > 0f && Time.time - leftPressTime > pressWindow)
            {
                leftPressTime = -1f;
            }

            if (rightPressTime > 0f && Time.time - rightPressTime > pressWindow)
            {
                rightPressTime = -1f;
            }
        }

        private void DisableControllers()
        {
            //Debug.Log("DisableControllers");
            foreach (var device in InputSystem.devices)
            {
                if (device is XRController)
                {
                    InputSystem.DisableDevice(device);
                }
            }
        }

        private void EnableControllers()
        {
            //Debug.Log("EnableControllers");
            foreach (var device in InputSystem.devices)
            {
                if (device is XRController)
                {
                    InputSystem.EnableDevice(device);
                }
            }
        }
    }

    public struct InputDeviceCollection
    {
        public readonly InputDevice head;
        public readonly InputDevice leftController;
        public readonly InputDevice rightController;

        public InputDeviceCollection(InputDevice head, InputDevice leftController, InputDevice rightController)
        {
            this.head = head;
            this.leftController = leftController;
            this.rightController = rightController;
        }
    }
    public enum CommonActions
    {
        LeftTrigger,
        RightTrigger
    }
    [Serializable]

    public struct InputDevicePositionData
    {
        public Vector3 headPosition;
        public Quaternion headRotation;
        public Vector3 leftPosition;
        public Quaternion leftRotation;
        public Vector3 rightPosition;
        public Quaternion rightRotation;

        public InputDevicePositionData(Vector3 headPosition, Quaternion headRotation, Vector3 leftPosition,
            Quaternion leftRotation, Vector3 rightPosition, Quaternion rightRotation)
        {
            this.headPosition = headPosition;
            this.headRotation = headRotation;
            this.leftPosition = leftPosition;
            this.leftRotation = leftRotation;
            this.rightPosition = rightPosition;
            this.rightRotation = rightRotation;
        }

        public InputDevicePositionData(InputDeviceCollection devices)
        {
            InputDevice head = devices.head;
            InputDevice leftController = devices.leftController;
            InputDevice rightController = devices.rightController;
            
            head.TryGetFeatureValue(CommonUsages.devicePosition, out headPosition);
            head.TryGetFeatureValue(CommonUsages.deviceRotation, out headRotation);

            leftController.TryGetFeatureValue(CommonUsages.devicePosition, out leftPosition);
            leftController.TryGetFeatureValue(CommonUsages.deviceRotation, out leftRotation);

            rightController.TryGetFeatureValue(CommonUsages.devicePosition, out rightPosition);
            rightController.TryGetFeatureValue(CommonUsages.deviceRotation, out rightRotation);
        }
    }
}