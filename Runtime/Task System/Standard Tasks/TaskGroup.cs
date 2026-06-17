using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Standard_Library
{
    [Serializable]
    public class TaskGroup : Task
    {
        [Header("Task Group Config")]
        [SerializeReference, SubclassSelector] private List<Task> taskOptions;
        [SerializeField] private TaskGroupType taskGroupType;
        [SerializeField] private int repetitions = 1;
        private bool running;
        private List<Task> tasks = new List<Task>();

        protected override void PerformTask()
        {
            if(running) return;
            if (taskGroupType == TaskGroupType.Blocked) //sort tasks into blocks
            {
                foreach (var task in taskOptions)
                {
                    for (int i = 0; i < repetitions; i++)
                    {
                        tasks.Add(task);
                    }
                }
            }
            else
            {
                for (int i = 0; i < repetitions; i++)
                {
                    foreach (var task in taskOptions)
                    {
                        tasks.Add(task);
                    }
                }       
            }
            StartSequence();
        }
        
        public async void StartSequence()
        {
            running = true;
            while(tasks.Count > 0)
            {
                bool taskCompleted = false;
                Task task = GetNextTask();
                task.Perform();
                task.onTaskComplete.AddOnce(() => taskCompleted = true);
                while (!taskCompleted)
                {
                    await System.Threading.Tasks.Task.Yield();
                }
                Debug.Log("Task completed, " + tasks.Count + " remaining");
            }
            running = false;
            Debug.Log(taskOptions.Count * repetitions + " task(s) have been completed.");
            onTaskComplete.Invoke();
        }

        private Task GetNextTask()
        {
            switch (taskGroupType)
            {
                case TaskGroupType.InOrder:
                case TaskGroupType.Blocked:
                    Task t = tasks[0];
                    tasks.RemoveAt(0);
                    return t;
                case TaskGroupType.Randomized:
                    int index = Random.Range(0, tasks.Count);
                    Task rand = tasks[index];
                    tasks.RemoveAt(index);
                    return rand;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
        private enum TaskGroupType
        {
            InOrder,
            Blocked,
            Randomized
        }
    }
}