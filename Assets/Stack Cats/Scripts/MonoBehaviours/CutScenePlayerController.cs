using UnityEngine;

namespace Tofuwu.StackCats
{
    public delegate void BeginHoldToSkip();
    public delegate void HoldingToSkip(float progress);
    public delegate void CancelHoldToSkip();
    public delegate void CompleteHoldToSkip();

    public class CutScenePlayerController : MonoBehaviour
    {
        public event BeginHoldToSkip onBeginHoldToStop;
        public event HoldingToSkip onHoldingToStop;
        public event CancelHoldToSkip onCancelHoldToStop;
        public event CompleteHoldToSkip onCompleteHoldToStop;

        public CutScenePlayer CutScenePlayer;
        public float HoldTimeToStop = 3.0f;

        private float? _beginHoldTime;

        protected void Update()
        {
            if (Input.GetMouseButtonDown(0))
            {
                _beginHoldTime = Time.time;

                if (onBeginHoldToStop != null) onBeginHoldToStop();
            }
            else if (Input.GetMouseButtonUp(0))
            {
                _beginHoldTime = null;

                if (onCancelHoldToStop != null) onCancelHoldToStop();
            }

            if (_beginHoldTime != null)
            {
                if (onHoldingToStop != null) onHoldingToStop(Mathf.Clamp((Time.time - _beginHoldTime.Value) / HoldTimeToStop, 0.0f, 1.0f));

                if (Time.time > _beginHoldTime.Value + HoldTimeToStop)
                {
                    CutScenePlayer.Stop();

                    _beginHoldTime = null;

                    if (onCompleteHoldToStop != null) onCompleteHoldToStop();
                }
            }
        }
    }
}