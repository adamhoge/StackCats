using System;
using UnityEngine;

namespace Tofuwu.StackCats.UI
{
    public class ReplayCutScenesSceneUI : MonoBehaviour
    {
        public CutScene TutorialCutScene;
        public ReplayCutScenesScene ReplayCutScenesScene;
        public RectTransform CutScenesRectTransform;
        public CutSceneInfoUI CutSceneInfoPrefab;

        protected void Start()
        {
            foreach (CutScene cutScene in ReplayCutScenesScene.UnlockedCutScenes)
            {
                CutSceneInfoUI cutSceneInfo = Instantiate(CutSceneInfoPrefab, CutScenesRectTransform);
                cutSceneInfo.CutSceneTitleText.text = cutScene.name;
                cutSceneInfo.Button.onClick.AddListener(delegate () { OnPlayCutScene(cutScene); });
            }
        }

        private void OnPlayCutScene(CutScene cutScene)
        {
            ReplayCutScenesScene.PlayCutScene(cutScene);
        }
    }
}