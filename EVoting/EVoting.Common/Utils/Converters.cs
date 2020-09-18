using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Text.Json;

namespace EVoting.Common.Utils
{
    public class Converters
    {
        public static byte[] ConvertToByteArray(Object obj)
        {
            // if (obj == null)
            //     return null;
            // var bf = new BinaryFormatter();
            // using (var ms = new MemoryStream())
            // {
            //     bf.Serialize(ms, obj);
            //     return ms.ToArray();
            // }

            var json = JsonSerializer.Serialize(obj, new JsonSerializerOptions() {PropertyNameCaseInsensitive = true});
            return Encoding.UTF8.GetBytes(json);
        }

        public static T ConvertToObject<T>(byte[] array)
        {
            // var bf = new BinaryFormatter();
            // using (var ms = new MemoryStream())
            // {
            //     ms.Write(array, 0, array.Length);
            //     ms.Seek(0, SeekOrigin.Begin);
            //     return (T)bf.Deserialize(ms);
            //
            // }

            var str = Encoding.UTF8.GetString(array);
            return JsonSerializer.Deserialize<T>(str, new JsonSerializerOptions() {PropertyNameCaseInsensitive = true});

        }
    }
}
