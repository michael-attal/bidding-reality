using System;
using System.Collections.Generic;

[System.Serializable]
public struct BackendUser
{
    public string id;
    public string name;
    public string email;
    public string role;
    public string password;
}

[System.Serializable]
public struct BackendItem
{
    public string id;
    public string name;
    public float startPrice;
    public string endDate;
    public string userID;
    public bool isSold;
}

[System.Serializable]
public struct BackendBid
{
    public string id;
    public string itemName;
    public string itemID;
    public string userID;
    public float amount;
}