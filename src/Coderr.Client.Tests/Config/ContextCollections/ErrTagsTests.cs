using System;
using Coderr.Client.ContextCollections;
using FluentAssertions;
using Xunit;

namespace Coderr.Client.NetStd.Tests.Config.ContextCollections
{
    public class ErrTagsTests
    {
        [Fact]
        public void should_include_tags_as_a_comma_separated_list()
        {
            
            var actual = CollectionBuilder.CreateTags("csharp", "ado.net");

            actual.Properties["ErrTags"].Should().Be("csharp,ado.net");
        }

        [Fact]
        public void should_not_be_able_to_Set_an_Empty_array_as_that_indicates_an_logical_Error_somewhere_in_the_code()
        {

            Action actual = () => CollectionBuilder.CreateTags();

            actual.ShouldThrow<ArgumentOutOfRangeException>();
        }

        [Fact]
        public void should_not_be_able_to_Set_null_as_that_indicates_an_logical_Error_somewhere_in_the_code()
        {

            Action actual = () => CollectionBuilder.CreateTags(null);

            actual.ShouldThrow<ArgumentNullException>();
        }
    }
}
