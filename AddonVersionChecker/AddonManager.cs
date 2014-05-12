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

using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;

using UnityEngine;

#endregion

namespace AddonVersionChecker
{
    [KSPAddon(KSPAddon.Startup.Instantly, false)]
    public class AddonManager : MonoBehaviour
    {
        #region Fields

        private static readonly List<AddonData> addons = new List<AddonData>();

        #endregion

        #region Constructors

        private void Awake()
        {
            // Populate all addons asynchronously so that the UI thread is not blocked.
            new Thread(() =>
            {
                lock (addons)
                {
                    foreach (var file in Directory.GetFiles(UrlDir.ApplicationRootPath, "*.version", SearchOption.AllDirectories))
                    {
                        var addon = new AddonData(File.ReadAllText(file));
                        if (addon.Url.Length > 0)
                        {
                            LoadRemoteAddonData(addon);
                        }
                        addons.Add(addon);
                    }
                }
            }).Start();
        }

        #endregion

        #region Properties

        /// <summary>
        ///     Gets all the addon data for addons that contain version information.
        /// </summary>
        public static List<AddonData> Addons
        {
            get { return addons; }
        }

        /// <summary>
        ///     Gets whether any update or compatibility issues have been found.
        /// </summary>
        public static bool HasIssues
        {
            get { return HasUpdateIssues || HasCompatibilityIssues; }
        }

        /// <summary>
        ///     Gets whether any update issues have been found.
        /// </summary>
        public static bool HasUpdateIssues
        {
            get { return addons.Any(addon => addon.UpdateAvailable); }
        }

        /// <summary>
        ///     Gets whether any compatibility issues have been found.
        /// </summary>
        public static bool HasCompatibilityIssues
        {
            get { return addons.Any(addon => !addon.GameCompatible); }
        }

        #endregion

        #region Private Methods

        private static void LoadRemoteAddonData(AddonData addon)
        {
            try
            {
                // Fetch the remote json data.
                var www = new WWW(addon.Url);

                // This method is run within a non-gui thread so blocking doesn't matter.
                while (!www.isDone) { }

                // Create the remote addon data using the remote jason data.
                addon.RemoteAddonData = new AddonData(www.text);
            }
            catch
            {
                print("Could not fetch remote version information for " + addon.Name + ".");
            }
        }

        #endregion
    }
}