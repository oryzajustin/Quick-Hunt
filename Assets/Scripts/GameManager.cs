using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using ExitGames.Client.Photon;

public class GameManager : MonoBehaviourPun, IPunObservable
{
    private static GameManager _instance;

    public static GameManager instance
    {
        get
        {
            return _instance;
        }
    }

    public Dictionary<string, GameObject> animal_go_dict;

    public List<Animal> animals;

    private List<Animal> animals_to_add;

    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            _instance = this;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        PhotonNetwork.SendRate = 20;
        PhotonNetwork.SerializationRate = 10;

        animal_go_dict = CreateAnimalGODictionary();

        animals = new List<Animal>();
        animals_to_add = new List<Animal>();
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void AddAnimalWrapper(Animal animal)
    {
        animals_to_add.Add(animal);
        photonView.RPC("AddAnimal", RpcTarget.AllBufferedViaServer);
    }

    [PunRPC]
    public void AddAnimal()
    {
        animals = animals_to_add;
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
        }
        else if (stream.IsReading)
        {
        }
    }

    // Convert an object to a byte array
    public static byte[] ObjectToByteArray(object obj)
    {
        BinaryFormatter bf = new BinaryFormatter();
        using (var ms = new MemoryStream())
        {
            bf.Serialize(ms, obj);
            return ms.ToArray();
        }
    }

    // Convert a byte array to an Object
    public static object ByteArrayToObject(byte[] arrBytes)
    {
        using (var memStream = new MemoryStream())
        {
            var binForm = new BinaryFormatter();
            memStream.Write(arrBytes, 0, arrBytes.Length);
            memStream.Seek(0, SeekOrigin.Begin);
            var obj = binForm.Deserialize(memStream);;
            return obj;
        }
    }

    public Dictionary<string, GameObject> CreateAnimalGODictionary()
    {
        Dictionary<string, GameObject> dict = new Dictionary<string, GameObject>();
        return dict;
    }
}
