using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Services.Lobbies.Models;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Networking;

public static class BackendHandler
{
    private static string apiUrl = "https://biddingreality.michaelattal.com/";
    
    public static BackendUser loggedInUser;
    public static string cachedLobbyId;
    public static string cachedItemId;

    private static string ToJsonPayload(Dictionary<string, string> payload)
    {
        string res = "{";
        foreach (var (key, value) in payload)
        {
            res += $"\"{key}\": {value},";
        }
        res = res.TrimEnd(',');
        res += "}";
        Debug.Log($"Generated payload: {res}");
        return res;
    }
    
    private static string ToJsonString(string str) => "\"" + str + "\"";
    private static string ToJsonFloat(float f) => f.ToString("0.00").Replace(",", ".");

    public static IEnumerator GetBids(UnityAction<List<BackendBid>> callback)
    {
        UnityWebRequest request = UnityWebRequest.Get(apiUrl + "bids");

        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            string rawResponse = request.downloadHandler.text;
            Debug.Log(rawResponse);
            callback(JsonHelper<BackendBid>.GetListFromJson(rawResponse));
        }
        else
            Debug.LogError($"Failed fetching bids: {request.error}");
    }
    
    public static IEnumerator GetBids(string itemId, UnityAction<List<BackendBid>> callback)
    {
        UnityWebRequest request = UnityWebRequest.Get(apiUrl + $"bids/item/{itemId}");

        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            string rawResponse = request.downloadHandler.text;
            callback(JsonHelper<BackendBid>.GetListFromJson(rawResponse));
        }
        else
            Debug.LogError($"Failed fetching bids: {request.error}");
    }

    public static IEnumerator GetHighestBid(string itemId, UnityAction<BackendBid> callback)
    {
        yield return GetBids(itemId, bids =>
        {
            var highestBid = bids.OrderByDescending(bid => bid.amount)
                .First();
            callback(highestBid);
        });
    }

    public static IEnumerator PostBid(BackendUser user, BackendItem item, float amount)
    {
        Dictionary<string, string> payload = new Dictionary<string, string>()
        {
            {"userID", ToJsonString(user.id)},
            {"itemID", ToJsonString(item.id)},
            {"itemName", ToJsonString(item.name)},
            {"amount", ToJsonFloat(amount)}
        };
        
        UnityWebRequest request = UnityWebRequest.Post(apiUrl + "bids", ToJsonPayload(payload), "application/json");

        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            Debug.Log($"Successfully added bid for item {item.name}!");
        }
        else
            Debug.LogError($"Failed uploading bid: {request.error}");
    }

    public static IEnumerator GetItems(UnityAction<List<BackendItem>> callback)
    {
        UnityWebRequest request = UnityWebRequest.Get(apiUrl + "items");

        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            string rawResponse = request.downloadHandler.text;
            callback(JsonHelper<BackendItem>.GetListFromJson(rawResponse));
        }
        else
            Debug.LogError($"Failed fetching bids: {request.error}");
    }

    public static IEnumerator PostItem(BackendItem item)
    {
        Dictionary<string, string> payload = new Dictionary<string, string>()
        {
            {"name", ToJsonString(item.name)},
            {"endDate", ToJsonString(item.endDate)},
            {"startPrice", ToJsonFloat(item.startPrice)},
            {"userID", ToJsonString(item.userID)},
            {"isSold", item.isSold.ToString().ToLower()}
        };
        
        UnityWebRequest request = UnityWebRequest.Post(apiUrl + "items", ToJsonPayload(payload), "application/json");

        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            Debug.Log($"Successfully added item {item.name}!");
        }
        else
            Debug.LogError($"Failed uploading item: {request.error}");
    }

    public static IEnumerator GetUsers(UnityAction<List<BackendUser>> callback)
    {
        UnityWebRequest request = UnityWebRequest.Get(apiUrl + "users/all?adminEmail=sacha@toutut.fr&adminPassword=TestPassword");

        yield return request.SendWebRequest();
        
        if (request.result == UnityWebRequest.Result.Success)
        {
            string rawResponse = request.downloadHandler.text;
            callback(JsonHelper<BackendUser>.GetListFromJson(rawResponse));
        }
        else
            Debug.LogError($"Failed fetching users: {request.error}");
    }

    public static IEnumerator PostUser(BackendUser user, UnityAction<BackendUser> callback)
    {
        Dictionary<string, string> payload = new Dictionary<string, string>()
        {
            {"name", ToJsonString(user.name)},
            {"email", ToJsonString(user.email)},
            {"role", ToJsonString(user.role)},
            {"password", ToJsonString(user.password)}
        };

        UnityWebRequest request = UnityWebRequest.Post(apiUrl + "users", ToJsonPayload(payload), "application/json");

        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            Debug.Log($"Successfully added user {user.name}!");
            yield return LoginUser(user.email, user.password, callback);
        }
        else
            Debug.LogError($"Failed uploading user: {request.error}");
    }
    
    public static IEnumerator LoginUser(string email, string password, UnityAction<BackendUser> callback)
    {
        UnityWebRequest request = UnityWebRequest.Get(apiUrl + $"users/login?email={email}&password={password}");

        yield return request.SendWebRequest();
        
        if (request.result == UnityWebRequest.Result.Success)
        {
            string rawResponse = request.downloadHandler.text;
            loggedInUser = JsonUtility.FromJson<BackendUser>(rawResponse);
            callback(loggedInUser);
        }
        else
            Debug.LogError($"Failed login: {request.error}");
    }

    public static IEnumerator PostCardToken(string number, string cvc, string month, string year)
        => PostCardToken(loggedInUser, number, cvc, month, year);
    public static IEnumerator PostCardToken(BackendUser user, string number, string cvc, string month, string year)
    {
        Dictionary<string, string> payload = new Dictionary<string, string>()
        {
            {"number", ToJsonString(number)},
            {"cvc", ToJsonString(cvc)},
            {"month", ToJsonString(month)},
            {"year", ToJsonString(year)}
        };

        UnityWebRequest request = UnityWebRequest.Post(apiUrl + "cardtoken/" + user.id, ToJsonPayload(payload), "application/json");

        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            Debug.Log($"Successfully added card!");
        }
        else
            Debug.LogError($"Failed adding card token: {request.error}");
    }
}