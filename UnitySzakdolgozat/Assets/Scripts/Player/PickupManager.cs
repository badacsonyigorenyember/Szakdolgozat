using UnityEngine;

public class PickupManager : MonoBehaviour
{
    private static GameObject heldObj;
    private static Rigidbody heldRb;
    
    private const float pickupForce = 180f;

    private static Transform cam;

    private static Vector3 previousCamPosition;

    private void Start() {
        cam = transform.Find("FirstPersonCamera");
    }

    public static void Controll() {
        if (heldObj == null) {
            Ray r = new Ray(cam.position, cam.forward);
            if (Physics.Raycast(r, out RaycastHit hit, 2f, LayerMask.GetMask("Interactable"))) {
                if (hit.collider.gameObject.TryGetComponent(out InteractableObject interactableScript)) {
                    if (!interactableScript.IsStationary) {
                        PickupObj(hit.collider.gameObject);
                    }
                }
            }
        }
        else {
            DropObj();
        }
    }

    private void FixedUpdate() {
        if (heldObj != null) {
            Movement();
        }
    }

    public static void PickupObj(GameObject obj) {
        if (obj.TryGetComponent(out heldRb)) {
            heldRb.useGravity = false;
            heldRb.constraints = RigidbodyConstraints.FreezeRotation;
            heldRb.drag = 13;
            heldObj = obj;
        }
        
    }
    
    private static void DropObj() {
        heldRb.useGravity = true;
        heldRb.constraints = RigidbodyConstraints.None;
        heldRb.drag = 0;
        heldObj = null;

    }

    public void Movement() {
        Vector3 targetPosition = cam.position + cam.forward * 2;

        if (Vector3.Distance(heldObj.transform.position, targetPosition) > 0.1f) {
            Vector3 direction = targetPosition - heldObj.transform.position;
            heldRb.AddForce(direction * pickupForce);
        }
    }
}
