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
using System.IO;
using System.Reflection;

using UnityEngine;

#endregion

namespace KSP_AVC
{
    public static class Utils
    {
        #region Fields

        private static readonly string textureDirectory = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "Textures");

        #endregion

        #region Methods: public

        public static Texture2D GetTexture(string file, int width, int height)
        {
            try
            {
                var texture = new Texture2D(width, height, TextureFormat.ARGB32, false);
                texture.LoadImage(File.ReadAllBytes(Path.Combine(textureDirectory, file)));
                return texture;
            }
            catch (Exception ex)
            {
                Logger.Exception(ex);
                return null;
            }
        }

        #endregion
    }
}