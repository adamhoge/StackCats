using System;
using UnityEngine;
using Tofuwu.StackCats.Procedural;

namespace Tofuwu.StackCats
{
    public class PuzzleGeneratorScene : SceneBehaviour
    {
        public enum GeneratorType
        {
            Farm,
            Jungle,
            Night,
            Desert
        }

        public PuzzleArea FarmFlavoredPuzzleArea;
        public JungleFlavoredPuzzleArea JungleFlavoredPuzzleArea;
        public PuzzleArea NightFlavoredPuzzleArea;
        public PuzzleArea DesertFlavoredPuzzleArea;
        public bool AutoBuild = false;
        public float AutoBuildInterval = 3.0f;
        public GeneratorType SelectedGeneratorType;
        public int Difficulty = 50;
        public float GenerationInterval = 0.1f;

        private FarmFlavoredPuzzleGenerator _farmFlavoredPuzzleGenerator;
        private JungleFlavoredPuzzleGenerator _jungleFlavoredPuzzleGenerator;
        private NightFlavoredPuzzleGenerator _nightFlavoredPuzzleGenerator;
        private DesertFlavoredPuzzleGenerator _desertFlavoredPuzzleGenerator;

        public bool IsGenerating
        {
            get
            {
                return _isGenerating;
            }
        }

        public int CurrentPuzzleNumMovesMade { get; private set; }
        
        public float CurrentPuzzleEstimatedDifficulty { get; private set; }
        
        private float _lastBuildTime;
        private Puzzle _lastBuiltPuzzle;
        private bool _isGenerating;

        private void OnPuzzleGenerated<TPuzzleArea, TPuzzle>(GeneratedPuzzleInfo<TPuzzleArea, TPuzzle> generatedPuzzleInfo) where TPuzzleArea : PuzzleArea where TPuzzle : Puzzle
        {
            _lastBuiltPuzzle = generatedPuzzleInfo.Puzzle;
            _isGenerating = false;
        }

        private void OnPerformedMoveAction<TPuzzleArea, TPuzzle>(GeneratedPuzzleInfo<TPuzzleArea, TPuzzle> generatedPuzzleInfo) where TPuzzleArea : PuzzleArea where TPuzzle : Puzzle
        {
            CurrentPuzzleNumMovesMade = generatedPuzzleInfo.NumMovesMade;
            CurrentPuzzleEstimatedDifficulty = generatedPuzzleInfo.EstimatedDifficulty;
        }

        protected override void Awake()
        {
            base.Awake();

            _farmFlavoredPuzzleGenerator = new FarmFlavoredPuzzleGenerator(FarmFlavoredPuzzleArea);
            _jungleFlavoredPuzzleGenerator = new JungleFlavoredPuzzleGenerator(JungleFlavoredPuzzleArea);
            _nightFlavoredPuzzleGenerator = new NightFlavoredPuzzleGenerator(NightFlavoredPuzzleArea);
            _desertFlavoredPuzzleGenerator = new DesertFlavoredPuzzleGenerator(DesertFlavoredPuzzleArea);

            _farmFlavoredPuzzleGenerator.onPerformedMoveAction += OnPerformedMoveAction;
            _jungleFlavoredPuzzleGenerator.onPerformedMoveAction += OnPerformedMoveAction;
            _nightFlavoredPuzzleGenerator.onPerformedMoveAction += OnPerformedMoveAction;
            _desertFlavoredPuzzleGenerator.onPerformedMoveAction += OnPerformedMoveAction;
            _farmFlavoredPuzzleGenerator.onPuzzleGenerated += OnPuzzleGenerated;
            _jungleFlavoredPuzzleGenerator.onPuzzleGenerated += OnPuzzleGenerated;
            _nightFlavoredPuzzleGenerator.onPuzzleGenerated += OnPuzzleGenerated;
            _desertFlavoredPuzzleGenerator.onPuzzleGenerated += OnPuzzleGenerated;
        }

        protected void Update()
        {
            if (!_isGenerating && (Input.GetKeyDown(KeyCode.Space) || (AutoBuild && Time.time > _lastBuildTime + AutoBuildInterval)))
            {
                if (_lastBuiltPuzzle)
                {
                    foreach(CatBlock catBlock in _lastBuiltPuzzle.GetAllBlocksOfComponent<CatBlock>())
                    {
                        catBlock.Yarn = 0;
                        catBlock.Cat = null;
                    }
                    GUIUtility.systemCopyBuffer = PuzzleBuilder.GetPuzzleJsonData(_lastBuiltPuzzle);
                    Destroy(_lastBuiltPuzzle.gameObject);
                }

                switch (SelectedGeneratorType)
                {
                    case GeneratorType.Farm:
                        _isGenerating = true;
                        StartCoroutine(_farmFlavoredPuzzleGenerator.GeneratePuzzleAtInterval(Difficulty, GenerationInterval));
                        break;
                    case GeneratorType.Jungle:
                        _isGenerating = true;
                        StartCoroutine(_jungleFlavoredPuzzleGenerator.GeneratePuzzleAtInterval(Difficulty, GenerationInterval));
                        break;
                    case GeneratorType.Night:
                        _isGenerating = true;
                        StartCoroutine(_nightFlavoredPuzzleGenerator.GeneratePuzzleAtInterval(Difficulty, GenerationInterval));
                        break;
                    case GeneratorType.Desert:
                        _isGenerating = true;
                        StartCoroutine(_desertFlavoredPuzzleGenerator.GeneratePuzzleAtInterval(Difficulty, GenerationInterval));
                        break;
                }

                _lastBuildTime = Time.time;
            }
        }
    }
}