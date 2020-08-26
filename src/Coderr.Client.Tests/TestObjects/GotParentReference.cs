namespace Coderr.Client.NetStd.Tests.TestObjects
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
