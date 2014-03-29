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

        public void Debug(string message)
        {
            if(_verbosity != Verbosity.Quiet)
                Console.WriteLine(message);
		}

        public void Info(string message)
        {
            if(_verbosity == Verbosity.Verbose)
                Console.WriteLine(message);
		}

        public void Debug(string message, params object[] arg)
        {
            if(_verbosity != Verbosity.Quiet)
                Console.WriteLine(message, arg);
		}

        public void Error(object message)
        {
            Console.Error.WriteLine(message);
        }
    }
}
