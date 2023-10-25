using UnityEngine;
using Button = UnityEngine.UI.Button;

public class QuitButton : MonoBehaviour
{
    
    void Start() {
        UnityEngine.UI.Button button = GetComponent<UnityEngine.UI.Button>();
        button.onClick.AddListener(GameManager.QuitGame);
    }
}
