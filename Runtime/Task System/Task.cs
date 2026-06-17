using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;

namespace Standard_Library
{
    [Serializable]
    public abstract class Task
    {
        [SerializeField, Tooltip("This doesn't actually do anything, but names the list item")] private string name;
        public readonly UnityEvent onTaskComplete = new UnityEvent();
        protected float timeStart;
        protected float timeEnd;
        public void Perform()
        {
            timeStart = Time.time;
            onTaskComplete.AddOnce(() => timeEnd = Time.time);
            PerformTask();
        }
        protected abstract void PerformTask();
    }
}