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
using System.Threading;

#endregion

namespace MiniAVC
{
    public static class AddonLibrary
    {
        #region Constructors

        static AddonLibrary()
        {
            ThreadPool.QueueUserWorkItem(ProcessAddonPopulation);
        }

        #endregion

        #region Properties

        public static List<Addon> Addons { get; private set; }

        public static bool Populated { get; private set; }

        public static List<AddonSettings> Settings { get; private set; }

        #endregion

        #region Methods: private

        private static void ProcessAddonPopulation(object state)
        {
            try
            {
                var threadAddons = new List<Addon>();
                var threadSettings = new List<AddonSettings>();
                foreach (var rootPath in AssemblyLoader.loadedAssemblies.Where(a => a.name == "MiniAVC").Select(a => Path.GetDirectoryName(a.path)))
                {
                    var settings = AddonSettings.Load(rootPath);
                    threadSettings.Add(settings);
                    threadAddons.AddRange(Directory.GetFiles(rootPath, "*.version", SearchOption.AllDirectories).Select(p => new Addon(p, settings)).ToList());
                }
                Addons = threadAddons;
                Settings = threadSettings;
                Populated = true;
            }
            catch (Exception ex)
            {
                Logger.Exception(ex);
            }
        }

        #endregion
    }
}