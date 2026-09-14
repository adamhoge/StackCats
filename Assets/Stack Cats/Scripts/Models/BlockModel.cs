using UnityEngine;

namespace Tofuwu.StackCats.Models
{
    [System.Serializable]
    public class BlockModel
    {
        public bool HasPuzzleBlock
        {
            get { return _hasPuzzleBlock; }
            set { _hasPuzzleBlock = value; }
        }
        public bool HasCatBlock
        {
            get { return _hasCatBlock; }
            set { _hasCatBlock = value; }
        }
        public bool HasWildBlock
        {
            get { return _hasWildBlock; }
            set { _hasWildBlock = value; }
        }
        public bool HasSumBlock
        {
            get { return _hasSumBlock; }
            set { _hasSumBlock = value; }
        }
        public bool HasTerrainBlock
        {
            get { return _hasTerrainBlock; }
            set { _hasTerrainBlock = value; }
        }
        public bool HasRemovalBlock
        {
            get { return _hasRemovalBlock; }
            set { _hasRemovalBlock = value; }
        }
        public bool HasPressureBlock
        {
            get { return _hasPressureBlock; }
            set { _hasPressureBlock = value; }
        }
        public bool HasJigsawBlock
        {
            get { return _hasJigsawBlock; }
            set { _hasJigsawBlock = value; }
        }
        public bool HasGalaxyBlock
        {
            get { return _hasGalaxyBlock; }
            set { _hasGalaxyBlock = value; }
        }
        public bool HasRestrictedBlock
        {
            get { return _hasRestrictedBlock; }
            set { _hasRestrictedBlock = value; }
        }
        public PuzzleBlockComponentModel PuzzleBlock
        {
            get { return _puzzleBlock; }
            set { _puzzleBlock = value; }
        }
        public CatBlockComponentModel CatBlock
        {
            get { return _catBlock; }
            set { _catBlock = value; }
        }
        public WildBlockComponentModel WildBlock
        {
            get { return _wildBlock; }
            set { _wildBlock = value; }
        }
        public SumBlockComponentModel SumBlock
        {
            get { return _sumBlock; }
            set { _sumBlock = value; }
        }
        public TerrainBlockComponentModel TerrainBlock
        {
            get { return _terrainBlock; }
            set { _terrainBlock = value; }
        }
        public RemovalBlockComponentModel RemovalBlock
        {
            get { return _removalBlock; }
            set { _removalBlock = value; }
        }
        public PressureBlockComponentModel PressureBlock
        {
            get { return _pressureBlock; }
            set { _pressureBlock = value; }
        }
        public JigsawBlockComponentModel JigsawBlock
        {
            get { return _jigsawBlock; }
            set { _jigsawBlock = value; }
        }
        public GalaxyBlockComponentModel GalaxyBlock
        {
            get { return _galaxyBlock; }
            set { _galaxyBlock = value; }
        }

        public RestrictedBlockComponentModel RestrictedBlock
        {
            get { return _restrictedBlock; }
            set { _restrictedBlock = value; }
        }

        [SerializeField]
        private bool _hasPuzzleBlock;

        [SerializeField]
        private bool _hasCatBlock;

        [SerializeField]
        private bool _hasWildBlock;

        [SerializeField]
        private bool _hasSumBlock;

        [SerializeField]
        private bool _hasTerrainBlock;

        [SerializeField]
        private bool _hasRemovalBlock;

        [SerializeField]
        private bool _hasPressureBlock;

        [SerializeField]
        private bool _hasJigsawBlock;

        [SerializeField]
        private bool _hasGalaxyBlock;

        [SerializeField]
        private bool _hasRestrictedBlock;

        [SerializeField]
        private PuzzleBlockComponentModel _puzzleBlock;

        [SerializeField]
        private CatBlockComponentModel _catBlock;

        [SerializeField]
        private WildBlockComponentModel _wildBlock;

        [SerializeField]
        private SumBlockComponentModel _sumBlock;

        [SerializeField]
        private TerrainBlockComponentModel _terrainBlock;

        [SerializeField]
        private RemovalBlockComponentModel _removalBlock;

        [SerializeField]
        private PressureBlockComponentModel _pressureBlock;

        [SerializeField]
        private JigsawBlockComponentModel _jigsawBlock;

        [SerializeField]
        private GalaxyBlockComponentModel _galaxyBlock;

        [SerializeField]
        private RestrictedBlockComponentModel _restrictedBlock;
    }
}
