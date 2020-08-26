using System;
using Coderr.Client.Config;
using Coderr.Client.NetStd.Tests.Processor.Helpers;
using Coderr.Client.Processor;
using Coderr.Client.Reporters;
using FluentAssertions;
using Xunit;

namespace Coderr.Client.NetStd.Tests.Processor
{
    public class ExceptionProcessorTestsForPartitions
    {
        [Fact]
        public void Should_include_partitions_in_reports()
        {
            var upl = new TestUploader();
            var config = new CoderrConfiguration();
            var ex = new Exception("hello");
            var ctx = new ErrorReporterContext(this, ex);
            config.Uploaders.Register(upl);
            config.AddPartition(x =>
            {
                x.AddPartition("Id", "42");
            });

            var processor = new ExceptionProcessor(config);
            processor.Process(ctx);

            upl.Report.GetCollectionProperty("CoderrData", "ErrPartition.Id").Should().Be("42");
        }
    }
}
