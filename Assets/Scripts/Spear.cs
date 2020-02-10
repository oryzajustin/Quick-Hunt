using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class Spear : MonoBehaviourPun
{
    public GameObject spear_go;
    public Rigidbody spear_rb;
    public bool can_pick_up;

    public int durability;

    public int spear_damage;

    [SerializeField] Hunter hunter;
    [SerializeField] Transform return_position;
    [SerializeField] GameObject fake_bunny_prefab;

    void Start()
    {
        spear_go = this.gameObject;
        spear_rb = this.GetComponent<Rigidbody>();
        if(this.transform.parent != null)
        {
            can_pick_up = false;
            hunter.has_spear = true;
        }
        return_position = hunter.right_hand;
    }

    void Update()
    {
        if (can_pick_up)
        {
            spear_damage = 0;
        }
        else
        {
            spear_damage = 1;
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Terrain") // hitting any terrain
        {
            // stick the spear
            spear_rb.Sleep();
            spear_rb.collisionDetectionMode = CollisionDetectionMode.ContinuousSpeculative;
            spear_rb.isKinematic = true;
            can_pick_up = true;
        }
        else if (collision.gameObject.tag == "Bunny") 
        {
            Bunny bunny = collision.gameObject.GetComponent<Bunny>();
            int remaining_health = bunny.GetHealth();
            remaining_health -= spear_damage;
            if(remaining_health <= 0)
            {
                bunny.SetHealth(remaining_health);
                SkewerBunny(bunny);
                // make bunny ghost mode
                bunny.transform.GetChild(0).GetComponent<MeshRenderer>().enabled = false;
                bunny.GetComponent<CharacterController>().enabled = false;
                bunny.GetComponent<BunnyController>().enabled = false;
                bunny.GetComponent<GhostMode>().enabled = true;
                bunny.Die();
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.tag == "Player" && can_pick_up)
        {
            //hunter.ReturnSpear(this.gameObject);
            hunter.ReturnSpearWrapper();
            can_pick_up = false;
        }   
    }
    public void SkewerBunny(Bunny bunny)
    {
        // skewer the bunny
        spear_rb.Sleep(); // put the RB to sleep to stop weird physics
        
        
        
        //photonView.RPC("SkewerFake", RpcTarget.All); // giving problems
    }

    [PunRPC]
    public void SkewerFake()
    {
        Debug.Log("SKEWER");
        GameObject temp = Instantiate(fake_bunny_prefab, this.transform.position, Quaternion.identity, this.transform);
        Vector3 skewer_position = new Vector3(this.transform.position.x, this.transform.position.y - 0.15f, this.transform.position.z);
        temp.transform.position = skewer_position;
    }
}
