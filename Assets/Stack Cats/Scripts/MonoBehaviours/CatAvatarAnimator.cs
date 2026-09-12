using UnityEngine;

namespace Tofuwu.StackCats
{
    /// <summary>
    /// Provides finer separation of animation from state.
    /// </summary>
    public class CatAvatarAnimator : MonoBehaviour
    {
        private enum State
        {
            Standing,
            Sitting,
            Sleeping
        }

        /// <summary>
        /// The animator used for cat animation.
        /// </summary>
        public Animator CatAnimator;

        /// <summary>
        /// The cat avatar used for cat animation.
        /// </summary>
        public CatAvatar CatAvatar;

        /// <summary>
        /// Particle system that emits hearts.
        /// </summary>
        public ParticleSystem HeartsParticleSystem;

        public int JumpTriggerHash = Animator.StringToHash("Jump");
        public int Stretch1TriggerHash = Animator.StringToHash("Stretch");
        public int Scratch1TriggerHash = Animator.StringToHash("Scratch");
        public int Bathe1TriggerHash = Animator.StringToHash("Bathe");
        public int ChuffingTriggerHash = Animator.StringToHash("Chuffing");
        public int Surprise1TriggerHash = Animator.StringToHash("Surprise");
        public int StateIntHash = Animator.StringToHash("State");

        public bool IsStanding { get { return _state == State.Standing; } }
        public bool IsSitting { get { return _state == State.Sitting; } }
        public bool IsSleeping { get { return _state == State.Sleeping; } }

        private State _state;

        public void Stand()
        {
            ChangeState(State.Standing);
        }

        public void Sit()
        {
            ChangeState(State.Sitting);
        }

        public void Sleep()
        {
            ChangeState(State.Sleeping);
        }

        // TODO: Jump height and duration might be accounted for here. Controlling the position might belong in another script.
        public void Jump()
        {
            ChangeState(State.Standing);
            CatAnimator.SetTrigger(JumpTriggerHash);
        }

        public void Stretch()
        {
            ChangeState(State.Standing);
            CatAnimator.SetTrigger(Stretch1TriggerHash);
        }

        public void Scratch()
        {
            ChangeState(State.Sitting);
            CatAnimator.SetTrigger(Scratch1TriggerHash);
        }

        public void Bathe()
        {
            ChangeState(State.Sitting);
            CatAnimator.SetTrigger(Bathe1TriggerHash);
        }

        public void Chuffing()
        {
            if (_state == State.Sitting || _state == State.Standing)
            {
                HeartsParticleSystem.Play();
                CatAnimator.SetTrigger(ChuffingTriggerHash);
            }
        }

        public void Surprised()
        {
            ChangeState(State.Standing);
            CatAnimator.SetTrigger(Surprise1TriggerHash);
        }

        private void ChangeState(State state)
        {
            if (_state == state) return;

            _state = state;
            CatAnimator.SetInteger(StateIntHash, (int)_state);
        }

        public void OpenEyes()
        {
            CatAvatar.SetEyesOpen(true);
        }

        public void CloseEyes()
        {
            CatAvatar.SetEyesOpen(false);
        }
    }
}