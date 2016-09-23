using System;
using System.Windows;
// ReSharper disable UseStringInterpolation

namespace OneTrueError.Client.Wpf.Utils
{
    public static class ActionWrapper
    {
        public static void SafeActionExecution(Action action)
        {
            try
            {
                action();
            }
            catch (Exception e)
            {
                var message = string.Format("An error occurred with this message:{0}{1}", Environment.NewLine, e.Message);
                MessageBox.Show(message);
            }
        }
    }
}
