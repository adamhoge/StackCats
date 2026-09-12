using System.Collections.Generic;
using System.Linq;

namespace Tofuwu.StackCats.UI
{
    public class ChangeFloorOverlayScreen : ChangeDecorSelectableOverlayScreen<FloorItem>
    {
        public override FloorItem CurrentSelection => HomeDecor.CurrentFloor;

        public override List<FloorItem> GetSelectionOptions()
        {
            return _stuffManager.FloorItems.List.Where(wi => _stuffManager.HasItem(wi)).ToList();
        }

        protected override void OnSelect(FloorItem selection)
        {
            HomeDecor.SetFloor(selection);
        }
    }
}