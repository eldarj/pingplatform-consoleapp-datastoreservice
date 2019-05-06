using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace DataSpaceMicroservice.RabbitMQ.Utils
{
    public static class MQSerializer
    {
        public static byte[] Serialize(this object obj)
        {
            if (obj == null)
            {
                return null;
            }

            var json = JsonConvert.SerializeObject(obj);
            return Encoding.UTF8.GetBytes(json);
        }

        public static object Deserialize(this byte[] arrBytes, Type type)
        {
            var json = Encoding.Default.GetString(arrBytes);
            return JsonConvert.DeserializeObject(json, type);
        }
    }
}
