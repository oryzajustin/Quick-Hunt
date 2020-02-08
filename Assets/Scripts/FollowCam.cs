using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowCam : MonoBehaviour
{
    [SerializeField] float mouse_sensitivity;
    [SerializeField] Transform target;
    [SerializeField] float distance_from_target;
    [SerializeField] Vector2 pitch_min_max;

    [SerializeField] float rotation_smooth_time;

    private Vector3 rotation_smooth_velocity;
    private Vector3 curr_rotation;

    private float yaw;
    private float pitch;

    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

    }

    private void LateUpdate()
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
