using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Tofuwu.StackCats
{
    public class StandardButton : MonoBehaviour, IPointerDownHandler
    {
        private GameManager _gameManager;

        public Button Button;
        public AudioEvent ButtonDownSoundEffect;
        public Image NoninteractableImage;

        public void OnPointerDown(PointerEventData eventData)
        {
            if (ButtonDownSoundEffect)
                _gameManager.Audio.PlaySoundEffect(ButtonDownSoundEffect);
        }

        protected void Awake()
        {
            _gameManager = GameManager.Instance;
        }

        private void Update()
        {
            if (NoninteractableImage)
                NoninteractableImage.gameObject.SetActive(!Button.interactable);
        }
    }
}
