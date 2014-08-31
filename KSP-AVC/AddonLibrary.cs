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
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;

namespace KSP_AVC
{
    public static class AddonLibrary
    {
        private static readonly string rootPath;

        static AddonLibrary()
        {
            rootPath = GetRootPath();
            Logger.Log("Checking Root: " + rootPath);
            ThreadPool.QueueUserWorkItem(ProcessAddonPopulation);
        }

        public static List<Addon> Addons { get; private set; }

        public static bool Populated { get; private set; }

        private static string GetRootPath()
        {
            var rootPath = Assembly.GetExecutingAssembly().Location;
            var gameDataIndex = rootPath.IndexOf("GameData", StringComparison.CurrentCultureIgnoreCase);
            return Path.Combine(rootPath.Remove(gameDataIndex, rootPath.Length - gameDataIndex), "GameData");
        }

        private static void ProcessAddonPopulation(object state)
        {
            try
            {
                var threadAddons = Directory.GetFiles(rootPath, "*.version", SearchOption.AllDirectories).Select(path => new Addon(path)).ToList();
                Addons = threadAddons;
                Populated = true;
            }
            catch (Exception ex)
            {
                Logger.Exception(ex);
            }
        }
    }
}