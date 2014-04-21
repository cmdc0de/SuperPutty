using System.Diagnostics;
using System.IO;
using SuperPutty.Manager;

namespace SuperPutty.Scp
{
    class ScpUtils
    {
        public static string GetHomeDirectory(SessionData sessionData)
        {
            var plink = SuperPuTTY.Settings.PlinkExe;

            if(!string.IsNullOrEmpty(plink) && File.Exists(plink))
            {
                var info = new ProcessStartInfo(plink);
                info.Arguments = string.Format("-load {0} pwd", sessionData.PuttySession);
                info.UseShellExecute = false;
                info.RedirectStandardOutput = true;
                info.RedirectStandardError = true;
                info.RedirectStandardInput = true;
                info.CreateNoWindow = true;

                using (var process = Process.Start(info))
                {
                    process.WaitForExit();
                    var directory = process.StandardOutput.ReadToEnd().Trim();
                    return directory;
                }
            }

            return (sessionData.Username == "root")
                ? "/root"
                : string.Format("/home/{0}", sessionData.Username);
        }
    }
}
