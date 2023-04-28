using UnityEngine;

[RequireComponent(typeof(Canvas))]
public class CanvasDepthTest : MonoBehaviour
{
    private Canvas canvas;
    public Camera cam;

    private void Start()
    {
        canvas = GetComponent<Canvas>();
        canvas.worldCamera = Camera.main;
        canvas.planeDistance = 1f;
    }
}