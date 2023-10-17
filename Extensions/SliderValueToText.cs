using UnityEngine;
using UnityEngine.UI;

namespace WardIsLove.Extensions
{
    public class SliderValueToText : MonoBehaviour
    {
        public Slider sliderUI = null!;
        private Text textSliderValue = null!;

        private void Start()
        {
            textSliderValue = GetComponent<Text>();
        }

        private void OnGUI()
        {
            ShowSliderValue();
        }

        public void ShowSliderValue()
        {
            string sliderMessage = $"{sliderUI.value}";
            textSliderValue.text = sliderMessage;
        }
    }
}