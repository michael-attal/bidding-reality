using System.Collections.Generic;
using Unity.Netcode;
using Unity.Services.Lobbies;
using Unity.Services.Lobbies.Models;
using UnityEngine;
using XRMultiplayer;

public class ItemLobbiesPresenter : MonoBehaviour
{
    [SerializeField] private ItemLobbyItem itemLobbyPrefab;
    [SerializeField] private Transform itemLobbyContainer;

    private Lobby currentLobby;
    
    void Start()
    {
        StartCoroutine(BackendHandler.GetItems(Initialize));
    }

    void Initialize(List<BackendItem> items)
    {
        foreach (var item in items)
        {
            var lobbyItem = Instantiate(itemLobbyPrefab, itemLobbyContainer);
            StartCoroutine(lobbyItem.Initialize(item));
            lobbyItem.OnButtonClicked.AddListener(() => JoinAuction(item.id));
        }
    }

    // Find and join the corresponding auction lobby. If there is none, create one yourself.
    private async void JoinAuction(string itemId)
    {
        XRINetworkGameManager.Connected.Subscribe(OnConnected);

        var lobbies = await LobbyManager.GetLobbiesAsync();
        bool foundLobby = false;
        foreach (var lobby in lobbies.Results)
        {
            if (foundLobby || lobby.Data["itemId"].Value != itemId)
                continue;

            foundLobby = true;
            XRINetworkGameManager.Instance.JoinLobbySpecific(lobby);
            currentLobby = lobby;
        }
        
        if (foundLobby)
            return;
        
        Debug.Log("Couldn't find a lobby. Creating one now...");
        var createdLobby = await XRINetworkGameManager.Instance.lobbyManager.CreateLobby();
        createdLobby.Data["itemId"] = new DataObject(DataObject.VisibilityOptions.Public, itemId);
        XRINetworkGameManager.Instance.JoinLobbySpecific(createdLobby);
        currentLobby = createdLobby;
    }

    private void OnConnected(bool success)
    {
        if (success)
        {
            Debug.Log($"Connected successfully. ItemID: {currentLobby.Data["itemId"].Value}");
        }
    }
}
