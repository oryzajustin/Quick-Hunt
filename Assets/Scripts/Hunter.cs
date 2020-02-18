using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.Rendering.PostProcessing;
using DG.Tweening;

public class Hunter : MonoBehaviourPun
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

    [SerializeField] bool is_hunter_mode;
    [SerializeField] PostProcessVolume pp_volume;
    private ChromaticAberration ca_layer = null;
    private LensDistortion lens_distortion_layer = null;
    private Vignette vignette_layer = null;
    private float transition = 1f;

    [SerializeField] ParticleSystem trail;
    private ParticleSeek particle_seek;
    private Transform trail_target = null;

    private void Start()
    {
        spear = spear_go.GetComponent<Spear>();
        particle_seek = trail.GetComponent<ParticleSeek>();
        pp_volume = Camera.main.GetComponent<PostProcessVolume>();
        pp_volume.profile.TryGetSettings(out ca_layer);
        pp_volume.profile.TryGetSettings(out lens_distortion_layer);
        pp_volume.profile.TryGetSettings(out vignette_layer);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            //ReturnSpear(spear_go);
            photonView.RPC("ReturnSpear", RpcTarget.All);
        }
        if (Input.GetKey(KeyCode.Q)) //&& photonView.IsMine)
        {
            trail.Play();
            is_hunter_mode = true;
        }
        else
        {
            trail.Stop();
            is_hunter_mode = false;
        }
        // Hunter Sense
        HunterSense(is_hunter_mode);
    }

    public void HunterSense(bool state)
    {
        if (state)
        {
            GameObject[] animals = GameObject.FindGameObjectsWithTag("Bunny");
            for(int i = 0; i < animals.Length; i++)
            {
                //reset closest dist to ensure all targets accounted for.
                float closestDist = 100f;

                //measure distance between self and target object in list per i.
                float dist = Vector3.Distance(gameObject.transform.position, animals[i].transform.position);

                //compare distance, if distance < last closest distance measured, new closest set.
                if (dist < closestDist)
                {
                    closestDist = dist;
                    trail_target = animals[i].transform;
                }

            }
            if (trail_target != null)
            { 
                particle_seek.target = trail_target;
                //move_trail.AppendCallback(() => trail.transform.DOMove(trail_target.localPosition, 1f));
            }
        }

        float vig = state ? 0.5f : 0f;
        float chrom = state ? 0.75f : 0f;
        float lens_dist = state ? 20f : 0f;
        float vig2 = state ? 0f : 0.5f;
        float chrom2 = state ? 0 : 0.75f;
        float lens_dist2 = state ? 0f : 20f;

        ca_layer.intensity.value = Mathf.Lerp(chrom2, chrom, transition);
        lens_distortion_layer.intensity.value = Mathf.Lerp(lens_dist2, lens_dist, transition);
        vignette_layer.intensity.value = Mathf.Lerp(vig2, vig, transition);
    }

    public void ThrowSpear()
    {
        // only start syncronizing the spear RB when it is thrown this prevents the spear from wandering from the hunter's hand on other photon views.
        spear.GetComponent<PhotonRigidbodyView>().enabled = true; 

        spear.spear_rb.isKinematic = false;
        spear.spear_rb.collisionDetectionMode = CollisionDetectionMode.Continuous;
        spear.spear_rb.transform.parent = null;
        spear.spear_rb.AddForce(spear_go.transform.forward * throw_power + transform.up * 2, ForceMode.Impulse);
        has_spear = false;
    }
    
    [PunRPC]
    public void ReturnSpear()
    {
        spear_go.transform.position = right_hand.position;
        spear_go.transform.rotation = right_hand.rotation;
        spear_go.transform.parent = right_hand;
        has_spear = true;

        // disable syncronizing the spear RB when it is held, this prevents the spear from wandering from the hunter's hand on other photon views.
        spear.GetComponent<PhotonRigidbodyView>().enabled = false;
    }

    public void ReturnSpearWrapper()
    {
        photonView.RPC("ReturnSpear", RpcTarget.All);
    }
}
