using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;

public class Enemy :MonoBehaviour
{
    public Room actualRoom;
    public GameObject target;

    private List<Vector3> patrolPoints;

    public float waitTime;
    private Coroutine waiting;
    private bool isMoving = false;
    private bool isWaiting;
    
    private NavMeshAgent agent;
    

    void Start() {
        agent = transform.GetComponent<NavMeshAgent>();
        patrolPoints = new List<Vector3>()
        {
            new (10, 0, 10),
            new (20, 0, 20),
            new (-10, 0, 10)
        };
        SelectPatrolPoints();
    }

    public void SetActualRoom(Room room) {
        actualRoom = room;
    }

    private void Update() {
        Patrol();

        if (Input.GetKeyDown(KeyCode.A)) {
            Debug.Log(waiting);
        }
    }

    void Patrol() {
        if (patrolPoints.Count > 0) {
            if (!isMoving) {
                isMoving = true;
                agent.SetDestination(patrolPoints.Last());
                
            }
            else {
                if (InStoppingRange() && !isWaiting) {
                    waiting = StartCoroutine(Waiting());
                }
            }
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
        return Vector2.Distance(transform.position, patrolPoints.Last()) < agent.stoppingDistance;
    }

    void StopWaiting() {
        Debug.Log("Stopped");
        StopCoroutine(waiting);
        waiting = null;
    }


    void SelectPatrolPoints() {
        int patrolPointNumber = actualRoom.area.width * actualRoom.area.height / 25;
        while (patrolPoints.Count <= patrolPointNumber) {
            Vector2 point = actualRoom.GetRandPositionInRoom();
            
            if (patrolPoints.Any(p => Vector2.Distance(p, point) < 2f)) 
                point = actualRoom.GetRandPositionInRoom();
            
            
            patrolPoints.Add(new(point.x, 0, point.y));
        }
    }

    
}