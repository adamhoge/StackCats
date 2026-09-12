using System;
using System.Collections.Generic;
using UnityEngine;

namespace Tofuwu.StackCats
{
    public class GalaxyFlavoredScenary : MonoBehaviour
    {
        public GameObject Background;

        [Tooltip("Rotation in degrees per second")]
        public float RotationSpeed = 3.0f;

        public List<GalaxyFlavoredScenaryConstellation> Constellations;

        private float _degreesPerConstellation;
        private int _currentConstellationIndex = -1;

        protected void Start()
        {
            _degreesPerConstellation = 360.0f / Constellations.Count;

            foreach (GalaxyFlavoredScenaryConstellation constellation in Constellations)
            {
                constellation.enabled = false;
            }
        }

        protected void Update()
        {
            float deltaRotation = Time.deltaTime * RotationSpeed;
            Background.transform.Rotate(Vector3.back, deltaRotation);

            float currentConstellationDegrees = Background.transform.eulerAngles.z % _degreesPerConstellation;

            int updatedConstellationIndex = -1;
            if (currentConstellationDegrees > _degreesPerConstellation * 0.25f && currentConstellationDegrees < _degreesPerConstellation * 0.75f)
            {
                updatedConstellationIndex = Mathf.FloorToInt(Background.transform.eulerAngles.z / _degreesPerConstellation) % Constellations.Count;
            }

            if (_currentConstellationIndex != updatedConstellationIndex)
            {
                if (_currentConstellationIndex != -1)
                {
                    Constellations[_currentConstellationIndex].enabled = false;
                }

                _currentConstellationIndex = updatedConstellationIndex;

                if (_currentConstellationIndex != -1)
                {
                    Constellations[_currentConstellationIndex].enabled = true;
                }
            }
        }
    }
}