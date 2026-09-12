using UnityEngine;

namespace Tofuwu.StackCats
{
    public delegate void StartCutScene(CutScene cutScene);
    public delegate void StopCutScene(CutScene cutScene);

    public class CutScenePlayer : MonoBehaviour
    {
        public event StartCutScene onStartCutScene;
        public event StopCutScene onStopCutScene;

        private CutScene _currentCutScenePrefab;
        private CutScene _cutSceneInstance;

        public void StartCutScene(CutScene cutScene)
        {
            if (!cutScene) return;

            _currentCutScenePrefab = cutScene;
            _cutSceneInstance = Instantiate(cutScene, transform);
            _cutSceneInstance.onCutSceneEnded += OnCutSceneEnded;

            if (onStartCutScene != null) onStartCutScene(_cutSceneInstance);
        }

        public void Stop()
        {
            if (_cutSceneInstance != null)
            {
                if (onStopCutScene != null) onStopCutScene(_cutSceneInstance);

                Destroy(_cutSceneInstance.gameObject);
            }
        }

        public void Restart()
        {
            Destroy(_cutSceneInstance.gameObject);

            StartCutScene(_currentCutScenePrefab);
        }

        private void OnCutSceneEnded(CutScene cutScene)
        {
            Stop();
        }
    }
}