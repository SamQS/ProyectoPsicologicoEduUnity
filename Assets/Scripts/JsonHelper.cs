public static class JsonHelper
{
    public static T[] FromJson<T>(string json)
    {
        string fixedJson = "{\"Items\":" + json + "}";
        Wrapper<T> wrapper = UnityEngine.JsonUtility.FromJson<Wrapper<T>>(fixedJson);
        return wrapper.Items;
    }

    [System.Serializable]
    private class Wrapper<T>
    {
        public T[] Items;
    }
}
