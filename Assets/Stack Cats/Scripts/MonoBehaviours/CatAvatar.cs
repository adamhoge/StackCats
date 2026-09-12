using UnityEngine;
using System;
using System.Collections.Generic;

namespace Tofuwu.StackCats
{
    public enum CatAvatarBody
    {
        ShortHair,
        MediumHair,
        SmooshFace
    }

    [Serializable]
    public class CatAvatarSettings
    {
        [SerializeField]
        public CatAvatarBody Body;

        [SerializeField]
        public Material BodyMaterial;

        [SerializeField]
        public Material EyesOpenMaterial;

        [SerializeField]
        public Material EyesClosedMaterial;

        [SerializeField]
        public Color EyeColorPrimary = new Color(0.25f, 0.25f, 0.25f, 1.0f);
    }

    [Serializable]
    public class CatAccessory
    {
        [SerializeField]
        public GameObject Prefab;

        [SerializeField]
        public Vector3 Position;

        [SerializeField]
        public Vector3 Rotation;

        [SerializeField]
        public Vector3 Scale = Vector3.one;
    }

    public class CatAvatar : MonoBehaviour
    {
        /// <summary>
        /// The transform to which head accessories should be attached.
        /// </summary>
        public Transform HeadAccessoryTransform;

        /// <summary>
        /// The mesh renderer used for short-haired cats.
        /// </summary>
        public SkinnedMeshRenderer ShortHairedCatMesh;

        /// <summary>
        /// The mesh renderer used for short-haired cat eyes.
        /// </summary>
        public SkinnedMeshRenderer ShortHairedCatEyesMesh;

        /// <summary>
        /// The mesh renderer used for medium-haired cats.
        /// </summary>
        public SkinnedMeshRenderer MediumHairedCatMesh;

        /// <summary>
        /// The mesh renderer used for medium-haired cat eyes.
        /// </summary>
        public SkinnedMeshRenderer MediumHairedCatEyesMesh;

        /// <summary>
        /// The mesh renderer used for smoosh-face cats.
        /// </summary>
        public SkinnedMeshRenderer SmooshFaceCatMesh;

        /// <summary>
        /// The mesh renderer used for smoosh-face cat eyes.
        /// </summary>
        public SkinnedMeshRenderer SmooshFaceCatEyesMesh;

        /// <summary>
        /// The default material used for the mesh when no other materials are provided.
        /// </summary>
        public Material DefaultBodyMaterial;

        /// <summary>
        /// The material used when the cat avatar is a shadow.
        /// </summary>
        public Material ShadowBodyMaterial;

        /// <summary>
        /// The default material used for the open eyes mesh when no other materials are provided.
        /// </summary>
        public Material DefaultEyesOpenMaterial;

        /// <summary>
        /// The default material used for the closed eyes mesh when no other materials are provided.
        /// </summary>
        public Material DefaultEyesClosedMaterial;

        /// <summary>
        /// The material used to create a color overlay for flash effect.
        /// </summary>
        public Material OverlayMaterial;

        /// <summary>
        /// The animator used to control the cat avatar.
        /// </summary>
        public CatAvatarAnimator Animator;

        /// <summary>
        /// The cat represented by the avatar.
        /// </summary>
        public Cat Cat { get { return _cat; } set { SetCat(value); } }

        /// <summary>
        /// Flag indicating whether or not the avatar should be built as a shadow.
        /// </summary>
        public bool IsShadow { get { return _isShadow; } set { SetIsShadow(value); } }

        [SerializeField] private Cat _cat;
        [SerializeField] private SkinnedMeshRenderer _activeBodyMesh;
        [SerializeField] private SkinnedMeshRenderer _activeEyesMesh;
        [SerializeField] private List<Material> _overlayMaterials = new List<Material>();
        private CatAvatarAnimator _animator;
        private bool _isShadow;
        private bool _isInitialized;
        private bool _areEyesOpen = true;

        public void SetEyesOpen(bool areEyesOpen = true)
        {
            if (_areEyesOpen != areEyesOpen)
            {
                _areEyesOpen = areEyesOpen;
                RebuildEyes();
            }
        }

        /// <summary>
        /// Flash the cat avatar the specified color.
        /// </summary>
        /// <param name="color">The color of the flash effect.</param>
        /// <param name="duration">The duration of the flash.</param>
        public void Flash(Color color, float duration)
        {
            foreach (Material overlayMaterial in _overlayMaterials)
            {
                LeanTween.value(1.0f, 0.0f, duration).setEase(LeanTweenType.easeInQuad)
                    .setOnUpdate((float alpha) =>
                    {
                        overlayMaterial.SetColor("_Color", new Color(color.r, color.g, color.b, alpha));
                        overlayMaterial.SetColor("_OutlineColor", new Color(color.r, color.g, color.b, alpha));
                    });
            }
        }

        public void Initialize()
        {
            _areEyesOpen = true;
            RebuildAvatar();
            foreach (CatAvatarMannerisms mannerisms in GetComponents<CatAvatarMannerisms>())
            {
                mannerisms.SetMannerisms(_cat);
            }
            _isInitialized = true;
        }

        protected void Awake()
        {
            _animator = GetComponent<CatAvatarAnimator>();
        }

        protected void Start()
        {
            if (!_isInitialized)
            {
                Initialize();
            }
        }

        private void RebuildAvatar()
        {
            RebuildBody();
            RebuildEyes();
            RebuildAccessories();
            RebuildOverlays();
        }

        private void RebuildBody()
        {
            CatAvatarSettings settings = _cat.AvatarSettings;
            switch (settings.Body)
            {
                case CatAvatarBody.ShortHair: _activeBodyMesh = ShortHairedCatMesh; break;
                case CatAvatarBody.MediumHair: _activeBodyMesh = MediumHairedCatMesh; break;
                case CatAvatarBody.SmooshFace: _activeBodyMesh = SmooshFaceCatMesh; break;
                default: _activeBodyMesh = null; break;
            }
            ShortHairedCatMesh.gameObject.SetActive(_activeBodyMesh == ShortHairedCatMesh);
            MediumHairedCatMesh.gameObject.SetActive(_activeBodyMesh == MediumHairedCatMesh);
            SmooshFaceCatMesh.gameObject.SetActive(_activeBodyMesh == SmooshFaceCatMesh);

            List<Material> bodyMaterials = new List<Material>();
            if (IsShadow)
            {
                bodyMaterials.Add(ShadowBodyMaterial);
            }
            else
            {
                bodyMaterials.Add(settings.BodyMaterial != null ? settings.BodyMaterial : DefaultBodyMaterial);
            }
            _activeBodyMesh.materials = bodyMaterials.ToArray();
        }

        private void RebuildEyes()
        {
            CatAvatarSettings settings = _cat.AvatarSettings;
            switch (settings.Body)
            {
                case CatAvatarBody.ShortHair: _activeEyesMesh = ShortHairedCatEyesMesh; break;
                case CatAvatarBody.MediumHair: _activeEyesMesh = MediumHairedCatEyesMesh; break;
                case CatAvatarBody.SmooshFace: _activeEyesMesh = SmooshFaceCatEyesMesh; break;
                default: _activeBodyMesh = null; break;
            }
            ShortHairedCatEyesMesh.gameObject.SetActive(_activeEyesMesh == ShortHairedCatEyesMesh);
            MediumHairedCatEyesMesh.gameObject.SetActive(_activeEyesMesh == MediumHairedCatEyesMesh);
            SmooshFaceCatEyesMesh.gameObject.SetActive(_activeEyesMesh == SmooshFaceCatEyesMesh);

            List<Material> eyesMaterials = new List<Material>();
            if (_areEyesOpen)
            {
                Material eyesOpenMaterial = settings.EyesOpenMaterial != null ? Instantiate(settings.EyesOpenMaterial) : Instantiate(DefaultEyesOpenMaterial);
                eyesOpenMaterial.SetColor("_Color", IsShadow ? new Color(1.0f, 1.0f, 1.0f, eyesOpenMaterial.GetColor("_Color").a) : settings.EyeColorPrimary);
                eyesMaterials.Add(eyesOpenMaterial);
            }
            else
            {
                Material eyesClosedMaterial = settings.EyesClosedMaterial != null ? Instantiate(settings.EyesClosedMaterial) : Instantiate(DefaultEyesClosedMaterial);
                var eyesClosedAlpha = eyesClosedMaterial.GetColor("_Color").a;
                eyesClosedMaterial.SetColor("_Color", IsShadow ? new Color(1.0f, 1.0f, 1.0f, eyesClosedAlpha) : new Color(0.25f, 0.25f, 0.25f, eyesClosedAlpha));
                eyesMaterials.Add(eyesClosedMaterial);
            }
            _activeEyesMesh.materials = eyesMaterials.ToArray();
        }

        private void RebuildAccessories()
        {
            foreach (Transform accessory in HeadAccessoryTransform)
            {
                Destroy(accessory.gameObject);
            }
            if (_cat.HeadAccessory.Prefab != null)
            {
                GameObject headAccessory = Instantiate(_cat.HeadAccessory.Prefab, HeadAccessoryTransform);
                headAccessory.transform.localPosition = _cat.HeadAccessory.Position;
                headAccessory.transform.localRotation = Quaternion.Euler(_cat.HeadAccessory.Rotation);
                headAccessory.transform.localScale = _cat.HeadAccessory.Scale;
                if (IsShadow)
                {
                    var headAcessoryMeshRenderers = headAccessory.GetComponentsInChildren<MeshRenderer>();
                    foreach (var headAccessoryMeshRenderer in headAcessoryMeshRenderers)
                    {
                        headAccessoryMeshRenderer.materials = new[] { ShadowBodyMaterial };
                    }
                }
            }
        }

        private void RebuildOverlays()
        {
            _overlayMaterials.Clear();

            List<Material> bodyMaterials = new List<Material>(_activeBodyMesh.materials);
            bodyMaterials.Add(CreateOverlayMaterialInstance());
            _activeBodyMesh.materials = bodyMaterials.ToArray();

            foreach (Transform accessory in HeadAccessoryTransform)
            {
                MeshRenderer[] meshRenderers = accessory.GetComponentsInChildren<MeshRenderer>();
                foreach (MeshRenderer meshRenderer in meshRenderers)
                {
                    List<Material> accessoryMaterials = new List<Material>(meshRenderer.materials);
                    accessoryMaterials.Add(CreateOverlayMaterialInstance());
                    meshRenderer.materials = accessoryMaterials.ToArray();
                }
            }
        }

        private Material CreateOverlayMaterialInstance()
        {
            Material overlayMaterialInstance = Instantiate(OverlayMaterial);
            overlayMaterialInstance.SetColor("_Color", Color.clear);
            _overlayMaterials.Add(overlayMaterialInstance);
            return overlayMaterialInstance;
        }

        private void SetCat(Cat value)
        {
            if (_cat == value) return;

            _cat = value;
            Initialize();
        }

        private void SetIsShadow(bool isShadow)
        {
            if (_isShadow == isShadow) return;

            _isShadow = isShadow;
            RebuildAvatar();
        }
    }
}