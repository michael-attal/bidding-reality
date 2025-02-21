using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Networking;

public static class BackendHandler
{
    private static string apiUrl = "https://biddingreality.michaelattal.com/";
    
    public static BackendUser loggedInUser;

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
    
    public static IEnumerator GetBids(BackendItem item, UnityAction<List<BackendBid>> callback)
    {
        UnityWebRequest request = UnityWebRequest.Get(apiUrl + $"bids/item/{item.id}");

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

    public static IEnumerator PostUser(BackendUser user)
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
}