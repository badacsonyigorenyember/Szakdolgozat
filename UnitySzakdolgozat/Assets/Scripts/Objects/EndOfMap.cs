using System.Collections;
using UnityEngine;

public class EndOfMap : MonoBehaviour, IInteractableObject
{
    public float duration;
    private bool Activated;
    public bool IsStationary { get; } = true;

    
    public void Action() {
        if (!Activated) {
            StartCoroutine(EndCoroutine());
        }
    }

    private IEnumerator EndCoroutine() {
        float timer = 0;
        while (timer < duration) {
            timer += Time.deltaTime;
            yield return null;
        }
        GameManager.NextLevel();
    }
}
