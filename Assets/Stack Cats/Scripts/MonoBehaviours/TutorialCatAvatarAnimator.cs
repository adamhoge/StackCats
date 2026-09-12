using UnityEngine;

namespace Tofuwu.StackCats
{
    public class TutorialCatAvatarAnimator : CatAvatarAnimator
    {
        public int MeetingCatTriggerHash = Animator.StringToHash("MeetingCat");
        public int BondingWithCatTriggerHash = Animator.StringToHash("BondingWithCat");
        public int ChatIntroductionsTriggerHash = Animator.StringToHash("ChatIntroductions");
        public int NoticingHouseTriggerHash = Animator.StringToHash("NoticingHouse");
        public int MovingToBlockStackingPuzzleTriggerHash = Animator.StringToHash("MovingToBlockStackingPuzzle");
        public int HeadingHomeTriggerHash = Animator.StringToHash("HeadingHome");

        public void PlayMeetingCat()
        {
            CatAnimator.Play(MeetingCatTriggerHash);
        }

        public void PlayBondingWithCat()
        {
            //CatAnimator.Emote(Emote.Surprise);
            CatAnimator.Play(BondingWithCatTriggerHash);
        }

        public void PlayChatIntroductions()
        {
            CatAnimator.Play(ChatIntroductionsTriggerHash);
        }

        public void PlayNoticingHouse()
        {
            CatAnimator.Play(NoticingHouseTriggerHash);
        }

        public void PlayMovingToBlockStackingPuzzle()
        {
            CatAnimator.Play(MovingToBlockStackingPuzzleTriggerHash);
        }

        public void PlayHeadingHome()
        {
            CatAnimator.Play(HeadingHomeTriggerHash);
        }
    }
}
