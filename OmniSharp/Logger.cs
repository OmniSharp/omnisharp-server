using System;

namespace OmniSharp
{
    public enum Verbosity
    {
        Quiet,
        Debug,
        Verbose
    }

    public class Logger
    {
        private Verbosity _verbosity;
		public Verbosity Verbosity { get { return _verbosity; } }

        public Logger(Verbosity verbosity)
        {
            _verbosity = verbosity;
        }

        public void Debug(string format, params object[] arg)
        {
            if(_verbosity != Verbosity.Quiet)
                Console.WriteLine(format, arg);
		}

        public void Error(object message)
        {
            Console.Error.WriteLine(message);
        }
    }
}
