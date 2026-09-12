using Tofuwu.StackCats.Procedural;
using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace Tofuwu.StackCats.UI
{
    public class CatBlockPullTesterSceneUI : MonoBehaviour
    {
        public bool ResetPullTextOnNewPull;
        public CatBlockPullTesterScene CatBlockPullTesterScene;
        public TextMeshProUGUI TotalNumPulls;
        public TextMeshProUGUI TotalCommonCatsText;
        public TextMeshProUGUI TotalUncommonCatsText;
        public TextMeshProUGUI TotalRareCatsText;
        public TextMeshProUGUI TotalSilverPawsText;
        public TextMeshProUGUI PullTextInfo;

        private void OnEnable()
        {
            CatBlockPullTesterScene.onPulled += OnPulled;
            CatBlockPullTesterScene.onTotalsChanged += OnTotalsChanged;
        }
        private void OnPulled(List<CatBlockContents> catBlockContentsList)
        {
            if (ResetPullTextOnNewPull) PullTextInfo.text = string.Empty;

            foreach (var catBlockContents in catBlockContentsList)
            {
                if (catBlockContents.Cat)
                {
                    PullTextInfo.text += $"Cat: {catBlockContents.Cat.Name} ({catBlockContents.Cat.Rarity})\n";
                }
                else if (catBlockContents.NumSilverPaws > 0)
                {
                    PullTextInfo.text += $"Silver Paws: {catBlockContents.NumSilverPaws}\n";
                }
                else
                {
                    PullTextInfo.text += "(nothing)\n";
                }
            }
        }
        private void OnTotalsChanged(int totalNumPulls, int numCommonCats, int numUncommonCats, int numRareCats, int numSilverPaws)
        {
            if (totalNumPulls == 0) PullTextInfo.text = string.Empty;
            TotalNumPulls.text = totalNumPulls.ToString();
            TotalCommonCatsText.text = numCommonCats.ToString();
            TotalUncommonCatsText.text = numUncommonCats.ToString();
            TotalRareCatsText.text = numRareCats.ToString();
            TotalSilverPawsText.text = numSilverPaws.ToString();
        }
    }
}