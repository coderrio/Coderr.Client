using Coderr.Client.ContextCollections;
using Coderr.Client.NetStd.Tests.Config.ContextCollections.TestObjects;
using FluentAssertions;
using Xunit;

namespace Coderr.Client.NetStd.Tests.Config.ContextCollections
{
    public class UserCredentialsTests
    {
        [Fact]
        public void userName_domainName_constructor_should_bE_able_To_Assign_values_propertly()
        {

            var actual = CollectionBuilder.CreateForCredentials("gauffin.com", "jgauffin");

            actual.Properties["UserName"].Should().Be("jgauffin");
            actual.Properties["DomainName"].Should().Be("gauffin.com");
        }

        [Fact]
        public void samAccountName_should_be_divided_into_domain_and_userName()
        {

            var actual =
                CollectionBuilder.CreateForCredentials(new MyPrincipal(new MyIdentity("gauffin.com\\jgauffin")));

            actual.Properties["UserName"].Should().Be("jgauffin");
            actual.Properties["DomainName"].Should().Be("gauffin.com");
        }

        [Fact]
        public void identity_should_be_divided_into_domain_and_userName()
        {

            var actual = CollectionBuilder.CreateForCredentials(new MyPrincipal("gauffin.com\\jgauffin"));

            actual.Properties["UserName"].Should().Be("jgauffin");
            actual.Properties["DomainName"].Should().Be("gauffin.com");
        }

        [Fact]
        public void should_also_support_just_userName_in_the_string_constructor()
        {

            var actual = CollectionBuilder.CreateForCredentials(new MyIdentity("jgauffin"));

            actual.Properties["UserName"].Should().Be("jgauffin");
            actual.Properties.ContainsKey("DomainName").Should().BeFalse();
        }

        [Fact]
        public void should_also_support_just_userName_in_the_Identity_constructor()
        {

            var actual = CollectionBuilder.CreateForCredentials(new MyIdentity("jgauffin"));

            actual.Properties["UserName"].Should().Be("jgauffin");
            actual.Properties.ContainsKey("Domain").Should().BeFalse();
        }
    }
}
