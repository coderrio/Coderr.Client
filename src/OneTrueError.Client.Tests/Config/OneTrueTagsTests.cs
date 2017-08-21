using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentAssertions;
using OneTrueError.Client.ContextCollections;
using Xunit;

namespace OneTrueError.Client.Tests.Config
{
    public class OneTrueTagsTests
    {
        [Fact]
        public void should_have_the_name_OneTrueTags()
        {
            

            var sut = new OneTrueTags(new[]{"a", "b"});

            ((IContextCollection) sut).CollectionName.Should().Be("OneTrueTags");
        }

        [Fact]
        public void should_include_the_given_tags()
        {


            var sut = new OneTrueTags(new[] { "a", "b" });
            var actual = ((IContextCollection) sut).Properties;

            actual["Tags"].Should().Be("a;b");
        }
    }
}
