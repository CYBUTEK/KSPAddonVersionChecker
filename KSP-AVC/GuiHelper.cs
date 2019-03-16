using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using System.Text.RegularExpressions;

//Helper class to keep up an update method which checks for the key combination needed to open the GUI
namespace KSP_AVC
{
    public enum OverrideType { name, version, ignore, locked };

    public class GuiHelper : MonoBehaviour
    {
        public static string userInput { get; set; }

        protected void Awake()
        {
            try
            {
                DontDestroyOnLoad(this);
            }
            catch (Exception ex)
            {
                Logger.Exception(ex);
            }
            this.name = "GuiHelper";
            Logger.Log("GuiHelper was created");
        }

        protected void Update()
        {
            bool modKey = GameSettings.MODIFIER_KEY.GetKey(); 
            if (modKey && Input.GetKeyDown(KeyCode.Alpha2))
            {
                if (this.GetComponent<CompatibilityOverrideGui>())
                {
                    Destroy(this.GetComponent<CompatibilityOverrideGui>());
                    return;
                }
                ToggleGUI();
            }
            if (HighLogic.LoadedScene == GameScenes.SPACECENTER)
            {
                Destroy(this.gameObject);
            }
        }

        protected void OnDestroy()
        {
            Logger.Log("GuiHelper was destroyed");
        }

        public void ToggleGUI()
        {
            if(Configuration.OverrideIsDisabledGlobal)
            {
                this.gameObject.AddComponent<CompatibilityOverrideEnableGui>();
                return;
            }
            if (!this.GetComponent<CompatibilityOverrideGui>())
            {
                this.gameObject.AddComponent<CompatibilityOverrideGui>();
                return;
            }
            else
            {
                Destroy(this.GetComponent<CompatibilityOverrideGui>());
            }
        }

        public static bool CompatibilityState(OverrideType type, Addon addon, string oldVersion = "") //requires -1 instead of *
        {
            switch (type)
            {
                case OverrideType.name:
                    {
                        return addon.IsForcedCompatibleByName;
                    }

                case OverrideType.version:
                    {
                        if (oldVersion != "")
                        {
                            oldVersion = oldVersion.Replace("*", "-1");
                             bool b = (from d in Configuration.CompatibleVersions
                                       where oldVersion == d.Key
                                       where d.Value.compatWithVersion.Contains(AddonInfo.ActualKspVersion)
                                       select d).Any();
                            return b;
                        }
                        return addon.IsForcedCompatibleByVersion;
                    }

                case OverrideType.ignore:
                    {
                        return Configuration.modsIgnoreOverride.Contains(addon.Name);
                    }
                case OverrideType.locked:
                    {
                        return addon.IsLockedByCreator;
                    }

                default:
                    {
                        return false;
                    }
            }
        }

        public static void UpdateCompatibilityState(OverrideType type, Addon addon = null, string versionInfo = "", bool remove = false)
        {
            switch (type)
            {
                case OverrideType.name:
                    {
                        if (!CompatibilityState(OverrideType.name, addon))
                        {
                            Configuration.AddOverrideName(addon);
                            return;
                        }
                        Configuration.RemoveOverrideName(addon);
                        return;
                    }

                case OverrideType.version:
                    {
                        if (validateInput(versionInfo))
                        {
                            userInput = "";
                            List<string> inputs = reformatInput(versionInfo);
                            var dictKeys = Configuration.CompatibleVersions.Keys;
              
                            if (dictKeys.Contains(inputs[0]) && remove)
                            {
                                int j = inputs.Count();
                                for (int i = 1; i < j; i++) //so far, the list will always have just two entries in this case but just to be sure, I've build the loop anyway
                                {
                                    Configuration.RemoveOverrideVersion(inputs[0], inputs[i]);
                                }
                                return;
                            }
                            int m = inputs.Count();
                            for (int i = 1; i < m; i++)
                            {
                                Configuration.AddOverrideVersion(inputs[0], inputs[i]);
                            }
                            return;
                        }
                        userInput = "INVALID";
                        return;
                    }

                case OverrideType.ignore:
                    {
                        if (!CompatibilityState(OverrideType.ignore, addon))
                        {
                            Configuration.AddToIgnore(addon);
                            return;
                        }
                        Configuration.RemoveFromIgnore(addon);
                        return;
                    }

                default:
                    {
                        Logger.Log("Unable to update compatibility override");
                        return;
                    }
            }
        }
        
        private static bool validateInput(string userInput)
        {
            if (!Regex.IsMatch(userInput, @"[^\d\.\-\*\,\s]")) //matches any character which isn't going to be valid at all
            {
                string regexPatternMulti = @"(\d\.\d)\,\s?(\d\.\d)"; //just a rough pattern, should filter out most invalid inputs but definitly not all of them
                string regexPatternSingle = @"(\d\.\d)"; //checks for at least 2 numbers, sparated by a dot
                string regexPatternWildcard = @"\d\.\d\.\*"; //at least one digit + dot + asterisk
                bool matchMulti = Regex.IsMatch(userInput, regexPatternMulti);
                bool matchSingle = Regex.IsMatch(userInput, regexPatternSingle);
                bool matchWildcard = Regex.IsMatch(userInput, regexPatternWildcard);
                if (matchWildcard)
                {
                    return true;
                }
                if (matchMulti)
                {
                    return true;
                }
                if (matchSingle)
                {
                    return true;
                }
                return false; 
            }
            return false;
        }

        //looks ridiculous stupid but I need a strict format ("old version","new version") for the version list
        //funny results if invalid user input slips through the validation
        private static List<string> reformatInput(string userInput)
        {
            string[] splitted = userInput.Split(',').Select(x => x.Trim()).ToArray();
            List<string> reformattedStrings = new List<string>();
            int m = splitted.Length;
            for (int i = 0; i < m; i++)
            {
                reformattedStrings.Add(splitted[i]);
            }
            if (reformattedStrings.Count == 1) //add the actual KSP version if just a single version number is given
            {
                VersionInfo actualKspVersion = new VersionInfo(Versioning.version_major, Versioning.version_minor, Versioning.Revision);
                reformattedStrings.Add(actualKspVersion.ToString());
            }

            return reformattedStrings;
        }
    }
}
