using UnityEngine;
using UnityEngine.UI;

namespace WardIsLove.Extensions
{
    [RequireComponent(typeof(Text))]
    public class SliderWithText : MonoBehaviour
    {
        public void UpdateLabel(float value)
        {
            Text lbl = GetComponent<Text>();
            if (lbl != null)
                lbl.text = "Repair Amount :" + Mathf.RoundToInt(value);
        }
    }
}