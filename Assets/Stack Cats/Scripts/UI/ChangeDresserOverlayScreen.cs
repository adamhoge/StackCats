using System.Collections.Generic;
using System.Linq;

namespace RockhopperGames.StackCats.UI
{
    public class ChangeDresserOverlayScreen : ChangeDecorSelectableOverlayScreen<DresserItem>
    {
        public override DresserItem CurrentSelection => HomeDecor.CurrentDresser;

        public override List<DresserItem> GetSelectionOptions()
        {
            return _stuffManager.DresserItems.List.Where(wi => _stuffManager.HasItem(wi)).ToList();
        }

        protected override void OnSelect(DresserItem selection)
        {
            HomeDecor.SetDresser(selection);
        }
    }
}
