using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace Codes
{
    public class NewLoad : MonoBehaviour
    {
        public Slider sliderToMove;
        private float _duration = 8f;

        void Start()
        {
            StartCoroutine(MoveSliderOverTime(sliderToMove, 1f, _duration));
        }

        IEnumerator MoveSliderOverTime(Slider slider, float target, float time)
        {
            float initialSliderValue = slider.value;
            float elapsed = 0f;

            while (elapsed < time)
            {
                elapsed += Time.deltaTime;
                slider.value = Mathf.Lerp(initialSliderValue, target, elapsed / time);
                yield return null;
            }

            slider.value = target;
        }
    }
}