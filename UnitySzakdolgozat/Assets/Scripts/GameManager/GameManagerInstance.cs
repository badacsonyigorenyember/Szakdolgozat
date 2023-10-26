using System;
using TMPro;
using UnityEngine;

public class GameManagerInstance : MonoBehaviour
{
    [Min(5)] 
    public int RoomCount;
    
    private void Awake() {
        if (GameManager.RoomCount < 5)
            GameManager.RoomCount = 5;

        GameManager.RoomCount = RoomCount;
        
        GameManager.GenerateMap();
        
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        GameManager.Menu.SetActive(false);
    }

}
