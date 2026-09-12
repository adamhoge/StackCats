using System.Collections.Generic;
using UnityEngine;

namespace Tofuwu.StackCats
{
    public class FerrisWheel : MonoBehaviour
    {
        public SpriteRenderer FerrisWheelFrameSprite;
        public Sprite CabinSprite;
        public int NumCabins = 12;
        public float RotationSpeed = 0.01f;
        public float Radius = 1.0f;

        private int _startNumCabins;
        private float _rotationPosition;
        private List<SpriteRenderer> _cabinSprites;

        protected void Start()
        {
            _cabinSprites = new List<SpriteRenderer>();
            _startNumCabins = NumCabins;
            for (int i = 0; i < NumCabins; i++)
            {
                SpriteRenderer cabinSprite = new GameObject("Cabin").AddComponent<SpriteRenderer>();
                cabinSprite.transform.SetParent(FerrisWheelFrameSprite.transform);
                float angle = 360.0f * (i/12.0f);
                cabinSprite.transform.localPosition = new Vector2(Mathf.Sin(angle * Mathf.Deg2Rad), Mathf.Cos(angle * Mathf.Deg2Rad)) * Radius;
                cabinSprite.sprite = CabinSprite;
                cabinSprite.sortingLayerID = SortingLayer.NameToID("Background");
                cabinSprite.sortingOrder = 1;
                _cabinSprites.Add(cabinSprite);
            }
        }

        protected void Update()
        {
            _rotationPosition = (_rotationPosition + Time.deltaTime * RotationSpeed * _startNumCabins) % 1.0f;
            FerrisWheelFrameSprite.transform.localRotation = Quaternion.AngleAxis(360.0f / _startNumCabins * _rotationPosition, Vector3.back);
            foreach(SpriteRenderer cabinSprite in _cabinSprites)
            {
                cabinSprite.transform.rotation = Quaternion.identity;
            }
        }
    }
}