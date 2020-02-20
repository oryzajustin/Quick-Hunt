using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class PlayerSpawner : MonoBehaviourPun
{
    [SerializeField] GameObject hunter_prefab;
    [SerializeField] GameObject bunny_prefab;
    [SerializeField] FollowCam follow_cam;

    [SerializeField] Transform spawn_point_player;
    [SerializeField] Transform[] spawn_point_bunny;

    private void Awake()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            GameObject player = PhotonNetwork.Instantiate(hunter_prefab.name, spawn_point_player.position, Quaternion.identity);
            //PhotonNetwork.LocalPlayer.TagObject = player;
            follow_cam.target = player.transform.Find("Look_Target");
        }
    }

    private void Start()
    {
        //if (PhotonNetwork.IsMasterClient)
        //{

        //    GameObject player = PhotonNetwork.Instantiate(hunter_prefab.name, spawn_point_player.position, Quaternion.identity);
        //    //PhotonNetwork.LocalPlayer.TagObject = player;
        //    follow_cam.target = player.transform.Find("Look_Target");
        //}
        //else
        //{
        if (!PhotonNetwork.IsMasterClient)
        {
            GameObject player = PhotonNetwork.Instantiate(bunny_prefab.name, spawn_point_bunny[Random.Range(0, spawn_point_bunny.Length)].position, Quaternion.identity);
            //PhotonNetwork.LocalPlayer.TagObject = player;
            follow_cam.target = player.transform.Find("Look_Target");
        }
        //}
    }
}
