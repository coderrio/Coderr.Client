using System.Security.Principal;

namespace Coderr.Client.NetStd.Tests.Config.ContextCollections.TestObjects
{
    public class MyIdentity : IIdentity
    {
        public MyIdentity(string userName)
        {
            Name = userName;
        }


        public string AuthenticationType { get; set; }
        public bool IsAuthenticated { get; set; }
        public string Name { get; set; }
    }
}