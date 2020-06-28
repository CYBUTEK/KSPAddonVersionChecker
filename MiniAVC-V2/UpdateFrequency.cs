using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using System.IO;

namespace MiniAVC_V2
{
    class UpdateFrequency
    {
        public static bool ConfigLoaded { get; private set; }
        public static int AvcInterval { get; private set; }
        public static DateTime NextRun { get; private set; }
        static string configPath = KSPUtil.ApplicationRootPath + "GameData/MiniAVCUpdateFrequency.dat";

        public static bool SkipRun
        {
            get
            {
                if (DateTime.Compare(DateTime.Now, NextRun) <= 0 && AvcInterval != 0)
                {
                    return true;
                }
                return false;
            }
        }

        public static bool DisableCheck
        {
            get
            {
                if (AvcInterval == -1)
                {
                    return true;
                }
                return false;
            }
        }

        public static void LoadConfig()
        {
            if (!File.Exists(configPath))
            {
                Logger.Log($"Config not found: {configPath}");
                return;
            }

            ConfigNode LoadNodeFromFile = ConfigNode.Load(configPath);
            ConfigNode node = LoadNodeFromFile.GetNode("MINI-AVC");

            var nodes = node.GetNodes();
            if(node.HasNode("UPDATE_FREQUENCY"))
            {
                try
                {
                    ConfigNode _temp = new ConfigNode();
                    _temp = node.GetNode("UPDATE_FREQUENCY");

                    AvcInterval = Int32.Parse(_temp.GetValue("MinTimeBetweenAvcRuns"));
                    if (_temp.HasValue("AvcRunsNext"))
                    {
                        NextRun = DateTime.Parse(_temp.GetValue("AvcRunsNext"));
                    }                    
                }
                catch { }
            }
            ConfigLoaded = true;
            Logger.Log("Config loaded");
        }

        public static void SaveConfig()
        {
            ConfigNode cfgnode = new ConfigNode();
            ConfigNode KSPAVC = cfgnode.AddNode("MINI-AVC");

            ConfigNode Interval = KSPAVC.AddNode("UPDATE_FREQUENCY");
            {
                Interval.AddValue("MinTimeBetweenAvcRuns", AvcInterval.ToString() + " //Timespan between AVC runs in hours (-1 = Disable MiniAVC, 0 = Run on each game start)");

                if (DateTime.Compare(DateTime.Now, NextRun) >= 0 && AvcInterval > 0)
                {
                    Interval.AddValue("AvcRunsNext", DateTime.Now.AddHours(AvcInterval).ToString());
                }
                else
                {
                    Interval.AddValue("AvcRunsNext", NextRun.ToString());
                }
            }
            cfgnode.Save(configPath);
            Logger.Log("Config saved!");
        }
    }
}
