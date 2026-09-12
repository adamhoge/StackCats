using UnityEngine;

namespace Tofuwu.StackCats
{
    public enum FillTransformDirection
    {
        Up,
        Right,
        Down,
        Left
    }

    public delegate void FillTransformFull();

    [RequireComponent(typeof(RectTransform))]
    public class FillTransform : MonoBehaviour
    {
        /// <summary>
        /// Invoked whenever the transform is goes from partially to completely filled in.
        /// </summary>
        public event FillTransformFull onFull;

        /// <summary>
        /// The current fill progress (between 0 and 1)
        /// </summary>
        [Tooltip("The current fill progress (between 0 and 1)")]
        [Range(0.0f, 1.0f)]
        public float Amount;

        /// <summary>
        /// The direction in which the transform fills.
        /// </summary>
        [Tooltip("The direction in which the mask fills.")]
        public FillTransformDirection Direction;

        /// <summary>
        /// The speed at which the transform fills (in percent-per-second)
        /// </summary>
        [Tooltip("The speed at which the mask fills (in percent-per-second)")]
        public float FillSpeed;

        /// <summary>
        /// Flag indicating whether or not the transform is currently filling.
        /// </summary>
        public bool IsFilling { get { return _lerpAmount != Amount; } }

        private RectTransform _rectTransform;
        private float _lerpAmount;

        /// <summary>
        /// Set the fill amount immediately, ignoring fill speed.
        /// </summary>
        /// <param name="amount">The new amount.</param>
        public void SetAmountImmediate(float amount)
        {
            Amount = Mathf.Clamp(amount, 0.0f, 1.0f);
            _lerpAmount = Amount;
            UpdateTransform();
        }

        protected void Awake()
        {
            _rectTransform = GetComponent<RectTransform>();
        }

        protected void Start()
        {
            _lerpAmount = Amount;

            UpdateTransform();
        }

        protected void Update()
        {
            if (_lerpAmount != Amount)
            {
                _lerpAmount = Mathf.MoveTowards(_lerpAmount, Amount, Time.deltaTime * FillSpeed);

                UpdateTransform();

                if (_lerpAmount == Amount)
                {
                    if (onFull != null) onFull();
                }
            }
        }

        private void UpdateTransform()
        {
            switch (Direction)
            {
                case FillTransformDirection.Up:
                    _rectTransform.anchorMin = new Vector2(0.0f, 0.0f);
                    _rectTransform.anchorMax = new Vector2(1.0f, _lerpAmount);
                    break;
                case FillTransformDirection.Right:
                    _rectTransform.anchorMin = new Vector2(0.0f, 0.0f);
                    _rectTransform.anchorMax = new Vector2(_lerpAmount, 1.0f);
                    break;
                case FillTransformDirection.Down:
                    _rectTransform.anchorMin = new Vector2(0.0f, 1.0f - _lerpAmount);
                    _rectTransform.anchorMax = new Vector2(1.0f, 1.0f);
                    break;
                case FillTransformDirection.Left:
                    _rectTransform.anchorMin = new Vector2(1.0f - _lerpAmount, 0.0f);
                    _rectTransform.anchorMax = new Vector2(1.0f, 1.0f);
                    break;
            }
        }
    }
}