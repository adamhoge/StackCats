namespace Tofuwu.StackCats
{
    public class PuzzleMakerDesertFlavoredActions : PuzzleMakerActions
    {
        private DesertFlavoredPuzzle _desertFlavoredPuzzle;

        public void FitStackHeightRequirementsToPuzzle()
        {
            _desertFlavoredPuzzle.StackHeightRequirements.Clear();

            foreach(Stack stack in _desertFlavoredPuzzle.Stacks)
            {
                _desertFlavoredPuzzle.StackHeightRequirements.Add(stack.Blocks.Count);
            }

            _desertFlavoredPuzzle.UpdateStackRequirementLine();
        }

        protected void Start()
        {
            _desertFlavoredPuzzle = (DesertFlavoredPuzzle)Puzzle;
        }
    }
}
