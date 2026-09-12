using UnityEngine;
using TMPro;

namespace Tofuwu.StackCats.UI
{
    [RequireComponent(typeof(TextMeshProUGUI))]
    public class RainbowText : MonoBehaviour
    {
        public float CycleOffsetInSeconds;

        private TextMeshProUGUI _text;

        protected void Awake()
        {
            _text = GetComponent<TextMeshProUGUI>();
        }

        protected void Update()
        {
            _text.color = HSBColor.ToColor(new HSBColor((Time.time / 4 + CycleOffsetInSeconds) % 1, 0.65f, 1));
        }
    }
}