using System;
using System.Linq;
using Coderr.Client.Config;
using Coderr.Client.ContextCollections;
using Coderr.Client.Reporters;
using FluentAssertions;
using NSubstitute;
using Xunit;

namespace Coderr.Client.NetStd.Tests.Config
{
    public class ContextProvidersRegistrarTests
    {
        [Fact]
        public void should_Be_able_to_add_a_contextcollection()
        {
            var collector = Substitute.For<IContextCollectionProvider>();
            var ctx = new ErrorReporterContext(this, new Exception("errror"));

            var sut = new ContextProvidersRegistrar();
            sut.Add(collector);
            sut.Collect(ctx);

            collector.Received().Collect(ctx);
        }

        [Fact]
        public void should_not_use_removed_collectors()
        {
            var collector = Substitute.For<IContextCollectionProvider>();
            var ctx = new ErrorReporterContext(this, new Exception("errror"));
            var sut = new ContextProvidersRegistrar();
            sut.Add(collector);

            sut.Remove(collector.Name);
            sut.Collect(ctx);

            collector.DidNotReceiveWithAnyArgs().Collect(null);
        }

        [Fact]
        public void failing_collectors_should_not_abort_the_processing()
        {
            var collector = Substitute.For<IContextCollectionProvider>();
            var collector2 = Substitute.For<IContextCollectionProvider>();
            var ctx = new ErrorReporterContext(this, new Exception("errror"));
            collector.When(x=>x.Collect(ctx)).Do(x=>throw new InvalidOperationException());

            var sut = new ContextProvidersRegistrar();
            sut.Add(collector);
            sut.Add(collector2);
            sut.Collect(ctx);

            collector2.Received().Collect(ctx);
        }

        [Fact]
        public void failing_collector_should_generate_a_collection_with_an_Error_to_be_Able_To_See_failures_serverSide()
        {
            var collector = Substitute.For<IContextCollectionProvider>();
            var ctx = new ErrorReporterContext(this, new Exception("errror"));
            collector.When(x => x.Collect(ctx)).Do(x => throw new InvalidOperationException());

            var sut = new ContextProvidersRegistrar();
            sut.Add(collector);
            sut.Collect(ctx);

            ctx.ContextCollections.Last().Properties.Should().NotBeEmpty();
        }

        [Fact]
        public void should_be_Able_To_Get_provider_names()
        {
            var collector = Substitute.For<IContextCollectionProvider>();
            collector.Name.Returns("Ada");

            var sut = new ContextProvidersRegistrar();
            sut.Add(collector);
            var actual = sut.GetAddedProviderNames();

            actual.Last().Should().Be("Ada");
        }

        [Fact]
        public void clear_should_remove_all_collectors()
        {
            var collector1 = Substitute.For<IContextCollectionProvider>();
            var collector2 = Substitute.For<IContextCollectionProvider>();
            var ctx = new ErrorReporterContext(this, new Exception("errror"));
            var sut = new ContextProvidersRegistrar();
            sut.Add(collector1);
            sut.Add(collector2);

            sut.Clear();
            sut.Collect(ctx);

            collector1.DidNotReceiveWithAnyArgs().Collect(null);
            collector2.DidNotReceiveWithAnyArgs().Collect(null);
        }


    }
}
