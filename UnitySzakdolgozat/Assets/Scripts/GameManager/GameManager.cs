using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
    
    public static Map map;

    public static PlayerController player;

    public int taskCount;
    

    private void Start() {
        instance = this;
        
        GenerateMap();
        
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        gameUI.gameObject.SetActive(false);
        
    }

    private void GenerateMap() {
        MapGeneration.GenerateMap(roomCount, mapSize, maxRoomSize);
        LogicGeneration.CreateGameMechanic(availableRooms, taskCount);
    }

    IEnumerator NextLevelRoutine() {
        yield return new WaitForSeconds(5);
        roomCount++;
        taskCount++;
        
        ClearObjects(new [] {"Map", "Player"});
        GenerateMap();
    }
    
    

    public void NextLevel() {
        StartCoroutine(NextLevelRoutine());
    }

    void ClearObjects(String[] toDestroyTags) {
        foreach (var t in toDestroyTags) {
            var objectsWithTag = GameObject.FindGameObjectsWithTag(t);
            foreach (var obj in objectsWithTag) {
                Destroy(obj);
            }
        }
    }

    public void Pause() {
            gameUI.gameObject.SetActive(true);
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
            player.CanLook(false);
    }
    
    public void Resume() {
        gameUI.gameObject.SetActive(false);
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        player.CanLook(true);
    }

    public static void EndGame() {
        Application.Quit();
    }
}
