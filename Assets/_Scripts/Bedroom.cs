using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum BedroomOrganizationLevel { bad, neutral, good}

public class Bedroom : MonoBehaviour
{
    [HideInInspector]
    public Door door;

    public void Update()
    {
        if (door==null) InitializeDoor();
    }

    public void InitializeDoor()
    {
        door = GetComponentInChildren<Door>();
        door.indexOfDoor = 0;
    }
}
