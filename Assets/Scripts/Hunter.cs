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
    [SerializeField] GameObject conjured_spear;
    public Material conjured_spear_mat;
    private Spear spear;
    private const string FADE_NAME = "_Fade_Amount";
    public bool can_conjure = true;
    [SerializeField] float conjure_transition;

    [Space]
    [Header("Controller")]
    [SerializeField] PlayerController controller;

    [SerializeField] float throw_power;
    
    [Space]
    [Header("Body Parts")]
    public Transform right_hand;

    [Space]
    [Header("Audio")]
    [SerializeField] AudioClip heart_beat;
    [SerializeField] AudioSource audio_source;

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

    [SerializeField] GameObject[] animals;

    private void Start()
    {
        //spear = spear_go.GetComponent<Spear>();
        particle_seek = trail.GetComponent<ParticleSeek>();
        pp_volume = Camera.main.GetComponent<PostProcessVolume>();
        pp_volume.profile.TryGetSettings(out ca_layer);
        pp_volume.profile.TryGetSettings(out lens_distortion_layer);
        pp_volume.profile.TryGetSettings(out vignette_layer);
        conjure_transition = 1f;
    }

    private void Update()
    {
        if (PhotonNetwork.IsMasterClient && photonView.IsMine)
        {
            //if (Input.GetKeyDown(KeyCode.R))
            //{
            //    //ReturnSpear(spear_go);
            //    photonView.RPC("ReturnSpear", RpcTarget.All);
            //}
            if (Input.GetKey(KeyCode.Q)) //&& photonView.IsMine)
            {
                trail.Play();
                is_hunter_mode = true;
            }
            else
            {
                audio_source.Stop();
                trail.Stop();
                is_hunter_mode = false;
            }
            if (Input.GetKeyDown(KeyCode.Q))
            {
                audio_source.clip = heart_beat;
                audio_source.Play();
            }
            // Hunter Sense
            HunterSense(is_hunter_mode);
        }
    }

    private void HunterSense(bool state)
    {
        if (state)
        {
            animals = GameObject.FindGameObjectsWithTag("Bunny");
            GameObject target = GetClosestAnimal(animals);
            if (target != null)
            {
                particle_seek.target = target.transform;
            }
            else
            {
                particle_seek.target = null;
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

    public void CreateSpearWrapper()
    {
        photonView.RPC("CreateSpear", RpcTarget.All);
        //CreateSpear();
    }

    [PunRPC]
    public void CreateSpear() // create a spear at the Hunter's hand across the network
    {
        conjured_spear = Instantiate(spear_go, right_hand.position, Quaternion.identity); // create a new spear
        conjured_spear.transform.up = right_hand.forward;
        conjured_spear.transform.SetParent(right_hand);
        conjured_spear_mat = conjured_spear.GetComponent<Renderer>().material;
        conjured_spear_mat.SetFloat(FADE_NAME, 1);
        spear = conjured_spear.GetComponent<Spear>();
    }

    public void FadeInSpearWrapper()
    {
        photonView.RPC("FadeInSpear", RpcTarget.All);
        //FadeInSpear();
    }

    [PunRPC]
    public void FadeInSpear()
    {
        //conjure_transition -= Time.deltaTime;
        //conjure_transition = Mathf.Clamp(conjure_transition, 0, 1);
        //conjured_spear_mat.SetFloat(FADE_NAME, conjure_transition);
        StartCoroutine(SpearFadeIn());
    }

    public void StopFadeInWrapper()
    {
        photonView.RPC("StopFadeIn", RpcTarget.All);
    }

    [PunRPC]
    public void StopFadeIn()
    {
        StopCoroutine(SpearFadeIn());
    }

    private IEnumerator SpearFadeIn()
    {
        float duration = 0;
        float totalTime = 1; // 1 second
        while (totalTime >= duration)
        {
            totalTime -= Time.deltaTime;
            conjure_transition = totalTime;
            conjure_transition = Mathf.Clamp(conjure_transition, 0, 1);
            conjured_spear_mat.SetFloat(FADE_NAME, conjure_transition);
            yield return null;
        }
        has_spear = true;
        conjure_transition = 0;
    }

    public void FadeOutSpearWrapper()
    {
        has_spear = false;
        photonView.RPC("FadeOutSpear", RpcTarget.All);
        //FadeOutSpear();
    }

    [PunRPC]
    public void FadeOutSpear()
    {
        StartCoroutine(SpearFadeOut());
    }

    public void StopFadeOutWrapper()
    {
        photonView.RPC("StopFadeOut", RpcTarget.All);
    }

    [PunRPC]
    public void StopFadeOut()
    {
        StopCoroutine(SpearFadeOut());
    }

    private IEnumerator SpearFadeOut()
    {
        float duration = 1f; // 1 second
        float totalTime = 0;
        while (totalTime <= duration)
        {
            totalTime += Time.deltaTime;
            conjure_transition = totalTime;
            conjure_transition = Mathf.Clamp(conjure_transition, 0, 1);
            conjured_spear_mat.SetFloat(FADE_NAME, conjure_transition);
            yield return null;
        }
        DestroySpearWrapper();
        conjure_transition = 1;
    }

    public void DestroySpearWrapper()
    {
        photonView.RPC("DestroySpear", RpcTarget.All);
        //DestroySpear();
    }

    [PunRPC]
    public void DestroySpear()
    {
        Destroy(conjured_spear);
        has_spear = false;
    }

    public void ThrowSpear()
    {
        // only start syncronizing the spear RB when it is thrown this prevents the spear from wandering from the hunter's hand on other photon views.
        has_spear = false;
        spear.GetComponent<PhotonRigidbodyView>().enabled = true;
        spear.spear_rb.isKinematic = false;
        spear.spear_rb.collisionDetectionMode = CollisionDetectionMode.Continuous;
        spear.spear_rb.transform.parent = null;
        spear.transform.up = Camera.main.transform.forward;
        spear.spear_rb.AddForce(Camera.main.transform.forward * throw_power + transform.up * 2, ForceMode.Impulse);
        photonView.RPC("MakeSolid", RpcTarget.All);
        //MakeSolid();
    }

    [PunRPC]
    public void MakeSolidWrapper()
    {
        photonView.RPC("MakeSolid", RpcTarget.All);
    }

    [PunRPC]
    public void MakeSolid()
    {
        conjured_spear_mat.SetFloat(FADE_NAME, 0);
        conjure_transition = 1;
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

    public void SetConjureTransition(float val)
    {
        conjure_transition = val;
    }

    public float GetConjureTransition()
    {
        return conjure_transition;
    }

    public GameObject GetConjuredSpear()
    {
        return conjured_spear;
    }

    public void SetConjureTransitionNetworkWrapper(float val)
    {
        photonView.RPC("SetConjureTransitionNetwork", RpcTarget.All, val);
    }

    [PunRPC]
    public void SetConjureTransitionNetwork(float val)
    {
        conjure_transition = val;
    }

    public void ReturnSpearWrapper()
    {
        photonView.RPC("ReturnSpear", RpcTarget.All);
    }

    private GameObject GetClosestAnimal(GameObject[] animals)
    {
        GameObject bestTarget = null;
        float closestDistanceSqr = Mathf.Infinity;
        Vector3 currentPosition = transform.position;
        foreach (GameObject potentialTarget in animals)
        {
            Vector3 directionToTarget = potentialTarget.transform.position - currentPosition;
            float dSqrToTarget = directionToTarget.sqrMagnitude;
            if (dSqrToTarget < closestDistanceSqr)
            {
                closestDistanceSqr = dSqrToTarget;
                bestTarget = potentialTarget;
            }
        }
        return bestTarget;
    }
}
