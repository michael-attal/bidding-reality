using System;
using System.Collections;
using TMPro;
using UnityEditor;
using UnityEngine;

public class AuctionHouseScript : MonoBehaviour
{
    [SerializeField] private float updateInterval = 5.00f;
    [SerializeField] private TMP_Text amountText;
    [SerializeField] private Transform display;

    private bool instantiatedModel = false;
    
    private void Start()
        => StartCoroutine(UpdateLobby());

    private IEnumerator UpdateLobby()
    {
        while (true)
        {
            if (!string.IsNullOrEmpty(BackendHandler.cachedItemId))
            {
                yield return BackendHandler.GetHighestBid(BackendHandler.cachedItemId, bid => amountText.text = $"Current Highest Bid: {bid.amount}$");
                if (!instantiatedModel)
                {
                    var path = $"ItemPrefabs/{BackendHandler.cachedItemId}";
                    try
                    {
                        var prefab = Resources.Load<GameObject>(path);
                        Instantiate(prefab, display);
                        instantiatedModel = true;
                    }
                    catch
                    {
                        Debug.LogWarning($"Could not load a model at path {path}");
                    }
                }
            }
            
            yield return new WaitForSeconds(updateInterval);
        }
    }
}
