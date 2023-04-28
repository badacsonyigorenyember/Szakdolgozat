using System;
using UnityEngine;

public class PlayerInteraction : MonoBehaviour
{
    public static PlayerInteraction instance;

    private GameObject menu;
    
    private Transform cam;
    private float interactRange;

    private bool menuActive;

    private void Awake() {
        instance = this;
    }

    private void Start() {
        cam = transform.Find("FirstPersonCamera");
        menuActive = false;
    }

    public void SetProperties(float i) {
        interactRange = i;
    }
    
    void Update() {
        if (Input.GetKeyDown(KeyCode.E)) {
            Debug.Log("E");
            Ray r = new Ray(cam.position, cam.forward);
            if (Physics.Raycast(r, out RaycastHit hit, interactRange, LayerMask.GetMask("Interactable"))) {
                Debug.Log(hit.collider);
                if (hit.collider.gameObject.TryGetComponent(out InteractableObject interactableObj)) {
                    interactableObj.Action();
                }
            }
        }

        if (Input.GetKeyDown(KeyCode.Escape)) {
            if (menuActive) {
                GameManager.instance.Resume();
                menuActive = false;
            }
            else {
                GameManager.instance.Pause();
                menuActive = true;
            }
                
        }
    }
}
