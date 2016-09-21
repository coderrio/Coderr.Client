using OneTrueError.Client.Contracts;

namespace OneTrueError.Client.Wpf
{
    public static class ErrorReportDetailsProvider
    {
        public static ErrorReportDTO DtoReport { get; set; }

        public static string ExceptionMessage { get; set; }
    }
}
