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

    private Material spear_material;
    private const string FADE_NAME = "_Fade_Amount";

    [SerializeField] Hunter hunter;
    [SerializeField] Transform return_position;
    [SerializeField] GameObject fake_bunny_prefab;

    [SerializeField] float timer = 5.0f;

    void Start()
    {
        spear_go = this.gameObject;
        spear_rb = this.GetComponent<Rigidbody>();
        if(this.transform.parent != null)
        {
            can_pick_up = false;
            hunter.has_spear = true;
            this.GetComponent<PhotonRigidbodyView>().enabled = false;
        }
        return_position = hunter.right_hand;
        spear_material = this.GetComponent<Renderer>().material;
    }

    void Update()
    {
        if (can_pick_up)
        {
            spear_damage = 0;
            //timer -= Time.deltaTime;
            //if (timer <= 0)
            //{
            //    Destroy(this.gameObject);
            //}
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
                bunny.spear_go = this.gameObject;
                bunny.SkewerBunnyWrapper();
                // make bunny ghost mode
                bunny.MakeGhostWrapper();
                bunny.Die();
            }
        }
        if (PhotonNetwork.IsMasterClient)
        {
            photonView.RPC("FadeOut", RpcTarget.All);
        }
        
    }

    [PunRPC]
    public void FadeOut()
    {
        StartCoroutine(Wait());
    }

    private IEnumerator Wait()
    {
        float duration = 5f; // 5 seconds
        float totalTime = 0;
        while (totalTime <= duration)
        {
            totalTime += Time.deltaTime;
            yield return null;
        }
        StartCoroutine(SpearFadeOut());
    }

    private IEnumerator SpearFadeOut()
    {
        float conjure_transition;
        float duration = 1f; // 1 second fade
        float totalTime = 0;
        while (totalTime <= duration)
        {
            totalTime += Time.deltaTime;
            conjure_transition = totalTime;
            conjure_transition = Mathf.Clamp(conjure_transition, 0, 1);
            spear_material.SetFloat(FADE_NAME, conjure_transition);
            yield return null;
        }
        Destroy(this.gameObject);
    }
}
