using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
// ReSharper disable AutoPropertyCanBeMadeGetOnly.Local

namespace Coderr.Client.Tests.Converts
{
    public class DtoWithPrivateSetter
    {
        public DtoWithPrivateSetter(string testProp)
        {
            TestProp = testProp ?? throw new ArgumentNullException(nameof(testProp));
        }

        protected DtoWithPrivateSetter()
        {
            
        }

        public string TestProp { get; private set; }
    }
}
