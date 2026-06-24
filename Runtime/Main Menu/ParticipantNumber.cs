using Standard_Library;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

namespace Main_Menu
{
    [RequireComponent(typeof(TMP_InputField))]
    public class ParticipantNumber : MonoBehaviour
    {
        private TMP_InputField inputField;
        private void Start()
        {
            inputField = GetComponent<TMP_InputField>();
            inputField.onValueChanged.AddListener(SetParticipantNumber);
            int participantNumber = DataTracker.GetInstance().GetParticipantNumber();
            Debug.Log("Participant Number: " + participantNumber);
            if (participantNumber > 0)
            {
                inputField.text = participantNumber.ToString();
            } 
        }

        private void SetParticipantNumber(string newNumber)
        {
            DataTracker.GetInstance().SetParticipantNumber(newNumber);
        }
        private void OnDestroy()
        {
            inputField.onValueChanged.RemoveAllListeners();
        }

    }
}
