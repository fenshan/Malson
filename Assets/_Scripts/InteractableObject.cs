using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractableObject : MonoBehaviour, Interactable
{
    [Header("Set in interface")]
    public RoomType room;
    GameManager gm;

    public void Interact()
    {
        if (room == gm.currentTargetRoom)
        {
            //TODO GANA
            Debug.Log("Win");
            gm.Win();
        }
        //TODO else, que diga que no es lo que está buscando 
    }

    private void Start()
    {
        if (gm == null) InitializeGM();
    }

    private void Update()
    {
        if (gm == null) InitializeGM();
        if (GetComponent<Collider2D>().Distance(gm.player.GetComponent<Collider2D>()).distance < 1.5f)
        {
            gm.player.closeInteractable = this;
        }
        else if (gm.player.closeInteractable == this as Interactable) gm.player.closeInteractable = null;
    }

    public void InitializeGM()
    {
        gm = FindObjectOfType<GameManager>();
    }
}
