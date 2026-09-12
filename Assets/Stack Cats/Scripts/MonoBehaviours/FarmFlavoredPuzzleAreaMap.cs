using System.Collections.Generic;
using UnityEngine;

namespace Tofuwu.StackCats
{
    public class FarmFlavoredPuzzleAreaMap : MonoBehaviour
    {
        [System.Serializable]
        public class CowPlacementLocation
        {
            public Vector2 Position;
            public float MovementArea;
        }

        public Transform WaterShine;
        public CowCritter Cow1;
        public CowCritter Cow2;
        public CowCritter Cow3;
        public List<CowPlacementLocation> Cow1PlacementLocations = new List<CowPlacementLocation>();
        public List<CowPlacementLocation> Cow2PlacementLocations = new List<CowPlacementLocation>();
        public List<CowPlacementLocation> Cow3PlacementLocations = new List<CowPlacementLocation>();

        public float RotationSpeed = -10.0f;

        protected void Start()
        {
            WaterShine.transform.localRotation = Quaternion.Euler(0, 0, Random.Range(0.0f, 359.0f));

            CowPlacementLocation cow1PlacementLocation = Cow1PlacementLocations.SelectRandom();
            Cow1.transform.position = cow1PlacementLocation.Position;
            Cow1.MovementArea = cow1PlacementLocation.MovementArea;

            CowPlacementLocation cow2PlacementLocation = Cow2PlacementLocations.SelectRandom();
            Cow2.transform.position = cow2PlacementLocation.Position;
            Cow2.MovementArea = cow2PlacementLocation.MovementArea;

            CowPlacementLocation cow3PlacementLocation = Cow3PlacementLocations.SelectRandom();
            Cow3.transform.position = cow3PlacementLocation.Position;
            Cow3.MovementArea = cow3PlacementLocation.MovementArea;
        }

        protected void Update()
        {
            WaterShine.transform.Rotate(Vector3.forward, Time.deltaTime * RotationSpeed);
        }
    }
}