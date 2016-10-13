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
using System.Linq;
using System.Reflection;

using UnityEngine;

#endregion

namespace KSP_AVC
{
    [KSPAddon(KSPAddon.Startup.Instantly, false)]
    public class Starter : MonoBehaviour
    {
        #region Fields

        private static bool hasAlreadyChecked;
        private CheckerProgressGui checkerProgressGui;
        private FirstRunGui firstRunGui;

        #endregion

        #region Methods: protected

        protected void Awake()
        {
            try
            {
                if (this.HasAlreadyChecked())
                {
                    return;
                }
                DontDestroyOnLoad(this);
            }
            catch (Exception ex)
            {
                Logger.Exception(ex);
            }
            Logger.Log("Starter was created.");
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
            }
            catch (Exception ex)
            {
                Logger.Exception(ex);
            }
            Logger.Log("Starter was destroyed.");
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
            if (HighLogic.LoadedScene == GameScenes.SPACECENTER)
            {
                Destroy(gameObject);
            }

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
                    this.checkerProgressGui = this.gameObject.AddComponent<CheckerProgressGui>();
                }
            }
            catch (Exception ex)
            {
                Logger.Exception(ex);
            }
        }

        #endregion

        #region Methods: private

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
            if (!AddonLibrary.Populated || !AddonLibrary.ProcessingComplete)
            {
                return false;
            }
            if (AddonLibrary.Addons.Any(a => a.IsUpdateAvailable || !a.IsCompatible))
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

        #endregion
    }
}