using System;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;

public class BiddingPanelScript : MonoBehaviour
{
    [Range(0, 90)] [SerializeField] private float upThreshold;
    
    private bool isPointingUp => (transform.up * 90.0f).y >= 90 - upThreshold;

    private float amountToBid;

    public void RaiseBid(InputAction.CallbackContext context)
    {
        // Detect if panel is pointing up, otherwise ignore input
        if (!context.started || !isPointingUp)
            return;

        StartCoroutine(BackendHandler.GetItems(items =>
        {
            var item = items.First(item => item.id == BackendHandler.cachedItemId);
            StartCoroutine(BackendHandler.PostBid(BackendHandler.loggedInUser, item, amountToBid));
        }));
    }

    public void UpdateIncrement(InputAction.CallbackContext context)
    {
        var direction = context.action.ReadValue<Vector2>();

        if (direction.y >= 0.5f)
            amountToBid += 10.0f;
        Debug.Log(amountToBid);
    }
}
