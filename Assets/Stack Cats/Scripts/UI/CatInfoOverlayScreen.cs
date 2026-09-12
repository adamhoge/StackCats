using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace Tofuwu.StackCats.UI
{
    public class CatInfoOverlayScreen : OverlayScreen
    {
        public TextMeshProUGUI NameText;
        public TextMeshProUGUI PersonalityText;
        public Image PortraitImage;
        public RectTransform CatAvatarWrapper;
        public CatAvatar CatAvatar;
        public Color NotBondedColor;
        public RectTransform InviteHomeRectTransform;
        public Toggle InviteHomeToggle;
        public TextMeshProUGUI InviteHomeLabel;
        public TextMeshProUGUI MaxInvitesLabel;
        public float RotationSensitivity = 0.5f;
        public bool Rotate;

        public Cat Cat { get { return _cat; } set { SetCat(value); } }

        private HomeManager _homeManager;
        private Cat _cat;
        private Vector3? _pointerDownAt = null;
        private Vector3 _lastRotation = Vector3.zero;
        private bool _isBonded;

        public override void OnTransitioningIn()
        {
            base.OnTransitioningIn();

            _pointerDownAt = null;
            _lastRotation = Vector3.zero;
            CatAvatar.transform.localRotation = Quaternion.Euler(_lastRotation);
        }

        public override void OnTransitioningOut()
        {
            base.OnTransitioningIn();
        }

        protected void Awake()
        {
            _homeManager = GameManager.Instance.Home;
        }

        protected void OnEnable()
        {
            InviteHomeToggle.onValueChanged.AddListener(OnToggleInviteCat);
        }

        protected void OnDisable()
        {
            InviteHomeToggle.onValueChanged.RemoveListener(OnToggleInviteCat);
        }

        protected override void Update()
        {
            base.Update();

            if (Input.GetMouseButtonDown(0))
            {
                _pointerDownAt = Input.mousePosition;
            }
            else if (Input.GetMouseButtonUp(0))
            {
                _pointerDownAt = null;
                _lastRotation = CatAvatar.transform.localRotation.eulerAngles;
            }

            if (_pointerDownAt != null)
            {
                float catRotation = (Input.mousePosition.x - _pointerDownAt.Value.x) * RotationSensitivity;
                CatAvatar.transform.localRotation = Quaternion.Euler(_lastRotation + CatAvatar.transform.up * -catRotation);
            }

            if (Rotate)
            {
                CatAvatar.transform.Rotate(Vector3.up, 45.0f * Time.deltaTime);
            }

            if (Input.GetButtonDown("Cancel")) Dismiss();
        }

        private void SetCat(Cat value)
        {
            if (_cat == value) return;

            _cat = value;
            _isBonded = GameManager.Instance.Cats.IsBonded(_cat);
            bool canInviteCat = _homeManager.CanInviteCats || _homeManager.InvitedCats.Contains(_cat);
            NameText.text = _isBonded ? _cat.Name : "???";
            PersonalityText.text = "Personality: " + _cat.Personality.ToCatPersonalityString();

            PortraitImage.sprite = _cat.Portrait;
            PortraitImage.color = _isBonded ? Color.white : NotBondedColor;
            
            InviteHomeRectTransform.gameObject.SetActive(_isBonded);
            InviteHomeToggle.onValueChanged.RemoveListener(OnToggleInviteCat);
            InviteHomeToggle.isOn = _homeManager.InvitedCats.Contains(_cat);
            InviteHomeToggle.onValueChanged.AddListener(OnToggleInviteCat);
            InviteHomeToggle.interactable = canInviteCat;
            InviteHomeLabel.gameObject.SetActive(canInviteCat);
            MaxInvitesLabel.gameObject.SetActive(!canInviteCat);

            // If bonded, enable the avatar.
            CatAvatar.gameObject.SetActive(false);
            CatAvatar.Cat = _cat;
            CatAvatar.IsShadow = !_isBonded;
            CatAvatar.gameObject.SetActive(true);
        }

        private void OnToggleInviteCat(bool isInvited)
        {
            if (isInvited)
            {
                _homeManager.InviteCat(_cat);
            }
            else
            {
                _homeManager.UninviteCat(_cat);
            }
        }
    }
}