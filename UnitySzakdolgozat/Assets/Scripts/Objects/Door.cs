using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door : MonoBehaviour
{
    private Vector3 startPos;
    private int duration = 5;

    public Room lockedRoom;

    public List<Task> tasks;

    private void Start() {
        startPos = transform.position;
    }

    public void Action() {
        foreach (var task in tasks) {
            if (!task.Activated) {
                return;
            }
        }

        lockedRoom.locked = false;
        
        StartCoroutine(RaiseDoor());
        
    }

    private IEnumerator RaiseDoor() {
        float elapsed = 0f;
        Vector3 targetPos= transform.position + Vector3.up * 3;
        while (elapsed < duration) {
            transform.position = Vector3.Lerp(startPos, targetPos, elapsed / duration);
            elapsed += Time.deltaTime;
            yield return null;
        }

        transform.position = targetPos;
        MapGeneration.BuildNavMesh();
    }
}
