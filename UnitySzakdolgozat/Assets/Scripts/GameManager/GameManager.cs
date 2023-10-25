using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using System;

public static class GameManager
{
    public static List<Room> Rooms;
    public static List<Room> AvailableRooms;
    public static int RoomCount = 6;
    private static int ActualRooms;
    public static int MapSize = 30;
    public static int MaxRoomSize = 20;
    
    public static Map Map;
    
    public static int TaskCount = 3;
    public static int EnemyCount = 1;
    private static int EveryNIsPressurePlate = 5;
    public static int EveryNIsSpike = 2;
    public static int EveryNIsFollowerEnemy = 5;
    public static int CompletedTasksCount;
    
    public static float LookingSensitivity = 400f;

    private static List<Enemy> Enemies = new ();

    public static GameObject Menu;
    public static PlayerController Player;
    private static TextMeshProUGUI TaskText;

    private static bool NeedsTutorial = true;
    

    public static void SetPlayer(PlayerController player) {
        Player = player;
    }

    public static void SetSensitivity(float value) {
        LookingSensitivity = value;
        Player.looking.sensitivity = value;
    }

    public static void AddEnemy(Enemy enemy) {
        Enemies.Add(enemy);
    }
    
    public static void GenerateMap() {
        MapGeneration.GenerateMap(RoomCount, MapSize, MaxRoomSize);
        LogicGeneration.CreateGameMechanic(AvailableRooms, TaskCount - 2, EveryNIsPressurePlate, EnemyCount);
        
        CompletedTasksCount = 0;
        
        Menu = GameObject.Find("Menu");
        
        TaskText = GameObject.Find("TaskCountText").GetComponent<TextMeshProUGUI>();
        TaskText.text = "Tasks: " + 0 + "/" + TaskCount;

        if (NeedsTutorial) {
            OutputTutorial();
            NeedsTutorial = false;
        }
        
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

    private static async void OutputTutorial() {
        TextMeshProUGUI TutorialText = GameObject.Find("TutorialText").GetComponent<TextMeshProUGUI>();
        TutorialText.text = "Hmm... Where am I? What happened?? Anyway, there is a button. What will happen, if I push it? (Press 'E' to interact! To pick up a box press MB1! Enemies Won't see you if you stand still!)";

        await System.Threading.Tasks.Task.Delay(TimeSpan.FromSeconds(10));

        TutorialText.text = "";
    }

    public static void Respawn() {
        Room playerRoom = Player.starterRoom;
        Player.GetComponent<CharacterController>().enabled = false;
        Player.transform.position = new Vector3(playerRoom.area.center.x, 0, playerRoom.area.center.y);
        Player.GetComponent<CharacterController>().enabled = true;

        
        foreach (var enemy in Enemies) {
            if(enemy.TryGetComponent(out MovingEnemy movingEnemy))
                movingEnemy.Respawn();
        }
    }

    public static void NextLevel() {
        TaskCount += 2;
        RoomCount++;
        EnemyCount = (RoomCount - 3) / 2;
        
        Enemies.Clear();
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
