using System.Collections.Generic;
using UnityEngine;

public class ItemLobbiesPresenter : MonoBehaviour
{
    [SerializeField] private ItemLobbyItem itemLobbyPrefab;
    [SerializeField] private Transform itemLobbyContainer;
    
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
            lobbyItem.OnButtonClicked.AddListener(() => Debug.Log($"Go To {item.name} lobby"));
        }
    }
}
