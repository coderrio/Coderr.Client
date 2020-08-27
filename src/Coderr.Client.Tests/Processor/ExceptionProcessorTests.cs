using System;
using Coderr.Client.Config;
using Coderr.Client.Contracts;
using Coderr.Client.Processor;
using Coderr.Client.Reporters;
using Coderr.Client.Uploaders;
using FluentAssertions;
using NSubstitute;
using Xunit;

namespace Coderr.Client.Tests.Processor
{
    public class ExceptionProcessorTests
    {
        [Fact]
        public void should_use_the_assigned_reportId_factory_to_assign_a_report_id()
        {
            var dispatcher = Substitute.For<IUploadDispatcher>();
            var config = new CoderrConfiguration(dispatcher);
            var ex = new Exception();
            var timesInvoked = 0;
            ReportIdGenerator.Assign(x => { timesInvoked++; return "a"; });

            var sut = new ExceptionProcessor(config);
            sut.Build(ex);
            sut.Build(ex, "Hello world");
            sut.Build(new ErrorReporterContext(this, ex));

            // since tests can run in parallell
            timesInvoked.Should().BeGreaterOrEqualTo(3);
        }

        [Fact]
        public void should_add_the_exception_to_the_report_so_that_it_can_be_uploaded()
        {
            var dispatcher = Substitute.For<IUploadDispatcher>();
            var config = new CoderrConfiguration(dispatcher);
            var ex = new Exception("hello");

            var sut = new ExceptionProcessor(config);
            var actual1 = sut.Build(ex);
            var actual2 = sut.Build(ex, "Hello world");
            var actual3 = sut.Build(new ErrorReporterContext(this, ex));

            actual1.Exception.Message.Should().Be(ex.Message);
            actual2.Exception.Message.Should().Be(ex.Message);
            actual3.Exception.Message.Should().Be(ex.Message);
        }

        [Fact]
        public void should_add_context_data_object_to_report()
        {
            var dispatcher = Substitute.For<IUploadDispatcher>();
            var config = new CoderrConfiguration(dispatcher);
            var ex = new Exception("hello");

            var sut = new ExceptionProcessor(config);
            var actual = sut.Build(ex, "Hello world");

            actual.GetCollectionProperty("ContextData", "Value").Should().Be("Hello world");
        }

        [Fact]
        public void should_add_custom_collection_to_report()
        {
            var dispatcher = Substitute.For<IUploadDispatcher>();
            var config = new CoderrConfiguration(dispatcher);
            var ex = new Exception("hello");
            var collection = "Hello you too".ToContextCollection("MyName");

            var sut = new ExceptionProcessor(config);
            var actual = sut.Build(ex, collection);

            actual.GetCollectionProperty("MyName", "Value").Should().Be("Hello you too");
        }

        [Fact]
        public void should_add_log_entries_from_context()
        {
            var dispatcher = Substitute.For<IUploadDispatcher>();
            var config = new CoderrConfiguration(dispatcher);
            var ex = new Exception("hello");
            var context = new ErrorReporterContext(this, ex)
            {
                LogEntries = new LogEntryDto[] { new LogEntryDto(DateTime.UtcNow, 1, "Hello") }
            };

            var sut = new ExceptionProcessor(config);
            var actual = sut.Build(context);

            actual.LogEntries[0].Message.Should().Be(context.LogEntries[0].Message);
        }

        [Fact]
        public void should_add_all_custom_collection_to_report()
        {
            var dispatcher = Substitute.For<IUploadDispatcher>();
            var config = new CoderrConfiguration(dispatcher);
            var ex = new Exception("hello");
            var collection1 = "Hello you too".ToContextCollection("MyName");
            var collection2 = "Hello you too2".ToContextCollection("MyName2");

            var sut = new ExceptionProcessor(config);
            var actual = sut.Build(ex, new[] { collection1, collection2 });

            actual.GetCollectionProperty("MyName", "Value").Should().Be("Hello you too");
            actual.GetCollectionProperty("MyName2", "Value").Should().Be("Hello you too2");
        }

        [Fact]
        public void should_not_upload_reports_if_the_filters_says_no()
        {
            var dispatcher = Substitute.For<IUploadDispatcher>();
            var config = new CoderrConfiguration(dispatcher);
            var filter = Substitute.For<IReportFilter>();
            var ex = new Exception("hello");
            filter.Invoke(Arg.Do<ReportFilterContext>(context => context.CanSubmitReport = false));
            config.FilterCollection.Add(filter);

            var sut = new ExceptionProcessor(config);
            sut.Process(ex);

            dispatcher.DidNotReceive().Upload(Arg.Any<ErrorReportDTO>());
        }

        [Fact]
        public void should_upload_reports_if_the_filters_says_ok()
        {
            var dispatcher = Substitute.For<IUploadDispatcher>();
            var config = new CoderrConfiguration(dispatcher);
            var filter = Substitute.For<IReportFilter>();
            var ex = new Exception("hello");
            filter.Invoke(Arg.Do<ReportFilterContext>(context => context.CanSubmitReport = true));
            config.FilterCollection.Add(filter);

            var sut = new ExceptionProcessor(config);
            sut.Process(ex);

            dispatcher.Received().Upload(Arg.Any<ErrorReportDTO>());
        }

        [Fact]
        public void should_upload_reports_if_there_are_no_filters()
        {
            var dispatcher = Substitute.For<IUploadDispatcher>();
            var config = new CoderrConfiguration(dispatcher);
            var ex = new Exception("hello");

            var sut = new ExceptionProcessor(config);
            sut.Process(ex);

            dispatcher.Received().Upload(Arg.Any<ErrorReportDTO>());
        }
    }
}
