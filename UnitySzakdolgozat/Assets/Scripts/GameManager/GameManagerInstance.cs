using System;
using TMPro;
using UnityEngine;

public class GameManagerInstance : MonoBehaviour
{
    public GameObject Menu;
    
    public int RoomCount;
    public int MapSize;
    public int MaxRoomSize;
    public int TaskCount;
    
    private void Start() {
        GameManager.GenerateMap();
        
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        Menu.SetActive(false);
    }

    public static void SetSensitivity(float value) {
        GameManager.Player.looking.sensitivity = value;
    }

}
