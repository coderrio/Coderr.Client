using System;
using Newtonsoft.Json;

namespace OneTrueError.Client.Converters
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