using UnityEngine;

namespace Tofuwu.StackCats
{
    public class CowCritter : MonoBehaviour
    {
        public enum State
        {
            None,
            Standing,
            Moving,
            Grazing
        }

        public GameObject RootGameObject;
        public GameObject HeadGameObject;
        public GameObject LegFrontRight;
        public GameObject LegBackRight;
        public GameObject LegBackLeft;
        public GameObject LegFrontLeft;
        public float MovementArea = 5.0f;
        public float StandingMinDuration = 0.0f;
        public float StandingMaxDuration = 10.0f;
        public Vector2 HeadDefaultLocalPosition;
        public Vector2 HeadGrazingLocalPosition;
        public float GrazeFrequency = 0.25f;
        public float GrazeDuration = 2.0f;
        public float MoveDistance = 0.2f;
        public int MaxMoves;                

        private State _state;
        private float _stateTimeElapsed;
        private float _rootPosition;
        private float _animationDuration;

        protected void Start()
        {
            _rootPosition = Random.Range(-MovementArea / 2, MovementArea / 2);
            RootGameObject.transform.localPosition = Vector3.right * _rootPosition;
            if (Random.value >= 0.5f)
            {
                RootGameObject.transform.localRotation = Quaternion.Euler(0.0f, 180.0f, 0.0f);
            }
            else
            {
                RootGameObject.transform.localRotation = Quaternion.identity;
            }

            ChangeState(State.Standing);
        }

        protected void Update()
        {
            _stateTimeElapsed += Time.deltaTime;

            switch (_state)
            {
                case State.Standing:
                    break;
                case State.Moving:
                    break;
                case State.Grazing:
                    break;
            }

            if(_stateTimeElapsed >= _animationDuration)
            {
                DoSomethingElse();
            }
        }

        private void ChangeState(State state)
        {
            if (_state == state) return;

            _state = state;
            _stateTimeElapsed = 0.0f;

            switch (_state)
            {
                case State.Standing:
                    _animationDuration = Stand();
                    break;
                case State.Moving:
                    _animationDuration = Move();
                    break;
                case State.Grazing:
                    _animationDuration = Graze();
                    break;
            }
        }

        private void DoSomethingElse()
        {
            switch (_state)
            {
                case State.Standing:
                    if (Random.value > GrazeFrequency)
                    {
                        ChangeState(State.Moving);
                    }
                    else
                    {
                        ChangeState(State.Grazing);
                    }
                    break;
                case State.Moving:
                    ChangeState(State.Standing);
                    break;
                case State.Grazing:
                    ChangeState(State.Standing);
                    break;
            }
        }

        private float Stand()
        {
            return Random.Range(StandingMinDuration, StandingMaxDuration);
        }

        private float Move()
        {
            float rightMovementArea = -_rootPosition + MovementArea / 2;
            float leftMovementArea = _rootPosition + MovementArea / 2;

            bool moveRight = Random.value >= 0.5f;
            if (moveRight && rightMovementArea < MoveDistance) moveRight = false;
            if (!moveRight && leftMovementArea < MoveDistance) moveRight = true;

            int maxMoves;
            if (moveRight)
            {
                maxMoves = Mathf.FloorToInt(rightMovementArea / MoveDistance);
                RootGameObject.transform.localRotation = Quaternion.identity;
            }
            else
            {
                maxMoves = Mathf.FloorToInt(leftMovementArea / MoveDistance);
                RootGameObject.transform.localRotation = Quaternion.Euler(0.0f, 180.0f, 0.0f);
            }

            if (maxMoves > MaxMoves) maxMoves = MaxMoves;
            int numMoves = Random.Range(1, maxMoves + 1);
            for (int i = 0; i < numMoves; i++)
            {
                float moveRootPosition = _rootPosition + (i + 1) * MoveDistance * (moveRight ? 1 : -1);
                float baseDelay = i * 0.5f;
                LeanTween.moveLocalX(RootGameObject, moveRootPosition, 0.5f).setEase(LeanTweenType.easeOutSine).setDelay(baseDelay);
                LeanTween.moveLocalY(RootGameObject, 0.15f, 0.25f).setEase(LeanTweenType.easeOutSine).setDelay(baseDelay);
                LeanTween.moveLocalY(RootGameObject, 0.0f, 0.25f).setEase(LeanTweenType.easeInSine).setDelay(baseDelay + 0.25f);

                float zRotation = i % 2 == 0 ? 25.0f : -25.0f;
                LeanTween.rotateZ(LegFrontRight, zRotation, 0.25f).setEase(LeanTweenType.easeOutSine).setDelay(baseDelay);
                LeanTween.rotateZ(LegBackRight, -zRotation, 0.25f).setEase(LeanTweenType.easeOutSine).setDelay(baseDelay);
                LeanTween.rotateZ(LegFrontLeft, -zRotation, 0.25f).setEase(LeanTweenType.easeOutSine).setDelay(baseDelay);
                LeanTween.rotateZ(LegBackLeft, zRotation, 0.25f).setEase(LeanTweenType.easeOutSine).setDelay(baseDelay);
                LeanTween.rotateZ(LegFrontRight, 0.0f, 0.25f).setEase(LeanTweenType.easeInSine).setDelay(baseDelay + 0.25f);
                LeanTween.rotateZ(LegBackRight, 0.0f, 0.25f).setEase(LeanTweenType.easeInSine).setDelay(baseDelay + 0.25f);
                LeanTween.rotateZ(LegFrontLeft, 0.0f, 0.25f).setEase(LeanTweenType.easeInSine).setDelay(baseDelay + 0.25f);
                LeanTween.rotateZ(LegBackLeft, 0.0f, 0.25f).setEase(LeanTweenType.easeInSine).setDelay(baseDelay + 0.25f);
            }

            _rootPosition += numMoves * MoveDistance * (moveRight ? 1 : -1);

            return 0.5f * numMoves;
        }

        private float Graze()
        {
            float beginGrazingDuration = GrazeDuration / 4.0f;
            float grazingDuration = GrazeDuration / 2.0f;
            float endGrazingDuration = GrazeDuration / 4.0f;
            LeanTween.moveLocalX(HeadGameObject, HeadGrazingLocalPosition.x, beginGrazingDuration).setEase(LeanTweenType.easeOutSine);
            LeanTween.moveLocalY(HeadGameObject, HeadGrazingLocalPosition.y, beginGrazingDuration).setEase(LeanTweenType.easeInSine);
            LeanTween.moveLocalY(HeadGameObject, HeadGrazingLocalPosition.y + 0.05f, grazingDuration / 8).setLoopPingPong(4).setDelay(beginGrazingDuration);
            LeanTween.moveLocalX(HeadGameObject, HeadDefaultLocalPosition.x, endGrazingDuration).setEase(LeanTweenType.easeInSine).setDelay(beginGrazingDuration + grazingDuration);
            LeanTween.moveLocalY(HeadGameObject, HeadDefaultLocalPosition.y, endGrazingDuration).setEase(LeanTweenType.easeOutSine).setDelay(beginGrazingDuration + grazingDuration);
            return GrazeDuration;
        }
    }
}