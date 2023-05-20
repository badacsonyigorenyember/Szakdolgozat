using UnityEngine;

public class EndOfMap : MonoBehaviour, InteractableObject
{
    public bool IsStationary { get; set; } = true;

    public void Action() {
        Debug.Log("End");
        GameManager.instance.NextLevel();
    }

}
