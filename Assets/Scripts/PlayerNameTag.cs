using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Photon.Pun;

public class PlayerNameTag : MonoBehaviourPun
{
    [SerializeField] TextMeshProUGUI name_text;
    // Start is called before the first frame update
    void Start()
    {
        if (!PhotonNetwork.IsMasterClient)
        {
            if (photonView.IsMine)
            {
                SetName("");
                return;
            }
            SetName(photonView.Owner.NickName);
        }
        else
        {
            SetName("");
        }
    }

    private void SetName(string name)
    {
        name_text.text = name;
    }
}
