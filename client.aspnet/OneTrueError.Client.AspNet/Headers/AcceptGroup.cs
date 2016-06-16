using System;

namespace OneTrueError.Client.AspNet.Headers
{
    /// <summary>
    ///     A group of content-types which we've received in the HTTP header "Accept" from the browser.
    /// </summary>
    /// <remarks>
    ///     <para>
    ///         Accept headers are grouped together with a quality value. The group with the highest quality should be picked
    ///         first.
    ///     </para>
    ///     <para>If a group contains more than one value you are free to choose the content type that works best for you.</para>
    /// </remarks>
    public class AcceptGroup
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="AcceptGroup" /> class.
        /// </summary>
        /// <param name="types">An array of content-types.</param>
        /// <param name="quality">The quality, 1.0 = max.</param>
        /// <exception cref="System.ArgumentNullException">types</exception>
        public AcceptGroup(string[] types, double quality)
        {
            if (types == null) throw new ArgumentNullException("types");
            ContentTypes = types;
            Quality = quality;
        }

        /// <summary>
        ///     Collection of content types
        /// </summary>
        public string[] ContentTypes { get; private set; }


        /// <summary>
        ///     Quality. 1.0 = max.
        /// </summary>
        public double Quality { get; private set; }
    }
}