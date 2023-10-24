using UnityEngine;

public class PlayerInteraction : MonoBehaviour
{
    private Transform cam;
    private float interactRange;
    private bool menuActive;

    public void Initialize(float i) {
        interactRange = i;
        cam = transform.Find("FirstPersonCamera");
        menuActive = false;
    }
    
    public void InteractionHandling() {
        if (!menuActive) {
            if (Input.GetKeyDown(KeyCode.E)) {
                Debug.Log("E");
                Ray r = new Ray(cam.position, cam.forward);
                if (Physics.Raycast(r, out RaycastHit hit, interactRange, LayerMask.GetMask("Interactable"))) {
                    Debug.Log(hit.collider);
                    if (hit.collider.gameObject.TryGetComponent(out IMechanism interactableScript)) {
                        interactableScript.Action();
                    }
                }
            }
            
            if (Input.GetMouseButtonDown(0)) {
                PickupManager.Controll();
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
