using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door : MonoBehaviour
{
    public int duration = 5;
    public float speed;
    public float timer;
    private Vector3 upPosition, downPosition, currentPosition, savedPosition;

    private void Start() {
        downPosition = transform.position;
        upPosition = downPosition + Vector3.up * 3;
        currentPosition = downPosition;
    }

    public void Action(bool doorOpening) {
        if (doorOpening) {
            StopCoroutine("CloseDoor");
            StartCoroutine("RaiseDoor");
        }
        else {
            StopCoroutine("RaiseDoor");
            StartCoroutine("CloseDoor");
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
