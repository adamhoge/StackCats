using System.Collections.Generic;
using UnityEngine;

public class FarmFlavoredScenary : MonoBehaviour
{
    public GameObject WindmillFan;
    public List<GameObject> Dandelions = new List<GameObject>();

    [Tooltip("Rotation speed in degrees per second.")]
    public float RotationSpeed;

    protected void Start()
    {
        for (int i = 0; i < Dandelions.Count; i++)
        {
            LeanTween
                .moveX(Dandelions[i], Dandelions[i].transform.position.x + 0.015f, 0.6f)
                .setEase(LeanTweenType.easeInOutSine)
                .setLoopPingPong()
                .setDelay(0.2f * (float)i);
        }
    }

    protected void Update()
    {
        WindmillFan.transform.Rotate(Vector3.back, RotationSpeed * Time.deltaTime);
    }
}
