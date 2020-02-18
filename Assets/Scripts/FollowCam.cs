using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using DG.Tweening;

public class FollowCam : MonoBehaviourPun
{
    [SerializeField] float mouse_sensitivity;
    public Transform target;
    [SerializeField] Hunter hunter;
    [SerializeField] float distance_from_target;
    public Vector2 pitch_min_max;

    [SerializeField] float rotation_smooth_time;

    private Vector3 rotation_smooth_velocity;
    private Vector3 curr_rotation;

    [SerializeField] Transform aim_pos;

    private float yaw;
    private float pitch;

    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        if(!PhotonNetwork.IsMasterClient)
        {
            pitch_min_max.x = 0;
            pitch_min_max.y = 80;
        }
        else
        {
            aim_pos = target.parent.Find("Cam Aim Pos");
            hunter = target.parent.GetComponent<Hunter>();
        }
    }

    private void LateUpdate()
    {
        MoveCamera();
    }

    private void MoveCamera()
    {
        if (Input.GetMouseButton(1) && PhotonNetwork.IsMasterClient && hunter.has_spear)
        {
            // aim mode
            yaw += Input.GetAxis("Mouse X") * mouse_sensitivity;
            pitch -= Input.GetAxis("Mouse Y") * mouse_sensitivity;
            pitch = Mathf.Clamp(pitch, -5, 35);

            curr_rotation = Vector3.SmoothDamp(curr_rotation, new Vector3(pitch, yaw), ref rotation_smooth_velocity, rotation_smooth_time);

            Vector3 target_rotation = new Vector3(pitch, yaw);
            this.transform.eulerAngles = target_rotation;

            this.transform.position = aim_pos.position;
        }
        else
        {
            yaw += Input.GetAxis("Mouse X") * mouse_sensitivity;
            pitch -= Input.GetAxis("Mouse Y") * mouse_sensitivity;
            pitch = Mathf.Clamp(pitch, pitch_min_max.x, pitch_min_max.y);

            curr_rotation = Vector3.SmoothDamp(curr_rotation, new Vector3(pitch, yaw), ref rotation_smooth_velocity, rotation_smooth_time);

            Vector3 target_rotation = new Vector3(pitch, yaw);
            this.transform.eulerAngles = target_rotation;

            this.transform.position = target.position - transform.forward * distance_from_target;
        }
        
    }
}
