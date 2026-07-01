using System;
using Standard_Library;
using UnityEngine;
using UnityEngine.Events;
using VIVE.OpenXR;
using VIVE.OpenXR.EyeTracker;

public class EyeTrackingManager : Singleton<EyeTrackingManager>
{
    [Header("Setup")]
    private Transform _originXRRig;
    [SerializeField] private LayerMask gazeTargetLayers = ~0;
    [SerializeField] private float gizmoRadius = 0.1f;
    [SerializeField] private bool updateEachFrame = false;
    [SerializeField, Range(20,1000), Tooltip("If update each frame is true this will be ignored")] private float pollRateInHz = 120;
    [SerializeField] private bool eyeTrackingEnabled = true;
    [SerializeField] private float maxGazeDistance = 50f;
    [Header("Eye Targets")]
    [SerializeField] private bool enableTrackingTargets;
    [SerializeField] private Transform leftEyeTarget;
    [SerializeField] private Transform rightEyeTarget;
    [SerializeField] private Transform averageEyeTarget;
    private Vector3 _leftEyeTargetPosition;
    private Vector3 _rightEyeTargetPosition;

    private Quaternion _leftOffset = Quaternion.identity;
    private Quaternion _rightOffset = Quaternion.identity;
    
    public static readonly UnityEvent<EyeData> OnEyesUpdated =  new UnityEvent<EyeData>();
    
    public void Start()
    {
        if (_originXRRig == null)
        {
            GameObject xrRigObj = GameObject.Find("XR Origin");
            if (xrRigObj != null) _originXRRig = xrRigObj.transform;
        }
        // OnEyesUpdated.AddListener((ctx) => {CalculateEyeTargetPosition(ctx.leftEyeData, ref _leftEyeTargetPosition);});
        // OnEyesUpdated.AddListener((ctx) => {CalculateEyeTargetPosition(ctx.rightEyeData, ref _rightEyeTargetPosition);});
        if (!updateEachFrame)
        {
            InvokeRepeating(nameof(UpdateEyeData), 1/pollRateInHz, 1/pollRateInHz);
        }
    }

    public void OnDestroy()
    {
        if(this == GetInstanceNoSpawn()) OnEyesUpdated.RemoveAllListeners();
    }
    private void Update()
    {
        if (updateEachFrame)
        {
            UpdateEyeData();
        }
    }

    private void UpdateEyeData()
    {
        if (!eyeTrackingEnabled) return;
        if (!_originXRRig)
        {
            GameObject xrRigObj = GameObject.Find("XR Origin");
            if (xrRigObj) _originXRRig = xrRigObj.transform;
            else
            {
                Debug.LogWarning("XR Origin not found in scene. Either add an origin or disable eye tracking.");
            }

            return;
        }
        XRSingleEyeData leftEyeData = new XRSingleEyeData();
        XRSingleEyeData rightEyeData = new XRSingleEyeData();
        if (XR_HTC_eye_tracker.Interop.GetEyeGazeData(out XrSingleEyeGazeDataHTC[] outGazes))
        {
            var leftGaze = outGazes[(int)XrEyePositionHTC.XR_EYE_POSITION_LEFT_HTC];
            var rightGaze = outGazes[(int)XrEyePositionHTC.XR_EYE_POSITION_RIGHT_HTC];
            leftEyeData.gazeData = leftGaze;
            rightEyeData.gazeData = rightGaze;
        }
        if (XR_HTC_eye_tracker.Interop.GetEyePupilData(out  XrSingleEyePupilDataHTC[] outPupils)) 
        {
            var leftPupil = outPupils[(int)XrEyePositionHTC.XR_EYE_POSITION_LEFT_HTC];
            var rightPupil = outPupils[(int)XrEyePositionHTC.XR_EYE_POSITION_RIGHT_HTC];
            leftEyeData.pupilData = leftPupil;
            rightEyeData.pupilData = rightPupil;
        }
        if (XR_HTC_eye_tracker.Interop.GetEyeGeometricData(out XrSingleEyeGeometricDataHTC[] outGeometricData))
        {
            var leftGeometric = outGeometricData[(int)XrEyePositionHTC.XR_EYE_POSITION_LEFT_HTC];
            var rightGeometric = outGeometricData[(int)XrEyePositionHTC.XR_EYE_POSITION_RIGHT_HTC];
            leftEyeData.geometricData = leftGeometric;
            rightEyeData.geometricData = rightGeometric;
        }
        
        CalculateEyeTargetPosition(leftEyeData, ref _leftEyeTargetPosition, _leftOffset);
        CalculateEyeTargetPosition(rightEyeData, ref _rightEyeTargetPosition, _rightOffset);
        
        leftEyeData.targetWorldPosition = _leftEyeTargetPosition;
        rightEyeData.targetWorldPosition = _rightEyeTargetPosition;
        leftEyeData.time = Time.time;
        rightEyeData.time = Time.time;
        EyeData eyeData = new EyeData
        {
            leftEyeData = leftEyeData,
            rightEyeData = rightEyeData
        };
        OnEyesUpdated?.Invoke(eyeData);
        
        if (!enableTrackingTargets) return;
        leftEyeTarget.position = _leftEyeTargetPosition;
        rightEyeTarget.position = _rightEyeTargetPosition;
        averageEyeTarget.position = (_leftEyeTargetPosition + _rightEyeTargetPosition)/2;
    }
    private void CalculateEyeTargetPosition(XRSingleEyeData singleEyeData, ref  Vector3 targetPosition, Quaternion localOffset)
    {
        if (!singleEyeData.gazeData.isValid) return;
        GetWorldSpaceGaze(singleEyeData.gazeData.gazePose, out var worldOrigin, out var worldDirection, localOffset); 
        targetPosition = GetWorldSpaceTarget(worldOrigin, worldDirection);
    }
    private void GetWorldSpaceGaze(XrPosef pose, out Vector3 worldOrigin, out Vector3 worldDirection, Quaternion localOffset)
    {
        Vector3 localOrigin = pose.position.ToUnityVector();
        Vector3 localDirection = (pose.orientation.ToUnityQuaternion() * Vector3.forward).normalized;

        worldOrigin = _originXRRig.TransformPoint(localOrigin);
        worldDirection = _originXRRig.TransformDirection(localOffset * localDirection);
    }

    private Vector3 GetWorldSpaceTarget(Vector3 worldOrigin, Vector3 worldDirection)
    {
        Vector3 worldTarget;
        if (Physics.Raycast(worldOrigin, worldDirection, out RaycastHit hit, maxGazeDistance, gazeTargetLayers))
        {
            worldTarget = hit.point;
        }
        else
        {
            worldTarget = worldOrigin + (worldDirection * maxGazeDistance);
        }
        return worldTarget;
    }
    
    [Serializable]
    public struct XRSingleEyeData
    {
        public float time;
        public Vector3 targetWorldPosition; //might need to update how this is set but it should be fine for now
        public XrSingleEyeGazeDataHTC gazeData;
        public XrSingleEyeGeometricDataHTC geometricData;
        public XrSingleEyePupilDataHTC pupilData;
    }
    [Serializable]
    public struct EyeData
    {
        public XRSingleEyeData leftEyeData;
        public XRSingleEyeData rightEyeData;
    }
    //#if UNITY_EDITOR
    public void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(_leftEyeTargetPosition, gizmoRadius);
        Gizmos.color = new Color(1.0f, 0.647f, 0.0f);
        Gizmos.DrawWireSphere(_rightEyeTargetPosition, gizmoRadius);
        
        Gizmos.color = (new Color(1.0f, 0.647f, 0.0f) + Color.blue)/2;
        Gizmos.DrawWireSphere((_leftEyeTargetPosition+_rightEyeTargetPosition)/2, gizmoRadius);

    }
    //#endif
    public void SetEyeTrackingEnabled(bool enableTracking)
    {
        eyeTrackingEnabled = enableTracking;
    }

    public GameObject GetXrOrigin()
    {
        return _originXRRig.gameObject;
    }
    
    public void UpdateEyeOffsets(Quaternion leftEyeOffset, Quaternion rightEyeOffset)
    {
        _leftOffset = leftEyeOffset;
        _rightOffset = rightEyeOffset;
    }
}


