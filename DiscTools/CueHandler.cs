using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace DiscTools
{
    public class CueHandler
    {
        public Dictionary<int, FileEntry> CueData { get; set; }

        public static string ParseCue(string cuePath)
        {
            if (!File.Exists(cuePath))
                return cuePath;

            if (!cuePath.ToLower().EndsWith(".cue"))
                return cuePath;

            string newCueData = string.Empty;

            // load cue into memory
            string ca = File.ReadAllText(cuePath);

            // split by FILE
            string[] split = ca.Split(new string[] { "FILE " }, StringSplitOptions.None);

            if (split.Length < 2)
                split = ca.Split(new string[] { "File " }, StringSplitOptions.None);

            if (split.Length < 2)
                split = ca.Split(new string[] { "file " }, StringSplitOptions.None);

            // begin iteration - we only want BINARY entries
            for (int i = 0; i < split.Length; i++)
            {
                if (split[i].ToUpper().Contains(" BINARY"))
                {
                    // we want this
                    newCueData += "FILE " + split[i];
                }
            }

            // write the new cuefile
            string cueFolder = Path.GetDirectoryName(cuePath);
            string oldCueNoExt = Path.GetFileNameWithoutExtension(cuePath);
            string newCue = Path.Combine(cueFolder, oldCueNoExt + Guid.NewGuid().ToString() + ".cue");

            File.WriteAllText(newCue, newCueData);

            return newCue;
        }
    }

    public class FileEntry
    {
        public string FileName { get; set; }
        public string FileType { get; set; }
        public Dictionary<int, string> Track { get; set; }
        public Dictionary<string, string> Indexes { get; set; }
    }
}
