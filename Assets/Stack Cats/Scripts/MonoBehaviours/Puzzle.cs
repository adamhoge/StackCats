using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using UnityEngine;

namespace Tofuwu.StackCats
{
    public delegate void BlockMoved(Puzzle puzzle, Stack source, Block block, Stack destination);
    public delegate void AfterBlockMoved(
        Puzzle puzzle,
        Stack source,
        Block block,
        Stack destination
    );
    public delegate void BlockMoveResolved(
        Puzzle puzzle,
        Stack source,
        Block block,
        Stack destination
    );
    public delegate void CatSightingEvent(Cat cat, Puzzle puzzle, int stackIndex, int puzzleIndex);
    public delegate void NewCatRevealedEvent(Cat cat);
    public delegate void CurrencyFound(CatBlock source, Currency currency, int amount);
    public delegate void PuzzleCompleted(Puzzle puzzle, PuzzleCompletionType puzzleCompletionType);

    /// <summary>
    /// Type of puzzle completion.
    /// </summary>
    public enum PuzzleCompletionType
    {
        PuzzleFailed,
        PuzzleSolved,
    }

    public abstract class Puzzle : MonoBehaviour, IIdentifiable
    {
        protected class FallingBlock
        {
            public Stack Stack;
            public Block Block;
            public int FromIndex;
            public int ToIndex;
            public float Delay;
        }

        /// <summary>
        /// The sound effect made when moving blocks.
        /// </summary>
        public AudioEvent BlockMovedSoundEffect;

        /// <summary>
        /// The sound effect made when receiving yarn.
        /// </summary>
        public AudioEvent GetYarnSoundEffect;

        /// <summary>
        /// Invoked whenever a block is moved.
        /// </summary>
        public event BlockMoved onBlockMoved;

        /// <summary>
        /// Invoked after a block is moved.
        /// </summary>
        public event AfterBlockMoved onAfterBlockMoved;

        /// <summary>
        /// Invoke after the block move is fully resolved (mechanically)
        /// </summary>
        public event BlockMoveResolved onBlockMoveResolved;

        /// <summary>
        /// Invoked whenever a cat is seen.
        /// </summary>
        public event CatSightingEvent onCatSighting;

        /// <summary>
        /// Invoked whenever currency is found inside a cat block.
        /// </summary>
        public event CurrencyFound onCurrencyFound;

        /// <summary>
        /// Invoked when the puzzle is completed.
        /// </summary>
        public event PuzzleCompleted onPuzzleCompleted;

        /// <summary>
        /// Prefab used for the creation of new stacks.
        /// </summary>
        [Tooltip("Prefab used for the creation of new stacks.")]
        public Stack StackPrefab;

        /// <summary>
        /// Prefab used for the creation of new puzzle blocks.
        /// </summary>
        [Tooltip("Prefab used for the creation of new puzzle blocks.")]
        public PuzzleBlock PuzzleBlockPrefab;

        /// <summary>
        /// Prefab used for the creation of new cat blocks.
        /// </summary>
        [Tooltip("Prefab used for the creation of new cat blocks.")]
        public CatBlock CatBlockPrefab;

        /// <summary>
        /// Prefab used for the creation of new wild blocks.
        /// </summary>
        [Tooltip("Prefab used for the creation of new wild blocks.")]
        public WildBlock WildBlockPrefab;

        /// <summary>
        /// Prefab used for the creation of new sum blocks.
        /// </summary>
        [Tooltip("Prefab used for the creation of new sum blocks.")]
        public SumBlock SumBlockPrefab;

        /// <summary>
        /// Prefab used for the creation of new terrain blocks.
        /// </summary>
        [Tooltip("Prefab used for the creation of new terrain blocks.")]
        public LockBlock TerrainBlockPrefab;

        /// <summary>
        /// Prefab used for the creation of new removal blocks.
        /// </summary>
        [Tooltip("Prefab used for the creation of new removal blocks.")]
        public KeyBlock RemovalBlockPrefab;

        /// <summary>
        /// Prefab used for the creation of new pressure blocks.
        /// </summary>
        [Tooltip("Prefab used for the creation of new pressure blocks.")]
        public PressureBlock PressureBlockPrefab;

        /// <summary>
        /// Prefab used for the creation of new restricted blocks.
        /// </summary>
        [Tooltip("Prefab used for the creation of new restricted blocks.")]
        public RestrictedBlock RestrictedBlockPrefab;

        /// <summary>
        /// The object pooler used to create cat block removal effects.
        /// </summary>
        [Tooltip("The object pooler used to create cat block removal effects.")]
        public ObjectPooler CatBoxRemovedObjectPooler;

        /// <summary>
        /// The object pooler used to create cat block removal effects.
        /// </summary>
        [Tooltip("The object pooler used to create special cat block removal effects.")]
        public ObjectPooler SpecialCatBoxRemovedObjectPooler;

        /// <summary>
        /// The prefab used to create rarity effects.
        /// </summary>
        [Tooltip("The prefab used to create rarity effects.")]
        public RarityEffect RarityEffectPrefab;

        /// <summary>
        /// Whether or not the puzzle is currently interactable.
        /// </summary>
        public bool IsInteractable = true;

        /// <summary>
        /// Indicates that normal rules will not be executed when blocks are moved.
        /// </summary>
        public bool IsEditMode = false;

        /// <summary>
        /// Indicates whether or not the puzzle has increased rewards.
        /// </summary>
        public bool IsSpecial = false;

        /// <summary>
        /// Indicates whether or not the puzzle has been completed.
        /// </summary>
        public bool IsCompleted
        {
            get { return _isComplete; }
        }

        /// <summary>
        /// The max stack height for stacks used in the puzzle.
        /// </summary>
        public int MaxStackHeight
        {
            get { return _maxStackHeight; }
            set { SetMaxStackHeight(value); }
        }

        /// <summary>
        /// The max movable stack height (adjustable height).
        /// </summary>
        public virtual int MaxMovableStackHeight
        {
            get { return _maxStackHeight; }
        }

        /// <summary>
        /// All stacks associated with the puzzle.
        /// </summary>
        public ReadOnlyCollection<Stack> Stacks
        {
            get { return _stacks.AsReadOnly(); }
        }

        /// <summary>
        /// The space between each stack.
        /// </summary>
        public float StackSpacing
        {
            get { return _stackSpacing; }
            set { SetStackSpacing(value); }
        }

        /// <summary>
        /// The cumulative time elapsed while the puzzle has been interactable.
        /// </summary>
        public float TimeElapsed
        {
            get { return _timeElapsed; }
        }

        /// <summary>
        /// The max blocks that can fit in the current puzzle (based on stack heights).
        /// </summary>
        public int MaxBlocks
        {
            get { return Stacks.Sum(s => s.MaxBlocks); }
        }

        /// <summary>
        /// The total number of moves made on the current puzzle.
        /// </summary>
        public int NumMovesMade
        {
            get { return _numMovesMade; }
            set { _numMovesMade = value; }
        }

        protected CatManager _cats;
        protected GameManager _gameManager;
        protected float _timeElapsed;
        protected int _numMovesMade;
        protected bool _isComplete;
        protected Dictionary<Stack, List<FallingBlock>> _fallingBlocks =
            new Dictionary<Stack, List<FallingBlock>>();

        [SerializeField]
        [HideInInspector]
        protected List<Stack> _stacks = new List<Stack>();

        [SerializeField]
        [HideInInspector]
        protected int _maxStackHeight = 10;

        [SerializeField]
        [HideInInspector]
        protected float _stackSpacing = 0.15f;

        [SerializeField]
        [HideInInspector]
        private Guid _guid;

        public string GetId()
        {
            if (_guid == Guid.Empty)
            {
                _guid = Guid.NewGuid();
            }

            return _guid.ToString();
        }

        /// <summary>
        /// Add a stack to the puzzle.
        /// </summary>
        /// <param name="stack">The stack to be added.</param>
        /// <returns>A flag indicating whether or not the stack was successfully added.</returns>
        public virtual bool AddStack(Stack stack)
        {
            // If the stack is null or already exists, don't add it.
            if (!stack || _stacks.Contains(stack))
                return false;

            // Add the stack.
            _stacks.Add(stack);
            stack.transform.SetParent(transform);

            // Update the position of the stacks in the puzzle.
            UpdateStackPositions();

            // Add a list for falling blocks animations for the stack.
            _fallingBlocks.Add(stack, new List<FallingBlock>());
            stack.onBlocksRemoved += OnBlocksRemoved;

            return true;
        }

        /// <summary>
        /// Remove a stack from the puzzle.
        /// </summary>
        /// <param name="stack">The stack to be removed.</param>
        /// <returns>A flag indicating whether or not the stack was successfully removed.</returns>
        public virtual bool RemoveStack(Stack stack)
        {
            // If the stack doesn't exist in the puzzle, don't attempt to remove it.
            if (!_stacks.Contains(stack))
                return false;

            // Remove the stack.
            _stacks.Remove(stack);
            stack.transform.SetParent(null);

            // Update the position of the stacks in the puzzle.
            UpdateStackPositions();

            // Remove the list for falling block animations for the stack.
            _fallingBlocks.Remove(stack);
            stack.onBlocksRemoved -= OnBlocksRemoved;

            return true;
        }

        /// <summary>
        /// Complete the puzzle.
        /// </summary>
        /// <param name="puzzleCompletionType">The puzzle completion type.</param>
        public void CompletePuzzle(PuzzleCompletionType puzzleCompletionType)
        {
            _isComplete = true;
            OnPuzzleCompleted();
            if (onPuzzleCompleted != null)
                onPuzzleCompleted(this, puzzleCompletionType);
        }

        /// <summary>
        /// Add a new stack to the puzzle.
        /// </summary>
        /// <returns>A flag indicating whether or not the stack was successfully added.</returns>
        public virtual Stack AddNewStack()
        {
            // If the stack prefab isn't defined, don't add a new stack.
            if (!StackPrefab)
                return null;

            // Create the new stack.
            Stack newStack = Instantiate(StackPrefab);
            newStack.name = "Stack";
            newStack.MaxBlocks = MaxStackHeight;

            // Add the new stack, or destroy it if unable to add it.
            bool wasAdded = AddStack(newStack);
            if (!wasAdded)
            {
                DestroyImmediate(newStack.gameObject);
                newStack = null;
            }

            // Return the newly created stack.
            return newStack;
        }

        /// <summary>
        /// Get the local position of a stack.
        /// </summary>
        /// <param name="stack">The stack checked.</param>
        /// <returns></returns>
        public Vector3 GetStackLocalPosition(Stack stack)
        {
            if (!_stacks.Contains(stack))
                return Vector3.zero;

            float xPosition =
                (-(float)(_stacks.Count - 1) / 2 + _stacks.IndexOf(stack)) * (1 + _stackSpacing);
            return new Vector3(xPosition, 0.0f, 0.0f);
        }

        /// <summary>
        /// Add a new puzzle block to the stack.
        /// </summary>
        /// <param name="stack">The stack to which the puzzle block should be added.</param>
        /// <param name="primaryNumber">The primary puzzle block value.</param>
        /// <param name="secondaryNumber">The secondary puzzle block value.</param>
        /// <returns>A flag indicating whether or not the puzzle block was successfully added.</returns>
        public bool AddNewPuzzleBlock(Stack stack, int primaryNumber = 1, int secondaryNumber = 0)
        {
            // If the stack isn't a part of the puzzle or the puzzle block prefab isn't defined, don't add anything.
            if (!_stacks.Contains(stack) || !PuzzleBlockPrefab)
                return false;

            // Create the new puzzle block.
            PuzzleBlock newPuzzleBlock = CreatePuzzleBlock(primaryNumber, secondaryNumber);

            // Add it to the stack, or destroy it if unable to add it.
            bool wasAdded = stack.AddBlock(newPuzzleBlock.GetComponent<Block>());
            if (!wasAdded)
                DestroyImmediate(newPuzzleBlock.gameObject);
            return wasAdded;
        }

        // TODO: Destroy puzzle blocks outside of function.
        /// <summary>
        /// Add a puzzle block to a stack.
        /// </summary>
        /// <param name="stack"></param>
        /// <param name="puzzleBlock"></param>
        /// <returns></returns>
        public bool AddPuzzleBlock(Stack stack, PuzzleBlock puzzleBlock)
        {
            if (!_stacks.Contains(stack))
            {
                DestroyImmediate(puzzleBlock.gameObject);
                return false;
            }

            // Add it to the stack, or destroy it if unable to add it.
            bool wasAdded = stack.AddBlock(puzzleBlock.GetComponent<Block>());
            if (!wasAdded)
                DestroyImmediate(puzzleBlock.gameObject);
            return wasAdded;
        }

        public bool AddBlock(Stack stack, Block block)
        {
            return InsertBlock(stack, stack.Blocks.Count, block);
        }

        public bool InsertBlock(Stack stack, int atIndex, Block block)
        {
            if (!block)
                return false;

            if (!_stacks.Contains(stack))
            {
                DestroyImmediate(block.gameObject);
                return false;
            }

            // Add it to the stack, or destroy it if unable to add it.
            bool wasAdded = stack.InsertBlock(block, atIndex);
            if (!wasAdded)
                DestroyImmediate(block.gameObject);
            return wasAdded;
        }

        /// <summary>
        /// Add a new cat block to the stack.
        /// </summary>
        /// <returns>A flag indicating whether or not the cat block was successfully added.</returns>
        public bool AddNewCatBlock(
            Stack stack,
            Cat cat = null,
            int yarn = 0,
            string catBlockId = ""
        )
        {
            // If the stack isn't a part of the puzzle or the cat block prefab isn't defined, don't add anything.
            if (!_stacks.Contains(stack) || !CatBlockPrefab)
                return false;

            // Create a new cat block.
            CatBlock newCatBlock = CreateCatBlock(cat, yarn, catBlockId);

            // Add it to the stack, or destroy it if unable to add it.
            bool wasAdded = stack.AddBlock(newCatBlock.GetComponent<Block>());
            if (!wasAdded)
                DestroyImmediate(newCatBlock.gameObject);
            newCatBlock.onCatBlockRemoved += OnCatBlockRemoved;

            return wasAdded;
        }

        /// <summary>
        /// Add a new wild block to the stack.
        /// </summary>
        /// <param name="stack">The stack to which the wild block should be added.</param>
        /// <returns>A flag indicating whether or not the wild block was successfully added.</returns>
        public bool AddNewWildBlock(Stack stack)
        {
            // If the stack isn't a part of the puzzle or the terrain block prefab isn't defined, don't add anything.
            if (!_stacks.Contains(stack) || !WildBlockPrefab)
                return false;

            // Create a new terrain block.
            WildBlock newWildBlock = CreateWildBlock();

            // Add it to the stack, or destroy it if unable to add it.
            bool wasAdded = stack.AddBlock(newWildBlock.GetComponent<Block>());
            if (!wasAdded)
                DestroyImmediate(newWildBlock.gameObject);
            return wasAdded;
        }

        /// <summary>
        /// Add a new sum block to the stack.
        /// </summary>
        /// <param name="stack"></param>
        /// <param name="sumValue"></param>
        /// <returns></returns>
        public bool AddNewSumBlock(Stack stack, int sumValue)
        {
            // If the stack isn't a part of the puzzle or the sum block prefab isn't defined, don't add anything.
            if (!_stacks.Contains(stack) || !SumBlockPrefab)
                return false;

            // Create a new sum block.
            SumBlock newSumBlock = Instantiate(SumBlockPrefab);
            newSumBlock.name = "Sum Block";
            newSumBlock.SumValue = sumValue;

            // Add it to the stack, or destroy it if unable to add it.
            bool wasAdded = stack.AddBlock(newSumBlock.GetComponent<Block>());
            if (!wasAdded)
                DestroyImmediate(newSumBlock.gameObject);
            return wasAdded;
        }

        /// <summary>
        /// Add a new terrain block to the stack.
        /// </summary>
        /// <param name="stack"></param>
        /// <returns></returns>
        public bool AddNewTerrainBlock(Stack stack)
        {
            // If the stack isn't a part of the puzzle or the terrain block prefab isn't defined, don't add anything.
            if (!_stacks.Contains(stack) || !TerrainBlockPrefab)
                return false;

            // Create a new terrain block.
            LockBlock newTerrainBlock = CreateTerrainBlock();

            // Add it to the stack, or destroy it if unable to add it.
            bool wasAdded = stack.AddBlock(newTerrainBlock.GetComponent<Block>());
            if (!wasAdded)
                DestroyImmediate(newTerrainBlock.gameObject);
            return wasAdded;
        }

        /// <summary>
        /// Add a new removal block to the stack.
        /// </summary>
        /// <param name="stack"></param>
        /// <returns></returns>
        public bool AddNewRemovalBlock(Stack stack)
        {
            // If the stack isn't a part of the puzzle or the removal block prefab isn't defined, don't add anything.
            if (!_stacks.Contains(stack) || !RemovalBlockPrefab)
                return false;

            // Create a new removal block.
            KeyBlock newRemovalBlock = CreateRemovalBlock();

            // Add it to the stack, or destroy it if unable to add it.
            bool wasAdded = stack.AddBlock(newRemovalBlock.GetComponent<Block>());
            if (!wasAdded)
                DestroyImmediate(newRemovalBlock.gameObject);
            return wasAdded;
        }

        /// <summary>
        /// Add a new pressure block to the stack.
        /// </summary>
        /// <param name="stack"></param>
        /// <returns></returns>
        public bool AddNewPressureBlock(Stack stack, int breakingPoint = 3)
        {
            // If the stack isn't a part of the puzzle or the removal block prefab isn't defined, don't add anything.
            if (!_stacks.Contains(stack) || !PressureBlockPrefab)
                return false;

            // Create a new pressure block.
            PressureBlock newPressureBlock = CreatePressureBlock(breakingPoint);

            // Add it to the stack, or destroy it if unable to add it.
            bool wasAdded = stack.AddBlock(newPressureBlock.GetComponent<Block>());
            if (!wasAdded)
                DestroyImmediate(newPressureBlock.gameObject);
            return wasAdded;
        }

        /// <summary>
        /// Add a new restricted block to the stack.
        /// </summary>
        /// <param name="stack"></param>
        /// <returns></returns>
        public bool AddNewRestrictedBlock(Stack stack)
        {
            // If the stack isn't a part of the puzzle or the restricted block prefab isn't defined, don't add anything.
            if (!_stacks.Contains(stack) || !RestrictedBlockPrefab)
                return false;

            // Create a new restricted block.
            RestrictedBlock newRestrictedBlock = CreateRestrictedBlock();

            // Add it to the stack, or destroy it if unable to add it.
            bool wasAdded = stack.AddBlock(newRestrictedBlock.GetComponent<Block>());
            if (!wasAdded)
                DestroyImmediate(newRestrictedBlock.gameObject);
            return wasAdded;
        }

        /// <summary>
        /// Check if a block can be moved from one stack to another.
        /// </summary>
        /// <param name="source">The stack from which the block should be moved.</param>
        /// <param name="block">The block that should be moved.</param>
        /// <param name="destination">The stack to which the block should be moved.</param>
        /// <returns>A flag indicating whether or not the block can be moved.</returns>
        public virtual bool CanMoveBlock(Stack source, Block block, Stack destination)
        {
            if (!source || !block || !destination || destination == source)
                return false;

            if (IsEditMode)
                return true;

            if (!CanFit(source, block, destination))
                return false;

            return !(!IsMovable(source, block) || !IsPlaceable(block, destination.TopBlock));
        }

        /// <summary>
        /// Check whether or not a block (an any blocks above) can fit in a stack.
        /// </summary>
        /// <param name="source">The stack from which the block should be moved.</param>
        /// <param name="block">The block that should be moved.</param>
        /// <param name="destination">The stack to which the block should be moved.</param>
        /// <returns>A flag indicating whether or not the block can fit.</returns>
        public virtual bool CanFit(Stack source, Block block, Stack destination)
        {
            int numBlocks = source.Blocks.Count - source.Blocks.IndexOf(block);
            return destination.Blocks.Count + numBlocks <= destination.MaxBlocks;
        }

        /// <summary>
        /// Move a block from one stack to another.
        /// </summary>
        /// <param name="source">The stack from which the block should be moved.</param>
        /// <param name="block">The block that should be moved.</param>
        /// <param name="destination">The stack to which the block should be moved.</param>
        /// <returns>A flag indicating whether or not the block was moved.</returns>
        public bool MoveBlock(
            Stack source,
            Block block,
            Stack destination,
            bool dataOnly = false,
            bool ignoreRules = false
        )
        {
            if (!ignoreRules && !CanMoveBlock(source, block, destination))
                return false;

            int blockIndex = source.Blocks.IndexOf(block);

            List<Block> removedBlocks = source.GetBlocksAt(block);
            if (source.RemoveBlocks(block))
            {
                if (!destination.AddBlocks(removedBlocks))
                {
                    source.AddBlocks(removedBlocks);
                }
            }

            if (!dataOnly)
            {
                if (BlockMovedSoundEffect)
                {
                    int stackIndex = _stacks.IndexOf(destination);
                    float panning = (float)stackIndex / (_stacks.Count - 1) * 0.5f - 0.25f;
                    _gameManager.Audio.PlaySoundEffect(BlockMovedSoundEffect, panning);
                }

                for (int i = 0; i < destination.Blocks.Count; i++)
                {
                    Block b = destination.Blocks[i];

                    int blockCount = destination.Blocks.Count;
                    int blocksAffected = blockCount < 5 ? destination.Blocks.Count : 5;
                    int animateIndex = blockCount - blocksAffected;
                    if (i >= animateIndex)
                    {
                        float impactNormal =
                            blockCount == 1
                                ? 0.25f
                                : (i + 1 - animateIndex) / (float)blocksAffected;

                        LeanTween.cancel(b.gameObject);
                        LeanTween
                            .scale(b.gameObject, Vector3.one * 1.025f, 0.25f)
                            .setEase(LeanTweenType.punch);
                        LeanTween
                            .moveLocal(
                                b.gameObject,
                                b.transform.localPosition + Vector3.down * 0.05f * impactNormal,
                                0.75f
                            )
                            .setEase(LeanTweenType.punch);
                    }
                }
            }

            if (!IsEditMode)
            {
                List<Block> movedBlocks = new List<Block>();
                movedBlocks.Add(block);
                movedBlocks.AddRange(block.GetBlocksAbove());
                foreach (Block movedBlock in movedBlocks)
                {
                    foreach (
                        BlockComponent blockComponent in movedBlock.GetComponents<BlockComponent>()
                    )
                    {
                        blockComponent.OnMove();
                    }
                }

                ++_numMovesMade;
                OnBlockMoved(source, block, destination);
                OnStackChanged(source);
                OnStackChanged(destination);

                if (IsComplete())
                    CompletePuzzle(PuzzleCompletionType.PuzzleSolved);

                OnAfterBlockMoved(source, block, destination);
            }

            OnBlockMoveResolved(source, block, destination);

            return true;
        }

        /// <summary>
        /// Get the first movable block in a stack.
        /// </summary>
        /// <param name="stack">The stack being checked.</param>
        /// <returns>The first movable block, or null if no movable blocks exist in the stack.</returns>
        public Block GetFirstMovableBlock(Stack stack)
        {
            if (!Stacks.Contains(stack))
                return null;

            foreach (Block block in stack.Blocks)
            {
                if (IsMovable(stack, block))
                    return block;
            }

            return null;
        }

        /// Get the number of blocks placeable on the stack.
        /// </summary>
        /// <param name="stack">The stack being checked.</param>
        /// <returns>The number of blocks placeable on the stack.</returns>
        public int GetNumBlocksPlaceableOnStack(Stack stack)
        {
            return MaxMovableStackHeight - stack.Blocks.Count;
        }

        /// <summary>
        /// Add currency found.
        /// </summary>
        /// <param name="source"></param>
        /// <param name="currency"></param>
        /// <param name="amount"></param>
        public void AddCurrencyFound(CatBlock source, Currency currency, int amount)
        {
            if (amount == 0)
                return;

            _gameManager.Currency.ChangeCurrency(currency, amount);
            if (onCurrencyFound != null)
                onCurrencyFound(source, currency, amount);
            if (GetYarnSoundEffect)
            {
                _gameManager.Audio.PlaySoundEffect(GetYarnSoundEffect);
            }
        }

        /// <summary>
        /// Add a cat sighting.
        /// </summary>
        /// <param name="cat"></param>
        /// <param name="puzzle"></param>
        /// <param name="stackIndex"></param>
        /// <param name="blockIndex"></param>
        public void AddCatSighting(Cat cat, Puzzle puzzle, int stackIndex, int blockIndex)
        {
            if (onCatSighting != null)
                onCatSighting(cat, puzzle, stackIndex, blockIndex);
        }

        public abstract bool IsMovable(Stack source, Block block);

        public abstract bool IsPlaceable(Block from, Block to);

        public abstract List<string> GetIsPlaceableRuleExceptions(Block from, Block to);

        public PuzzleBlock CreatePuzzleBlock(int primaryNumber = 1, int secondaryNumber = 0)
        {
            if (!PuzzleBlockPrefab)
                return null;

            // Create the new puzzle block.
            PuzzleBlock newPuzzleBlock = Instantiate(PuzzleBlockPrefab);
            newPuzzleBlock.name = "Puzzle Block";
            newPuzzleBlock.PrimaryNumber = primaryNumber;
            newPuzzleBlock.SecondaryNumber = secondaryNumber;

            return newPuzzleBlock;
        }

        public CatBlock CreateCatBlock(Cat cat = null, int yarn = 0, string catBlockId = "")
        {
            if (!CatBlockPrefab)
                return null;

            // Create the new cat block.
            CatBlock newCatBlock = Instantiate(CatBlockPrefab);
            newCatBlock.name = "Cat Block";
            newCatBlock.CatBlockId = string.IsNullOrEmpty(catBlockId)
                ? Guid.NewGuid().ToString()
                : catBlockId;
            newCatBlock.Cat = cat;
            newCatBlock.Yarn = yarn;
            newCatBlock.IsSpecial = IsSpecial;

            return newCatBlock;
        }

        public WildBlock CreateWildBlock()
        {
            if (!WildBlockPrefab)
                return null;

            // Create the new sum block.
            WildBlock newWildBlock = Instantiate(WildBlockPrefab);
            newWildBlock.name = "Wild Block";

            return newWildBlock;
        }

        public SumBlock CreateSumBlock(int sumValue)
        {
            if (!SumBlockPrefab)
                return null;

            // Create the new sum block.
            SumBlock newSumBlock = Instantiate(SumBlockPrefab);
            newSumBlock.name = "Sum Block";
            newSumBlock.SumValue = sumValue;

            return newSumBlock;
        }

        public LockBlock CreateTerrainBlock()
        {
            if (!TerrainBlockPrefab)
                return null;

            // Create the new terrain block.
            LockBlock newTerrainBlock = Instantiate(TerrainBlockPrefab);
            newTerrainBlock.name = "Terrain Block";

            return newTerrainBlock;
        }

        public KeyBlock CreateRemovalBlock()
        {
            if (!RemovalBlockPrefab)
                return null;

            // Create the new terrain block.
            KeyBlock newRemovalBlock = Instantiate(RemovalBlockPrefab);
            newRemovalBlock.name = "Removal Block";

            return newRemovalBlock;
        }

        public PressureBlock CreatePressureBlock(int breakingPoint = 3)
        {
            if (!PressureBlockPrefab)
                return null;

            // Create the new terrain block.
            PressureBlock newPressureBlock = Instantiate(PressureBlockPrefab);
            newPressureBlock.name = "Pressure Block";
            newPressureBlock.BreakingPoint = breakingPoint;

            return newPressureBlock;
        }

        public RestrictedBlock CreateRestrictedBlock()
        {
            if (!RestrictedBlockPrefab)
                return null;

            // Create the new restricted block.
            RestrictedBlock newRestrictedBlock = Instantiate(RestrictedBlockPrefab);
            newRestrictedBlock.name = "Restricted Block";

            return newRestrictedBlock;
        }

        protected virtual void OnBlockMoved(Stack source, Block block, Stack destination)
        {
            if (onBlockMoved != null)
                onBlockMoved(this, source, block, destination);
        }

        protected virtual void OnAfterBlockMoved(Stack source, Block block, Stack destination)
        {
            if (onAfterBlockMoved != null)
                onAfterBlockMoved(this, source, block, destination);
        }

        protected void OnBlockMoveResolved(Stack source, Block block, Stack destination)
        {
            if (onBlockMoveResolved != null)
                onBlockMoveResolved(this, source, block, destination);
        }

        protected virtual bool IsComplete()
        {
            return false;
        }

        protected virtual void OnCatBlockRemoved(CatBlock catBlock)
        {
            Stack stack = catBlock.Block.ParentStack;
            Cat cat = catBlock.Cat;
            if (cat)
            {
                AddCatSighting(
                    cat,
                    this,
                    Stacks.IndexOf(stack),
                    stack.Blocks.IndexOf(catBlock.Block)
                );
                RarityEffect rarityEffect = Instantiate(RarityEffectPrefab, transform);
                rarityEffect.Cat = cat;
                rarityEffect.transform.position = catBlock.transform.position;
            }

            if (catBlock.Yarn > 0)
            {
                AddCurrencyFound(catBlock, Currency.SilverPaw, catBlock.Yarn);
            }

            ObjectPooler catRemovedObjectPooler = IsSpecial
                ? SpecialCatBoxRemovedObjectPooler
                : CatBoxRemovedObjectPooler;
            PoolObject destructionParticles = catRemovedObjectPooler.BorrowInstance();
            destructionParticles.transform.SetParent(transform);
            destructionParticles.transform.position =
                catBlock.transform.position + Vector3.up * 0.5f;
        }

        protected virtual void OnPuzzleCompleted() { }

        protected void Awake()
        {
            _gameManager = GameManager.Instance;
            _cats = _gameManager.Cats;

            foreach (Stack stack in _stacks)
            {
                // Add a list for falling blocks animations for the stack.
                _fallingBlocks.Add(stack, new List<FallingBlock>());
                stack.onBlocksRemoved += OnBlocksRemoved;

                foreach (Block block in stack.Blocks)
                {
                    CatBlock catBlock = block.GetComponent<CatBlock>();
                    if (catBlock)
                        catBlock.onCatBlockRemoved += OnCatBlockRemoved;
                }
            }
        }

        protected void Update()
        {
            if (IsInteractable)
                _timeElapsed += Time.deltaTime;
        }

        protected void AnimateFallingBlock(
            Stack stack,
            Block block,
            int fromIndex,
            int toIndex,
            float delay
        )
        {
            List<FallingBlock> fallingBlocks = _fallingBlocks[stack];
            FallingBlock fallingBlock = fallingBlocks.FirstOrDefault(fb => fb.Block == block);
            if (fallingBlock == null)
            {
                fallingBlock = new FallingBlock
                {
                    Stack = stack,
                    Block = block,
                    FromIndex = fromIndex,
                    ToIndex = toIndex,
                    Delay = delay,
                };
                fallingBlocks.Add(fallingBlock);
            }
            else
            {
                fallingBlock.ToIndex = toIndex;
            }

            // TODO: block is null when two pairs of galaxy blocks trigger in one stack.
            LeanTween.cancel(block.gameObject);
            block.transform.position =
                stack.transform.position + stack.GetBlockLocalPosition(fromIndex);
            LeanTween
                .moveLocal(
                    block.gameObject,
                    block.transform.localPosition + Vector3.up * 0.15f,
                    0.1f
                )
                .setEase(LeanTweenType.easeOutSine);
            LeanTween
                .moveLocal(block.gameObject, stack.GetBlockLocalPosition(toIndex), 0.35f)
                .setEase(LeanTweenType.easeInCubic)
                .setDelay(0.1f + delay)
                .setOnComplete(CompleteFallingBlockAnimation, fallingBlock);
        }

        private void CompleteFallingBlockAnimation(object fallingBlockObject)
        {
            FallingBlock fallingBlock = (FallingBlock)fallingBlockObject;
            Block block = fallingBlock.Block;
            LeanTween
                .moveLocal(
                    block.gameObject,
                    block.transform.localPosition + Vector3.down * 0.025f,
                    0.75f
                )
                .setEase(LeanTweenType.punch);

            _fallingBlocks[fallingBlock.Stack].Remove(fallingBlock);
        }

        private void SetMaxStackHeight(int value)
        {
            if (_maxStackHeight == value)
                return;

            _maxStackHeight = value;
            foreach (Stack stack in _stacks)
            {
                stack.MaxBlocks = _maxStackHeight;
            }
        }

        private void SetStackSpacing(float value)
        {
            if (value == _stackSpacing)
                return;

            _stackSpacing = value;
            UpdateStackPositions();
        }

        private void UpdateStackPositions()
        {
            int numStacks = _stacks.Count;
            for (int i = 0; i < numStacks; i++)
            {
                Stack stack = _stacks[i];
                float xPosition = (-(float)(numStacks - 1) / 2 + i) * (1 + _stackSpacing);
                stack.transform.localPosition = new Vector3(xPosition, 0.0f, 0.0f);
            }
        }

        private void OnStackChanged(Stack stack)
        {
            List<Block> blocksToTrigger = new List<Block>(stack.Blocks);
            while (blocksToTrigger.Count > 0)
            {
                Block blockToTrigger = blocksToTrigger[blocksToTrigger.Count - 1];

                foreach (
                    BlockComponent blockComponent in blockToTrigger.GetComponents<BlockComponent>()
                )
                {
                    blockComponent.OnStackChanged();
                    if (!blockToTrigger)
                        break;
                }

                blocksToTrigger.Remove(blockToTrigger);
            }
        }

        private void OnBlocksRemoved(Stack sender, List<Block> blocks, int fromIndex, int toIndex)
        {
            // TODO: Test if animation exists after object removal.
            if (!IsEditMode)
            {
                List<FallingBlock> fallingBlocks = _fallingBlocks[sender];
                fallingBlocks.RemoveAll(fb => blocks.Contains(fb.Block));

                int numBlocksRemoved = toIndex - fromIndex;
                for (int i = fromIndex; i < sender.Blocks.Count; i++)
                {
                    Block curBlockAbove = sender.Blocks[i];
                    FallingBlock existingFallingBlock = _fallingBlocks[sender]
                        .FirstOrDefault(fb => fb.Block == curBlockAbove);
                    int fallFromIndex;
                    if (existingFallingBlock != null)
                    {
                        fallFromIndex = existingFallingBlock.FromIndex;
                        _fallingBlocks[sender].Remove(existingFallingBlock);
                    }
                    else
                    {
                        fallFromIndex = i + numBlocksRemoved + 1;
                    }
                    int fallToIndex = i;
                    float delay = (i - fromIndex) * 0.05f;

                    AnimateFallingBlock(sender, curBlockAbove, fallFromIndex, fallToIndex, delay);
                }
            }
        }
    }
}
