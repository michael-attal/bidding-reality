using System.Collections;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ItemLobbyItem : MonoBehaviour
{
    [SerializeField] private TMP_Text itemNameText;
    [SerializeField] private TMP_Text itemStartPriceText;
    [SerializeField] private TMP_Text itemCurrentBidText;
    [SerializeField] private Button button;
    
    public Button.ButtonClickedEvent OnButtonClicked => button.onClick;
    
    public IEnumerator Initialize(BackendItem backendItem)
    {
        bool isSold = !backendItem.isSold;

        button.interactable = !isSold;
        itemNameText.text = backendItem.name + (isSold ? " (Sold)" : "");
        itemStartPriceText.text = $"Starting Price: {backendItem.startPrice.ToString("0.00")}$";
        
        yield return BackendHandler.GetHighestBid(backendItem.id, result =>
        {
            itemCurrentBidText.text = result.amount.ToString("0.00") + "$";
        });
    }
}
