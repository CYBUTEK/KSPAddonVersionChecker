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

using LitJson;

using UnityEngine;

#endregion

namespace AddonVersionChecker
{
    public class AddonData
    {
        #region Fields

        private readonly string json;
        private System.Version addonVersion = new System.Version();
        private string download = string.Empty;
        private System.Version gameVersion = new System.Version();
        private string name = string.Empty;
        private AddonData remoteAddonData;
        private string url = string.Empty;

        #endregion

        #region Constructors

        public AddonData(string json)
        {
            this.json = json;
            this.ParseJson();
            this.remoteAddonData = this;
        }

        #endregion

        #region Properties

        /// <summary>
        ///     Gets the addon name.
        /// </summary>
        public string Name
        {
            get { return this.name; }
        }

        /// <summary>
        ///     Gets the url of the remote version file.
        /// </summary>
        public string Url
        {
            get { return this.url; }
        }

        /// <summary>
        ///     Gets the url of the download location.
        /// </summary>
        public string Download
        {
            get { return this.download; }
        }

        /// <summary>
        ///     Gets the addon version.
        /// </summary>
        public System.Version AddonVersion
        {
            get { return this.addonVersion; }
        }

        /// <summary>
        ///     Gets the game version for which the addon was created to run on.
        /// </summary>
        public System.Version GameVersion
        {
            get { return this.gameVersion; }
        }

        /// <summary>
        ///     Gets the remote addon data.
        /// </summary>
        public AddonData RemoteAddonData
        {
            get { return this.remoteAddonData; }
            set { this.remoteAddonData = value; }
        }

        /// <summary>
        ///     Gets the raw json data.
        /// </summary>
        public string Json
        {
            get { return this.json; }
        }

        /// <summary>
        ///     Gets whether there is an update available.
        /// </summary>
        public bool UpdateAvailable
        {
            get { return this.addonVersion.CompareTo(this.remoteAddonData.addonVersion) < 0; }
        }

        /// <summary>
        ///     Gets whether the addon is compatible with the current game version.
        /// </summary>
        public bool GameCompatible
        {
            get { return this.gameVersion.CompareTo(new System.Version(Versioning.version_major, Versioning.version_minor, Versioning.Revision)) == 0; }
        }

        /// <summary>
        ///     Gets the compatibility difference with the current game version.
        /// </summary>
        public int GameCompatibility
        {
            get { return this.gameVersion.CompareTo(new System.Version(Versioning.version_major, Versioning.version_minor, Versioning.Revision)); }
        }

        #endregion

        #region Private Methods

        private void ParseJson()
        {
            var data = JsonMapper.ToObject(this.json);

            // NAME
            try
            {
                this.name = (string)data["NAME"];
            }
            catch
            {
                MonoBehaviour.print("There was a problem reading the addon name from a version file.");
            }

            // URL
            try
            {
                this.url = (string)data["URL"];
            }
            catch
            {
                MonoBehaviour.print(this.Name + ": Does not include a remote version file URL.");
            }

            // DOWNLOAD
            try
            {
                this.download = (string)data["DOWNLOAD"];
            }
            catch
            {
                MonoBehaviour.print(this.Name + ": Does not include a download location.");
            }

            // VERSION
            try
            {
                this.addonVersion = ParseVersion(data["VERSION"]);
            }
            catch
            {
                MonoBehaviour.print(this.Name + ": Does not include a valid addon version.");
            }

            // KSP_VERSION
            try
            {
                this.gameVersion = ParseVersion(data["KSP_VERSION"]);
            }
            catch
            {
                MonoBehaviour.print(this.Name + ": Does not include a valid game version.");
            }
        }

        private static System.Version ParseVersion(JsonData data)
        {
            // Data is not an object so it must be a string value.
            if (!data.IsObject)
            {
                return new System.Version((string)data);
            }

            // Data is an object so it must contain MAJOR, MINOR, PATCH and BUILD values.
            try
            {
                switch (data.Count)
                {
                    case 2:
                        return new System.Version((int)data["MAJOR"], (int)data["MINOR"]);

                    case 3:
                        return new System.Version((int)data["MAJOR"], (int)data["MINOR"], (int)data["PATCH"]);

                    case 4:
                        return new System.Version((int)data["MAJOR"], (int)data["MINOR"], (int)data["PATCH"], (int)data["BUILD"]);

                    default:
                        return new System.Version();
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }
        }

        #endregion
    }
}