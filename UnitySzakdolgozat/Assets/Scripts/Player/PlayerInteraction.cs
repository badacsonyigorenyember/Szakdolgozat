using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInteraction : MonoBehaviour
{
    public Transform player;
    public float interactRange;

    void Update() {
        Interact();
    }

    void Interact() {
        if (Input.GetKeyDown(KeyCode.E)) {
            Ray r = new Ray(player.position, player.forward);
            if (Physics.Raycast(r, out RaycastHit hit, interactRange)) {
                if(hit.collider.gameObject.TryGetComponent(out InteractableObject interactableObj))
                    interactableObj.Action();
            }
        }
    }
}
