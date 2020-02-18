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

    [SerializeField] Transform spawn_point;

    private void Start()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            
            GameObject player = PhotonNetwork.Instantiate(hunter_prefab.name, spawn_point.position, Quaternion.identity);
            PhotonNetwork.LocalPlayer.TagObject = player;
            follow_cam.target = player.transform.Find("Look_Target");
        }
        else
        {
            GameObject player = PhotonNetwork.Instantiate(bunny_prefab.name, spawn_point.position, Quaternion.identity);
            PhotonNetwork.LocalPlayer.TagObject = player;
            follow_cam.target = player.transform.Find("Look_Target");
        }
    }
}
