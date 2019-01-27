using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct InstantiatedRoomInfo
{
    public RoomType type;
    public bool visited;
}

public class BoardManager : MonoBehaviour
{
    [Header("Set in interface")]
    public Room[] prefabsArray;
    public Bedroom bedroomPrefab;
    [Range(7, 16)]
    public int preferredNumberOfRooms;

    //Set in code
    public static BoardManager instance = null;
    GameManager gm;
    Dictionary<RoomType, Room> prefabs;
    BedroomOrganizationLevel bedroomInfo;
    static List<List<System.Tuple<int, int>>> map;
    Dictionary<int, InstantiatedRoomInfo> mapRooms; //el mapRooms[0] ES EL BEDROOM (aunque habrá algo random en esa posición) TODO acordarse

    int maxCorridors;
    int corridorsCount;

    //Current position in-game
    int currentRoomIndex;
    GameObject currentRoomGameobject;

    public void Awake()
    {
        if (instance == null) instance = this;
        else if (instance != this) Destroy(gameObject);

        //Initialize dictionary of prefabs
        InitializePrefabs();
    }

    public void Start()
    {
        gm = FindObjectOfType<GameManager>();
    }

    //Se llama desde fuera (Scripts door)
    public void ChangeRoom(int doorTaken)
    {
        //for (int i = 0; i < map.Count; ++i)
        //{
        //    Debug.Log(mapRooms[i].type.ToString());
        //    for (int j = 0; j < map[i].Count; ++j)
        //    {
        //        Debug.Log(i + " " + j + " " + map[i][j]);
        //    }
        //}

        //Debug.Log(currentRoomIndex + " " + doorTaken + " " + map[currentRoomIndex][doorTaken]);
        int x = map[currentRoomIndex][doorTaken].Item1;
        int y = map[currentRoomIndex][doorTaken].Item2;
        //Debug.Log(x + " " + y);
        ShowRoom(x, y);
    }

    public void ShowRoom(int x, int y)
    {
        if (gm == null) gm = FindObjectOfType<GameManager>();
        Debug.Log(mapRooms[x].type.ToString());
        if (x == 0) gm.StopMainMusic();
        else if (x == 1) gm.StartMainMusic();

        //Debug.Log(x + " " + y);

        //x es el número de la habitación en mapRooms
        //y es la puerta delante de la que tiene que aparecer

        currentRoomIndex = x;

        //DESTROY OLD ROOM
        Destroy(currentRoomGameobject);

        //SI NO ES EL BEDROOM
        Vector3 delta;
        if (x != 0)
        {
            //SHOW ROOM 
            currentRoomGameobject = Instantiate(prefabs[mapRooms[x].type].gameObject);
            currentRoomGameobject.transform.parent = this.transform;
            //use prefabs[mapRooms[x].visited TODO

            //poner al player en el sitio adecuado 
            currentRoomGameobject.GetComponent<Room>().InitializeArrayDoors();
            delta = gm.player.transform.position;
            gm.player.transform.position = currentRoomGameobject.GetComponent<Room>().arrayDoors[y].gameObject.transform.GetChild(0).position;
            delta -= gm.player.transform.position;

        }
        else
        {
            currentRoomGameobject = Instantiate(bedroomPrefab.gameObject);
            currentRoomGameobject.transform.parent = this.transform;

            //poner al player en el sitio adecuado 
            currentRoomGameobject.GetComponent<Bedroom>().InitializeDoor();
            delta = gm.player.transform.position;
            gm.player.transform.position = currentRoomGameobject.GetComponent<Bedroom>().door.gameObject.transform.GetChild(0).position;
            delta -= gm.player.transform.position;

            //que afecte la bedroomInfo TODO
        }
        //que la camera no se reposicione con lerp
        CinemachineCore.Instance.GetVirtualCamera(0).OnTargetObjectWarped(gm.player.transform, delta);

    }

    #region Initialization
    void InitializePrefabs()
    {
        prefabs = new Dictionary<RoomType, Room>();
        foreach (Room r in prefabsArray)
        {
            prefabs[r.type] = r;
        }
    }

    //Llamada desde InitMap
    void AddMapRoom(int i, int j, RoomType t)
    {
        if ((int)t == (int)RoomType.corridor) ++corridorsCount;
        //Map
        map.Add(new List<System.Tuple<int, int>>(System.Linq.Enumerable.Repeat(new System.Tuple<int, int>(-1, -1), prefabs[t].numberOfDoors))); //La lista con el número de puertas y todas inicializadas a -1
        int x = map.Count - 1;
        int y = Random.Range(0, map[x].Count);
        map[i][j] = new System.Tuple<int, int>(x, y);
        //Debug.Log(new System.Tuple<int, int>(x, y));
        map[x][y] = new System.Tuple<int, int>(i, j);

        //MapRoom
        mapRooms[x] = new InstantiatedRoomInfo { type = t, visited = false };

    }

    void AssignMapRoom(int i, int j, int x, int y)
    {
        //Map
        map[i][j] = new System.Tuple<int, int>(x, y);
        map[x][y] = new System.Tuple<int, int>(i, j);

    }

    public void InitMap(RoomType targetType)
    {
        map = new List<List<System.Tuple<int, int>>>();
        mapRooms = new Dictionary<int, InstantiatedRoomInfo>();
        corridorsCount = 0;
        maxCorridors = 3; //TODO comprobar que no haya más corridors de la cuenta

        //initialize room 0 (bedroom)
        map.Add(new List<System.Tuple<int, int>>(System.Linq.Enumerable.Repeat(new System.Tuple<int, int>(-1, -1), 1 /*number of rooms del bedroom*/)));
        mapRooms[map.Count - 1] = new InstantiatedRoomInfo();//TODO a esto no sé qué valores ponerle porque es el bedroom 

        //initialize room 1 (corridor)
        AddMapRoom(0, 0, RoomType.corridor);

        //inicializar cada una de las habitaciones a las que da el pasillo a cosas que no sean el target
        for (int a = 0; a < prefabs[RoomType.corridor].numberOfDoors; ++a)
        {
            if (map[1][a].Item1 == -1)
            {
                RoomType t;
                do { t = Room.GetRandomType(targetType); } while ((int)t == (int)targetType || (corridorsCount >= maxCorridors && (int)t == (int)RoomType.corridor));
                AddMapRoom(1, a, t);
            }
        }

        bool targetRoomPut = false;

        //Recorrer el mapa entero
        for (int i = 2; i < map.Count; ++i)
        {
            int max = map[i].Count; //si no, no se asigna distinto en cada ronda
            for (int j = 0; j < max; ++j)
            {
                if (map[i][j].Item1 == -1)
                {
                    //el mínimo de rooms tal y como está diseñado, va a ser siempre 7
                    //New room
                    if (map.Count < preferredNumberOfRooms)
                    {
                        //TYPE
                        RoomType t;
                        if (targetRoomPut)
                        {
                            do { t = Room.GetRandomType(targetType); } while ((int)t == (int)targetType || (corridorsCount >= maxCorridors && (int)t == (int)RoomType.corridor));
                        }
                        else
                        {
                            do { t = Room.GetRandomType(); } while (corridorsCount >= maxCorridors && (int)t == (int)RoomType.corridor);
                            if ((int)t == (int)targetType) targetRoomPut = true;
                        }
                        //ADD ROOM OF THAT TYPE
                        AddMapRoom(i, j, t);
                    }
                    //Assing already existing room
                    else
                    {
                        //si no está puesta la room target, se pone ya
                        //New room
                        if (!targetRoomPut)
                        {
                            AddMapRoom(i, j, targetType);
                            targetRoomPut = true;
                        }
                        //intentar asignar todo a habitaciones ya creadas
                        else
                        {
                            bool satisfactory = false;
                            int x, y;

                            //hacer varios intentos random
                            int numberOfRandomTries = 3;
                            do
                            {
                                --numberOfRandomTries;
                                x = Random.Range(i, map.Count); //PUEDE TOCAR LA PROPIA HABITACIÓN (decisión de diseño)
                                for (y = 0; y < map[x].Count && !satisfactory; ++y)
                                {
                                    if (map[x][y].Item1 == -1)
                                    {
                                        AssignMapRoom(i, j, x, y);
                                        satisfactory = true;
                                    }
                                }
                            } while (!satisfactory || numberOfRandomTries == 0);

                            //si no sale, ir en orden
                            if (!satisfactory)
                            {
                                for (x = i + 1; x < map.Count && !satisfactory; ++x) //solo probar desde la habitación siguiente a la que estamos probando
                                {
                                    int var = map[x].Count;
                                    for (y = 0; y < var && !satisfactory; ++y)
                                    {
                                        if (map[x][y].Item1 == -1)
                                        {
                                            AssignMapRoom(i, j, x, y);
                                            satisfactory = true;
                                        }
                                    }
                                }
                            }
                            //probar las puertas que quedan de la misma habitación (distintas de la propia puerta)
                            if (!satisfactory)
                            {
                                x = i;
                                for (y = 0; y < map[x].Count && !satisfactory; ++y)
                                {
                                    if (map[x][y].Item1 == -1 && (y != j))
                                    {
                                        AssignMapRoom(i, j, x, y);
                                        satisfactory = true;
                                    }
                                }
                            }
                            //asignar la propia puerta a sí misma
                            if (!satisfactory)
                            {
                                AssignMapRoom(i, j, i, j);
                                satisfactory = true;
                            }

                            //NOTAS: ahora pudiendo asignar una puerta a sí mismma, no tengo que hacer toda la parafernalia de crear más habitaciones en el caso de que las puertas sean impares
                            ////si sigue sin salir (no hay ninguna puerta sin nada asignado), crear nuevo elemento                            
                            //if (!satisfactory)
                            //{
                            //    //A. si el baño no es el target, crear un baño (que solo tiene una puerta)
                            //    if ((int)targetType == (int)RoomType.toilet)
                            //    {
                            //        AddMapRoom(i, j, RoomType.toilet); //PORQUE SOLO TIENE UNA PUERTA
                            //        satisfactory = true;
                            //    }

                            //    //B. else, crear uno de 3 puertas, y dos de 2 puertas (para que enlace todo bien)
                            //    else
                            //    {
                            //        //SABEMOS QUE EL SALON EL ESTUDIO Y EL BROTHERBEDROOM SE PUEDEN PONER PORQUE EL TARGET ES EL TOILET
                            //        AddMapRoom(i, j, RoomType.salon); //TIENE 3 PUERTAS
                            //        x = map.Count - 1;
                            //        do { y = Random.Range(0, 3); } while (map[x][y].Item1 != -1);
                            //        AddMapRoom(x, y, RoomType.studio); //TIENE 2 PUERTAS
                            //        do { y = Random.Range(0, 3); } while (map[x][y].Item1 != -1);
                            //        AddMapRoom(x, y, RoomType.brotherBedroom); //TIENE 2 PUERTAS

                            //        //ENLAZAR LAS PUERTAS RESTANTES DEL ESTUDIO Y EL BROTHERBEDROOM
                            //        x = map.Count - 2; //penúltimo objeto creado
                            //        do { y = Random.Range(0, 2); } while (map[x][y].Item1 != -1);
                            //        int a = map.Count - 1; //último objeto creado
                            //        int b;
                            //        do { b = Random.Range(0, 2); } while (map[a][b].Item1 != -1);

                            //        AssignMapRoom(x, y, a, b);
                            //        satisfactory = true;
                            //    }
                            //}

                        }

                    }
                }
            }
        }
        //Debug.Log("CUANTAS VECES");
        ////fin del bucle
        //for (int i = 0; i < map.Count; ++i)
        //{
        //    Debug.Log(mapRooms[i].type.ToString());
        //    for (int j = 0; j < map[i].Count; ++j)
        //    {
        //        Debug.Log(i + " " + j + " " + map[i][j]);
        //    }
        //}
    }
    #endregion
}
