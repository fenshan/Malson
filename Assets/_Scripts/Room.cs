using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public enum RoomType { kitchen, salon, toilet, corridor, studio, brotherBedroom }

public class Room : MonoBehaviour
{
    [Header("Set in inspector")]
    public RoomType type;
    //Los valores de 1 a 4
    public int numberOfDoors; //TODO esto quizás debería ser una lista con las posiciones de los triggers
    [HideInInspector]
    public Door[] arrayDoors;

    public void Start()
    {
        InitializeArrayDoors();        
    }

    public void InitializeArrayDoors()
    {
        arrayDoors = GetComponentsInChildren<Door>();
        for (int i = 0; i < arrayDoors.Length; ++i)
        {
            arrayDoors[i].indexOfDoor = i;
        }
    }

    public static RoomType GetRandomType(RoomType notThis)
    {
        RoomType r;
        do
        {
            r = (RoomType)UnityEngine.Random.Range(0, Enum.GetValues(typeof(RoomType)).Length);
        }
        while ((int)r == (int)notThis);
        return r;
    }

    public static RoomType GetRandomType()
    {
        return (RoomType)UnityEngine.Random.Range(0, Enum.GetValues(typeof(RoomType)).Length);
    }


}
