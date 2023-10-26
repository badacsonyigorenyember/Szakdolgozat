using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

public class RoamerEnemy : MovingEnemy
{
    public Room PreviousRoom;

    public List<Vector3> PatrolPoints;

    public float WaitTime;
    private float Timer;
    
    public bool playerInRange;

    public int MaxPatrolPointNumber;

    public float TargetRange, TargetAngle, FollowTime;
    public bool WasChasing;
    
    
    protected override void Start() {
        base.Start();
        Init();
        Type = EnemyType.Roamer;
        ChasingState = EnemyState.Chasing;
    }

    protected override void Init() {
        base.Init();
        PatrolPoints = new List<Vector3>();
        PreviousRoom = null;
        Target = null;
        SelectPatrolPoints();
    }

    protected override void Update() {
        base.Update();
        
        ScanArea();
        Patrol();
    }

    void Patrol() {
        switch (State) {
            case EnemyState.Idle: {
                Agent.speed = 2f;
                if (PatrolPoints.Count > 0) {
                    Move(PatrolPoints[0]);
                    PatrolPoints.RemoveAt(0);
                }
                else {
                    if (WasChasing) {
                        SelectClosestRoomToPatrol();
                        WasChasing = false;
                    }
                    SelectNextRoom();
                }
                break;
            }

            case EnemyState.Waiting: {
                if (Timer >= WaitTime) {
                    State = EnemyState.Idle;
                    Timer = 0;
                }
                else {
                    Timer += Time.deltaTime;
                }
                break;
            }

            case EnemyState.Moving: {
                if (Agent.remainingDistance < 1f) {
                    State = EnemyState.Waiting;
                }
                
                break;
            }

            case EnemyState.Chasing: {
                Agent.speed = 3.5f;
                PatrolPoints.Clear();
                Agent.SetDestination(Target.transform.position);
                break;
            }

        }
    }

    void ScanArea() {
        if (Player == null) {
            Player = GameObject.Find("Player").transform;
        }
        Transform t = transform;
        Vector3 pos = t.position;
        Vector3 dir = Player.position - pos;

        float angle = Vector3.Angle(t.forward, dir);

        if (angle > 45f || dir.sqrMagnitude > TargetRange) {
            if (State == EnemyState.Chasing) {
                if (Timer < FollowTime) {
                    Timer += Time.deltaTime;
                }
                else {
                    Timer = 0;
                    Move(Target.transform.position);
                    Target = null;
                    WasChasing = true;
                }
            }
            return;
        }

        Ray ray = new Ray(pos, dir.normalized);
        
        if (Physics.Raycast(ray, out RaycastHit hit, TargetRange)) {
            if (hit.transform.gameObject.layer != LayerMask.NameToLayer("Player")) {
                return;
            }
        }

        if (Player.GetComponent<PlayerController>().movement.isMoving) {
        
            State = EnemyState.Chasing;
            Target = hit.transform.gameObject;
        }
    }

    void SelectNextRoom() {
        List<Room> possiblerooms = ActualRoom.neighbours.Where(r => !r.locked).ToList();

        if (possiblerooms.Count == 0) {
            SelectPatrolPoints();
            return;
        }

        if (possiblerooms.Count == 1) {
            ActualRoom = possiblerooms[0];
            SelectPatrolPoints();
            return;
        }

        possiblerooms.Remove(PreviousRoom);

        PreviousRoom = ActualRoom;
        ActualRoom = possiblerooms[Random.Range(0, possiblerooms.Count)];
        
        possiblerooms.Remove(ActualRoom);
        
        
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

        ActualRoom = closest;
        SelectPatrolPoints();
    }
    
    void SelectPatrolPoints() {
        int patrolPointNumber = Math.Min(MaxPatrolPointNumber, ActualRoom.area.width * ActualRoom.area.height / 30);
        while (PatrolPoints.Count <= patrolPointNumber) {
            Vector2 point = ActualRoom.GetRandPositionInRoom(false);
            
            if (PatrolPoints.Any(p => Vector3.Distance(p, new Vector3(point.x, 0, point.y)) < 2f)) {
                point = ActualRoom.GetRandPositionInRoom(false);
            }

            PatrolPoints.Add(new(point.x, 0, point.y));
        }
    }
}
