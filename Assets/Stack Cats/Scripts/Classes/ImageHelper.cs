using UnityEngine;
using UnityEngine.UI;

namespace Tofuwu.StackCats
{
    public static class ImageHelper
    {
        public static Image CloneMaterial(this Image image)
        {
            image.material = Object.Instantiate(image.material);

            return image;
        }

        public static void SetMaterialForItem(this Image image, ItemColorShift colorShift)
        {
            if (colorShift == null) return;

            image.CloneMaterial();
            image.material.SetFloat("_Hue", colorShift.Hue);
            image.material.SetFloat("_Saturation", colorShift.Saturation);
            image.material.SetFloat("_Brightness", colorShift.Brightness);
            image.material.SetFloat("_Contrast", colorShift.Contrast);
        }
    }
}
