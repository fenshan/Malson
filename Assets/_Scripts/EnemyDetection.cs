using UnityEngine;
using System.Collections;

//[RequireComponent(typeof(AudioSource))]
public class EnemyDetection : MonoBehaviour
{
    public AudioClip Child_1;
    public AudioClip Mom_1;
    public AudioClip Father_1;
    public AudioSource audioX;
   
    public AudioDistortionFilter distortionFilter;

    //[Range(0f, 1f)]
   // public float parameter;

    void Start()
    {
        audioX = GetComponent<AudioSource>();
    }

    void OnTriggerEnter2D(Collider2D col)
    {
        //Si entra en el area, activa los efectos
        if (col.gameObject.CompareTag("Mom_Rad"))
        { 
           audioX.PlayOneShot(Mom_1, 0.9F);
         //distortionFilter.distortionLevel = parameter;

        }
        if (col.gameObject.CompareTag("Bro_Rad"))
        {
           audioX.PlayOneShot(Child_1, 0.9F);
           // distortionFilter.distortionLevel = parameter;
            //Brotha.Play();
        }
        if (col.gameObject.CompareTag("Dad_Rad"))
        {
            audioX.PlayOneShot(Father_1, 0.9F);
          //  distortionFilter.distortionLevel = parameter;
        }
    }
    void OnTriggerExit2D(Collider2D col)
    {
        //Debug.Log("Trig");
        if (col.gameObject.CompareTag("Mom_Rad"))
        {
            audioX.Stop();
            audioX.PlayOneShot(Mom_1, 0.1F);
           // Play();
        }
        if (col.gameObject.CompareTag("Bro_Rad"))
        {
            audioX.PlayOneShot(Child_1, 0.1F);
            audioX.Stop();

        }
        if (col.gameObject.CompareTag("Dad_Rad"))
        {
            audioX.PlayOneShot(Father_1, 0.1F);
            audioX.Stop();

        }

    }

}


