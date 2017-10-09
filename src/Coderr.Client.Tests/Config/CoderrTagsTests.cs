using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentAssertions;
using codeRR.Client.ContextCollections;
using Xunit;

namespace codeRR.Client.Tests.Config
{
    public class CoderrTagsTests
    {
        [Fact]
        public void should_have_the_name_ErrTags()
        {
            

            var sut = new CoderrTags(new[]{"a", "b"});

            ((IContextCollection) sut).CollectionName.Should().Be("CoderrTags");
        }

        [Fact]
        public void should_include_the_given_tags()
        {


            var sut = new CoderrTags(new[] { "a", "b" });
            var actual = ((IContextCollection) sut).Properties;

            actual["ErrTags"].Should().Be("a;b");
        }
    }
}
