// 
//     Copyright (C) 2014 CYBUTEK
// 
//     This program is free software: you can redistribute it and/or modify
//     it under the terms of the GNU General Public License as published by
//     the Free Software Foundation, either version 3 of the License, or
//     (at your option) any later version.
// 
//     This program is distributed in the hope that it will be useful,
//     but WITHOUT ANY WARRANTY; without even the implied warranty of
//     MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//     GNU General Public License for more details.
// 
//     You should have received a copy of the GNU General Public License
//     along with this program.  If not, see <http://www.gnu.org/licenses/>.
// 

#region Using Directives

using System;
using System.IO;
using System.Reflection;
using System.Xml.Serialization;
using System.Collections.Generic;

#endregion

namespace KSP_AVC
{
    public enum LocalRemotePriority { none, local, remote };
    public class CompatVersions
    {
        public string currentVersion;
        public List<string> compatibleWithVersion;

        public VersionInfo curVersion;
        public List<VersionInfo> compatWithVersion;
    }

    public class Configuration
    {
        #region Fields

        private static readonly string fileName = Path.ChangeExtension(Assembly.GetExecutingAssembly().Location, "xml");

        #endregion

        #region Constructors

        static Configuration()
        {
            Instance = new Configuration
            {
                FirstRun = true,
                Version = Assembly.GetExecutingAssembly().GetName().Version.ToString()
            };
            Load();
        }

        #endregion

        #region Properties

        public static Configuration Instance { get; private set; }

        public bool FirstRun { get; set; }

        public string Version { get; set; }

        #endregion

        #region Methods: public

        public static bool GetFirstRun()
        {
            return Instance.FirstRun;
        }

        public static string GetVersion()
        {
            return Instance.Version;
        }

        public static void Load()
        {
            try
            {
                if (!File.Exists(fileName))
                {
                    return;
                }

                using (var stream = new FileStream(fileName, FileMode.Open))
                {
                    var xml = new XmlSerializer(Instance.GetType());
                    Instance = xml.Deserialize(stream) as Configuration;
                    stream.Close();
                }
            }
            catch (Exception ex)
            {
                Logger.Exception(ex);
            }
        }

        public static void Save()
        {
            try
            {
                using (var stream = new FileStream(fileName, FileMode.Create))
                {
                    var xml = new XmlSerializer(Instance.GetType());
                    xml.Serialize(stream, Instance);
                    stream.Close();
                }
            }
            catch (Exception ex)
            {
                Logger.Exception(ex);
            }
        }

        public static void SetFirstRun(bool value)
        {
            Instance.FirstRun = value;
            Save();
        }

        public static void SetVersion(string value)
        {
            Instance.Version = value;
            Save();
        }

        /////////

         
        public static void LoadCfg()
        {
            Logger.Log("LoadCfg");
            OverridePriority = LocalRemotePriority.none;
            SimplePriority = LocalRemotePriority.none;
            Logger.Log("KSP-AVC node count: " + GameDatabase.Instance.GetConfigNodes("KSP-AVC").Length);
            foreach (ConfigNode node in GameDatabase.Instance.GetConfigNodes("KSP-AVC"))
            {
                if (node.HasValue("OVERRIDE_PRIORITY﻿﻿"))
                {
                    try
                    {
                        OverridePriority = (LocalRemotePriority)Enum.Parse(typeof(LocalRemotePriority), node.GetValue("OVERRIDE_PRIORITY﻿﻿"));
                        Logger.Log("OverridePriority: " + OverridePriority);
                    }
                    catch { }
                }
                if (node.HasValue("SIMPLE_PRIORITY﻿﻿"))
                {
                    try
                    {
                        SimplePriority = (LocalRemotePriority)Enum.Parse(typeof(LocalRemotePriority), node.GetValue("SIMPLE_PRIORITY﻿﻿"));
                        Logger.Log("SimplePriority: " + SimplePriority);
                    }
                    catch { }
                }
                if(node.HasValue("COMPATIBLE_VERSION_OVERRIDE"))
                {
                    try
                    {
                        var compatVerList = node.GetValuesList("COMPATIBLE_VERSION_OVERRIDE");
                        foreach (var a in compatVerList)
                        {
                            CompatVersions cv = new CompatVersions();
                            var ar = a.Split(',');

                            cv.currentVersion = ar[0];
                            cv.curVersion = new VersionInfo(cv.currentVersion);
                            cv.compatibleWithVersion = new List<string>();
                            cv.compatWithVersion = new List<VersionInfo>(); //initializing the list before adding stuff to it helps to prevent a NRE :) 
                            for (int i = 1; i < ar.Length; i++)
                            {
                                cv.compatibleWithVersion.Add(ar[i]);
                                cv.compatWithVersion.Add(new VersionInfo(ar[i]));
                                Logger.Log("COMPATIBLE_VERSION_OVERRIDE, currentVersion: " + ar[0] + ", compatibleWithVersion: " + ar[i]);
                            }
                            CompatibleVersions.Add(cv.currentVersion, cv);
                        }
                    }
                    catch { }
                    
                }
                if(node.HasValue("IGNORE_OVERRIDE"))
                {
                    try
                    {
                        List<string> ignoredMods = node.GetValuesList("IGNORE_OVERRIDE");
                        foreach (string modName in ignoredMods)
                        {                            
                            modsIgnoreOverride.Add(modName);
                            Logger.Log($"IGNORE_OVERRIDE: {modName}");
                        }
                    }
                    catch { }
                }
                
            }
            CfgLoaded = true;
            Logger.Flush();
        }
        public static LocalRemotePriority OverridePriority { get; private set; }
        public static LocalRemotePriority SimplePriority { get; private set; }
        public static bool CfgLoaded = false;

        public static List<string> modsIgnoreOverride = new List<string>();

        public static Dictionary<string, CompatVersions> CompatibleVersions = new Dictionary<string, CompatVersions>();

        #endregion
    }
}