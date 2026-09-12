using Tofuwu.StackCats;
using UnityEngine;

public class PuzzleFlasher : MonoBehaviour
{
    public float FlashDuration = 1.0f;
    public float FlashInterval = 0.25f;
    public Color FlashColor = new Color(1.0f, 1.0f, 1.0f, 0.8f);

    private Puzzle _puzzle;
    private int _nextFlashIndex;
    private float _nextFlashAt;
    private bool _isComplete = true;

    public void FlashPuzzle(Puzzle puzzle)
    {
        _puzzle = puzzle;
        _nextFlashIndex = 0;
        _isComplete = false;
        _nextFlashAt = Time.time;
    }

    protected void Update()
    {
        if(!_isComplete && Time.time >= _nextFlashAt)
        {
            _isComplete = true;
            foreach(Stack stack in _puzzle.Stacks)
            {
                if(_nextFlashIndex < stack.Blocks.Count)
                {
                    stack.Blocks[_nextFlashIndex].FlashBlock(FlashColor, FlashDuration, LeanTweenType.easeInSine);
                    _isComplete = false;
                }
            }

            if (!_isComplete)
            {
                ++_nextFlashIndex;
                _nextFlashAt = Time.time + FlashInterval;
            }
        }
    }
}
