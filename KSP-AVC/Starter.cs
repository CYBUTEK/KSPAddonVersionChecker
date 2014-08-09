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

        private CheckGui checkGui;
        private FirstRunGui firstRunGui;

        #endregion

        #region Initialisation

        private void Awake()
        {
            try
            {
                DontDestroyOnLoad(this);
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
                if (new System.Version(Settings.Instance.Version) < Assembly.GetExecutingAssembly().GetName().Version)
                {
                    Settings.Instance.Version = Assembly.GetExecutingAssembly().GetName().Version.ToString();
                    Settings.Save();
                    this.firstRunGui = this.gameObject.AddComponent<FirstRunGui>();
                    this.firstRunGui.HasBeenUpdated = true;
                }
                else if (Settings.Instance.FirstRun)
                {
                    Settings.Instance.FirstRun = false;
                    Settings.Save();
                    this.firstRunGui = this.gameObject.AddComponent<FirstRunGui>();
                }
            }
            catch (Exception ex)
            {
                Logger.Exception(ex);
            }
        }

        #endregion

        #region Updating

        private void Update()
        {
            try
            {
                if (this.firstRunGui != null)
                {
                    return;
                }

                if (AddonLibrary.Populated && AddonLibrary.Addons.All(a => a.IsProcessingComplete))
                {
                    if (AddonLibrary.Addons.Any(a => !a.HasError && (a.IsUpdateAvailable || !a.IsCompatible)))
                    {
                        this.gameObject.AddComponent<IssueGui>();
                    }
                    Destroy(this.checkGui);
                    Destroy(this);
                }

                if (this.checkGui == null)
                {
                    this.checkGui = this.gameObject.AddComponent<CheckGui>();
                }
            }
            catch (Exception ex)
            {
                Logger.Exception(ex);
            }
        }

        #endregion

        #region Destruction

        private void OnDestroy()
        {
            Logger.Log("Starter was destroyed.");
        }

        #endregion
    }
}