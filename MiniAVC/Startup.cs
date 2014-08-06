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
using System.Collections.Generic;
using System.IO;
using System.Linq;

using UnityEngine;

#endregion

namespace MiniAVC
{
    [KSPAddon(KSPAddon.Startup.Instantly, false)]
    public class Startup : MonoBehaviour
    {
        #region Fields

        private static bool alreadyRunning;
        private readonly List<Addon> addons = new List<Addon>();
        private FirstRunGui shownFirstRunGui;
        private IssueGui shownIssueGui;

        #endregion

        #region Initialisation

        private void Awake()
        {
            try
            {
                // Only allow one instance to run.
                if (alreadyRunning)
                {
                    Destroy(this);
                    return;
                }
                alreadyRunning = true;

                // Unload if KSP-AVC is detected.
                if (AssemblyLoader.loadedAssemblies.Any(a => a.name == "KSP-AVC"))
                {
                    Logger.Log("KSP-AVC was detected...  Unloading MiniAVC!");
                    Destroy(this);
                    return;
                }
                Logger.Log("Starter was created.");
            }
            catch (Exception ex)
            {
                Logger.Exception(ex);
            }
        }

        private void Start()
        {
            try
            {
                // Load all the addons which are being supported by MiniAVC.
                foreach (var path in AssemblyLoader.loadedAssemblies.Where(a => a.name == "MiniAVC").Select(a => Path.GetDirectoryName(a.path)))
                {
                    var addon = new Addon(path);
                    if (addon.HasVersionFile)
                    {
                        this.addons.Add(addon);
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Exception(ex);
            }
        }

        #endregion

        private void Update()
        {
            try
            {
                // Stop updating if there is already a gui being shown.
                if (this.shownFirstRunGui != null || this.shownIssueGui != null)
                {
                    return;
                }

                // Create and show first run gui if required.
                if (this.addons.Any(a => a.Settings.FirstRun && a.IsLocalReady && !string.IsNullOrEmpty(a.LocalInfo.Url)))
                {
                    foreach (var addon in this.addons.Where(a => a.Settings.FirstRun && a.IsLocalReady))
                    {
                        this.shownFirstRunGui = this.gameObject.AddComponent<FirstRunGui>();
                        this.shownFirstRunGui.Addon = addon;
                        this.shownFirstRunGui.enabled = true;
                        break;
                    }
                    return;
                }

                // Create and show issue gui if required.
                var removeAddons = new List<Addon>();

                foreach (var addon in this.addons.Where(a => a.IsProcessingComplete))
                {
                    if (addon.IsUpdateAvailable || !addon.IsCompatible)
                    {
                        this.shownIssueGui = this.gameObject.AddComponent<IssueGui>();
                        this.shownIssueGui.Addon = addon;
                        this.shownIssueGui.enabled = true;
                        removeAddons.Add(addon);
                        break;
                    }

                    removeAddons.Add(addon);
                }

                this.addons.RemoveAll(removeAddons.Contains);
            }
            catch (Exception ex)
            {
                Logger.Exception(ex);
            }
        }
    }
}