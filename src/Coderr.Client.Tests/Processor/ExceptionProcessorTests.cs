using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using codeRR.Client.Config;
using codeRR.Client.Processor;
using FluentAssertions;
using Xunit;

namespace Coderr.Client.Tests.Processor
{
    public class ExceptionProcessorTests
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
    }
}
