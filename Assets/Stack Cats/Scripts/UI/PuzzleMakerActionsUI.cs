using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Tofuwu.StackCats.UI
{
    [Serializable]
    public class PuzzleMakerActionOptionUI
    {
        public string Label;
        public string ControllerLabel;
        public RectTransform Panel;
        public bool Invertable;
    }

    public abstract class PuzzleMakerActionsUI : MonoBehaviour
    {
        private static readonly KeyCode[] BlockTypeKeyboardShortcuts = new KeyCode[]
        {
            KeyCode.Q,
            KeyCode.W,
            KeyCode.E,
            KeyCode.R,
            KeyCode.T,
            KeyCode.Y,
            KeyCode.U,
            KeyCode.I,
            KeyCode.O,
            KeyCode.P,
        };

        public PuzzleMakerActions PuzzleMakerActions;
        public PuzzleMakerActionsController PuzzleMakerActionsController;
        public List<PuzzleMakerActionOptionUI> PuzzleMakerActionsOptions =
            new List<PuzzleMakerActionOptionUI>();
        public Dropdown PuzzleMakerActionsDropdown;
        public RectTransform CurrentActionRectTransform;
        public RectTransform UnselectedActionsRectTransform;
        public Toggle InvertActionToggle;
        public Button SetCatBlockButton;
        public Button SetPuzzleBlockButton;
        public Button SetWildBlockButton;
        public Button SetSumBlockButton;
        public Button SetTerrainBlockButton;
        public Button SetRemovalBlockButton;
        public Button SetPressureBlockButton;
        public Button SetRestrictedBlockButton;
        public InputField ResizePuzzleStacksInputField;
        public InputField ResizePuzzleMaxBlocksInputField;
        public Button ResizePuzzleButton;
        public Color BlockSelectionColor;

        private const int DEFAULT_NUM_STACKS = 5;
        private const int DEFAULT_MAX_BLOCKS = 8;
        private PuzzleMakerActionOptionUI _currentAction;

        protected virtual void OnEnable()
        {
            PuzzleMakerActionsDropdown.onValueChanged.AddListener(OnPuzzleMakerActionSelected);
            InvertActionToggle.onValueChanged.AddListener(OnToggleInvertAction);
            SetCatBlockButton.onClick.AddListener(OnSetCatBlockButtonClicked);
            SetPuzzleBlockButton.onClick.AddListener(OnSetPuzzleBlockButtonClicked);
            SetWildBlockButton.onClick.AddListener(OnSetWildBlockButtonClicked);
            SetSumBlockButton.onClick.AddListener(OnSetSumBlockButtonClicked);
            SetTerrainBlockButton.onClick.AddListener(OnSetTerrainBlockButtonClicked);
            SetRemovalBlockButton.onClick.AddListener(OnSetRemovalBlockButtonClicked);
            SetPressureBlockButton.onClick.AddListener(OnSetPressureBlockButtonClicked);
            SetRestrictedBlockButton.onClick.AddListener(OnSetRestrictedBlockButtonClicked);
            ResizePuzzleButton.onClick.AddListener(OnResizePuzzleButtonClicked);
        }

        protected virtual void OnDisable()
        {
            PuzzleMakerActionsDropdown.onValueChanged.RemoveListener(OnPuzzleMakerActionSelected);
            InvertActionToggle.onValueChanged.RemoveListener(OnToggleInvertAction);
            SetCatBlockButton.onClick.RemoveListener(OnSetCatBlockButtonClicked);
            SetPuzzleBlockButton.onClick.RemoveListener(OnSetPuzzleBlockButtonClicked);
            SetWildBlockButton.onClick.RemoveListener(OnSetWildBlockButtonClicked);
            SetSumBlockButton.onClick.RemoveListener(OnSetSumBlockButtonClicked);
            SetTerrainBlockButton.onClick.RemoveListener(OnSetTerrainBlockButtonClicked);
            SetRestrictedBlockButton.onClick.RemoveListener(OnSetRestrictedBlockButtonClicked);
            SetRemovalBlockButton.onClick.RemoveListener(OnSetRemovalBlockButtonClicked);
            SetPressureBlockButton.onClick.RemoveListener(OnSetPressureBlockButtonClicked);
            ResizePuzzleButton.onClick.RemoveListener(OnResizePuzzleButtonClicked);
        }

        protected virtual void Start()
        {
            PuzzleMakerActionsController.onBlockTypeChanged += OnAddBlockTypeChanged;

            List<Dropdown.OptionData> dropdownOptionData = new List<Dropdown.OptionData>();
            foreach (PuzzleMakerActionOptionUI actionOption in PuzzleMakerActionsOptions)
            {
                dropdownOptionData.Add(new Dropdown.OptionData(actionOption.Label));
            }
            PuzzleMakerActionsDropdown.options = dropdownOptionData;

            ResizePuzzleStacksInputField.text = DEFAULT_NUM_STACKS.ToString();
            ResizePuzzleMaxBlocksInputField.text = DEFAULT_MAX_BLOCKS.ToString();

            SelectPuzzleMakerAction(PuzzleMakerActionsOptions[0]);

            PuzzleMakerActionsController.ChangeBlockType(BlockType.CatBlock);
        }

        protected virtual void Update()
        {
            PuzzleMakerActionsController.enabled =
                PuzzleMakerActionsDropdown.transform.childCount != 4;

            if (Input.GetKeyDown(KeyCode.I) || Input.GetKeyDown(KeyCode.Tab))
            {
                InvertActionToggle.isOn = !InvertActionToggle.isOn;
            }

            for (int i = 0; i < PuzzleMakerActionsOptions.Count; i++)
            {
                if (Input.GetKeyDown((i + 1).ToString()) && !GetIsActionInputFocused())
                {
                    PuzzleMakerActionsDropdown.value = i;
                }
            }

            for (int i = 0; i < Enum.GetValues(typeof(BlockType)).Length; i++)
            {
                if (Input.GetKeyDown(BlockTypeKeyboardShortcuts[i]) && !GetIsActionInputFocused())
                {
                    PuzzleMakerActionsController.ChangeBlockType((BlockType)i);
                }
            }
        }

        private void SelectPuzzleMakerAction(PuzzleMakerActionOptionUI action)
        {
            if (_currentAction != null)
            {
                _currentAction.Panel.gameObject.SetActive(false);
                _currentAction.Panel.transform.SetParent(UnselectedActionsRectTransform);
            }

            _currentAction = action;
            _currentAction.Panel.gameObject.SetActive(true);
            _currentAction.Panel.transform.SetParent(CurrentActionRectTransform);
            PuzzleMakerActionsController.SetPointerAction(_currentAction.ControllerLabel);

            InvertActionToggle.gameObject.SetActive(_currentAction.Invertable);
            float anchorMaxX = _currentAction.Invertable ? 0.9f : 1.0f;
            CurrentActionRectTransform.anchorMax = new Vector2(anchorMaxX, 1.0f);
            _currentAction.Panel.FitParent();
        }

        private bool GetIsActionInputFocused()
        {
            InputField[] actionInputs = _currentAction.Panel.GetComponentsInChildren<InputField>();
            foreach (InputField actionInput in actionInputs)
            {
                if (actionInput.isFocused)
                {
                    return true;
                }
            }

            return false;
        }

        private void OnToggleInvertAction(bool value)
        {
            PuzzleMakerActionsController.InvertAction = !value;
        }

        private void OnPuzzleMakerActionSelected(int value)
        {
            SelectPuzzleMakerAction(PuzzleMakerActionsOptions[value]);
        }

        private void OnSetCatBlockButtonClicked()
        {
            PuzzleMakerActionsController.ChangeBlockType(BlockType.CatBlock);
        }

        private void OnSetPuzzleBlockButtonClicked()
        {
            PuzzleMakerActionsController.ChangeBlockType(BlockType.PuzzleBlock);
        }

        private void OnSetWildBlockButtonClicked()
        {
            PuzzleMakerActionsController.ChangeBlockType(BlockType.WildBlock);
        }

        private void OnSetSumBlockButtonClicked()
        {
            PuzzleMakerActionsController.ChangeBlockType(BlockType.SumBlock);
        }

        private void OnSetTerrainBlockButtonClicked()
        {
            PuzzleMakerActionsController.ChangeBlockType(BlockType.TerrainBlock);
        }

        private void OnSetRemovalBlockButtonClicked()
        {
            PuzzleMakerActionsController.ChangeBlockType(BlockType.RemovalBlock);
        }

        private void OnSetPressureBlockButtonClicked()
        {
            PuzzleMakerActionsController.ChangeBlockType(BlockType.PressureBlock);
        }

        private void OnSetRestrictedBlockButtonClicked()
        {
            PuzzleMakerActionsController.ChangeBlockType(BlockType.RestrictedBlock);
        }

        private void OnResizePuzzleButtonClicked()
        {
            int numStacks;
            if (!int.TryParse(ResizePuzzleStacksInputField.text, out numStacks))
                numStacks = DEFAULT_NUM_STACKS;

            int numBlocks;
            if (!int.TryParse(ResizePuzzleMaxBlocksInputField.text, out numBlocks))
                numBlocks = DEFAULT_MAX_BLOCKS;

            PuzzleMakerActions.ResizePuzzle(numStacks, numBlocks);
        }

        protected virtual void OnAddBlockTypeChanged(BlockType blockType)
        {
            SetCatBlockButton.image.color =
                blockType == BlockType.CatBlock ? BlockSelectionColor : Color.white;
            SetPuzzleBlockButton.image.color =
                blockType == BlockType.PuzzleBlock ? BlockSelectionColor : Color.white;
            SetSumBlockButton.image.color =
                blockType == BlockType.SumBlock ? BlockSelectionColor : Color.white;
            SetTerrainBlockButton.image.color =
                blockType == BlockType.TerrainBlock ? BlockSelectionColor : Color.white;
            SetRemovalBlockButton.image.color =
                blockType == BlockType.RemovalBlock ? BlockSelectionColor : Color.white;
            SetPressureBlockButton.image.color =
                blockType == BlockType.PressureBlock ? BlockSelectionColor : Color.white;
            SetWildBlockButton.image.color =
                blockType == BlockType.WildBlock ? BlockSelectionColor : Color.white;
            SetRestrictedBlockButton.image.color =
                blockType == BlockType.RestrictedBlock ? BlockSelectionColor : Color.white;
        }
    }
}
