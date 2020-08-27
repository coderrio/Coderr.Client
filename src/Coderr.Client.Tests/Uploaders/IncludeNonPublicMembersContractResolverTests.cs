using Coderr.Client.Tests.Uploaders.TestObjects;
using Coderr.Client.Uploaders;
using FluentAssertions;
using Newtonsoft.Json;
using Xunit;

namespace Coderr.Client.Tests.Uploaders
{
    public class IncludeNonPublicMembersContractResolverTests
    {
        [Fact]
        public void should_serialize_private_property()
        {
            var settings =new JsonSerializerSettings {ContractResolver = new IncludeNonPublicMembersContractResolver()};
            var item = new ClassWithPrivateSetter("Hello world");

            var json = JsonConvert.SerializeObject(item, settings);
            var actual = JsonConvert.DeserializeObject<ClassWithPrivateSetter>(json, settings);


            actual.Prop.Should().Be(item.Prop);
        }
    }
}
