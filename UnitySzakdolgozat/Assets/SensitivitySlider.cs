using UnityEngine;
using UnityEngine.UI;

public class SensitivitySlider : MonoBehaviour
{
    private Slider slider;
    
    void Start() {
        slider = GetComponent<Slider>();
        slider.onValueChanged.AddListener(GameManager.SetSensitivity);
        slider.value = GameManager.LookingSensitivity;
    }
    
}
