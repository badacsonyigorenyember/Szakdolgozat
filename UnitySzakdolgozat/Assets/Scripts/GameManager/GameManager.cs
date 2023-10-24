using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
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

    public GameObject menu;
    
    public static Map map;

    public static PlayerController player;
    public static Room starterRoom;
    
    public TextMeshProUGUI taskCountText;
    public int taskCount;
    public static int completedTasksCount;

    public TextMeshProUGUI tutorialText;

    public static float lookingSensitivity = 200f;

    private static Enemy[] enemies;


    private void Start() {
        instance = this;
        
        GenerateMap();

        Tutorial();
        
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        menu.SetActive(false);

    }

    private void GenerateMap() {
        MapGeneration.GenerateMap(roomCount, mapSize, maxRoomSize);
        LogicGeneration.CreateGameMechanic(availableRooms, taskCount - 1);
        MapGeneration.BuildNavMesh();
        completedTasksCount = 0;
        taskCountText.text = "Tasks: " + taskCount + "/" + completedTasksCount;
        enemies = FindObjectsOfType<Enemy>();
    }

    public static void SetStarterRoom(Room room) {
        starterRoom = room;
    }

    

    public void TaskCompleted() {
        completedTasksCount++;
        taskCountText.text = "Tasks: " + taskCount + "/" + completedTasksCount;
    }

    IEnumerator NextLevelRoutine() {
        yield return new WaitForSeconds(5);
        roomCount++;
        taskCount++;
        
        ClearObjects(new [] {"Map", "Player"});
        foreach (var enemy in enemies) {
            Destroy(enemy);
        }
        GenerateMap();
    }

    IEnumerator TutorialRoutine() {
        tutorialText.text = "Hmm... Where am I? What happened?? Anyway, there is a button. What will happen, if I push it? (Press 'E' to interact!)";
        yield return new WaitForSeconds(10);
        tutorialText.text = "";
    }

    public static void Respawn() {
        player.transform.position = new Vector3(starterRoom.area.center.x, 0, starterRoom.area.center.y);
        foreach (var enemy in enemies) {
            enemy.Respawn();
        }
    }

    public void NextLevel() {
        StartCoroutine(NextLevelRoutine());
    }

    public void Tutorial() {
        StartCoroutine(TutorialRoutine());
    }

    void ClearObjects(String[] toDestroyTags) {
        foreach (var t in toDestroyTags) {
            var objectsWithTag = GameObject.FindGameObjectsWithTag(t);
            foreach (var obj in objectsWithTag) {
                Destroy(obj);
            }
        }
    }

    public void SetSensitivity(float value) {
        lookingSensitivity = value;
    }

    public void Pause() {
            menu.SetActive(true);
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
            player.CanLook(false);
            foreach (var enemy in enemies) {
                enemy.Stop();
            }
    }
    
    public void Resume() {
        menu.SetActive(false);
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        player.CanLook(true);
        foreach (var enemy in enemies) {
            enemy.Resume();
        }
    }

    public static void EndGame() {
        Application.Quit();
    }
}
