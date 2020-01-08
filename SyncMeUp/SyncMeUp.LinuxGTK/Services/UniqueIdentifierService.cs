using System.Diagnostics;
using SyncMeUp.Domain.Services;

namespace SyncMeUp.LinuxGTK.Services
{
    public class UniqueIdentifierService : IUniqueIdentifierService
    {
        public string GetDeviceUniqueId()
        {
            var dmidecode = new ProcessStartInfo()
            {
                CreateNoWindow = true,
                FileName = "gnome-terminal",
                Arguments = "-x sh -c 'dmidecode -t 4 | grep ID'",
                RedirectStandardOutput = true,
            };
            var dmidecodeProcess =System.Diagnostics.Process.Start(dmidecode);
            var cpuid = dmidecodeProcess.StandardOutput.ReadLine();
            return cpuid; //TODO does this work?
        }
    }
}