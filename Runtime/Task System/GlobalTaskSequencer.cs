using System.Collections;
#if UNITY_EDITOR
using Standard_Library.Editor;
#endif
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;

namespace Standard_Library
{
    public class GlobalTaskSequencer : Singleton<GlobalTaskSequencer>
    {
        [Header("Global task sequencer config")]
        #if UNITY_EDITOR
        [Scene]
        #endif
        [SerializeField] private string startScene;
        [SerializeField] private TaskSequencer taskSequencer;
        public static readonly UnityEvent OnSequenceReset = new UnityEvent();
        public static readonly UnityEvent<TaskSequence> OnSequenceChange = new UnityEvent<TaskSequence>();
        
        
        private void ReloadStartScene()
        {
            Debug.Log("Reloading start scene");
            taskSequencer.onTasksCompleted.RemoveListener(ReloadStartScene);
            OnSequenceReset.Invoke();
            EyeTrackingManager.GetInstance().SetEyeTrackingEnabled(false);
            SceneManager.LoadScene(startScene);
        }
        public void StartSequence()
        {
            Debug.Log("Start sequence");
            if (taskSequencer.GetActiveSequence() == null) return;
            taskSequencer.onTasksCompleted.AddOnce(ReloadStartScene);
            InputManager.EnableController?.Invoke(true);
            StartCoroutine(taskSequencer.StartSequence());   
        }

        public void SetActiveSequence(TaskSequence sequence)
        {
            taskSequencer.SetSequence(sequence);
            OnSequenceChange.Invoke(sequence);
        }
    }
}