using UnityEngine;
using System.Collections.Generic;
using System.Linq;

namespace Tofuwu.StackCats
{
    public class DesertFlavoredPuzzle : FarmFlavoredPuzzle
    {
        public LineRenderer RequirementsLineRenderer;
        public List<int> StackHeightRequirements = new List<int>();

        public override Stack AddNewStack()
        {
            Stack stack = base.AddNewStack();

            StackHeightRequirements.Add(0);

            return stack;
        }

        public void UpdateStackRequirementLine()
        {
            int numStacks = _stacks.Count;
            float blockOffset = 0.5f + StackSpacing / 2;
            List<Vector3> linePositions = new List<Vector3>();
            linePositions.Add(GetStackLocalPosition(_stacks[0]) - Vector3.left * (-0.5f - StackSpacing / 2));
            for (int i = 0; i < _stacks.Count; i++)
            {
                Stack stack = _stacks[i];
                int stackRequirement = StackHeightRequirements[i];
                if (i > 0 && StackHeightRequirements[i - 1] != stackRequirement)
                {
                    linePositions.Add(GetStackLocalPosition(stack) + Vector3.left * blockOffset + stack.GetBlockLocalPosition(StackHeightRequirements[i - 1]));
                    linePositions.Add(GetStackLocalPosition(stack) + Vector3.left * blockOffset + stack.GetBlockLocalPosition(stackRequirement));
                }
                else if (stackRequirement != 0)
                {
                    linePositions.Add(GetStackLocalPosition(stack) + Vector3.left * blockOffset + stack.GetBlockLocalPosition(stackRequirement));
                }
            }

            Stack lastStack = _stacks[_stacks.Count - 1];
            int lastStackRequirement = StackHeightRequirements[StackHeightRequirements.Count - 1];
            linePositions.Add(GetStackLocalPosition(lastStack) + Vector3.right * blockOffset + lastStack.GetBlockLocalPosition(lastStackRequirement));
            if (lastStackRequirement != 0)
            {
                linePositions.Add(GetStackLocalPosition(lastStack) + Vector3.right * blockOffset);
            }

            RequirementsLineRenderer.positionCount = linePositions.Count;
            RequirementsLineRenderer.SetPositions(linePositions.ToArray());

        }

        protected new void Start()
        {
            base.Start();

            UpdateStackRequirementLine();
        }

        protected override bool IsComplete()
        {
            if (!IsEditMode)
            {
                for (int i = 0; i < _stacks.Count; i++)
                {
                    if (_stacks[i].Blocks.Count != StackHeightRequirements[i]) return false;
                }
            }

            return true;
        }
    }
}