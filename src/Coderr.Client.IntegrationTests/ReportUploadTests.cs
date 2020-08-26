//using System;
//using System.Threading.Tasks;
//using FluentAssertions;
//using OneTrueError.IntegrationTests.Tools.Client;
//using Xunit;

//namespace Coderr.Client.NetStd.IntegrationTests
//{
//    public class ReportUploadTests
//    {
//        private readonly ServerClient _client = new ServerClient();

//        public ReportUploadTests()
//        {
//            Err.Configuration.Credentials(ServerKeys.Url,
//                ServerKeys.AppAppKey,
//                ServerKeys.AppSharedSecret);
//        }


//        [Fact]
//        public async Task Upload_a_report_with_anonymous_object()
//        {
//            var methodName = nameof(Upload_a_report_with_anonymous_object);
//            var uid = Guid.NewGuid().ToString();

//            try
//            {
//                throw new InvalidOperationException(methodName + " " + uid);
//            }
//            catch (Exception ex)
//            {
//                Err.Report(ex, new { Mark = true });
//            }

//            var actual = await _client.GetReport(methodName, uid);
//            actual.CollectionProperty("ContextData", "Mark").Should().Be("True");
//        }

//        [Fact]
//        public async Task Upload_a_report_without_extra_data()
//        {
//            var methodName = nameof(Upload_a_report_without_extra_data);
//            var uid = Guid.NewGuid().ToString();

//            try
//            {
//                throw new InvalidOperationException(methodName + " " + uid);
//            }
//            catch (Exception ex)
//            {
//                Err.Report(ex);
//            }

//            var actual = await _client.CheckIfReportExists(methodName, uid);
//            actual.Should().BeTrue();
//        }
//    }
//}
