using System;
using Newtonsoft.Json;

namespace Coderr.Client.Converters
{
    internal class Serializer
    {
        public static string Serialize(Exception err)
        {
            return JsonConvert.SerializeObject(err);
            //return "";
        }
    }
}