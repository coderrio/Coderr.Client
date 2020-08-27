using System.Security.Principal;

namespace Coderr.Client.Tests.Config.ContextCollections.TestObjects
{
    public class MyPrincipal : IPrincipal
    {
        public MyPrincipal(string identityNAme)
        {
            Identity=new MyIdentity(identityNAme);
        }

        public MyPrincipal(IIdentity identity)
        {
            Identity = identity;
        }

        public bool IsInRole(string role)
        {
            return false;
        }

        public IIdentity Identity { get; set; }
    }
}