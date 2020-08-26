using Coderr.Client.ContextCollections;
using FluentAssertions;
using Xunit;

namespace Coderr.Client.NetStd.Tests.Config.ContextCollections
{
    public class UserSuppliedInformationTests
    {
        [Fact]
        public void should_be_possible_to_leave_both_blank_since_both_are_optiona()
        {

            var actual = CollectionBuilder.Feedback(null, null);

            actual.Properties.Should().BeEmpty();
        }

        [Fact]
        public void should_add_email_to_the_collection_if_specified()
        {

            var actual = CollectionBuilder.Feedback("email@somewhere.com", null);

            actual.Properties["EmailAddress"].Should().Be("email@somewhere.com");
        }

        [Fact]
        public void should_add_feedback_to_the_collection_if_specified()
        {

            var actual = CollectionBuilder.Feedback(null, "Hello world");

            actual.Properties["Description"].Should().Be("Hello world");
        }

        [Fact]
        public void should_add_both_to_the_collection_if_specified()
        {

            var actual = CollectionBuilder.Feedback("email@somewhere.com", "Hello world");

            actual.Properties["EmailAddress"].Should().Be("email@somewhere.com");
            actual.Properties["Description"].Should().Be("Hello world");
        }
    }
}
