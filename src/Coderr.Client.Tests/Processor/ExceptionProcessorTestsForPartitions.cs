using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Coderr.Client.Config;
using Coderr.Client.Processor;
using Coderr.Client.Reporters;
using FluentAssertions;
using Xunit;

namespace Coderr.Client.Tests.Processor
{
    public class ExceptionProcessorTestsForPartitions
    {
        [Fact]
        public void Should_ignore_reports_that_have_already_been_reported_since_same_frameworks_have_multiple_injection_points_which_would_Report_the_same_exception()
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

            upl.Report.GetCollectionProperty("ErrPartitions", "Id").Should().Be("42");
        }
    }
}
