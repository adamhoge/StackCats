using System.Collections.Generic;
using UnityEngine;

namespace Tofuwu.StackCats
{
    public static class SpriteRendererHelper
    {
        public static SpriteRenderer CloneMaterials(this SpriteRenderer spriteRenderer)
        {
            var clonedMaterials = new List<Material>();
            foreach(var material in spriteRenderer.materials)
            {
                var clonedMaterial = Object.Instantiate(material);
                clonedMaterials.Add(clonedMaterial);
            }

            spriteRenderer.materials = clonedMaterials.ToArray();

            return spriteRenderer;
        }

        public static void SetMaterialForItem(this SpriteRenderer spriteRenderer, ItemColorShift colorShift)
        {
            if (colorShift == null) return;

            spriteRenderer.CloneMaterials();
            spriteRenderer.material.SetFloat("_Hue", colorShift.Hue);
            spriteRenderer.material.SetFloat("_Saturation", colorShift.Saturation);
            spriteRenderer.material.SetFloat("_Brightness", colorShift.Brightness);
            spriteRenderer.material.SetFloat("_Contrast", colorShift.Contrast);
        }
    }
}
