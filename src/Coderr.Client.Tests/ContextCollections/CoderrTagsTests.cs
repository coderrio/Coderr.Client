using System;
using FluentAssertions;
using codeRR.Client.ContextCollections;
using Xunit;

namespace codeRR.Client.Tests.ContextCollections
{
    public class CoderrTagsTests
    {
        [Fact]
        public void should_include_tags_as_a_semicolon_separated_list()
        {
            
            var sut = (IContextCollection)new CoderrTags(new [] {"csharp", "ado.net"});

            sut.Properties["ErrTags"].Should().Be("csharp;ado.net");
        }

        [Fact]
        public void should_not_be_able_to_Set_an_Empty_array_as_that_indicates_an_logical_Error_somewhere_in_the_code()
        {

            Action sut = () => new CoderrTags(new string[0]);

            sut.ShouldThrow<ArgumentOutOfRangeException>();
        }

        [Fact]
        public void should_not_be_able_to_Set_null_as_that_indicates_an_logical_Error_somewhere_in_the_code()
        {

            Action sut = () => new CoderrTags(null);

            sut.ShouldThrow<ArgumentNullException>();
        }
    }
}
