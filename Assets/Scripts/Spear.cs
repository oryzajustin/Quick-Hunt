using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spear : MonoBehaviour
{
    public GameObject spear_go;
    public Rigidbody spear_rb;
    public bool can_pick_up;

    [SerializeField] Hunter hunter;
    [SerializeField] Transform return_position;

    void Start()
    {
        spear_rb = this.GetComponent<Rigidbody>();
        if(this.transform.parent != null)
        {
            can_pick_up = false;
            hunter.has_spear = true;
        }
        return_position = hunter.right_hand;
    }

    void FixedUpdate()
    {
        
    }

    private void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.tag != "Player")
        {
            spear_rb.Sleep();
            spear_rb.collisionDetectionMode = CollisionDetectionMode.ContinuousSpeculative;
            spear_rb.isKinematic = true;
            can_pick_up = true;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.tag == "Player" && can_pick_up)
        {
            hunter.ReturnSpear(this.gameObject);
            can_pick_up = false;
        }   
    }
}
