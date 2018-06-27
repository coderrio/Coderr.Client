using System;

namespace Coderr.Client.Processor
{
    /// <summary>
    ///     Shortens GUIDs using Base64 and convert the result to a URL friendly string.
    /// </summary>
    public class ShortGuid
    {
        /// <summary>
        ///     Get a short GUID as a Guid object.
        /// </summary>
        /// <param name="shortGuid">The short GUID.</param>
        /// <returns>Restored GUID</returns>
        /// <exception cref="System.ArgumentNullException">shortGuid</exception>
        /// <exception cref="System.FormatException">Input string was not in a correct format.</exception>
        public static Guid Decode(string shortGuid)
        {
            if (shortGuid == null)
                throw new ArgumentNullException("shortGuid");
            if (shortGuid.Length != 22)
                throw new FormatException("Input string was not in a correct format.");

            return new Guid(Convert.FromBase64String
                (shortGuid.Replace("_", "/").Replace("-", "+") + "=="));
        }

        /// <summary>
        ///     Creates a 22-character case-sensitive short GUID.
        /// </summary>
        /// <param name="guid">The GUID.</param>
        /// <returns>Compacted representation of the GUID</returns>
        /// <exception cref="System.ArgumentNullException">guid</exception>
        public static string Encode(Guid guid)
        {
            if (guid == null)
                throw new ArgumentNullException("guid");

            return Convert.ToBase64String(guid.ToByteArray())
                .Substring(0, 22)
                .Replace("/", "_")
                .Replace("+", "-");
        }
    }
}