using System;
using Coderr.Client.Processor;
using FluentAssertions;
using NCrunch.Framework;
using Xunit;

namespace Coderr.Client.Tests.Processor
{
    public class ReportIdGeneratorTests
    {
        [Fact, ExclusivelyUses("ReportIdGenerator")]
        public void should_be_able_to_generate_an_id_per_default()
        {
            var sut = new ReportIdGenerator();

            var actual = sut.GenerateImp(new Exception());

            actual.Should().NotBeNullOrWhiteSpace();
        }

        [Fact, ExclusivelyUses("ReportIdGenerator")]
        public void should_use_the_new_factory_when_configured()
        {
            var sut = new ReportIdGenerator();

            var actual = sut.GenerateImp(new Exception());

            actual.Should().NotBeNullOrWhiteSpace();
        }
    }
}
