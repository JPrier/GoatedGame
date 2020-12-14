using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;
using Hashtable = ExitGames.Client.Photon.Hashtable;

public class MainMenu : MonoBehaviourPunCallbacks
{

    // Connecting to Opponents
    [SerializeField] private GameObject findRoomPanel = null;
    [SerializeField] private GameObject waitingStatusPanel = null;
    [SerializeField] private GameObject lobbyPanel = null;
    [SerializeField] private TextMeshProUGUI waitingStatusText = null;

    private bool isConnecting = false;
    private const string GameVersion = "0.1";
    private const int MaxPlayersPerRoom = 10;
    private static string[] playerList = new string[MaxPlayersPerRoom];
    private int readyUpCount = 0;


    // Nickname
    [SerializeField] private TMP_InputField nameInputField = null;

    private string nickname = "Player";
    
    public void SetName()
    {
        nickname = nameInputField.text;
    }

    /* Networking
     * 
     * Creates room for players to connect to each other
     * Connects players to the room if there are rooms available
     * 
     */

    private void Awake()
    {
        // Set Photon Settings
        PhotonNetwork.PhotonServerSettings.AppSettings.FixedRegion = "us";
        PhotonNetwork.AutomaticallySyncScene = true;
        CreateCustomProperites();
    }

    // https://forum.photonengine.com/discussion/9937/example-for-custom-properties
    private void CreateCustomProperites()
    {
        Hashtable roomProperties = new Hashtable();
        
        roomProperties.Add("readyCount", readyUpCount);

        SetRoomProperties(roomProperties);
    }

    public void FindRoom()
    {

        isConnecting = true;

        findRoomPanel.SetActive(false);

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
        lobbyPanel.SetActive(false);
        //waitingStatusPanel.SetActive(false);
        findRoomPanel.SetActive(true);

        Debug.Log($"Disconnected due to: {cause}");
    }

    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        Debug.Log("No open rooms, creating a new room");

        PhotonNetwork.CreateRoom(null, new RoomOptions { MaxPlayers = MaxPlayersPerRoom });
        waitingStatusPanel.SetActive(false);
        lobbyPanel.SetActive(true);
    }

    public override void OnJoinedRoom()
    {
        Debug.Log("Client successfully joined a room");
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        UpdatePlayerList();
        
        if (PhotonNetwork.CurrentRoom.PlayerCount == MaxPlayersPerRoom)
        {
            PhotonNetwork.CurrentRoom.IsOpen = false;
            //waitingStatusText.text = "Opponent Found";
            Debug.Log("Max Players connected");

            // LoadNextGame();
        }
    }

    public override void OnRoomPropertiesUpdate(Hashtable propertiesThatChanged)
    {   
        Debug.Log("Room Properties Updated");
        if ((int)GetRoomProperties()["readyCount"] >= GetCurrentPlayerCount())
        {
            LoadNextGame();
        }
    }

    private static void UpdatePlayerList()
    {
        // Change list of players that is displayed in the lobby

        // Loop through player list and add to playerList var
        int lastPlayer = 0;
        Dictionary<int, Photon.Realtime.Player> pList = PhotonNetwork.CurrentRoom.Players;
        foreach (KeyValuePair<int, Photon.Realtime.Player> p in pList)
        {
            playerList[p.Key] = p.Value.NickName;
            lastPlayer += 1;
        }

        for (int i = 0; i < playerList.Length-lastPlayer; i++)
        {
            playerList[i + lastPlayer] = "Bot ";// + i.ToString;
        }
        Debug.Log(playerList.ToString());
    }

    private void CreateLobby()
    {
        waitingStatusPanel.SetActive(false);
        lobbyPanel.SetActive(true);
        UpdatePlayerList();
        // TODO: Set player list in lobby Panel
    }

    // Load Next Scene

    public void PlayGame()
    {

        Debug.Log("Play Button Pressed");

        PhotonNetwork.PhotonServerSettings.AppSettings.FixedRegion = "us";
        PhotonNetwork.NickName = nickname;

        findRoomPanel.SetActive(true);
        FindRoom();
    }

    public void LoadNextGame()
    {
        // TODO: Set next game randomly from set of scenes (except for main menu and loading scenes)
        PhotonNetwork.LoadLevel(SceneManager.GetActiveScene().buildIndex + 1);
    }
    
    public void ReadyUp()
    {
        Hashtable roomProperties = GetRoomProperties();
        roomProperties["readyCount"] = (int)roomProperties["readyCount"]+1;
        SetRoomProperties(roomProperties);
    }


    /*
     * Getters and Setters
     */

    public Hashtable GetRoomProperties()
    {
        return PhotonNetwork.CurrentRoom.CustomProperties;
    }

    public void SetRoomProperties(Hashtable roomProperties)
    {
        PhotonNetwork.CurrentRoom.SetCustomProperties(roomProperties);
    }
    
    public static string[] GetPlayerNickNameList()
    {
        // Return list of player ids
        UpdatePlayerList();
        return playerList;
    }

    public static int GetMaxPlayerCount()
    {
        return MaxPlayersPerRoom;
    }

    public static int GetCurrentPlayerCount()
    {
        return PhotonNetwork.CurrentRoom.PlayerCount;
    }

    public static Dictionary<int, Photon.Realtime.Player>  GetPlayerDictionary()
    {
        return PhotonNetwork.CurrentRoom.Players;
    }
}
