using UnityEngine;
using System.Collections.Generic;

namespace Tofuwu.StackCats
{
    public class TitleScene : SceneBehaviour
    {
        //public PuzzleBlock puzzleBlockPrefab;
        //public CatBlock catBlockPrefab;
        //public SumBlock sumBlockPrefab;
        //public float ScrollingStacksSpeed = 1.0f;

        //private const int SCROLLING_STACK_HEIGHT = 24;
        //private const int NUM_SCROLLING_STACKS = 8;

        private bool _isExiting;
        private DataManager _data;
        //private List<List<Block>> _scrollingStacks;
        //private float _scrollingStackOffset;

        public void GoHome()
        {
            if (!_isExiting)
            {
                if (_data.TutorialData.TutorialState != TutorialSceneState.Completed)
                {
                    _gameManager.GoToTutorial();
                }
                else
                {
                    _gameManager.GoHome();
                }
                _isExiting = true;
            }
        }

        protected override void Awake()
        {
            base.Awake();

            _data = GameManager.Instance.Data;

            //_scrollingStacks = new List<List<Block>>();
            //for (int i = 0; i < NUM_SCROLLING_STACKS; i++)
            //{
            //    _scrollingStacks.Add(new List<Block>());
            //    for (int j = 0; j < SCROLLING_STACK_HEIGHT; j++)
            //    {
            //        EnqueueNewBlock(_scrollingStacks[i]);
            //    }
            //}
            //UpdateScrollingBlocks();
        }

        protected void Update()
        {
            //_scrollingStackOffset += Time.deltaTime * ScrollingStacksSpeed;
            //if (_scrollingStackOffset > 1.0f)
            //{
            //    _scrollingStackOffset -= 1.0f;
            //    for (int i = 0; i < _scrollingStacks.Count; i++)
            //    {
            //        List<Block> scrollingStack = _scrollingStacks[i];
            //        scrollingStack.RemoveAt(0);
            //        EnqueueNewBlock(scrollingStack);
            //    }

            //    UpdateScrollingBlocks();
            //}

            //Camera.transform.position = new Vector3(0.0f, _scrollingStackOffset, -10.0f);

            if (!_isExiting && Time.time > 5.0f)
            {
                GoHome();
            }
        }

        //private void UpdateScrollingBlocks()
        //{
        //    for (int i = 0; i < _scrollingStacks.Count; i++)
        //    {
        //        for (int j = 0; j < _scrollingStacks[i].Count; j++)
        //        {
        //            Block block = _scrollingStacks[i][j];
        //            block.transform.position = new Vector2(-NUM_SCROLLING_STACKS / 2 + i + 0.5f, -SCROLLING_STACK_HEIGHT / 2 + j + (i % 2 * 0.5f));
        //        }
        //    }
        //}

        //private void EnqueueNewBlock(List<Block> scrollingStack)
        //{
        //    Block topBlock = scrollingStack.Count > 0 ? scrollingStack[scrollingStack.Count - 1] : null;
        //    Block newBlock;
        //    if (topBlock == null)
        //    {
        //        PuzzleBlock newPuzzleBlock = Instantiate(puzzleBlockPrefab, transform);
        //        newPuzzleBlock.PrimaryNumber = Random.Range(1, 10);
        //        newPuzzleBlock.SecondaryNumber = Random.Range(0, 2);
        //        newBlock = newPuzzleBlock.Block;
        //    }
        //    else
        //    {
        //        PuzzleBlock puzzleBlock = topBlock.GetComponent<PuzzleBlock>();

        //        int blockType;
        //        if (!puzzleBlock)
        //        {
        //            blockType = 0;
        //        }
        //        else
        //        {
        //            float randomNumber = Random.value;

        //            if (randomNumber > 0.4f)
        //            {
        //                blockType = 0;
        //            }
        //            else if (randomNumber > 0.15f)
        //            {
        //                blockType = 1;
        //            }
        //            else
        //            {
        //                blockType = 2;
        //            }
        //        }

        //        if (blockType == 0)
        //        {
        //            PuzzleBlock newPuzzleBlock = Instantiate(puzzleBlockPrefab, transform);
        //            if (puzzleBlock)
        //            {
        //                newPuzzleBlock.PrimaryNumber = Random.value > 0.5f ? puzzleBlock.PrimaryNumber + 1 : puzzleBlock.PrimaryNumber - 1;
        //                if (newPuzzleBlock.PrimaryNumber == 10) newPuzzleBlock.PrimaryNumber = 8;
        //                else if (newPuzzleBlock.PrimaryNumber == 0) newPuzzleBlock.PrimaryNumber = 2;
        //                newPuzzleBlock.SecondaryNumber = (puzzleBlock.SecondaryNumber + 1) % 2;
        //            }
        //            else
        //            {
        //                newPuzzleBlock.PrimaryNumber = Random.Range(1, 10);
        //                newPuzzleBlock.SecondaryNumber = Random.Range(0, 2);
        //            }
        //            newBlock = newPuzzleBlock.Block;
        //        }
        //        else if (blockType == 1)
        //        {
        //            CatBlock newCatBlock = Instantiate(catBlockPrefab, transform);
        //            newBlock = newCatBlock.Block;
        //        }
        //        else
        //        {
        //            SumBlock newSumBlock = Instantiate(sumBlockPrefab, transform);
        //            newSumBlock.SumValue = Random.value > 0.5f ? Random.Range(1, 4) : Random.Range(-2, 0);
        //            newBlock = newSumBlock.Block;
        //        }
        //    }

        //    scrollingStack.Add(newBlock);
        //}
    }
}