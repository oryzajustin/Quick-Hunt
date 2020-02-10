using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class PlayerSpawner : MonoBehaviour
{
    [SerializeField] GameObject hunter_prefab;
    [SerializeField] GameObject bunny_prefab;
    [SerializeField] FollowCam follow_cam;
    private void Start()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            GameObject player = PhotonNetwork.Instantiate(hunter_prefab.name, Vector3.zero, Quaternion.identity);
            follow_cam.target = player.transform.Find("Look_Target");
        }
        else
        {
            GameObject player = PhotonNetwork.Instantiate(bunny_prefab.name, Vector3.zero, Quaternion.identity);
            follow_cam.target = player.transform.Find("Look_Target");
        }
    }
}
