using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;
using Random = UnityEngine.Random;

public class Enemy :MonoBehaviour
{
    public Room actualRoom;
    public Room previousRoom;
    public GameObject target;

    public List<Vector3> patrolPoints;

    public float waitTime;
    private float timer;
    
    private NavMeshAgent agent;

    public Transform player;
    public bool playerInRange;

    public int patrolPointNumber;
    public EnemyState state;

    public float targetRange, targetAngle, followTime;
    public bool wasChasing;

    private bool isStopped;
    
    
    void Start() {
        agent = transform.GetComponent<NavMeshAgent>();
        patrolPoints = new List<Vector3>();
        previousRoom = null;
        player = GameObject.Find("Player").transform;
        state = EnemyState.Idle;
        SelectPatrolPoints();
        isStopped = false;
    }

    public void SetActualRoom(Room room) {
        actualRoom = room;
    }

    private void Update() {
        if (isStopped) {
            return;
        }
        ScanArea();
        Patrol();
    }

    void Patrol() {
        switch (state) {
            case EnemyState.Idle: {
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
                patrolPoints.Clear();
                agent.SetDestination(target.transform.position);
                break;
            }

        }
    }

    void ScanArea() {
        Vector3 pos = transform.position;
        Vector3 dir = player.position - pos;

        float angle = Vector3.Angle(transform.forward, dir);

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

        Debug.Log("Player");
        Ray ray = new Ray(pos, dir.normalized);
        RaycastHit hit;
        
        if (Physics.Raycast(ray, out hit, targetRange)) {
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
        int roomsLength = possiblerooms.Count;
        Room nextRoom;

        if (roomsLength == 0) {
            previousRoom = actualRoom;
            SelectPatrolPoints();
            return;
        }

        if (roomsLength == 1) {
            previousRoom = actualRoom;
            actualRoom = possiblerooms[0];
            SelectPatrolPoints();
            return;
        }
        
        nextRoom = possiblerooms[Random.Range(0, roomsLength)];
        possiblerooms.Remove(nextRoom);
        
        if (nextRoom == previousRoom) {
            nextRoom = possiblerooms[Random.Range(0, roomsLength)];
        }
        
        actualRoom = nextRoom;
        previousRoom = actualRoom;
        
        SelectPatrolPoints(); 
    }


    void SelectClosestRoomToPatrol() {
        List<Room> rooms = GameManager.rooms.Where(r => !r.locked).ToList();
        Room closest = rooms[0];

        foreach (var room in rooms) {
            if (Vector3.Distance(transform.position, room.area.center) <
                Vector3.Distance(transform.position, closest.area.position)) {
                closest = room;
            }
        }

        actualRoom = closest;
        SelectPatrolPoints();
    }
    
    

    void SelectPatrolPoints() {
        while (patrolPoints.Count <= patrolPointNumber) {
            Vector2 point = actualRoom.GetRandPositionInRoom();
            
            if (patrolPoints.Any(p => Vector3.Distance(p, new Vector3(point.x, 0, point.y)) < 2f)) {
                point = actualRoom.GetRandPositionInRoom();
            }

            patrolPoints.Add(new(point.x, 0, point.y));
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
    
    
    
}