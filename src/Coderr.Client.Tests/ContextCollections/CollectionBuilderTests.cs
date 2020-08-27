using System.Security.Principal;
using System.Text;
using Coderr.Client.ContextCollections;
using Coderr.Client.ContextCollections.Providers;
using FluentAssertions;
using Xunit;

namespace Coderr.Client.Tests.ContextCollections
{
    public class CollectionBuilderTests
    {
        [Fact]
        public void CreateForCredentials_should_support_domain_names()
        {
            var identity = new GenericIdentity("myjob\\someone");

            var actual = CollectionBuilder.CreateForCredentials(identity);

            actual.Properties["UserName"].Should().Be("someone");
            actual.Properties["DomainName"].Should().Be("myjob");
        }

        [Fact]
        public void CreateForCredentials_should_work_without_domain_names()
        {
            var identity = new GenericIdentity("someone");

            var actual = CollectionBuilder.CreateForCredentials(identity);

            actual.Properties["UserName"].Should().Be("someone");
            actual.Properties.Should().NotContainKey("DomainName");
        }

        [Fact]
        public void CreateTokenForCredentials_should_support_domain_names()
        {
            var identity = new GenericIdentity("myjob\\someone");
            var expected = CreateHash("someone");

            var actual = CollectionBuilder.CreateTokenForCredentials(identity);

            actual.Properties["UserToken"].Should().Be(expected);
            actual.Properties["DomainName"].Should().Be("myjob");
        }

        [Fact]
        public void CreateTokenForCredentials_should_work_without_domain_names()
        {
            var identity = new GenericIdentity("someone");
            var expected = CreateHash("someone");

            var actual = CollectionBuilder.CreateTokenForCredentials(identity);

            actual.Properties["UserToken"].Should().Be(expected);
            actual.Properties.Should().NotContainKey("DomainName");
        }

        private string CreateHash(string value)
        {
            const uint murmurSeed = 0xc17ca3d3;
            var buffer = Encoding.UTF8.GetBytes(value);
            var hash = MurmurHash2.Hash(buffer, murmurSeed);
            return hash.ToString("x");
        }
    }
}
