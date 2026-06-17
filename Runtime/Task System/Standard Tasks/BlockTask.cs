#if UNITY_EDITOR
using Standard_Library.Editor;
#endif
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Standard_Library
{
    [System.Serializable]
    public class BlockTask : Task
    {
        #if UNITY_EDITOR
        [Scene]
        #endif
        [SerializeField] private string sceneName;
        [SerializeField] private bool logData;
        [SerializeReference, SubclassSelector] private BlockRunner runner;

        protected override void PerformTask()
        {
            SceneManager.sceneLoaded += OnSceneLoaded;
            SceneManager.LoadScene(sceneName);
        }

        private void OnSceneLoaded(Scene arg0, LoadSceneMode arg1)
        {
            SceneManager.sceneLoaded -= OnSceneLoaded;
            runner.InitBlock();
            runner.RunBlock(logData);
            runner.onBlockComplete.AddOnce(() => onTaskComplete?.Invoke());
        }
    }
}