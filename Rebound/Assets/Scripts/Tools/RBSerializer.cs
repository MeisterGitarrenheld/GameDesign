using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

public class RBSerializer
{
    private class RBJsonContractResolver : DefaultContractResolver
    {
        /// <summary>
        /// Allows the serialization of private fields, too.
        /// </summary>
        /// <param name="objectType"></param>
        /// <returns></returns>
        protected override List<MemberInfo> GetSerializableMembers(Type objectType)
        {
            var flags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;
            MemberInfo[] fields = objectType.GetFields(flags);
            return fields
                .Concat(objectType.GetProperties(flags).Where(propInfo => propInfo.CanWrite))
                .ToList();
        }

        protected override IList<JsonProperty> CreateProperties(Type type, MemberSerialization memberSerialization)
        {
            return base.CreateProperties(type, MemberSerialization.Fields);
        }
    }

    private static readonly JsonSerializerSettings contractResolver = new JsonSerializerSettings() { ContractResolver = new RBJsonContractResolver() };

    /// <summary>
    /// Serializes the object and returns the string representation of the object.
    /// </summary>
    /// <returns></returns>
    public static string Serialize(object obj, bool indented = false)
    {
        return JsonConvert.SerializeObject(obj, indented ? Formatting.Indented : Formatting.None, contractResolver);
    }

    /// <summary>
    /// Deserializes the object.
    /// </summary>
    /// <param name="data"></param>
    /// <returns></returns>
    public static T Deserialize<T>(string data)
    {
        return JsonConvert.DeserializeObject<T>(data);
    }
}
