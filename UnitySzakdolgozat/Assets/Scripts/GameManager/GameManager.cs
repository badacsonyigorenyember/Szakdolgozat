using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public static class GameManager
{
    public static List<Room> Rooms;
    public static List<Room> AvailableRooms;
    public static int RoomCount = 3;
    private static int ActualRooms;
    public static int MapSize = 30;
    public static int MaxRoomSize = 20;
    
    public static Map Map;
    
    public static int TaskCount = 3;
    public static int EnemyCount;
    public static int CompletedTasksCount;
    
    public static float LookingSensitivity = 200f;

    private static List<Enemy> Enemies = new List<Enemy>();

    private static GameObject Menu;
    public static PlayerController Player;
    private static TextMeshProUGUI TaskText;
    

    public static void SetPlayer(PlayerController player) {
        Player = player;
    }

    public static void AddEnemy(Enemy enemy) {
        Enemies.Add(enemy);
    }
    
    public static void GenerateMap() {
        Debug.Log(RoomCount);
        MapGeneration.GenerateMap(RoomCount, MapSize, MaxRoomSize);
        LogicGeneration.CreateGameMechanic(AvailableRooms, TaskCount - 1);
        
        CompletedTasksCount = 0;
        
        Menu = GameObject.Find("Menu");
        
        TaskText = GameObject.Find("TaskCountText").GetComponent<TextMeshProUGUI>();
        TaskText.text = "Tasks: " + 0 + "/" + TaskCount;
    }

    public static void TaskCompleted(bool completed) {
        if (completed) {
            CompletedTasksCount++;
        }
        else {
            CompletedTasksCount--;
        }

        TaskText.text = "Tasks: " + CompletedTasksCount + "/" + TaskCount;
    }

    private static void TutorialText(TextMeshProUGUI tmp) {
        float timer = 0;
        tmp.text = "Hmm... Where am I? What happened?? Anyway, there is a button. What will happen, if I push it? (Press 'E' to interact!)";

        while (timer < 10) {
            timer += Time.deltaTime;
        }
        
        tmp.text = "";
    }

    public static void Respawn() {
        Room playerRoom = Player.starterRoom;
        Player.transform.position = new Vector3(playerRoom.area.center.x, 0, playerRoom.area.center.y);
        
        foreach (var enemy in Enemies) {
            enemy.Respawn();
        }
    }

    public static void NextLevel() {
        TaskCount++;
        RoomCount++;
        SceneManager.LoadScene("SampleScene");
    }

    public static void Pause() {
            Menu.SetActive(true);
            
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
            
            Player.CanLook(false);
            
            foreach (var enemy in Enemies) {
                enemy.Stop();
            }
    }
    
    public static void Resume() {
        Menu.SetActive(false);
        
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        
        Player.CanLook(true);
        
        foreach (var enemy in Enemies) {
            enemy.Resume();
        }
    }

    public static void QuitGame() {
        Application.Quit();
    }
}
