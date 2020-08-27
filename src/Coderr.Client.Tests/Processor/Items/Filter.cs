using Coderr.Client.Processor;

namespace Coderr.Client.Tests.Processor.Items
{
    class Filter : IReportFilter
    {
        public bool Answer { get; set; }

        public void Invoke(ReportFilterContext context)
        {
            context.CanSubmitReport = Answer;
        }
    }
}
