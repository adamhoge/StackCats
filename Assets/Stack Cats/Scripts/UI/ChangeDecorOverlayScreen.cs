using UnityEngine.UI;

namespace Tofuwu.StackCats.UI
{
    public class ChangeDecorOverlayScreen : OverlayScreen
    {
        public Image ChangeWallpaperButtonImage;
        public Image ChangeFloorButtonImage;
        public Image ChangeWindowButtonImage;
        public Image ChangeDresserButtonImage;

        private DataManager _dataManager;
        private StuffManager _stuffManager;

        public override void OnTransitioningIn()
        {
            base.OnTransitioningIn();

            WallpaperItem wallpaperItem = _stuffManager.WallpaperItems.GetById(_dataManager.HomeData.CurrentWallpaperId);
            ChangeWallpaperButtonImage.sprite = wallpaperItem.Icon;
            ChangeWallpaperButtonImage.SetMaterialForItem(wallpaperItem.ColorShift); ;

            FloorItem floorItem = _stuffManager.FloorItems.GetById(_dataManager.HomeData.CurrentFloorId);
            ChangeFloorButtonImage.sprite = floorItem.Icon;
            ChangeFloorButtonImage.SetMaterialForItem(floorItem.ColorShift);

            ChangeWindowButtonImage.sprite = _stuffManager.WindowItems.GetById(_dataManager.HomeData.CurrentWindowId).Icon;

            DresserItem dresserItem = _stuffManager.DresserItems.GetById(_dataManager.HomeData.CurrentDresserId);
            ChangeDresserButtonImage.sprite = dresserItem.Icon;
            ChangeDresserButtonImage.SetMaterialForItem(dresserItem.ColorShift);
        }

        protected void Awake()
        {
            GameManager gameManager = GameManager.Instance;
            _dataManager = gameManager.Data;
            _stuffManager = gameManager.Stuff;
        }
    }
}