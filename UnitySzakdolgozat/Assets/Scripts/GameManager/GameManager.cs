using System.Collections.Generic;
using System.Linq;
using GeometryUtils;
using UnityEngine;
using Random = UnityEngine.Random;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    
    public static List<Room> rooms;
    public static List<Room> availableRooms;
    public int roomCount;
    private int actualRooms;
    public int mapSize;
    public int maxRoomSize;

    public Canvas gameUI;
    
    public static bool[,] map;

    public static Transform player;
    
    private Room playerRoom;
    private Room endRoom;




    private void Start() {
        instance = this;
        
        GameStart();
        
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        gameUI.gameObject.SetActive(false);
        
    }

    void GameStart() {
        MapGeneration.GenerateMap(roomCount, mapSize, maxRoomSize);
        GenerationUtils.CreateGameMechanic(availableRooms, playerRoom, endRoom);
    }
    
    public void Pause() {
            gameUI.gameObject.SetActive(true);
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
            player.Find("FirstPersonCamera").GetComponent<PlayerLooking>().enabled = false;
    }
    
    public void Resume() {
        gameUI.gameObject.SetActive(false);
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        player.Find("FirstPersonCamera").GetComponent<PlayerLooking>().enabled = true;
    }

    public static void EndGame() {
        Application.Quit();
    }
}
