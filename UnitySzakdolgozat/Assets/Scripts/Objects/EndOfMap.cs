using UnityEngine;

public class EndOfMap : MonoBehaviour, InteractableObject
{
    public void Action() {
        Debug.Log("End");
        GameManager.instance.NextLevel();
    }
}
