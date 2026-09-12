using System;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Tofuwu.StackCats
{
    public delegate void PlaceableObjectActivated(PlaceableObject placeableObject, DateTime activationTime);

    public class PlaceableObject : MonoBehaviour, IPointerClickHandler
    {
        public event PlaceableObjectActivated onPlaceableObjectActivated;

        public DateTime LastActivationTime;
        public bool Activatable;
        public int ActivationCooldownInSeconds;
        public AudioEvent ClickAudioEvent;

        protected Vector3 _initialScale;

        public virtual void OnPlaced() { }

        public virtual void OnPutAway() { }

        public virtual void OnPointerClick(PointerEventData eventData)
        {
            LeanTween.cancel(gameObject);
            gameObject.transform.localScale = _initialScale;
            LeanTween.scale(gameObject, _initialScale * 1.075f, 0.5f).setEase(LeanTweenType.punch);

            GameManager.Instance.Audio.PlaySoundEffect(ClickAudioEvent);

            if (GetLastActivationInSeconds() >= ActivationCooldownInSeconds)
            {
                OnActivate();
            }
        }

        public int GetLastActivationInSeconds()
        {
            return (int)(DateTime.Now - LastActivationTime).TotalSeconds;
        }

        protected virtual void Start()
        {
            _initialScale = transform.localScale;
        }

        protected virtual void OnActivate()
        {
            LastActivationTime = DateTime.Now;
            if (onPlaceableObjectActivated != null) onPlaceableObjectActivated(this, LastActivationTime);
        }
    }
}