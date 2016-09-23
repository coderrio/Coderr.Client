using System.ComponentModel;

namespace OneTrueError.Client.Wpf.Presenters
{
    public class ErrorMessagePresenter : INotifyPropertyChanged
    {
        private string _exceptionMessage;

        public string ExceptionMessage
        {
            get { return _exceptionMessage; }
            set
            {
                _exceptionMessage = value; 
                OnPropertyChanged("ExceptionMessage");
            }
        }

        public ErrorMessagePresenter(string exceptionMessage)
        {
            ExceptionMessage = exceptionMessage;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
