using System;
using System.Collections.Generic;
using Unity.Services.Authentication;
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
            if (item.isSold)
                return;
            
            DateTime endDate = DateTime.Parse(item.endDate);
            if (endDate < DateTime.Now)
            {
                
            }
            else
            {
                var lobbyItem = Instantiate(itemLobbyPrefab, itemLobbyContainer);
                StartCoroutine(lobbyItem.Initialize(item));
                lobbyItem.OnButtonClicked.AddListener(() => JoinAuction(item.id));
            }
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
            if (foundLobby || (lobby.Data.ContainsKey("itemId") && lobby.Data["itemId"].Value != itemId))
                continue;

            foundLobby = true;
            XRINetworkGameManager.Instance.JoinLobbySpecific(lobby);
            currentLobby = lobby;
        }
        
        if (foundLobby)
            return;
        
        Debug.Log("Couldn't find a lobby. Creating one now...");
        var createdLobby = await XRINetworkGameManager.Instance.lobbyManager.CreateLobby();
        XRINetworkGameManager.Instance.JoinLobbySpecific(createdLobby);

        UpdateLobbyOptions lobbyOptions = new UpdateLobbyOptions();
        lobbyOptions.Name = $"{itemId} Bid";
        lobbyOptions.MaxPlayers = 20;
        lobbyOptions.IsPrivate = false;
        lobbyOptions.HostId = AuthenticationService.Instance.PlayerId;
        lobbyOptions.Data = new Dictionary<string, DataObject>()
        {
            { "itemId", new DataObject(DataObject.VisibilityOptions.Public, itemId) }
        };
        var updatedLobby = await LobbyService.Instance.UpdateLobbyAsync(createdLobby.Id, lobbyOptions);
        currentLobby = updatedLobby;
    }

    private void OnConnected(bool success)
    {
        if (success)
        {
            Debug.Log($"Connected successfully.");
            BackendHandler.cachedLobbyId = currentLobby.Id;
            BackendHandler.cachedItemId = currentLobby.Data["itemId"].Value;
        }
    }
}
