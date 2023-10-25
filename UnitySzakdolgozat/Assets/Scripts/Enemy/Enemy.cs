using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;
using Random = UnityEngine.Random;

public class Enemy :MonoBehaviour
{
    public Room actualRoom;
    public Room previousRoom;
    public Room starterRoom;
    public GameObject target;

    public List<Vector3> patrolPoints;

    public float waitTime;
    private float timer;
    
    private NavMeshAgent agent;

    public Transform player;
    public bool playerInRange;

    public int maxPatrolPointNumber;
    public EnemyState state;

    public float targetRange, targetAngle, followTime;
    public bool wasChasing;

    private bool isStopped;
    
    
    void Start() {
        transform.position = new Vector3(starterRoom.area.center.x, 0, starterRoom.area.center.y);
        player = GameObject.Find("Player").transform;
        Init();
        SelectPatrolPoints();
        GameManager.AddEnemy(this);
    }

    void Init() {
        agent = transform.GetComponent<NavMeshAgent>();
        agent.Warp(new Vector3(starterRoom.area.center.x, 0, starterRoom.area.center.y));
        patrolPoints = new List<Vector3>();
        actualRoom = starterRoom;
        previousRoom = null;
        target = null;
        state = EnemyState.Idle;
        isStopped = false;
        SelectPatrolPoints();
    }

    private void Update() {
        if (isStopped) {
            return;
        }
        ScanArea();
        Patrol();
        PlayerCatch();
    }

    void Patrol() {
        switch (state) {
            case EnemyState.Idle: {
                agent.speed = 2f;
                if (patrolPoints.Count > 0) {
                    Move(patrolPoints[0]);
                    patrolPoints.RemoveAt(0);
                }
                else {
                    if (wasChasing) {
                        SelectClosestRoomToPatrol();
                        wasChasing = false;
                    }
                    SelectNextRoom();
                }
                break;
            }

            case EnemyState.Waiting: {
                if (timer >= waitTime) {
                    state = EnemyState.Idle;
                    timer = 0;
                }
                else {
                    timer += Time.deltaTime;
                }
                break;
            }

            case EnemyState.Moving: {
                if (agent.remainingDistance < 1f) {
                    state = EnemyState.Waiting;
                }
                
                break;
            }

            case EnemyState.Chasing: {
                agent.speed = 3.5f;
                patrolPoints.Clear();
                agent.SetDestination(target.transform.position);
                break;
            }

        }
    }

    void ScanArea() {
        if (player == null) {
            player = GameObject.Find("Player").transform;
        }
        Transform t = transform;
        Vector3 pos = t.position;
        Vector3 dir = player.position - pos;

        float angle = Vector3.Angle(t.forward, dir);

        if (angle > 45f || dir.sqrMagnitude > targetRange) {
            if (state == EnemyState.Chasing) {
                if (timer < followTime) {
                    timer += Time.deltaTime;
                }
                else {
                    timer = 0;
                    Move(target.transform.position);
                    target = null;
                    wasChasing = true;
                }
                
            }

            return;
        }

        Ray ray = new Ray(pos, dir.normalized);
        
        if (Physics.Raycast(ray, out RaycastHit hit, targetRange)) {
            if (hit.transform.gameObject.layer != LayerMask.NameToLayer("Player")) {
                return;
            }
        }

        if (player.GetComponent<PlayerController>().movement.isMoving) {
        
            state = EnemyState.Chasing;
            target = hit.transform.gameObject;
        }
        
        

    }

    void SelectNextRoom() {
        List<Room> possiblerooms = actualRoom.neighbours.Where(r => !r.locked).ToList();

        if (possiblerooms.Count == 0) {
            SelectPatrolPoints();
            return;
        }

        if (possiblerooms.Count == 1) {
            actualRoom = possiblerooms[0];
            SelectPatrolPoints();
            return;
        }

        possiblerooms.Remove(previousRoom);

        previousRoom = actualRoom;
        actualRoom = possiblerooms[Random.Range(0, possiblerooms.Count)];
        
        possiblerooms.Remove(actualRoom);
        
        
        SelectPatrolPoints(); 
    }


    void SelectClosestRoomToPatrol() {
        List<Room> rooms = GameManager.Rooms;
        Room closest = rooms[0];

        foreach (var room in rooms) {
            if (Vector3.Distance(transform.position, room.area.center) <
                Vector3.Distance(transform.position, closest.area.position) && !room.locked) {
                closest = room;
            }
        }

        actualRoom = closest;
        SelectPatrolPoints();
    }
    
    

    void SelectPatrolPoints() {
        int patrolPointNumber = Math.Min(maxPatrolPointNumber, actualRoom.area.width * actualRoom.area.height / 30);
        while (patrolPoints.Count <= patrolPointNumber) {
            Vector2 point = actualRoom.GetRandPositionInRoom(false);
            
            if (patrolPoints.Any(p => Vector3.Distance(p, new Vector3(point.x, 0, point.y)) < 2f)) {
                point = actualRoom.GetRandPositionInRoom(false);
            }

            patrolPoints.Add(new(point.x, 0, point.y));
        }
    }

    void PlayerCatch() {
        if (Vector3.Distance(player.transform.position, transform.position) < 0.5f) {
            GameManager.Respawn();
        }
    }

    void Move(Vector3 position) {
        state = EnemyState.Moving;
        agent.SetDestination(position);
    }

    public void Stop() {
        agent.isStopped = true;
        isStopped = true;
    }

    public void Resume() {
        agent.isStopped = false;
        isStopped = false;
    }

    public void Respawn() {
        Init();
    }
    
    
    
}