using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;

namespace Standard_Library
{
    [System.Serializable]
    public class TaskSequencer
    {
        [SerializeField] private TaskSequence taskSequence;
        private bool running;
        public readonly UnityEvent onTasksCompleted = new UnityEvent();
        public IEnumerator StartSequence()
        {
            if(taskSequence.logEyeTracking)
            {
                DataTracker.GetInstance().EnableEyeLogging();
                EyeTrackingManager.GetInstance().SetEyeTrackingEnabled(true);
            }
            if (running) yield break;
            running = true;
            foreach (Task task in taskSequence.tasks)
            {
                bool taskCompleted = false;
                task.Perform();
                task.onTaskComplete.AddOnce(() => taskCompleted = true);
                while (!taskCompleted)
                {
                    yield return null;
                }
            }
            running = false;
            Debug.Log(taskSequence.tasks.Count + " task(s) have been completed.");
            onTasksCompleted?.Invoke();
        }
        public void SetSequence(TaskSequence sequence)
        {
            taskSequence = sequence;
        }

        public TaskSequence GetActiveSequence()
        {
            return taskSequence;
        }
    }
}