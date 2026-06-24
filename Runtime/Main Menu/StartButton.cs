using System.Collections;
using Standard_Library;
using UnityEngine;
using UnityEngine.UI;

namespace Main_Menu
{
    public class StartButton : MonoBehaviour
    {
        private Button button;
        public void Start()
        {
            button = GetComponent<Button>();
            button.onClick.AddListener(StartSequence);
            StartCoroutine(OnStartTask());

        }

        private void StartSequence()
        {
            Debug.Log("Start button clicked");
            GlobalTaskSequencer.GetInstance().StartSequence();
        }
        public void OnDestroy()
        {
            button.onClick.RemoveAllListeners();
        }

        public void LateUpdate()
        {
            button.interactable = DataTracker.GetInstance().IsValidParticipantNumber();
        }
        private IEnumerator OnStartTask()
        {
            yield return new WaitForSeconds(1);
            InputManager.EnableController?.Invoke(false);
        }
        
    }
}