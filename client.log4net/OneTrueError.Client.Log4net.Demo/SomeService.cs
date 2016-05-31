using System;
using log4net;

namespace OneTrueError.Client.Log4net.Demo
{
    public class SomeService
    {
        private readonly ILog _logger = LogManager.GetLogger(typeof(SomeService));

        public void DoSomeStuff()
        {
            try
            {
                throw new AnnoyingException("Why don't you code better, mofo?");
            }
            catch (Exception ex)
            {
                _logger.Debug("Failed doing some crazy stuff.", ex);
            }
        }
    }
}