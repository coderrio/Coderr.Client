using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OneTrueError.Client.Tests.TestObjects
{
    class GotParentReference
    {
        public string Name { get; set; }
        public GotParentReferenceChild Child { get; set; }
    }

    class GotParentReferenceChild
    {
        public string Title { get; set; }
        public GotParentReference Parent { get; set; }
    }
}
