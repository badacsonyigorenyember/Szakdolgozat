using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door : MonoBehaviour
{
    private Vector3 startPos;
    private int duration = 5;
    private Vector3 upPosition, downPosition, currentPosition;
    
    private void Start() {
        startPos = transform.position;
    }

    public void Action() {
        StartCoroutine(RaiseDoor());
    }

    private void Update() {
        
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
