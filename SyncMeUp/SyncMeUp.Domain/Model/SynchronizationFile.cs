using System;
using Newtonsoft.Json;

namespace SyncMeUp.Domain.Model
{
    public class SynchronizationFile
    {
        public string FileName { get; set; }
        [JsonIgnore]
        public SynchronizationFolder Parent { get; set; }
        public DateTime LastChangeDate { get; set; }
        public ulong SizeInBytes { get; set; }
        public byte[] ContentHash { get; set; }

        public override string ToString()
        {
            double kibi = 1024;
            double mibi = kibi * kibi;
            string size;
            if (SizeInBytes > 1 * mibi)
            {
                size = Math.Round(SizeInBytes / mibi, 1).ToString("0.#") + "MiB";
            }
            else if (SizeInBytes > 1 * kibi)
            {
                size = Math.Round(SizeInBytes / kibi, 1).ToString("0.#") + "KiB";
            }
            else
            {
                size = $"{SizeInBytes}B";
            }
            return $"{FileName} - {size}";
        }
    }
}