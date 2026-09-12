using UnityEngine;
using System.Collections;

namespace Tofuwu.StackCats
{
    public class CatAvatarStandardMannerisms : CatAvatarMannerisms
    {
        public float StretchMinInterval = 30.0f;
        public float ScratchMinInterval = 30.0f;
        public float BatheMinInterval = 30.0f;
        public float SurprisedMinInterval = 90.0f;
        public float SleepMinInterval = 240.0f;
        public float SleepMinDuration = 30.0f;
        public float SleepMaxDuration = 120.0f;
        public float StretchOdds = 0.05f;
        public float ScratchOdds = 0.05f;
        public float BatheOdds = 0.05f;
        public float SurpriseOdds = 0.05f;
        public float SleepOdds = 0.01f;

        private float _wakeTime;
        private float _lastStretchTime;
        private float _lastScratchTime;
        private float _lastBatheTime;
        private float _lastSurprisedTime;
        private float _lastSleepTime;

        public override void CheckNewBehaviour()
        {
            if (!_catAvatar.Animator.IsSleeping)
            {
                if (Time.time >= _lastStretchTime + StretchMinInterval && Random.value > 1.0f - StretchOdds)
                {
                    _catAvatar.Animator.Stretch();
                    _lastStretchTime = Time.time;
                }
                else if (Time.time >= _lastScratchTime + ScratchMinInterval && Random.value > 1.0f - ScratchOdds)
                {
                    _catAvatar.Animator.Scratch();
                    _lastScratchTime = Time.time;
                }
                else if (Time.time >= _lastBatheTime + BatheMinInterval && Random.value > 1.0f - BatheOdds)
                {
                    _catAvatar.Animator.Bathe();
                    _lastBatheTime = Time.time;
                }
                else if (Time.time >= _lastSurprisedTime + SurprisedMinInterval && Random.value > 1.0f - SurpriseOdds)
                {
                    _catAvatar.Animator.Surprised();
                    _lastSurprisedTime = Time.time;
                }
                else if (Time.time >= _lastSleepTime + SleepMinInterval && Random.value > 1.0f - SleepOdds)
                {
                    _catAvatar.Animator.Sleep();
                    _wakeTime = Time.time + Random.Range(SleepMinDuration, SleepMaxDuration);
                }
            }
            else if(Time.time >= _wakeTime)
            {
                _catAvatar.Animator.Stretch();
                _lastStretchTime = Time.time;
                _lastSleepTime = Time.time;
            }
        }

        public override void SetMannerisms(Cat cat)
        {
            // TODO: Create a dictionary of attributes
            if (cat.Personality == CatPersonality.Sleepy)
            {
                StretchOdds = 0.05f;
                ScratchOdds = 0.05f;
                BatheOdds = 0.05f;
                SurpriseOdds = 0.05f;
                SleepOdds = 0.95f;
                SleepMinInterval = 5.0f;
            }
            else
            {
                StretchOdds = 0.05f;
                ScratchOdds = 0.05f;
                BatheOdds = 0.05f;
                SurpriseOdds = 0.05f;
                SleepOdds = 0.01f;
            }
        }

        protected void OnEnable()
        {
            _wakeTime = Time.time;
            _lastStretchTime = Time.time - StretchMinInterval;
            _lastScratchTime = Time.time - ScratchMinInterval;
            _lastBatheTime = Time.time - BatheMinInterval;
            _lastSurprisedTime = Time.time - SurprisedMinInterval;
            _lastSleepTime = Time.time - SleepMinInterval;
        }
    }
}