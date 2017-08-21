using System.Collections;
using System.Collections.Generic;

namespace OneTrueError.Client.Tests.TestObjects
{
    public class CustomDictionary : IEnumerable<KeyValuePair<string, object>>
    {
        public CustomDictionary()
        {
            Dictionary = new Dictionary<string, object>();
        }

        public Dictionary<string, object> Dictionary { get; set; }

        public IEnumerator<KeyValuePair<string, object>> GetEnumerator()
        {
            return Dictionary.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}