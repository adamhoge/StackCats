using System.Collections.Generic;
using System.Linq;

namespace Tofuwu.StackCats.UI
{
    public class ChangeWallpaperOverlayScreen : ChangeDecorSelectableOverlayScreen<WallpaperItem>
    {
        public override WallpaperItem CurrentSelection => HomeDecor.CurrentWallpaper;

        public override List<WallpaperItem> GetSelectionOptions()
        {
            return _stuffManager.WallpaperItems.List.Where(wi => _stuffManager.HasItem(wi)).ToList();
        }

        protected override void OnSelect(WallpaperItem selection)
        {
            HomeDecor.SetWallpaper(selection);
        }
    }
}