using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace OneTrueError.Client.Converters
{
    /// <summary>
    ///     JSON.NET class which also includes all private fields.
    /// </summary>
    internal class IncludeNonPublicMembersContractResolver : DefaultContractResolver
    {
        public IncludeNonPublicMembersContractResolver()
        {
            DefaultMembersSearchFlags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;
        }

        protected override JsonProperty CreateProperty(MemberInfo member, MemberSerialization memberSerialization)
        {
            //TODO: Maybe cache
            var prop = base.CreateProperty(member, memberSerialization);

            if (!prop.Writable)
            {
                var property = member as PropertyInfo;
                if (property != null)
                {
                    var hasPrivateSetter = property.GetSetMethod(true) != null;
                    prop.Writable = hasPrivateSetter;
                }
            }

            return prop;
        }

        protected override List<MemberInfo> GetSerializableMembers(Type objectType)
        {
            var members = base.GetSerializableMembers(objectType);
            return members.Where(m => !m.Name.EndsWith("k__BackingField")).ToList();
        }
    }
}