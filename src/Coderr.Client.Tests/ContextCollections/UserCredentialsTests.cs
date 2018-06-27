using System.Security.Principal;
using Coderr.Client.ContextCollections;
using FluentAssertions;
using Xunit;

namespace codeRR.Client.Tests.ContextCollections
{
    public class UserCredentialsTests
    {
        [Fact]
        public void userName_domainName_constructor_should_bE_able_To_Assign_values_propertly()
        {

            var sut = (IContextCollection)new UserCredentials("gauffin.com", "jgauffin");

            sut.Properties["UserName"].Should().Be("jgauffin");
            sut.Properties["DomainName"].Should().Be("gauffin.com");
        }

        [Fact]
        public void samAccountName_should_be_divided_into_domain_and_userName()
        {

            var sut = (IContextCollection)new UserCredentials("gauffin.com\\jgauffin");

            sut.Properties["UserName"].Should().Be("jgauffin");
            sut.Properties["DomainName"].Should().Be("gauffin.com");
        }

        [Fact]
        public void identity_should_be_divided_into_domain_and_userName()
        {

            var sut = (IContextCollection)new UserCredentials(new GenericIdentity("gauffin.com\\jgauffin"));

            sut.Properties["UserName"].Should().Be("jgauffin");
            sut.Properties["DomainName"].Should().Be("gauffin.com");
        }

        [Fact]
        public void should_also_support_just_userName_in_the_string_constructor()
        {

            var sut = (IContextCollection)new UserCredentials("jgauffin");

            sut.Properties["UserName"].Should().Be("jgauffin");
            sut.Properties.ContainsKey("DomainName").Should().BeFalse();
        }

        [Fact]
        public void should_also_support_just_userName_in_the_Identity_constructor()
        {

            var sut = (IContextCollection)new UserCredentials(new GenericIdentity("jgauffin"));

            sut.Properties["UserName"].Should().Be("jgauffin");
            sut.Properties.ContainsKey("Domain").Should().BeFalse();
        }
    }
}
