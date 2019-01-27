using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door : MonoBehaviour, Interactable
{
    GameManager gm;
    public int indexOfDoor; //Se settea desde la room

    public void Start()
    {
        //if (gm == null) InitializeGM();
    }

    public void Update()
    {
        if (gm == null) InitializeGM();
        if (Vector3.Distance(transform.GetChild(0).position, gm.player.transform.position)<1.5f)
        {
            gm.player.closeInteractable = this;
        }
        else if (gm.player.closeInteractable == this as Interactable) gm.player.closeInteractable = null;        
    }

    public void InitializeGM()
    {
        gm = FindObjectOfType<GameManager>();
    }

    public void Interact()
    {
        //if (gm == null) InitializeGM();
        gm.bm.ChangeRoom(indexOfDoor);
    }

    //private void OnTriggerStay2D(Collider2D collision)
    //{ //faltaba controlar que fuera el player el que entra
    //    if (gm == null) InitializeGM();
    //    gm.player.closeInteractable = this;
    //}

    //private void OnTriggerExit2D(Collider2D collision)
    //{
    //    if (gm == null) InitializeGM();
    //    if (gm.player.closeInteractable == this as Interactable) gm.player.closeInteractable = null;
    //}



}
