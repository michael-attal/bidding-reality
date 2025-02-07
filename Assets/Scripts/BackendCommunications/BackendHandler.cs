using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Networking;

public static class BackendHandler
{
    private static string apiUrl = "https://biddingreality.michaelattal.com/";

    private static string ToJsonPayload(Dictionary<string, string> payload)
    {
        string res = "{";
        foreach (var (key, value) in payload)
        {
            res += $"\"{key}\": \"{value}\",";
        }

        res += "}";
        Debug.Log($"Generated payload: {res}");
        return res;
    }

    public static IEnumerator GetBids(UnityAction<List<BackendBid>> callback)
    {
        UnityWebRequest request = UnityWebRequest.Get(apiUrl + "bids");

        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            string rawResponse = request.downloadHandler.text;
            callback(JsonHelper<BackendBid>.GetListFromJson(rawResponse));
        }
        else
            Debug.LogError($"Failed fetching bids: {request.error}");
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
            {"name", user.name},
            {"email", user.email},
            {"role", user.role},
            {"password", user.password}
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
            callback(JsonUtility.FromJson<BackendUser>(rawResponse));
        }
        else
            Debug.LogError($"Failed login: {request.error}");
    }
}