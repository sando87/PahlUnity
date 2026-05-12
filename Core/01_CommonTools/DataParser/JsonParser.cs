using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;

namespace PahlUnity
{
    public static class JsonParser
    {
        public static string ToJsonArray<T>(T[] objs)
        {
            List<T> list = new List<T>();
            list.AddRange(objs);
            string ret = JsonConvert.SerializeObject(list, Formatting.Indented);
            return ret;
        }

        public static T[] FromJsonArray<T>(string stream)
        {
            List<T> list = JsonConvert.DeserializeObject<List<T>>(stream);
            return list.ToArray();
        }

        public static List<T> FromJsonList<T>(string stream)
        {
            List<T> list = JsonConvert.DeserializeObject<List<T>>(stream);
            return list;
        }

        public static string ToJson<T>(T obj)
        {
            string ret = JsonConvert.SerializeObject(obj, Formatting.Indented);
            return ret;
        }
        public static T FromJson<T>(string stream)
        {
            T ret = JsonConvert.DeserializeObject<T>(stream);
            return ret;
        }
    }
}