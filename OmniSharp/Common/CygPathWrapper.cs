using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace OmniSharp.Common
{
    public class CygPathWrapper
    {
        private static readonly IDictionary<bool, Process> CygpathProcesses 
            = new Dictionary<bool, Process>();
        private static readonly IDictionary<bool, IDictionary<String, String>> CygpathCache 
            = new Dictionary<bool, IDictionary<String, String>>
        {
            {true, new Dictionary<String,String>()},
            {false, new Dictionary<String,String>()},
        };

        public static string GetCygpath(string path, bool toUnix)
        {
            string convertedPath;
            if (!CygpathCache[toUnix].TryGetValue(path, out convertedPath))
            {
                lock (CygpathProcesses)
                {
                    Process process;
                    if (!CygpathProcesses.TryGetValue(toUnix, out process) || process.HasExited)
                    {
                        var processStartInfo = new ProcessStartInfo("cygpath", "-f - " + (toUnix ? "-u" : "-w"))
                        {
                            CreateNoWindow = true,
                            UseShellExecute = false,
                            RedirectStandardInput = true,
                            RedirectStandardOutput = true,
                        };

                        process = CygpathProcesses[toUnix] = Process.Start(processStartInfo);
                    }

                    process.StandardInput.WriteLine(path);
                    convertedPath = process.StandardOutput.ReadLine();
                }
            }
            return convertedPath;
        }
    }
}
