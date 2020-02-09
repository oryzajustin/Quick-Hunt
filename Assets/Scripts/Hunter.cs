using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hunter : MonoBehaviour
{
    [Header("Spear Settings")]
    [SerializeField] GameObject spear_go;
    private Spear spear;

    [SerializeField] float throw_power;
    
    [Space]
    [Header("Body Parts")]
    public Transform right_hand;

    [Space]
    public bool has_spear;
    private void Start()
    {
        spear = spear_go.GetComponent<Spear>();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            ReturnSpear(spear_go);
        }
    }

    public void ThrowSpear()
    {
        spear.spear_rb.isKinematic = false;
        spear.spear_rb.collisionDetectionMode = CollisionDetectionMode.Continuous;
        spear.spear_rb.transform.parent = null;
        spear.spear_rb.AddForce(spear_go.transform.forward * throw_power + transform.up * 2, ForceMode.Impulse);
        has_spear = false;
    }

    public void ReturnSpear(GameObject spear)
    {
        spear.transform.position = right_hand.position;
        spear.transform.rotation = right_hand.rotation;
        spear.transform.parent = right_hand;
        has_spear = true;
    }
}
