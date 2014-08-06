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

#endregion

namespace KSP_AVC
{
    public static class AddonLibrary
    {
        #region Fields

        private static readonly string rootPath;

        #endregion

        #region Constructors

        static AddonLibrary()
        {
            rootPath = UrlDir.ApplicationRootPath;
            ThreadPool.QueueUserWorkItem(ProcessAddonPopulation);
        }

        #endregion

        #region Properties

        public static List<Addon> Addons { get; private set; }

        public static bool Populated { get; private set; }

        #endregion

        #region Private Methods

        private static void ProcessAddonPopulation(object state)
        {
            var threadAddons = Directory.GetFiles(rootPath, "*.version", SearchOption.AllDirectories).Select(p => p.Replace(rootPath, string.Empty)).Select(path => new Addon(path)).ToList();
            Addons = threadAddons;
            Populated = true;
        }

        #endregion
    }
}