using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Coderr.Client.Config;
using Coderr.Client.Processor;
using FluentAssertions;
using NSubstitute;
using Xunit;

namespace Coderr.Client.Tests.Processor
{
    public class ExceptionProcessorTestsForExceptions
    {
        [Fact]
        public void Should_unpack_collections_that_are_attached_to_the_exception()
        {
            var upl = new TestUploader();
            var config = new CoderrConfiguration();
            config.Uploaders.Register(upl);
            var json =
                @"{""$type"":""System.Collections.Generic.List`1[[codeRR.Client.Contracts.ContextCollectionDTO, Coderr.Client]], mscorlib"",""$values"":[{""Name"":""SqlCommand"",""Properties"":{""CommandText"":""WaitFor Delay '00:00:05'"",""CommandTimeout"":""3"",""ExecutionTime"":""00:00:03.0313327"",""OtherCommand[0]"":""select * from accounts where id=@id""}},{""Name"":""DbConnection"",""Properties"":{""ConnectionString"":""Data Source=.;Initial Catalog=OneTrueError;Integrated Security=True;Connect Timeout=30;multipleactiveresultsets=true"",""DataSource"":""."",""Database"":""OneTrueError"",""RunTime"":""00:00:03.0681702"",""State"":""Open"",""IsDisposed"":""False"",""ServerVersion"":""12.00.5207""}}]}";
            var ex = new InvalidOperationException();
            ex.Data["ErrCollections"] = json;

            var processor = new ExceptionProcessor(config);
            processor.Process(ex);

            upl.Report.ContextCollections.Should().Contain(x => x.Name == "SqlCommand");
        }

        [Fact]
        public void Should_ignore_reports_that_have_already_been_reported_since_same_frameworks_have_multiple_injection_points_which_would_Report_the_same_exception()
        {
            var upl = new TestUploader();
            var config = new CoderrConfiguration();
            config.Uploaders.Register(upl);
            var ex = new Exception("hello");
            ex.Data[ExceptionProcessor.AlreadyReportedSetting] = 1;

            var processor = new ExceptionProcessor(config);
            processor.Process(ex);

            upl.Report.Should().BeNull("because report should have been ignored");
        }

        [Fact]
        public void Should_be_able_to_filter_Reports()
        {
            var upl = new TestUploader();
            var config = new CoderrConfiguration();
            var filter = Substitute.For<IReportFilter>();
            config.Uploaders.Register(upl);
            config.FilterCollection.Add(filter);
            filter.Invoke(Arg.Do((ReportFilterContext context) => context.CanSubmitReport = false));
            

            var processor = new ExceptionProcessor(config);
            processor.Process(new Exception());

            upl.Report.Should().BeNull("because report should have been filtered away");
        }

        [Fact]
        public void Filter_should_not_affect_non_filtered_reports()
        {
            var upl = new TestUploader();
            var config = new CoderrConfiguration();
            var filter = Substitute.For<IReportFilter>();
            config.Uploaders.Register(upl);
            config.FilterCollection.Add(filter);

            var processor = new ExceptionProcessor(config);
            processor.Process(new Exception());

            upl.Report.Should().NotBeNull("because the report is not affected by the filter");
            filter.ReceivedWithAnyArgs().Invoke(null);
        }

        [Fact]
        public void Should_be_able_to_build_a_report()
        {
            var upl = new TestUploader();
            var config = new CoderrConfiguration();
            var filter = Substitute.For<IReportFilter>();
            config.Uploaders.Register(upl);
            config.FilterCollection.Add(filter);

            var processor = new ExceptionProcessor(config);
            var report = processor.Build(new Exception());

            report.Should().NotBeNull();
        }

        [Fact]
        public void Should_not_filter_BuildReport_as_nothing_is_sent_by_that_method()
        {
            var upl = new TestUploader();
            var config = new CoderrConfiguration();
            var filter = Substitute.For<IReportFilter>();
            config.Uploaders.Register(upl);
            config.FilterCollection.Add(filter);
            filter.Invoke(Arg.Do((ReportFilterContext context) => context.CanSubmitReport = false));

            var processor = new ExceptionProcessor(config);
            var report = processor.Build(new Exception());

            report.Should().NotBeNull();
        }

        [Fact]
        public void BuildReport_should_ignore_reports_that_have_already_been_reported_since_same_frameworks_have_multiple_injection_points_which_would_Report_the_same_exception()
        {
            var upl = new TestUploader();
            var config = new CoderrConfiguration();
            config.Uploaders.Register(upl);
            var ex = new Exception("hello");
            ex.Data[ExceptionProcessor.AlreadyReportedSetting] = 1;

            var processor = new ExceptionProcessor(config);
            processor.Build(ex);

            upl.Report.Should().BeNull("because report should have been ignored");
        }
    }
}
