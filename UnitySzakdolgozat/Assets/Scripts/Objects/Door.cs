using System.Collections;
using UnityEngine;

public class Door : MonoBehaviour
{
    public int duration;
    public float timer;
    private Vector3 upPosition, downPosition;
    private Room room;

    private void Start() {
        downPosition = transform.position;
        upPosition = downPosition + Vector3.up * 3;
    }

    public void Action(bool doorOpening) {
        if (doorOpening) {
            StopCoroutine(nameof(CloseDoor));
            StartCoroutine(nameof(RaiseDoor));
        }
        else {
            StopCoroutine(nameof(RaiseDoor));
            StartCoroutine(nameof(CloseDoor));
        }
        
    }

    public void SetRoom(Room r) {
        room = r;
    }

    private IEnumerator RaiseDoor() {
        while (timer < duration) {
            timer += Time.deltaTime;
            
            transform.position = Vector3.LerpUnclamped(downPosition, upPosition, timer / duration);
            
            yield return null;
        }

        timer = duration;
        room.locked = false;
        
        MapGeneration.BuildNavMesh();
    }

    private IEnumerator CloseDoor() {
        while (timer > 0) {
            timer -= Time.deltaTime;
            
            transform.position = Vector3.LerpUnclamped(downPosition, upPosition, timer / duration);
            
            yield return null;
        }

        timer = 0;
        room.locked = true;
        
        MapGeneration.BuildNavMesh();
    }
}
