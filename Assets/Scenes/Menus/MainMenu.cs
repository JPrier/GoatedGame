using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;

public class MainMenu : MonoBehaviourPunCallbacks
{

    // Connecting to Opponents
    [SerializeField] private GameObject findOpponentPanel = null;
    [SerializeField] private GameObject waitingStatusPanel = null;
    [SerializeField] private TextMeshProUGUI waitingStatusText = null;

    private bool isConnecting = false;
    private const string GameVersion = "0.1";
    private const int MaxPlayersPerRoom = 2;


    // Nickname
    [SerializeField] private TMP_InputField nameInputField = null;

    private string nickname = "Player";
    
    public void SetName()
    {
        nickname = nameInputField.text;
    }

    // Networking -- Currently Finds One other player and starts game immediately
    // TODO: Create a lobby/waiting room where players can see others and wait to start (possibly with a host or readyup system)

    private void Awake()
    {
        PhotonNetwork.PhotonServerSettings.AppSettings.FixedRegion = "us";
        PhotonNetwork.AutomaticallySyncScene = true;
    }

    public void FindOpponent()
    {

        isConnecting = true;

        findOpponentPanel.SetActive(false);
        waitingStatusPanel.SetActive(true);

        waitingStatusText.text = "Searching...";

        if (PhotonNetwork.IsConnected)
        {
            PhotonNetwork.JoinRandomRoom();
        }
        else
        {
            PhotonNetwork.GameVersion = GameVersion;
            PhotonNetwork.ConnectUsingSettings();
        }
    }

    public override void OnConnectedToMaster()
    {
        Debug.Log("Connected To Master");

        if (isConnecting)
        {
            PhotonNetwork.JoinRandomRoom();
        }
    }

    public override void OnDisconnected(DisconnectCause cause)
    {
        waitingStatusPanel.SetActive(false);
        findOpponentPanel.SetActive(true);

        Debug.Log($"Disconnected due to: {cause}");
    }

    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        Debug.Log("No clients are waiting for an opponent, creating a new room");

        PhotonNetwork.CreateRoom(null, new RoomOptions { MaxPlayers = MaxPlayersPerRoom });
    }

    public override void OnJoinedRoom()
    {
        Debug.Log("Client successfully joined a room");

        int playerCount = PhotonNetwork.CurrentRoom.PlayerCount;

        if (playerCount != MaxPlayersPerRoom)
        {
            waitingStatusText.text = "Waiting For Opponent";
            Debug.Log("Client is waiting for an opponent");
        }
        else
        {
            waitingStatusText.text = "Opponent Found";
            Debug.Log("Match is ready to begin");
        }
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        if (PhotonNetwork.CurrentRoom.PlayerCount == MaxPlayersPerRoom)
        {
            PhotonNetwork.CurrentRoom.IsOpen = false;

            waitingStatusText.text = "Opponent Found";
            Debug.Log("Match is ready to begin");

            PhotonNetwork.LoadLevel(SceneManager.GetActiveScene().buildIndex + 1);
        }
    }

    // Load Next Scene

    public void PlayGame()
    {
        PhotonNetwork.PhotonServerSettings.AppSettings.FixedRegion = "us";
        PhotonNetwork.NickName = nickname;

        /*SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);*/
        findOpponentPanel.SetActive(true);
        FindOpponent();
    }

}
