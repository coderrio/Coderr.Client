namespace OneTrueError.Client.AspNet.ErrorPages
{
    /// <summary>
    ///     Checks if JSON or XML is prefered over HTML.
    /// </summary>
    public abstract class PageGeneratorBase : IErrorPageGenerator
    {
        /// <summary>
        ///     Generate
        /// </summary>
        /// <param name="context">information about which page to generate</param>
        public void Generate(PageGeneratorContext context)
        {
            var htmlIndex = GetIndex(context.Request.AcceptTypes, "text/html");
            var jsonIndex = GetIndex(context.Request.AcceptTypes, "/json");
            var xmlIndex = GetIndex(context.Request.AcceptTypes, "application/xml");

            if (jsonIndex < htmlIndex && jsonIndex < xmlIndex)
            {
                GenerateJson(context);
                return;
            }

            if (xmlIndex < htmlIndex)
            {
                GenerateXml(context);
                return;
            }

            GenerateHtml(context);
        }


        /// <summary>
        ///     Generate HTML document
        /// </summary>
        /// <param name="context"></param>
        protected abstract void GenerateHtml(PageGeneratorContext context);


        /// <summary>
        ///     Generate JSON-based error page
        /// </summary>
        /// <param name="context"></param>
        protected virtual void GenerateJson(PageGeneratorContext context)
        {
            var json =
                string.Format(
                    @"{{""error"": {{ ""msg""=""{0}"", ""reportId""=""{1}""}}, hint=""Use the report id when contacting us if you need further assistance."" }}",
                    context.ReporterContext.ErrorMessage, context.ReportId);

            context.SendResponse("application/json", json);
        }

        /// <summary>
        ///     Generate XML
        /// </summary>
        /// <param name="context"></param>
        protected virtual void GenerateXml(PageGeneratorContext context)
        {
            var xml =
                string.Format(
                    @"<Error ReportId=""{0}"" hint=""Use the report id when contacting us if you need further assistance"">{1}</Error>",
                    context.ReportId, context.ReporterContext.ErrorMessage);
            context.SendResponse("application/xml", xml);
        }

        private static int GetIndex(string[] acceptTypes, string content)
        {
            if (acceptTypes == null)
                return int.MaxValue;

            var htmlIndex = 0;
            foreach (var type in acceptTypes)
            {
                if (type.Contains(content))
                    return htmlIndex;

                ++htmlIndex;
            }

            return int.MaxValue;
        }
    }
}