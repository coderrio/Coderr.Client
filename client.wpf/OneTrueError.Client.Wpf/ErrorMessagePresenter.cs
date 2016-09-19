namespace OneTrueError.Client.Wpf
{
    public class ErrorMessagePresenter
    {
        public string ExceptionMessage { get; set; }

        public ErrorMessagePresenter(string exceptionMessage)
        {
            ExceptionMessage = exceptionMessage;
        }
    }
}
