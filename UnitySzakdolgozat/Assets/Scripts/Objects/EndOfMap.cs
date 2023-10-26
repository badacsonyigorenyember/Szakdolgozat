using UnityEngine;

public class EndOfMap : MonoBehaviour
{
    public float duration;

    private void OnTriggerEnter(Collider other) {
        if (other.CompareTag("Player")) {
            GameManager.NextLevel();
        }
    }
    
}
