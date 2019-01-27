using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum DirectionType { none, UpLeft, DownLeft, UpRight, DownRight }

public interface Interactable
{
    void Interact();
}

public class PlayerController : MonoBehaviour
{
    Dictionary<DirectionType, KeyCode> controls;
    Dictionary<DirectionType, Vector2> vectors;
    public float speed = 4;
    DirectionType dir;
    [HideInInspector]
    public Interactable closeInteractable; //Se cambia desde los triggers de los interactables
    Animator anim;

    private void Start()
    {
        anim = GetComponent<Animator>();
        closeInteractable = null;

        //Initialize controls
        controls = new Dictionary<DirectionType, KeyCode>();
        controls[DirectionType.UpLeft] = KeyCode.W;
        controls[DirectionType.DownLeft] = KeyCode.A;
        controls[DirectionType.UpRight] = KeyCode.D;
        controls[DirectionType.DownRight] = KeyCode.S;
        //Initialize vectors
        vectors = new Dictionary<DirectionType, Vector2>();
        vectors[DirectionType.UpLeft] = new Vector2(-1, .5f);
        vectors[DirectionType.DownLeft] = new Vector2(-1, -.5f);
        vectors[DirectionType.UpRight] = new Vector2(1, .5f);
        vectors[DirectionType.DownRight] = new Vector2(1, -.5f);
        //Initialize current direction
        dir = DirectionType.none;
    }

    void FixedUpdate()
    {
        Movement();
        Interaction();
    }

    public void Movement()
    {
        //Si no está pulsando el que está activo (en otro caso no se hace nada)
        if (dir != DirectionType.none && !Input.GetKey(controls[dir]))
        {
            bool tecla = false;
            for (int i = 1; i < System.Enum.GetValues(typeof(DirectionType)).Length && !tecla; ++i)
            {
                if (Input.GetKey(controls[(DirectionType)i]))
                {
                    //caso en el que cambia de direción (el único)
                    dir = (DirectionType)i;
                    switch ((DirectionType)i)
                    {
                        case DirectionType.UpLeft: anim.SetTrigger("UpLeft"); break;
                        case DirectionType.UpRight: anim.SetTrigger("UpRight"); break;
                        case DirectionType.DownLeft: anim.SetTrigger("DownLeft"); break;
                        case DirectionType.DownRight: anim.SetTrigger("DownRight"); break;
                    }                 
                    
                    tecla = true;
                }
            }
            if (!tecla)
            {
                anim.SetTrigger("Idle");
                dir = DirectionType.none;
            }
        }
        else if (dir == DirectionType.none)
        {
            bool tecla = false;
            for (int i = 1; i < System.Enum.GetValues(typeof(DirectionType)).Length && !tecla; ++i)
            {
                if (Input.GetKey(controls[(DirectionType)i]))
                {
                    //caso en el que cambia de direción (el único)
                    dir = (DirectionType)i;
                    switch ((DirectionType)i)
                    {
                        case DirectionType.UpLeft: anim.SetTrigger("UpLeft"); break;
                        case DirectionType.UpRight: anim.SetTrigger("UpRight"); break;
                        case DirectionType.DownLeft: anim.SetTrigger("DownLeft"); break;
                        case DirectionType.DownRight: anim.SetTrigger("DownRight"); break;
                    }

                    tecla = true;
                }
            }
        }

        if (dir != DirectionType.none)
        {
            ////comprobar la distancia hasta un collider con el linecast TODO
            //Vector3 start = new Vector3(transform.Find("Feet").transform.position.x, transform.Find("Feet").transform.position.y, 0);
            //Vector3 direction = new Vector3(vectors[dir].x, vectors[dir].y, 0);
            //Debug.DrawRay(start, direction);
            //int layerMask = LayerMask.NameToLayer("Default");
            //RaycastHit hit;

            ////MOVE
            /*if (!Physics.Raycast(start, direction, out hit, 10) || hit.distance > 0.01)*/
            transform.Translate(vectors[dir] * speed * Time.deltaTime);
            //Debug.Log(hit.distance);
        }
    }

    public void Interaction()
    {
        //Debug.Log("lo intento");
        //if (Input.GetKeyDown(KeyCode.E)) Debug.Log("lakj");
        //if (closeInteractable != null) Debug.Log("yep");
        if (Input.GetKeyDown(KeyCode.E) && closeInteractable != null) closeInteractable.Interact();
    }
}
