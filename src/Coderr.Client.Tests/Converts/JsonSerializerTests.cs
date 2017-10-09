using codeRR.Client.Converters;
using FluentAssertions;
using Newtonsoft.Json;
using Xunit;

namespace Coderr.Client.Tests.Converts
{
    public class JsonSerializerTests
    {
        [Fact]
        public void Should_include_private_members()
        {
            var dto = new DtoWithPrivateSetter("Hello");
            var settings = new JsonSerializerSettings
            {
                ContractResolver = new IncludeNonPublicMembersContractResolver()
            };

            var json = JsonConvert.SerializeObject(dto, settings);

            json.Should().Contain("Hello");
        }
    }
}
