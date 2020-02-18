using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Billboard : MonoBehaviour
{
    private Transform main_cam_transform;
    // Start is called before the first frame update
    void Start()
    {
        main_cam_transform = Camera.main.transform;
    }

    private void LateUpdate()
    {
        transform.LookAt(transform.position + main_cam_transform.rotation * Vector3.forward, main_cam_transform.rotation * Vector3.up);
    }
}
