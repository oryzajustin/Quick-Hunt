using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public enum Gender
{
    male,
    female
}

public class Bunny : Animal
{
    private Gender gender;
    private string type;

    public GameObject spear_go;
    [SerializeField] GameObject fake_bunny;
    private void Start()
    {
        SetHealth(1);
        SetDamage(0);

        if(Random.Range(0, 1) == 0)
        {
            SetGender(Gender.male);
        }
        else
        {
            SetGender(Gender.female);
        }
    }

    public void SetGender(Gender gender)
    {
        this.gender = gender;
    }

    public Gender GetGender()
    {
        return this.gender;
    }

    public void MakeGhostWrapper()
    {
        photonView.RPC("MakeGhost", RpcTarget.All);
    }

    [PunRPC]
    public void MakeGhost()
    {
        // make me invisible for everyone
        this.transform.GetChild(0).GetComponent<MeshRenderer>().enabled = false;
        if (photonView.IsMine) // only disable my controllers for me
        {
            this.GetComponent<CharacterController>().enabled = false;
            this.GetComponent<BunnyController>().enabled = false;
            this.GetComponent<GhostMode>().enabled = true;
            this.GetComponent<BoxCollider>().enabled = true; // collider for ghost mode
            Camera.main.GetComponent<FollowCam>().pitch_min_max.x = -25f;
        }
    }

    public void SkewerBunnyWrapper()
    {
        photonView.RPC("SkewerBunny", RpcTarget.All);
    }

    [PunRPC]
    public void SkewerBunny()
    {
        Debug.Log("SKEWERED");
        GameObject temp = Instantiate(fake_bunny, spear_go.transform.position, Quaternion.identity);
        temp.transform.parent = spear_go.transform;
        Vector3 skewer_position = new Vector3(spear_go.transform.position.x, spear_go.transform.position.y - 0.15f, spear_go.transform.position.z);
        temp.transform.position = skewer_position;
    }
}
