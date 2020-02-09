using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;

public class MainMenu : MonoBehaviourPunCallbacks
{
    [Header("Name Selection")]
    [SerializeField] GameObject name_panel;
    [SerializeField] TMP_InputField name_input_field;
    [SerializeField] Button name_continue_button;

    [Header("Networking Panels")]
    [SerializeField] GameObject find_players_panel;
    [SerializeField] GameObject waiting_panel;
    [SerializeField] TextMeshProUGUI waiting_status_text;
    [SerializeField] Button find_players_button;
    [SerializeField] Button start_button;

    private bool is_connecting = false;

    private const string game_version = "0.1";
    private const int max_players = 10;

    private const string player_prefs_key_name = "PlayerName";

    private void Awake()
    {
        PhotonNetwork.AutomaticallySyncScene = true;
    }

    // Start is called before the first frame update
    void Start()
    {
        SetUpInputField();
        name_input_field.onValueChanged.AddListener(delegate { SetPlayerName(name_input_field.text); });
        find_players_button.onClick.AddListener(delegate { FindPlayers(); });
        name_continue_button.onClick.AddListener(delegate { SavePlayerName(); });
        start_button.onClick.AddListener(delegate { StartMatch(); });
    }

    // Update is called once per frame
    void Update()
    {

    }

    #region Networking
    public void FindPlayers()
    {
        is_connecting = true;

        find_players_panel.SetActive(false);
        waiting_panel.SetActive(true);

        waiting_status_text.text = "Searching for players...";

        if (PhotonNetwork.IsConnected)
        {
            PhotonNetwork.JoinRandomRoom();
        }
        else
        {
            PhotonNetwork.GameVersion = game_version;
            PhotonNetwork.ConnectUsingSettings();
        }
    }

    public override void OnConnectedToMaster()
    {
        Debug.Log("Connected to Master");
        if (is_connecting)
        {
            PhotonNetwork.JoinRandomRoom();
        }
    }

    public override void OnDisconnected(DisconnectCause cause)
    {
        waiting_panel.SetActive(false);
        find_players_panel.SetActive(true);
        Debug.Log($"Disconnected due to: {cause}");
    }

    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        Debug.Log("No clients are waiting for players; creating new room");

        PhotonNetwork.CreateRoom(null, new RoomOptions { MaxPlayers = max_players });
    }

    public override void OnJoinedRoom()
    {
        Debug.Log("Client successfully connected to room");

        Debug.Log(PhotonNetwork.IsMasterClient);
        int player_count = PhotonNetwork.CurrentRoom.PlayerCount;

        if(player_count < 2)
        {
            waiting_status_text.text = "Waiting for other players...";
            start_button.interactable = false;
            Debug.Log("Client waiting for players");
        }
        else
        {
            if(player_count != max_players)
            {
                waiting_status_text.text = player_count + " Players found! ";
                Debug.Log("More players can join; Match is ready");
                if (PhotonNetwork.IsMasterClient && photonView.IsMine)
                {
                    start_button.interactable = true;
                }
            }
            else
            {
                waiting_status_text.text = "Lobby full!";
                Debug.Log("Lobby is full! Match is ready");
            }
        }
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        if(PhotonNetwork.CurrentRoom.PlayerCount == max_players)
        {
            PhotonNetwork.CurrentRoom.IsOpen = false; // don't let any other players join

            Debug.Log("Match is ready and full");
            waiting_status_text.text = PhotonNetwork.CurrentRoom.PlayerCount + " Players found!";

            StartMatch(); // start the game
        }
        else
        {
            Debug.Log("Match is ready; but not full");
            Debug.Log(PhotonNetwork.IsMasterClient);
            waiting_status_text.text = PhotonNetwork.CurrentRoom.PlayerCount + " Players found!";
            if (PhotonNetwork.IsMasterClient) // only master client can start the match
            {
                start_button.interactable = true;
            }
            
        }
    }

    public void StartMatch()
    {
        Debug.Log("Starting Match");
        PhotonNetwork.LoadLevel("MainGame");
    }

    #endregion

    #region Name Input
    private void SetUpInputField()
    {
        if (!PlayerPrefs.HasKey(player_prefs_key_name))
        {
            return;
        }
        string default_name = PlayerPrefs.GetString(player_prefs_key_name);
        name_input_field.text = default_name;
        SetPlayerName(default_name);
    }

    public void SetPlayerName(string name)
    {
        Debug.Log(name);
        name_continue_button.interactable = !string.IsNullOrEmpty(name);
    }

    public void SavePlayerName()
    {
        string player_name = name_input_field.text;

        PhotonNetwork.NickName = player_name;

        PlayerPrefs.SetString(player_prefs_key_name, player_name);

        name_panel.SetActive(false);
        find_players_panel.SetActive(true);
    }

    #endregion
}
