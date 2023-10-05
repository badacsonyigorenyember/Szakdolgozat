using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;

public class Enemy :MonoBehaviour
{
    public Room actualRoom;
    public Room previousRoom;
    public GameObject target;

    private List<Vector3> patrolPoints;
    private Vector3 destination;

    public float waitTime;
    private Coroutine waiting;
    private bool isMoving;
    private bool isWaiting;
    
    private NavMeshAgent agent;

    public Transform player;
    public float sightRange, attackRange;
    public bool playerInRange;
    
    

    void Start() {
        agent = transform.GetComponent<NavMeshAgent>();
        patrolPoints = new List<Vector3>();
        SelectPatrolPoints();
        previousRoom = null;
        player = GameObject.Find("Player").transform;
    }

    public void SetActualRoom(Room room) {
        actualRoom = room;
    }

    private void Update() {
        Patrol();
    }

    void Patrol() {
        if (patrolPoints.Count > 0) {
            if (!isMoving) {
                Move(patrolPoints.Last());
            }
            else {
                if (InStoppingRange() && !isWaiting) {
                    waiting = StartCoroutine(Waiting());
                }
            }
        }
        else {
            MoveToAnotherRoom();
        }
        
    }
    
    IEnumerator Waiting() {
        isWaiting = true;
        yield return new WaitForSeconds(waitTime);
        isWaiting = false;
        StopWaiting();
        isMoving = false;
        patrolPoints.RemoveAt(patrolPoints.Count - 1);
    }

    bool InStoppingRange() {
        return Vector3.Distance(transform.position, destination) < agent.stoppingDistance;
    }

    void StopWaiting() { 
        StopCoroutine(waiting);
        waiting = null;
    }


    void SelectPatrolPoints() {
        int patrolPointNumber = actualRoom.area.width * actualRoom.area.height / 25;
        while (patrolPoints.Count <= patrolPointNumber) {
            Vector2 point = actualRoom.GetRandPositionInRoom();
            
            if (patrolPoints.Any(p => Vector3.Distance(p, new Vector3(point.x, 0, point.y)) < 2f)) {
                point = actualRoom.GetRandPositionInRoom();
            }
            
            
            patrolPoints.Add(new(point.x, 0, point.y));
        }
    }

    void Move(Vector3 position) {
        agent.SetDestination(position);
        isMoving = true;
        destination = position;
    }
    
    void MoveToAnotherRoom() {
        Room newRoom = actualRoom.neighbours.FirstOrDefault(r => !r.locked && r != previousRoom);
        
        if (newRoom != null) {
            previousRoom = actualRoom;
            actualRoom = newRoom;
            Vector3 newRoomPosition = new Vector3(newRoom.area.center.x, 0, newRoom.area.center.y);
            Move(newRoomPosition);
            SelectPatrolPoints();
            patrolPoints.Add(newRoomPosition);
        }
        else {
            SelectPatrolPoints();
            previousRoom = null;
        }
        
    }

    void FollowTarget() {
        
    }
    
}