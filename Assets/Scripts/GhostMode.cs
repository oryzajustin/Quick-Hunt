using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GhostMode : MonoBehaviour
{
    public float camera_sensitivity = 5f;
    public float climb_speed = 4f;
    public float normal_move_speed = 10f;
    public float slow_move_factor = 0.25f;
    public float fast_move_factor = 3f;

    private float rotation_x, rotation_y = 0f;

    public Camera cam;

    private void Start()
    {
        cam = Camera.main;
    }

    private void Update()
    {
        GhostMovement();
    }

    private void GhostMovement()
    {
        rotation_x += Input.GetAxisRaw("Mouse X") * camera_sensitivity * Time.deltaTime;
        rotation_y += Input.GetAxisRaw("Mouse Y") * camera_sensitivity * Time.deltaTime;
        rotation_y = Mathf.Clamp(rotation_y, -90, 90);

        transform.localRotation = Quaternion.AngleAxis(rotation_x, Vector3.up);
        transform.localRotation *= Quaternion.AngleAxis(rotation_y, Vector3.left);

        if (Input.GetKey(KeyCode.LeftShift))
        {
            transform.position += cam.transform.forward * (normal_move_speed * fast_move_factor) * Input.GetAxis("Vertical") * Time.deltaTime;
            transform.position += cam.transform.right * (normal_move_speed * fast_move_factor) * Input.GetAxis("Horizontal") * Time.deltaTime;
        }
        else if (Input.GetKey(KeyCode.LeftControl))
        {
            transform.position += cam.transform.forward * (normal_move_speed * slow_move_factor) * Input.GetAxis("Vertical") * Time.deltaTime;
            transform.position += cam.transform.right * (normal_move_speed * slow_move_factor) * Input.GetAxis("Horizontal") * Time.deltaTime;
        }
        else
        {
            transform.position += cam.transform.forward * normal_move_speed * Input.GetAxis("Vertical") * Time.deltaTime;
            transform.position += cam.transform.right * normal_move_speed * Input.GetAxis("Horizontal") * Time.deltaTime;
        }

        if (Input.GetKey(KeyCode.Q))
        {
            transform.position += cam.transform.up * climb_speed * Time.deltaTime;
        }
        if (Input.GetKey(KeyCode.E))
        {
            transform.position -= cam.transform.up * climb_speed * Time.deltaTime;
        }
    }
}
