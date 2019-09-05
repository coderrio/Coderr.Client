namespace Coderr.Client.ContextCollections
{
    /// <summary>
    ///     Name of properties in the "CoderrData" collection which is used by the backend to analyze data.
    /// </summary>
    public class CoderrCollectionProperties
    {
        /// <summary>
        ///     Bypass the lookup algorithm in the backend and use this value to identify unique errors.
        /// </summary>
        public const string HashSource = "HashSource";

        /// <summary>
        ///     Add these tags to the incident (value should be a comma separated list)
        /// </summary>
        public const string Tags = "ErrTags";

        /// <summary>
        ///     Used when identifying related incidents
        /// </summary>
        public const string CorrelationId = "CorrelationId";

        /// <summary>
        ///     These collections should be displayed directly on the front page (above the stacktrace)
        /// </summary>
        /// <remarks>
        ///     <para>
        ///         Value should be a comma separated list with context collection names.
        ///     </para>
        /// </remarks>
        public const string HighlightCollection = "HighlightCollections";

        /// <summary>
        ///     These collections should be displayed directly on the front page (above the stacktrace)
        /// </summary>
        /// <remarks>
        ///     <para>
        ///         Value should be a comma separated list with "ContextCollectionName.PropertyName".
        ///     </para>
        /// </remarks>
        public const string HighlightProperties = "HighlightProps";

        /// <summary>
        ///     Display this property directly in the quick facts box in the Coderr UI
        /// </summary>
        /// <remarks>
        ///     <para>
        ///         String replace "{Name}" in the constant with your own property name.
        ///     </para>
        /// </remarks>
        public const string QuickFact = "QuickFact.{Name}";

    }
}