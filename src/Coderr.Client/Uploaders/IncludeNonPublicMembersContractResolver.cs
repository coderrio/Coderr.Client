using System.Reflection;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Coderr.Client.Uploaders
{
    /// <summary>
    ///     JSON.NET class which also includes all private fields.
    /// </summary>
    internal class IncludeNonPublicMembersContractResolver : DefaultContractResolver
    {
        protected override JsonProperty CreateProperty(MemberInfo member, MemberSerialization memberSerialization)
        {
            //TODO: Maybe cache
            var prop = base.CreateProperty(member, memberSerialization);

            if (prop.Writable)
                return prop;

            var property = member as PropertyInfo;
            if (property == null)
                return prop;

            var hasPrivateSetter = property.SetMethod != null;
            prop.Writable = hasPrivateSetter;
            return prop;
        }
    }
}