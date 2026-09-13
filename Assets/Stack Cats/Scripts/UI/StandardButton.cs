using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Tofuwu.StackCats
{
    public class StandardButton : MonoBehaviour, IPointerDownHandler
    {
        private static readonly Color ContentImageDisabledColor = new(255, 255, 255, 0.5f);

        private GameManager _gameManager;

        public Button Button;
        public AudioEvent ButtonDownSoundEffect;
        public Image ContentImage;
        public Image NoninteractableImage;

        private Color _contentImageOriginalColor;
        private bool _lastInteractableState;

        public void OnPointerDown(PointerEventData eventData)
        {
            if (ButtonDownSoundEffect)
                _gameManager.Audio.PlaySoundEffect(ButtonDownSoundEffect);
        }

        protected void Awake()
        {
            _gameManager = GameManager.Instance;

            if (Button == null)
                Button = GetComponent<Button>();
        }

        protected void Start()
        {
            if (ContentImage)
                _contentImageOriginalColor = ContentImage.color;

            UpdateButtonVisualState();

            _lastInteractableState = Button.interactable;
        }

        private void Update()
        {
            if (Button == null || _lastInteractableState == Button.interactable)
                return;

            UpdateButtonVisualState();

            _lastInteractableState = Button.interactable;
        }

        private void UpdateButtonVisualState()
        {
            if (NoninteractableImage)
                NoninteractableImage.gameObject.SetActive(!Button.interactable);

            if (ContentImage)
                ContentImage.color = Button.interactable
                    ? _contentImageOriginalColor
                    : ContentImageDisabledColor;
        }
    }
}
