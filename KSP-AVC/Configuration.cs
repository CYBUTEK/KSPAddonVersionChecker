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
using System.Linq;
#endregion

namespace KSP_AVC
{
    public enum LocalRemotePriority { none, local, remote };
    public class CompatVersions
    {
        public string currentVersion; //old
        public List<string> compatibleWithVersion; //new

        public VersionInfo curVersion; //old
        public List<VersionInfo> compatWithVersion;//new

        public void AddCompatibleWithVersion(string version)
        {
            if(!this.compatibleWithVersion.Contains(version))
            {
                this.compatibleWithVersion.Add(version);
                this.compatWithVersion.Add(new VersionInfo(version));
                return;
            }
            Logger.Log($"Cannot add {version} to the compatibility list, entry already exists");
        }

        public void RemoveCompatibleWithVersion(string version)
        {
            if(this.compatibleWithVersion.Contains(version))
            {
                this.compatibleWithVersion.Remove(version);
                this.compatWithVersion.Remove(new VersionInfo(version));
                return;
            }
            Logger.Log($"Cannot remove {version} from the compatibility list, entry doesn't exists");
        }
    }

    public class Configuration
    {
        #region Fields
        
        private static readonly string fileName = Path.ChangeExtension(Assembly.GetExecutingAssembly().Location, "xml");
        readonly static string AvcConfigFile = KSPUtil.ApplicationRootPath + "GameData/KSP-AVC/PluginData/AVC.cfg";

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

        public static List<string> OverrideCompatibilityByName { get; private set; }

        public static List<string> OverrideCompatibilityByVersion { get; private set; } //Do I need this one or can I use the CompatVersions class?

        public static bool OverrideIsDisabledGlobal { get; private set; }

        public static int AvcInterval { get; private set; }

        public static DateTime NextRun { get; private set; }

        public static bool CfgUpdated { get; set; }

        public static List<string> toDelete { get; private set; } = new List<string>();

        #endregion

        #region Methods: public

        public static void AddOverrideName(Addon addon)
        {
            if(!OverrideCompatibilityByName.Contains(addon.Name))
            {
                OverrideCompatibilityByName.Add(addon.Name);
                Logger.Log($"Compatibility Override (by name) enabled for {addon.Name}");
                CfgUpdated = true;
                return;
            }
            Logger.Log($"Cannot add {addon.Name}, entry already exists."); //Should never happen but just in case
        }

        public static void RemoveOverrideName(Addon addon)
        {
            if (OverrideCompatibilityByName.Contains(addon.Name))
            {
                OverrideCompatibilityByName.Remove(addon.Name);
                Logger.Log($"Compatibility Override (by name) disabled for {addon.Name}");
                CfgUpdated = true;
                return;
            }
            Logger.Log($"Cannot remove {addon.Name}. Entry doesn't exists."); //Should never happen but just in case
        }
        
        public static void AddOverrideVersion(string oldVersion, string newVersion)
        {
            //check if there is already an dictionary key which contains the oldversion
            var dictKeys = CompatibleVersions.Keys;
            if (dictKeys.Contains(oldVersion))
            {
                //add an additional new version to the list of compatible versions
                CompatibleVersions[oldVersion].AddCompatibleWithVersion(newVersion);
                CfgUpdated = true;
                return;
            }

            //If the key doesn't match already, we have to create a whole new dictionary entry
            //Basically the same code which is used to load the config
            CompatVersions cv = new CompatVersions();

            cv.currentVersion = oldVersion;
            cv.curVersion = new VersionInfo(cv.currentVersion);
            cv.compatibleWithVersion = new List<string>();
            cv.compatWithVersion = new List<VersionInfo>();

            cv.compatibleWithVersion.Add(newVersion);
            cv.compatWithVersion.Add(new VersionInfo(newVersion));
            CompatibleVersions.Add(cv.currentVersion, cv);
            CfgUpdated = true;
        }

        public static void RemoveOverrideVersion(string oldVersion, string newVersion)
        {
            var dictKeys = CompatibleVersions.Keys;

            if (dictKeys.Contains(oldVersion))
            {
                if(CompatibleVersions[oldVersion].compatibleWithVersion.Count == 1)
                {
                    //CompatibleVersions.Remove(oldVersion);
                    toDelete.Add(oldVersion); //need to collect keys which are meant to be deleted, bad things will happen if you try this while iterating over the dictionary :o
                    CfgUpdated = true;
                    Logger.Log("Key removed!");
                    return;
                }
                CompatibleVersions[oldVersion].RemoveCompatibleWithVersion(newVersion);
                CfgUpdated = true;
            }
        }

        public static void deleteFinally()
        {
            if (toDelete.Count > 0)
            {
                foreach (string version in toDelete)
                {
                    CompatibleVersions.Remove(version);
                }
                toDelete.Clear(); 
            }
        }

        public static void AddToIgnore(Addon addon)
        {
            if(!modsIgnoreOverride.Contains(addon.Name))
            {
                modsIgnoreOverride.Add(addon.Name);
                Logger.Log($"{addon.Name} added to ignore list.");
                CfgUpdated = true;
            }
            Logger.Log($"Cannot add {addon.Name} to ignore list, entry already exists.");
        }

        public static void RemoveFromIgnore(Addon addon)
        {
            if (modsIgnoreOverride.Contains(addon.Name))
            {
                modsIgnoreOverride.Remove(addon.Name);
                Logger.Log($"{addon.Name} removed from ignore list.");
                CfgUpdated = true;
            }
            Logger.Log($"Cannot remove {addon.Name} to ignore list, entry already exists.");
        }

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

        public static void SaveCfg()
        {
            Logger.Log("SaveCfg");
            CfgUpdated = false;

            if (!File.Exists(AvcConfigFile))
            {
                File.Create(AvcConfigFile);
                OverrideCompatibilityByName = new List<string>();
                //OverrideCompatibilityByVersion = new List<string>();
                OverrideIsDisabledGlobal = true;
                AvcInterval = 0;
            }
            
            ConfigNode cfgnode = new ConfigNode();
            ConfigNode KSPAVC = cfgnode.AddNode("KSP-AVC");

            KSPAVC.AddValue("OVERRIDE_PRIORITY", OverridePriority);
            KSPAVC.AddValue("SIMPLE_PRIORITY", SimplePriority);
            KSPAVC.AddValue("DISABLE_COMPATIBLE_VERSION_OVERRIDE", OverrideIsDisabledGlobal);

            ConfigNode OverrideName = KSPAVC.AddNode("OVERRIDE_NAME");
            foreach (string ModName in OverrideCompatibilityByName)
            {
                OverrideName.AddValue("OverrideEnabled", ModName);
            }

            ConfigNode OverrideVersion = KSPAVC.AddNode("OVERRIDE_VERSION");            
            foreach (KeyValuePair<string, CompatVersions> kvp in CompatibleVersions)
            {
                string temp = kvp.Key;
                for (int i = 0; i < kvp.Value.compatibleWithVersion.Count; i++)
                {
                    //sb.Append($", {kvp.Value.compatibleWithVersion[i]}");
                    temp = temp + $", {kvp.Value.compatibleWithVersion[i]}";
                }
                OverrideVersion.AddValue("OverrideEnabled", temp);
            }

            ConfigNode OverrideIgnore = KSPAVC.AddNode("OVERRIDE_IGNORE");
            foreach (string ModName in modsIgnoreOverride)
            {
                OverrideIgnore.AddValue("IgnoreOverride", ModName);
            }

            ConfigNode Interval = KSPAVC.AddNode("INTERVAL");
            {
                Interval.AddValue("MinTimeBetweenAvcRuns", AvcInterval.ToString() + " //Timespan between AVC runs in hours");

                if(DateTime.Compare(DateTime.Now, NextRun) >= 0 && AvcInterval > 0)
                {
                    Interval.AddValue("AvcRunsNext", DateTime.Now.AddHours(AvcInterval).ToString());
                    CfgUpdated = true;
                }
                else
                {
                    Interval.AddValue("AvcRunsNext", NextRun.ToString());
                }
            }
            cfgnode.Save(AvcConfigFile);
        }
         
        public static void LoadCfg()
        {
            Logger.Log("LoadCfg");
            OverridePriority = LocalRemotePriority.none;
            SimplePriority = LocalRemotePriority.none;
            CfgUpdated = false;
            //Logger.Log("KSP-AVC node count: " + GameDatabase.Instance.GetConfigNodes("KSP-AVC").Length);
            //foreach (ConfigNode node in GameDatabase.Instance.GetConfigNodes("KSP-AVC"))
            if (!File.Exists(AvcConfigFile))
            {
                SaveCfg();
            }
            
            ConfigNode LoadNodeFromFile = ConfigNode.Load(AvcConfigFile);
            ConfigNode node = LoadNodeFromFile.GetNode("KSP-AVC"); 

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
            if(node.HasValue("DISABLE_COMPATIBLE_VERSION_OVERRIDE"))
            {
                try
                {
                    if (node.GetValue("DISABLE_COMPATIBLE_VERSION_OVERRIDE").ToLower() == "false")
                        OverrideIsDisabledGlobal = false;
                    else
                        OverrideIsDisabledGlobal = true;
                    Logger.Log($"OverrideIsDisabled: {OverrideIsDisabledGlobal}");
                }
                catch { }
            }
            if (node.HasNode("OVERRIDE_NAME"))
            {
                try
                {
                    ConfigNode _temp = new ConfigNode();
                    _temp = node.GetNode("OVERRIDE_NAME");

                    OverrideCompatibilityByName = _temp.GetValuesList("OverrideEnabled");
                    foreach (var item in OverrideCompatibilityByName)
                    {
                        Logger.Log($"OverrideCompatibilityByName: {item}");
                    }
                }
                catch { }
            }
            if (node.HasNode("OVERRIDE_VERSION"))
            {
                try
                {
                    ConfigNode _temp = new ConfigNode();
                    _temp = node.GetNode("OVERRIDE_VERSION");
                    //OverrideCompatibilityByVersion = new List<string>();
                    var compatVerList = _temp.GetValuesList("OverrideEnabled");
                    foreach (var a in compatVerList)
                    {
                        CompatVersions cv = new CompatVersions();
                        var ar = a.Split(',').Select(x => x.Trim()).ToArray();

                        cv.currentVersion = ar[0];
                        cv.curVersion = new VersionInfo(cv.currentVersion);
                        cv.compatibleWithVersion = new List<string>();
                        cv.compatWithVersion = new List<VersionInfo>(); //initializing the list before adding stuff to it helps to prevent a NRE :) 
                        for (int i = 1; i < ar.Length; i++)
                        {
                            cv.compatibleWithVersion.Add(ar[i]);
                            cv.compatWithVersion.Add(new VersionInfo(ar[i]));
                            Logger.Log("OVERRIDE_VERSION, currentVersion: " + ar[0] + ", compatibleWithVersion: " + ar[i]);
                            //OverrideCompatibilityByVersion.Add(ar[0] + ", " + ar[i].Trim());
                        }
                        CompatibleVersions.Add(cv.currentVersion, cv);
                    }
                }
                catch { }                    
            }
            if(node.HasNode("OVERRIDE_IGNORE"))
            {
                try
                {
                    ConfigNode _temp = new ConfigNode();
                    _temp = node.GetNode("OVERRIDE_IGNORE");

                    List<string> ignoredMods = _temp.GetValuesList("IgnoreOverride");
                    foreach (string modName in ignoredMods)
                    {                            
                        modsIgnoreOverride.Add(modName);
                        Logger.Log($"IGNORE_OVERRIDE: {modName}");
                    }
                }
                catch { }
            }
            if(node.HasNode("INTERVAL"))
            {
                try
                {
                    ConfigNode _temp = new ConfigNode();
                    _temp = node.GetNode("INTERVAL");

                    AvcInterval = Int32.Parse(_temp.GetValue("MinTimeBetweenAvcRuns"));
                    if(!_temp.HasValue("AvcRunsNext"))
                    {
                        CfgUpdated = true;
                    }
                    NextRun = DateTime.Parse(_temp.GetValue("AvcRunsNext"));
                    Logger.Log($"INTERVAL: {AvcInterval}");
                    Logger.Log($"NextRun: {NextRun}");
                }
                catch { }
            }
            if (DateTime.Compare(DateTime.Now, NextRun) >= 0 && AvcInterval > 0)
            {
                CfgUpdated = true;
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