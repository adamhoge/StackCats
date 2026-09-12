using UnityEngine;
using System.Collections.Generic;
using System;

namespace Tofuwu.StackCats
{
    public class CutSceneScene : SceneBehaviour
    {
        public CutScenePlayer CutScenePlayer;
        public CutScene DefaultCutScene;

        private CutSceneManager _cutSceneManager;

        protected override void Awake()
        {
            base.Awake();

            _cutSceneManager = GameManager.Instance.CutScenes;
        }

        protected override void OnEnable()
        {
            base.OnEnable();

            CutScenePlayer.onStopCutScene += OnStopCutScene;
        }

        protected override void OnDisable()
        {
            base.OnDisable();

            CutScenePlayer.onStopCutScene -= OnStopCutScene;
        }

        protected override void Start()
        {
            base.Start();

            CutScenePlayer.StartCutScene(_cutSceneManager.PendingCutScene ?? DefaultCutScene);
            _cutSceneManager.DequeueCutScene();
        }

        private void OnStopCutScene(CutScene cutScene)
        {
            GameScene returnScene = _cutSceneManager.ReturnScene != null ? _cutSceneManager.ReturnScene.Value : GameScene.Home;
            _gameManager.GameScenes.GoToScene(returnScene, _cutSceneManager.ReturnSceneTransitionSettings);
        }
    }
}