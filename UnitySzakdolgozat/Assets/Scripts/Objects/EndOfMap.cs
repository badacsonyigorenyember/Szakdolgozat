using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndOfMap : MonoBehaviour, InteractableObject
{
    public void Action() {
        Debug.Log("End");
        Application.Quit();
    }
}
