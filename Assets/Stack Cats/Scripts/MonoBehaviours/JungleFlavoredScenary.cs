using UnityEngine;
using System.Collections.Generic;
using Tofuwu.StackCats;
using System.Linq;

// TODO: Troubleshoot light beams staying active, probably due to the dictionary value
public class JungleFlavoredScenary : MonoBehaviour
{
    public List<SpriteRenderer> LightBeamSpriteRenderers;
    public int StartingLightBeams = 3;
    public float LightBeamMinActivationDelay = 0.5f;
    public float LightBeamMaxActivationDelay = 1.5f;
    public float LightBeamDuration = 3.0f;
    public LeanTweenType LightBeamEasing = LeanTweenType.easeOutSine;

    private Dictionary<SpriteRenderer, float> _activeLightBeams = new Dictionary<SpriteRenderer, float>();
    private float _nextLightBeamTime;

    protected void Start()
    {
        foreach (SpriteRenderer lightBeam in LightBeamSpriteRenderers)
        {
            lightBeam.color = Constants.ClearWhite;
        }
    }

    protected void Update()
    {
        List<SpriteRenderer> lightBeamsToRemove = new List<SpriteRenderer>();
        foreach (KeyValuePair<SpriteRenderer, float> activeLightBeam in _activeLightBeams)
        {
            if (Time.time >= activeLightBeam.Value)
            {
                lightBeamsToRemove.Add(activeLightBeam.Key);
            }
        }
        foreach (SpriteRenderer spriteRenderer in lightBeamsToRemove)
        {
            _activeLightBeams.Remove(spriteRenderer);
        }

        if (Time.time >= _nextLightBeamTime)
        {
            List<SpriteRenderer> availableLightBeams = LightBeamSpriteRenderers.Where(lbr => !_activeLightBeams.ContainsKey(lbr)).ToList();
            if (availableLightBeams.Count > 0)
            {
                ActivateLightBeam(availableLightBeams.SelectRandom());
                _nextLightBeamTime = Time.time + Random.Range(LightBeamMinActivationDelay, LightBeamMaxActivationDelay);
            }
        }
    }

    private void ActivateLightBeam(SpriteRenderer lightBeam)
    {
        if (_activeLightBeams.ContainsKey(lightBeam))
        {
            return;
        }

        LeanTween.cancel(lightBeam.gameObject);
        lightBeam.color = Constants.ClearWhite;
        LeanTween.alpha(lightBeam.gameObject, 0.5f, LightBeamDuration / 2.0f)
            .setEase(LightBeamEasing)
            .setLoopPingPong(1);
        _activeLightBeams.Add(lightBeam, Time.time + LightBeamDuration);
    }
}
