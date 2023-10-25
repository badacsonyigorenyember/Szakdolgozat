using System;
using TMPro;
using UnityEngine;

public class GameManagerInstance : MonoBehaviour
{
    private void Awake() {
        GameManager.GenerateMap();
        
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        GameManager.Menu.SetActive(false);
    }

}
