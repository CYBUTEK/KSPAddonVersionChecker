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

using System;
using System.Linq;
using System.Reflection;

using UnityEngine;

namespace KSP_AVC
{
    [KSPAddon(KSPAddon.Startup.Instantly, false)]
    public class Starter : MonoBehaviour
    {
        private static bool hasAlreadyChecked;
        private CheckGui checkerProgressGui;
        private FirstRunGui firstRunGui;

        protected void Awake()
        {
            try
            {
                if (this.HasAlreadyChecked())
                {
                    return;
                }
                DontDestroyOnLoad(this);
                Logger.Log("Starter was created.");
            }
            catch (Exception ex)
            {
                Logger.Exception(ex);
            }
        }

        protected void OnDestroy()
        {
            try
            {
                if (this.firstRunGui != null)
                {
                    Destroy(this.firstRunGui);
                }
                if (this.checkerProgressGui != null)
                {
                    Destroy(this.checkerProgressGui);
                }
                Logger.Log("Starter was destroyed.");
            }
            catch (Exception ex)
            {
                Logger.Exception(ex);
            }
        }

        protected void Start()
        {
            try
            {
                if (new System.Version(Configuration.GetVersion()) < Assembly.GetExecutingAssembly().GetName().Version)
                {
                    this.ShowUpdatedWindow();
                }
                else if (Configuration.GetFirstRun())
                {
                    this.ShowInstalledWindow();
                }
            }
            catch (Exception ex)
            {
                Logger.Exception(ex);
            }
        }

        protected void Update()
        {
            try
            {
                if (this.firstRunGui != null)
                {
                    return;
                }
                if (this.ShowIssuesWindow())
                {
                    return;
                }
                if (this.checkerProgressGui == null)
                {
                    this.checkerProgressGui = this.gameObject.AddComponent<CheckGui>();
                }
            }
            catch (Exception ex)
            {
                Logger.Exception(ex);
            }
        }

        private bool HasAlreadyChecked()
        {
            if (hasAlreadyChecked)
            {
                Destroy(this);
                return true;
            }
            hasAlreadyChecked = true;
            return false;
        }

        private void ShowInstalledWindow()
        {
            Configuration.SetFirstRun(false);
            this.firstRunGui = this.gameObject.AddComponent<FirstRunGui>();
        }

        private bool ShowIssuesWindow()
        {
            if (!AddonLibrary.Populated || !AddonLibrary.Addons.All(a => a.IsProcessingComplete))
            {
                return false;
            }
            if (AddonLibrary.Addons.Any(a => !a.HasError && (a.IsUpdateAvailable || !a.IsCompatible)))
            {
                this.gameObject.AddComponent<IssueGui>();
            }
            Destroy(this);
            return true;
        }

        private void ShowUpdatedWindow()
        {
            Configuration.SetVersion(Assembly.GetExecutingAssembly().GetName().Version.ToString());
            this.firstRunGui = this.gameObject.AddComponent<FirstRunGui>();
            this.firstRunGui.HasBeenUpdated = true;
        }
    }
}