using System;
using System.Reflection;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;

/// <summary>
/// 구글폼으로 데이터 전송(주로 로깅 용도로 사용)
/// 
/// Example
// public class GoogleFormParam
// {
//     [GoogleFormField("entry.819340141")] public string utcTime = "";
//     [GoogleFormField("entry.960706979")] public string deviceID = "";
//     [GoogleFormField("entry.940914066")] public string sessionID = "";
//     [GoogleFormField("entry.228461063")] public string userCountry = "";
//     [GoogleFormField("entry.605889500")] public string platform = "";
//     [GoogleFormField("entry.1148761957")] public string version = "";
//     [GoogleFormField("entry.700129920")] public string eventName = "";
//     [GoogleFormField("entry.3597794")] public string comments = "";
// }

// WriterGoogleForm logger = new WriterGoogleForm("https://docs.google.com/forms/u/0/d/e/1FAIpQLSfYBTPREXeVkeGr_QdPIFumCmgriDoQvcClgLfqaiwm-9C9kg/formResponse");
// GoogleFormParam param = new GoogleFormParam();
// param.eventName = "MonsterKill";
// param.comments = "Boss defeated";
// await logger.SendAsync(param);

// "entry.819340141"와 같은 필드ID값 정보 확인을 위한 링크는 별도
// https://docs.google.com/forms/d/1ac4UvLZhDmZjtOgmoJ-ROiQXoFL5RNwqBLUaMt-XiyY/edit
/// </summary>

namespace PahlUnity
{
    public class WriterGoogleForm : IDataProvider
    {
        private string mGoogleFormURL;

        public WriterGoogleForm(string googleFormURL)
        {
            mGoogleFormURL = googleFormURL;
        }

        public UniTask<string> LoadAsync()
        {
            throw new NotImplementedException();
        }

        public async UniTask SaveAsync(string data)
        {
            await SendAsync(data);
        }

        private async UniTask SendAsync<T>(T data)
        {
            WWWForm form = new WWWForm();

            Type type = typeof(T);

            BindingFlags flags = BindingFlags.Public | BindingFlags.Instance;

            // field
            FieldInfo[] fields = type.GetFields(flags);
            foreach (FieldInfo field in fields)
            {
                GoogleFormFieldAttribute attr = field.GetCustomAttribute<GoogleFormFieldAttribute>();
                if (attr == null)
                    continue;

                object value = field.GetValue(data);
                form.AddField(attr.EntryID, value?.ToString() ?? "");
            }

            // property
            PropertyInfo[] properties = type.GetProperties(flags);
            foreach (PropertyInfo property in properties)
            {
                GoogleFormFieldAttribute attr = property.GetCustomAttribute<GoogleFormFieldAttribute>();
                if (attr == null)
                    continue;

                if (!property.CanRead)
                    continue;

                object value = property.GetValue(data);

                form.AddField(attr.EntryID, value?.ToString() ?? "");
            }

            using UnityWebRequest www = UnityWebRequest.Post(mGoogleFormURL, form);
            await www.SendWebRequest();

            if (www.result != UnityWebRequest.Result.Success)
            {
                throw new Exception(www.error);
            }
        }
    }

    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
    public class GoogleFormFieldAttribute : Attribute
    {
        public string EntryID;

        public GoogleFormFieldAttribute(string entryID)
        {
            EntryID = entryID;
        }
    }
}