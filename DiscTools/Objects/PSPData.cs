using DiscTools.Inspection.Statics.SonyMethods;
using System;
using System.Collections.Generic;
using System.Text;

namespace DiscTools.Objects
{
    /// <summary>
    /// PSP specific data obtained from PARAM.SFO
    /// </summary>
    public class PSPData
    {
        /// <summary>
        /// Application or Patch Version
        /// </summary>
        public string APP_VER { get; set; }
        /// <summary>
        /// Various parameter
        /// </summary>
        public int ATTRIBUTE { get; set; }
        /// <summary>
        /// Game is auto-launched at bootup
        /// </summary>
        public bool? BOOTABLE { get; set; }
        /// <summary>
        /// Category of PSF
        /// </summary>
        public PSPCategory CATEGORY { get; set; }
        /// <summary>
        /// Product number of the game (serial number)
        /// </summary>
        public string DISC_ID { get; set; }
        /// <summary>
        /// Which disc (out of DISC_TOTAL) is this? (Counts from 1.)
        /// </summary>
        public int? DISC_NUMBER { get; set; }
        /// <summary>
        /// Total number of UMD discs for this game.
        /// </summary>
        public int? DISC_TOTAL { get; set; }
        /// <summary>
        /// Version of the game(?), e.g. "1.00"
        /// </summary>
        public string DISC_VERSION { get; set; }
        /// <summary>
        /// Unknown
        /// </summary>
        public string DRIVER_PATH { get; set; }
        /// <summary>
        /// Unknown
        /// </summary>
        public int? HRKGMP_VER { get; set; }
        /// <summary>
        /// License information
        /// </summary>
        public string LICENSE { get; set; }
        /// <summary>
        /// add extra RAM for eboot (Not for PSP-1000)
        /// </summary>
        public int? MEMSIZE { get; set; }
        /// <summary>
        /// Language of the game.
        /// </summary>
        public string LANGUAGE { get; set; }
        /// <summary>
        /// Parental Lock Level
        /// </summary> <remarks>Minimum parental control level needed to access this file (1-11, 1=general audience, 5=12 years, 7=15 years, 9=18 years)</remarks>
        public int? PARENTAL_LEVEL { get; set; }
        /// <summary>
        /// Used on PBOOT.PBP (Update)
        /// </summary>
        public string PBOOT_TITLE { get; set; }
        /// <summary>
        /// Version of PSP system software required to run the game
        /// </summary>
        public string PSP_SYSTEM_VER { get; set; }
        /// <summary>
        /// Bitmask of allowed regions
        /// </summary> <remarks>Currently not understood</remarks>
        public int? REGION { get; set; }
        /// <summary>
        /// Game Title (Default language)
        /// </summary>
        public string TITLE { get; set; }

        // Localized language titles

        /// <summary>
        /// Japanease
        /// </summary>
        public string TITLE_0 { get; set; }
        /// <summary>
        /// French
        /// </summary>
        public string TITLE_2 { get; set; }
        /// <summary>
        /// Spanish
        /// </summary>
        public string TITLE_3 { get; set; }
        /// <summary>
        /// German
        /// </summary>
        public string TITLE_4 { get; set; }
        /// <summary>
        /// Italian
        /// </summary>
        public string TITLE_5 { get; set; }
        /// <summary>
        /// Dutch
        /// </summary>
        public string TITLE_6 { get; set; }
        /// <summary>
        /// Portuguese
        /// </summary>
        public string TITLE_7 { get; set; }
        /// <summary>
        /// Russian
        /// </summary>
        public string TITLE_8 { get; set; }
        /// <summary>
        /// unknown language
        /// </summary>
        public string TITLE_9 { get; set; }
        /// <summary>
        /// unknown language
        /// </summary>
        public string TITLE_10 { get; set; }
        /// <summary>
        /// unknown language
        /// </summary>
        public string TITLE_11 { get; set; }
        /// <summary>
        /// Used by the firmware updater program to denote the version it upgrades the firmware to. Category MG, on PSP_GAME: SYSDIR/UPDATE/PARAM.SFO if not empty
        /// </summary>
        public string UPDATER_VER { get; set; }
        /// <summary>
        /// unknown
        /// </summary>
        public bool? USE_USB { get; set; }

        

        public static void ParsePSPData(DiscInspector di, SFO sfo)
        {
            PSPData data = new PSPData();

            foreach (var p in sfo.Parameters)
            {
                switch (p.Name)
                {
                    case "APP_VER": data.APP_VER = p.ValueString; break;
                    case "ATTRIBUTE": data.ATTRIBUTE = p.ValueInt.GetValueOrDefault(); break;
                    case "BOOTABLE": data.BOOTABLE = p.ValueInt.GetValueOrDefault() != 0; break;
                    case "CATEGORY": data.CATEGORY = GetPSPCategory(p.ValueString); break;
                    case "DISC_ID": data.DISC_ID = FormatDiscID(p.ValueString); break;
                    case "DISC_NUMBER": data.DISC_NUMBER = p.ValueInt.GetValueOrDefault(); break;
                    case "DISC_TOTAL": data.DISC_TOTAL = p.ValueInt.GetValueOrDefault(); break;
                    case "DISC_VERSION": data.DISC_VERSION = p.ValueString; break;
                    case "DRIVER_PATH": data.DRIVER_PATH = p.ValueString; break;
                    case "HRKGMP_VER": data.HRKGMP_VER = p.ValueInt.GetValueOrDefault(); break;
                    case "LICENSE": data.LICENSE = p.ValueString; break;
                    case "MEMSIZE": data.MEMSIZE = p.ValueInt.GetValueOrDefault(); break;
                    case "LANGUAGE": data.LANGUAGE = p.ValueString; break;
                    case "PARENTAL_LEVEL": data.PARENTAL_LEVEL = p.ValueInt.GetValueOrDefault(); break;
                    case "PBOOT_TITLE": data.PBOOT_TITLE = p.ValueString; break;
                    case "PSP_SYSTEM_VER": data.PSP_SYSTEM_VER = p.ValueString; break;
                    case "REGION": data.REGION = p.ValueInt.GetValueOrDefault(); break;
                    case "TITLE": data.TITLE = p.ValueString; break;
                    case "TITLE_0": data.TITLE_0 = p.ValueString; break;
                    case "TITLE_10": data.TITLE_10 = p.ValueString; break;
                    case "TITLE_11": data.TITLE_11 = p.ValueString; break;
                    case "TITLE_2": data.TITLE_2 = p.ValueString; break;
                    case "TITLE_3": data.TITLE_3 = p.ValueString; break;
                    case "TITLE_4": data.TITLE_4 = p.ValueString; break;
                    case "TITLE_5": data.TITLE_5 = p.ValueString; break;
                    case "TITLE_6": data.TITLE_6 = p.ValueString; break;
                    case "TITLE_7": data.TITLE_7 = p.ValueString; break;
                    case "TITLE_8": data.TITLE_8 = p.ValueString; break;
                    case "TITLE_9": data.TITLE_9 = p.ValueString; break;
                    case "UPDATER_VER": data.UPDATER_VER = p.ValueString; break;
                    case "USE_USB": data.USE_USB = p.ValueInt.GetValueOrDefault() != 0; break;
                }
            }

            di.Data._PSPData = data;
        }

        /// <summary>
        /// chars-ints
        /// </summary>
        /// <param name="discId"></param>
        /// <returns></returns>
        public static string FormatDiscID(string discId)
        {
            if (discId.Contains("-"))
                return discId;

            string result = "";
            int len = discId.Length;
            bool demarkFound = false;
            for (int i = 0; i < len; i++)
            {
                if (!demarkFound)
                {
                    int n;
                    bool isNumeric = int.TryParse(discId[i].ToString(), out n);
                    if (isNumeric)
                    {
                        demarkFound = true;
                        result += "-";
                    }
                }
                result += discId[i];
            }
            return result;
        }

        public static PSPCategory GetPSPCategory(string catCode)
        {
            switch (catCode)
            {
                case "EG": return PSPCategory.Remaster;
                case "MA": return PSPCategory.Apps;
                case "ME": return PSPCategory.PS1Classic;
                case "MG": return PSPCategory.MemoryStickGame;
                case "MS": return PSPCategory.MemoryStickSave;
                case "PG": return PSPCategory.GameUpdate;
                case "UG": return PSPCategory.UMDDiscGame;
                default: return PSPCategory.Unknown;
            }
        }
    }

    public enum PSPCategory
    {
        Unknown,
        MemoryStickSave,
        MemoryStickGame,
        UMDDiscGame,
        GameUpdate,
        Apps,
        PS1Classic,
        Remaster        
    }

    
}
