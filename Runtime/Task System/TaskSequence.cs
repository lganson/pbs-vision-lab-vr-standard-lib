using System.Collections.Generic;
using UnityEngine;

namespace Standard_Library
{
    [CreateAssetMenu(fileName = "New Task Sequence", menuName = "TaskSystem/Task Sequence")]
    public class TaskSequence : ScriptableObject
    {
        [field:SerializeField] public bool editorOnly { get; private set;}
        [field: SerializeField] public string sequenceName {get; private set;}
        [field:SerializeField] public string logFilePath {get; private set;}
        [field: SerializeField] public bool logEyeTracking {get; private set;}
        [field:SerializeReference, SubclassSelector] public List<Task> tasks { get; private set;}

        public string GetLogPath()
        {
            return logFilePath;
        }
    }
}
