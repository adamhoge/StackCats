using System.Collections.Generic;
using System.Linq;

namespace RockhopperGames.StackCats.UI
{
    public class ChangeWindowOverlayScreen : ChangeDecorSelectableOverlayScreen<WindowItem>
    {
        public override WindowItem CurrentSelection => HomeDecor.CurrentWindow;

        public override List<WindowItem> GetSelectionOptions()
        {
            return _stuffManager.WindowItems.List.Where(wi => _stuffManager.HasItem(wi)).ToList();
        }

        protected override void OnSelect(WindowItem selection)
        {
            HomeDecor.SetWindow(selection);
        }
    }
}
