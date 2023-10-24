using UnityEngine;

public class EndOfMap : MonoBehaviour, InteractableObject
{
    public void Action() {
        GameManager.instance.NextLevel();
    }
    
    public bool IsStationary { get; } = true;
}
