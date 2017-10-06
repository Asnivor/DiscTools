using DiscTools.Inspection;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace DiscTools
{
    public class CueHandler
    {
        public Dictionary<int, FileEntry> CueData { get; set; }

        /// <summary>
        /// checks whether valid detection has occured
        /// if not it will attempt to create a new cuefile without non-BINARY entries and parse this
        /// </summary>
        /// <param name="res"></param>
        /// <param name="cuePath"></param>
        /// <param name="intensive"></param>
        /// <returns></returns>
        public static DiscInspector CueRoutine(DiscInspector res, string cuePath, bool intensive)
        {
            if (res == null || res.DetectedDiscType == DetectedDiscType.UnknownFormat || res.DetectedDiscType == DetectedDiscType.UnknownCDFS)
            {
                // try again after calling the cue parser
                string newCue = CueHandler.ParseCue(cuePath);

                // if newcue is the same as cuepath return straight away
                if (newCue == cuePath)
                    return res;

                // run interrogator again with the newly generated cue (sans non-binary tracks)
                var inter = new Interrogator(newCue, intensive);
                res = inter.Start();

                // check for null
                if (res == null)
                    return null;

                // set res cue back
                res.CuePath = cuePath;

                // delete newCue if it is different from cuePath (i.e a new cue file HAS been generated)
                if (File.Exists(newCue) && newCue != cuePath)
                    File.Delete(newCue);
            }

            return res;
        }

        /// <summary>
        /// cue parsing and creation
        /// </summary>
        /// <param name="cuePath"></param>
        /// <returns></returns>
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
