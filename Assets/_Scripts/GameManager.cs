using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


//Toilet - papel Y
//kitchen - comida en la mesa Y
//salon - tv Y
//studio - mobile Y
//brotherBedroom - Tortug Y
public enum Task { paper, hungry, tv, mobile, Tortug }

public class GameManager : MonoBehaviour
{
    [HideInInspector]
    public PlayerController player;
    [HideInInspector]
    public BoardManager bm;
    [HideInInspector]
    public MechanicsText UIText;
    [HideInInspector]
    public AudioSource music;
    public AudioClip Set_1;

    Dictionary<Task, RoomType> roomTask;
    Dictionary<Task, string> textTask;

    Task currentTask;
    public RoomType currentTargetRoom;

    private void Start()
    {
        player = FindObjectOfType<PlayerController>();
        bm = FindObjectOfType<BoardManager>();
        UIText = FindObjectOfType<MechanicsText>();
        //music = FindObjectOfType<AudioSource>();
        music = GetComponent<AudioSource>();
        InitializeTasks();
        NewRun();
    }

    public void InitializeTasks()
    {
        //Inicializar cada task a su habitación
        roomTask = new Dictionary<Task, RoomType>();
        roomTask[Task.paper] = RoomType.toilet;
        roomTask[Task.hungry] = RoomType.kitchen;
        roomTask[Task.tv] = RoomType.salon;
        roomTask[Task.mobile] = RoomType.studio;
        roomTask[Task.Tortug] = RoomType.brotherBedroom;
        //Inicializar cada task a su texto de ui
        textTask = new Dictionary<Task, string>();
        textTask[Task.paper] = "I need some toilet paper";
        textTask[Task.hungry] = "I’m hungry";
        textTask[Task.tv] = "I can’t stand the TV’s noise";
        textTask[Task.mobile] = "I forgot my cell phone at the studio";
        textTask[Task.Tortug] = "Tortug must be starving";
    }

    public void NewRun()
    {
        currentTask = (Task)Random.Range(0, System.Enum.GetValues(typeof(Task)).Length);
        currentTargetRoom = roomTask[currentTask];
        bm.InitMap(currentTargetRoom);

        //Show bedroom
        bm.ShowRoom(0, 0);
        UIText.gameObject.SetActive(true);
        UIText.EnableUI(true);
        UIText.SetText(textTask[currentTask]);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape)) SceneManager.LoadScene("Menu");
    }

    public void Win()
    {
        //subir la calidad de la bedroom
        NewRun();
    }

    public void StartMainMusic()
    {
        Debug.Log("sdf00");
        if (!music.isPlaying) music.PlayOneShot(Set_1, 0.9F);
    }
    public void StopMainMusic()
    {
        music.Stop();
    }

}
