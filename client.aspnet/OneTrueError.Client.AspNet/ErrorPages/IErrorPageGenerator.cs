namespace OneTrueError.Client.AspNet.ErrorPages
{
    /// <summary>
    /// Used to generate error pages.
    /// </summary>
    public interface IErrorPageGenerator
    {
        /// <summary>
        /// Generate
        /// </summary>
        /// <param name="context">information about which page to generate</param>
        void Generate(PageGeneratorContext context);
    }
}
