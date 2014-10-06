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
using System.Reflection;
using System.Threading;

#endregion

namespace KSP_AVC
{
    public static class AddonLibrary
    {
        #region Fields

        private static readonly string rootPath;
        private static List<Addon> addons;

        #endregion

        #region Constructors

        static AddonLibrary()
        {
            rootPath = GetRootPath();
            Logger.Log("Checking Root: " + rootPath);
            ThreadPool.QueueUserWorkItem(ProcessAddonPopulation);
        }

        #endregion

        #region Properties

        public static IEnumerable<Addon> Addons
        {
            get { return (addons != null) ? addons.Where(a => !a.HasError).ToList() : null; }
        }

        public static bool Populated { get; private set; }

        public static int ProcessCount
        {
            get { return (addons != null) ? addons.Count(a => a.IsProcessingComplete) : 0; }
        }

        public static IEnumerable<Addon> Processed
        {
            get { return (addons != null) ? addons.Where(a => !a.HasError && a.IsProcessingComplete) : null; }
        }

        public static bool ProcessingComplete
        {
            get { return addons != null && addons.All(a => a.IsProcessingComplete); }
        }

        public static int TotalCount
        {
            get { return (addons != null) ? addons.Count : 0; }
        }

        #endregion

        #region Methods: private

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
                Populated = false;
                addons = Directory.GetFiles(rootPath, "*.version", SearchOption.AllDirectories).Select(path => new Addon(path)).ToList();
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