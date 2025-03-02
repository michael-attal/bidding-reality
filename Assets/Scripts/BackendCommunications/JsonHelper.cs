using System.Collections.Generic;
using UnityEngine;

public static class JsonHelper<T> where T : struct
{
    [System.Serializable]
    private struct JsonIntermediateList
    {
        public List<T> values;
    }

    public static List<T> GetListFromJson(string json)
    {
        string newJson = "{ \"values\": " + json + " }";
        return JsonUtility.FromJson<JsonIntermediateList>(newJson).values;
    }
}
