using System.Collections.Generic;
using UnityEngine;

public class Lever : MonoBehaviour, InteractableObject
{
    public List<GameObject> doors;

    public void Action() {
        foreach (var door in doors) {
            door.TryGetComponent(out Door doorComponent);
            doorComponent.Action();
        }
    }
}
