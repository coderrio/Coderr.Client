//using System;
//using System.IO;
//using System.Threading.Tasks;
//using Coderr.Client.ContextCollections;
//using FluentAssertions;
//using OneTrueError.IntegrationTests.Tools.Client;
//using Xunit;

//namespace Coderr.Client.NetStd.IntegrationTests
//{
//    public class ContextCollectionTests
//    {
//        private readonly ServerClient _client = new ServerClient();

//        public ContextCollectionTests()
//        {
//            Err.Configuration.Credentials(ServerKeys.Url,
//                ServerKeys.AppAppKey,
//                ServerKeys.AppSharedSecret);
//        }

//        [Fact]
//        public async Task should_tag_incident_when_ErrTags_is_added_to_a_collection()
//        {
//            var methodName = nameof(should_tag_incident_when_ErrTags_is_added_to_a_collection);
//            var id = Guid.NewGuid().ToString();
            
//            try
//            {
//                throw new InvalidDataException(methodName + " " + id);
//            }
//            catch (Exception ex)
//            {
//                Err.Report(ex, new {ErrTags = "important,data"});
//            }

//            var incident = await _client.GetIncident(methodName, id);
//            incident.Tags.Should().Contain("important");
//            incident.Tags.Should().Contain("data");
//        }

//        [Fact]
//        public async Task should_tag_incident_when_collectionBuilder_is_used()
//        {
//            var methodName = nameof(should_tag_incident_when_collectionBuilder_is_used);
//            var id = Guid.NewGuid().ToString();

//            try
//            {
//                throw new InvalidDataException(methodName + " " + id);
//            }
//            catch (Exception ex)
//            {
//                var collection = CollectionBuilder.CreateTags("max", "overdrive");
//                Err.Report(ex, collection);
//            }

//            var incident = await _client.GetIncident(methodName, id);
//            incident.Tags.Should().Contain("max");
//            incident.Tags.Should().Contain("overdrive");
//        }


//    }
//}
