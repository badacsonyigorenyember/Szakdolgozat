using System.Collections;
using UnityEngine;

public class Door : MonoBehaviour
{
    public int duration;
    public float timer;
    private Vector3 upPosition, downPosition;

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

    

    private IEnumerator RaiseDoor() {
        while (timer < duration) {
            timer += Time.deltaTime;
            
            transform.position = Vector3.LerpUnclamped(downPosition, upPosition, timer / duration);
            
            yield return null;
        }

        timer = duration;

        MapGeneration.BuildNavMesh();
    }

    private IEnumerator CloseDoor() {
        while (timer > 0) {
            timer -= Time.deltaTime;
            
            transform.position = Vector3.LerpUnclamped(downPosition, upPosition, timer / duration);
            
            yield return null;
        }

        timer = 0;
        
        MapGeneration.BuildNavMesh();
    }
}
