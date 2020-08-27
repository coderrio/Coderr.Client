using System;
using Coderr.Client.Reporters;
using FluentAssertions;
using Xunit;

namespace Coderr.Client.Tests.Reporters
{
    public class ErrorReporterContextTests
    {
        [Fact]
        public void should_assign_the_source_so_that_we_can_See_where_the_exception_Was_detected()
        {

            var sut = new ErrorReporterContext(this, new Exception());

            sut.Reporter.Should().Be(this);
        }

        [Fact]
        public void should_assign_the_exception_so_that_we_have_something_to_work_with()
        {
            var ex = new Exception();

            var sut = new ErrorReporterContext(this, ex);

            sut.Exception.Should().Be(ex);
        }

        [Fact]
        public void should_initialize_the_colleciton_to_avoid_confusion_and_nullreference()
        {
            var ex = new Exception();

            var sut = new ErrorReporterContext(this, ex);

            sut.ContextCollections.Should().BeEmpty();
        }

        [Fact]
        public void exception_should_be_required_since_nothing_works_otherwise()
        {

            Action actual = () => new ErrorReporterContext(this, null);

            actual.Should().Throw<ArgumentNullException>();
        }
    }
}
