using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using UnityEngine;

//Class to handle the settings from the Override GUI
//Sattings are saved in a config file, which should always have a file extension which is NOT .cfg or the file need to be located in the PluginData folder
//otherwise ModuleManager will rebuild the Cache everytime a mod is added or removed from the override list
//namespace KSP_AVC
//{
//    class AVCSettings
//    {
//        [KSPField(isPersistant = true)]
//        static readonly AVCSettings instance = new AVCSettings();
//        public static AVCSettings Instance
//        {
//            get
//            {
//                if (!instance.isLoaded)
//                {
//                    instance.Load();
//                }
//                return instance;
//            }
//        }

//        bool isLoaded = false;

//        readonly string ConfigFile = KSPUtil.ApplicationRootPath + "GameData/KSP-AVC/PluginData/Override.dat";

//        readonly string TopLevelNode = "AVC_OVERRIDE_COMPATIBILITY";
//        public List<string> OverrideModCompatibility { get; set; }

//        public AVCSettings()
//        {
//            Logger.Log("Running Constructor OverrideSettings");
//            OverrideModCompatibility = new List<string>();
//            Load();
//        }

//        public void Add(AddonInfo addon)
//        {
//            if(!OverrideModCompatibility.Contains(addon.Name))
//            {
//                OverrideModCompatibility.Add(addon.Name);
//            }
//            else
//            {
//                return;
//            }
//        }

//        public void Remove(AddonInfo addon)
//        {
//            if (OverrideModCompatibility.Contains(addon.Name))
//            {
//                OverrideModCompatibility.Remove(addon.Name);
//            }
//            else
//            {
//                return;
//            }
//        }

//        public void Save()
//        {
//            if (!File.Exists(ConfigFile))
//            {
//                File.Create(ConfigFile);
//            }

//            ConfigNode cfgnode = new ConfigNode();
//            ConfigNode temp = cfgnode.AddNode(TopLevelNode);

//            foreach (string ModName in OverrideModCompatibility)
//            {
//                temp.AddValue("OverrideEnabled", ModName);
//            }

//            cfgnode.Save(ConfigFile);
//        }

//        public void Load()
//        {

//            if(!File.Exists(ConfigFile))
//            {
//                Logger.Log("Cannot find override settings, nothing to load");
//                return;
//            }
            
//            try
//            {
//                ConfigNode loadedConfig = ConfigNode.Load(ConfigFile);
//                ConfigNode avcOverrideNode = loadedConfig.GetNode("AVC_OVERRIDE_COMPATIBILITY");
//                OverrideModCompatibility = avcOverrideNode.GetValuesList("OverrideEnabled");

//            }
//            catch
//            {
//                Logger.Log("Unable to load override settings from config file");
//            }

//            isLoaded = true;
//        }
//    }
//}
