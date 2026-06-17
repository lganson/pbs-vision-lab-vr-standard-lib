using System;
using System.Collections.Generic;
using UnityEngine;

namespace Standard_Library
{
    public class DataTracker : Singleton<DataTracker>
    {
        [Header("Data tracker config")]
        [SerializeField] private bool requireParticipantNumber = true;
        [SerializeField] private int participantNumber = -1;
        [SerializeField] private string project;
        private bool eyeLoggingEnabled;
        private string folder;
        private List<DataSerializer> data = new List<DataSerializer>();
        private readonly GenericTrialData genericTrialData = new GenericTrialData();
        private readonly EyeTrackingData eyeData = new EyeTrackingData();
        public void Start()
        {
            GlobalTaskSequencer.OnSequenceReset.AddListener(OnSequenceReset);
            GlobalTaskSequencer.OnSequenceChange.AddListener(OnSequenceChange);
            AppendData(genericTrialData);
        }
        
        private void OnDestroy()
        {
            DisableEyeLogging();
            GlobalTaskSequencer.OnSequenceReset.RemoveListener(OnSequenceReset);
            GlobalTaskSequencer.OnSequenceChange.RemoveListener(OnSequenceChange);
        }

        private void OnSequenceChange(TaskSequence sequence)
        {
            folder = sequence.GetLogPath();
            DisableEyeLogging();
        }
        private void OnSequenceReset()
        {
            SaveData();
            data = new List<DataSerializer>();
            participantNumber = -1;
        }
        
        public void SetParticipantNumber(string newNumber)
        {
            Debug.Log(newNumber + " " + participantNumber);
            if (newNumber.Length <= 0) return;
            participantNumber = int.Parse(newNumber);
            genericTrialData.participantNumber = participantNumber;
        }

        public bool IsValidParticipantNumber()
        {
            return participantNumber > 0 || !requireParticipantNumber;
        }

        public int GetParticipantNumber()
        {
            return participantNumber;
        }
        
        public void AppendData(DataSerializer serializer)
        {
            data.Add(serializer);
        }

        private void SaveData()
        {
            SaveManager.Save(data, project, folder,participantNumber + "_saveData");
            if(eyeLoggingEnabled) SaveManager.Save(new List<DataSerializer>(){eyeData}, project, folder,participantNumber + "_eyeData");
            DisableEyeLogging();
        }

        public void EnableEyeLogging()
        {
            if(eyeLoggingEnabled)
            {
                Debug.Log("Eye logging already enabled");
                return;
            }
            eyeLoggingEnabled = true;
            eyeData.Clear();
            EyeTrackingManager.OnEyesUpdated.AddListener(AddEyeData);
        }

        private void DisableEyeLogging()
        {
            eyeData.Clear();
            EyeTrackingManager.OnEyesUpdated.RemoveListener(AddEyeData);
            eyeLoggingEnabled = false;
        }
        private void AddEyeData(EyeTrackingManager.EyeData data)
        {
            eyeData.eyeData.Add(data);
        }
    }
    [Serializable]
    public class GenericTrialData : DataSerializer
    {
        public int participantNumber;
    }

    [Serializable]
    public class EyeTrackingData : DataSerializer
    {
        public List<EyeTrackingManager.EyeData> eyeData = new List<EyeTrackingManager.EyeData>();

        public void Clear()
        {
            eyeData.Clear();
        }
    }
}
