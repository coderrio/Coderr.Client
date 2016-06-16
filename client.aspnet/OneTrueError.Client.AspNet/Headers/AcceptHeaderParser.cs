using System;
using System.Collections.Generic;

namespace OneTrueError.Client.AspNet.Headers
{
    /// <summary>
    ///     Small utility class used to parse the "Accept" HTTP header.
    /// </summary>
    /// <remarks>We need to check the accept header to determine which format we should return the response in.</remarks>
    public class AcceptHeaderParser
    {
        /// <summary>
        ///     Parses the header.
        /// </summary>
        /// <param name="value">Header value (from a HTTP request).</param>
        public void Parse(string value)
        {
            if (value == null) throw new ArgumentNullException("value");
            //text/html,application/xhtml+xml,application/xml;q=0.9,*/*;q=0.8
            var tokens = value.Split(new[] {','}, StringSplitOptions.RemoveEmptyEntries);

            var groups = new List<AcceptGroup>();
            var types = new List<string>();
            foreach (var token in tokens)
            {
                if (token.Contains(";"))
                {
                    var parts = token.Split(';');
                    types.Add(parts[0]);
                    groups.Add(new AcceptGroup(types.ToArray(), double.Parse(parts[1])));
                    types.Clear();
                }
                else
                {
                    types.Add(token);
                }
            }
        }
    }
}