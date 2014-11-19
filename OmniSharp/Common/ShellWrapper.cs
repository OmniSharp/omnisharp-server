using System;
using System.Diagnostics;

namespace OmniSharp
{
    public static class ShellWrapper
    {
        public static string GetShellOuput(string command, string arguments = null)
        {
            var startInfo = new ProcessStartInfo();
            var p = new Process();

            startInfo.CreateNoWindow = true;
            startInfo.RedirectStandardOutput = true;

            startInfo.UseShellExecute = false;
            startInfo.Arguments = arguments;
            startInfo.FileName = command;
            var path = Environment.GetEnvironmentVariable("PATH");
            Console.WriteLine(path);
            startInfo.EnvironmentVariables["PATH"] = path;
            p.StartInfo = startInfo;
            p.Start();
            var stdout = p.StandardOutput.ReadToEnd();
            p.WaitForExit();
            return stdout;
        }
    }
}

