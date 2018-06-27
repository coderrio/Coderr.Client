using System;
using System.Collections.Generic;
using System.Linq;
using Coderr.Client.Contracts;
using Coderr.Client.Reporters;
using FluentAssertions;
using Newtonsoft.Json;
using Xunit;

namespace Coderr.Client.Tests.Reporters
{
    public class ErrorReportContextTests
    {
        [Fact]
        public void Should_be_able_to_move_collection()
        {
            var ex = new Exception();
            var col = new ContextCollectionDTO("MyName", new Dictionary<string, string>() {{"Key", "Value"}});
            ex.Data["ErrCollection.MyName"] = JsonConvert.SerializeObject(col);

            var sut = new ErrorReporterContext(this, ex);

            var actual = sut.GetCollectionProperty("MyName", "Key");
            actual.Should().Be("Value");
        }

        [Fact]
        public void Should_be_able_to_move_collections()
        {
            var ex = new Exception();
            var col1 = new ContextCollectionDTO("MyName", new Dictionary<string, string>() { { "Key", "Value" } });
            var col2 = new ContextCollectionDTO("MyName2", new Dictionary<string, string>() { { "Key2", "Value2" } });
            ex.Data["ErrCollections"] = JsonConvert.SerializeObject(new[]{col1,col2});

            var sut = new ErrorReporterContext(this, ex);

            var actual1 = sut.GetCollectionProperty("MyName", "Key");
            var actual2 = sut.GetCollectionProperty("MyName2", "Key2");
            actual1.Should().Be("Value");
            actual2.Should().Be("Value2");
        }

        [Fact]
        public void Should_ignore_invalid_json_in_collections_alternative()
        {
            var ex = new Exception();
            ex.Data["ErrCollections"] = "MEX";

            var sut = new ErrorReporterContext(this, ex);

            var actual = sut.GetCollectionProperty("ErrCollections", "JSON");
            actual.Should().Be("MEX");
        }

        [Fact]
        public void Should_ignore_invalid_json_in_collection_alternative()
        {
            var ex = new Exception();
            ex.Data["ErrCollection.MyName2"] = "MEX";

            var sut = new ErrorReporterContext(this, ex);

            var actual = sut.GetCollectionProperty("MyName2", "JSON");
            actual.Should().Be("MEX");
        }
    }
}
