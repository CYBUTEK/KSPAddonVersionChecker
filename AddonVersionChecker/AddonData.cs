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

#endregion

namespace AddonVersionChecker
{
    public class AddonData
    {
        #region Fields

        private readonly System.Version currentGameVersion = CurrentGameVersion;
        private readonly string filename = string.Empty;
        private readonly string json = string.Empty;
        private System.Version addonVersion = new System.Version();
        private string download = string.Empty;
        private System.Version gameVersion = CurrentGameVersion;
        private System.Version gameVersionMaximum = DefaultMaximumVersion;
        private System.Version gameVersionMinimum = DefaultMinimumVersion;
        private string name = string.Empty;
        private AddonData remoteAddonData;
        private string url = string.Empty;

        #endregion

        #region Constructors

        public AddonData(string json, string filename)
        {
            this.filename = filename;
            this.json = json;
            this.ParseJson();
            this.remoteAddonData = this;
        }

        #endregion

        #region Properties

        /// <summary>
        ///     Gets the version filename including path.
        /// </summary>
        public string FileName
        {
            get { return this.filename; }
        }

        /// <summary>
        ///     Gets the add-on name.
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
        ///     Gets the game version for which the add-on was created to run on.
        /// </summary>
        public System.Version GameVersion
        {
            get { return this.gameVersion; }
        }

        /// <summary>
        ///     Gets the minimum game version for which the add-on was created to run on.
        /// </summary>
        public System.Version GameVersionMinimum
        {
            get { return this.gameVersionMinimum; }
        }

        /// <summary>
        ///     Gets the maximum game version for which the add-on was created to run on.
        /// </summary>
        public System.Version GameVersionMaximum
        {
            get { return this.gameVersionMaximum; }
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
            get { return this.addonVersion.CompareTo(this.remoteAddonData.addonVersion) < 0 && this.remoteAddonData.GameCompatible; }
        }

        /// <summary>
        ///     Gets whether the add-on is compatible with the current game version.
        /// </summary>
        public bool GameCompatible
        {
            get { return this.GameCompatibleVersion && this.GameCompatibleMininmum && this.GameCompatibleMaximum; }
        }

        /// <summary>
        ///     Gets whether the add-on is compatible with only the current game version.
        /// </summary>
        public bool GameCompatibleVersion
        {
            get { return this.gameVersion.CompareTo(this.currentGameVersion) == 0; }
        }

        /// <summary>
        ///     Gets whether the add-on is compatible with a game version of the same or more than the minimum.
        /// </summary>
        public bool GameCompatibleMininmum
        {
            get { return this.gameVersionMinimum.CompareTo(this.currentGameVersion) <= 0; }
        }

        /// <summary>
        ///     Gets whether the add-on is compatible with a game version of the same of less than the maximum.
        /// </summary>
        public bool GameCompatibleMaximum
        {
            get { return this.gameVersionMaximum.CompareTo(this.currentGameVersion) >= 0; }
        }

        public static System.Version DefaultMinimumVersion
        {
            get { return new System.Version(); }
        }

        public static System.Version DefaultMaximumVersion
        {
            get { return new System.Version(int.MaxValue, int.MaxValue, int.MaxValue, int.MaxValue); }
        }

        /// <summary>
        ///     Gets the current game version.
        /// </summary>
        public static System.Version CurrentGameVersion
        {
            get
            {
                return Versioning.Revision == 0
                    ? new System.Version(Versioning.version_major, Versioning.version_minor)
                    : new System.Version(Versioning.version_major, Versioning.version_minor, Versioning.Revision);
            }
        }

        #endregion

        #region Private Methods

        private void ParseJson()
        {
            try
            {
                var data = JsonMapper.ToObject(this.json);

                this.SetPrimaryFields(data);
                this.SetGameVersion(data);
            }
            catch (Exception ex)
            {
                Logger.Exception(ex, "AddonData->ParseJson");
            }
        }

        private void SetPrimaryFields(JsonData data)
        {
            // NAME
            try
            {
                this.name = (string)data["NAME"];
                Logger.Log(this.filename.Replace(UrlDir.ApplicationRootPath, string.Empty) + " \"NAME\" = " + this.name);
            }
            catch
            {
                Logger.Log(this.filename.Replace(UrlDir.ApplicationRootPath, string.Empty) + " \"NAME\" is not valid or missing. (required field)");
            }

            // URL
            try
            {
                this.url = (string)data["URL"];
                Logger.Log(this.filename.Replace(UrlDir.ApplicationRootPath, string.Empty) + " \"URL\" = " + this.url);
            }
            catch
            {
                Logger.Log(this.filename.Replace(UrlDir.ApplicationRootPath, string.Empty) + " \"URL\" is not valid or missing. (required field)");
            }

            // DOWNLOAD
            try
            {
                this.download = (string)data["DOWNLOAD"];
                Logger.Log(this.filename.Replace(UrlDir.ApplicationRootPath, string.Empty) + " \"DOWNLOAD\" = " + this.download);
            }
            catch
            {
                Logger.Log(this.filename.Replace(UrlDir.ApplicationRootPath, string.Empty) + " \"DOWNLOAD\" is not valid or missing. (optional field)");
            }

            // VERSION
            try
            {
                this.addonVersion = this.ParseVersion(data["VERSION"]);
                Logger.Log(this.filename.Replace(UrlDir.ApplicationRootPath, string.Empty) + " \"VERSION\" = " + this.addonVersion);
            }
            catch
            {
                Logger.Log(this.filename.Replace(UrlDir.ApplicationRootPath, string.Empty) + " \"VERSION\" is not valid or missing. (required field)");
            }
        }

        private void SetGameVersion(JsonData data)
        {
            // KSP_VERSION
            try
            {
                this.gameVersion = this.ParseVersion(data["KSP_VERSION"]);
                Logger.Log(this.filename.Replace(UrlDir.ApplicationRootPath, string.Empty) + " \"KSP_VERSION\" = " + this.gameVersion);
            }
            catch
            {
                Logger.Log(this.filename.Replace(UrlDir.ApplicationRootPath, string.Empty) + " \"KSP_VERSION\" is not valid or missing. (optional field)");
            }

            // KSP_VERSION_MIN
            try
            {
                this.gameVersionMinimum = this.ParseVersion(data["KSP_VERSION_MIN"]);
                Logger.Log(this.filename.Replace(UrlDir.ApplicationRootPath, string.Empty) + " \"KSP_VERSION_MIN\" = " + this.gameVersionMinimum);
            }
            catch
            {
                Logger.Log(this.filename.Replace(UrlDir.ApplicationRootPath, string.Empty) + " \"KSP_VERSION_MIN\" is not valid or missing. (optional field)");
            }

            // KSP_VERSION_MAX
            try
            {
                this.gameVersionMaximum = this.ParseVersion(data["KSP_VERSION_MAX"]);
                Logger.Log(this.filename.Replace(UrlDir.ApplicationRootPath, string.Empty) + " \"KSP_VERSION_MAX\" = " + this.gameVersionMaximum);
            }
            catch
            {
                Logger.Log(this.filename.Replace(UrlDir.ApplicationRootPath, string.Empty) + " \"KSP_VERSION_MAX\" is not valid or missing. (optional field)");
            }
        }

        private System.Version ParseVersion(JsonData data)
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
                        return (int)data["PATCH"] == 0
                            ? new System.Version((int)data["MAJOR"], (int)data["MINOR"])
                            : new System.Version((int)data["MAJOR"], (int)data["MINOR"], (int)data["PATCH"]);

                    case 4:
                        return (int)data["PATCH"] == 0
                            ? new System.Version((int)data["MAJOR"], (int)data["MINOR"])
                            : (int)data["BUILD"] == 0
                                ? new System.Version((int)data["MAJOR"], (int)data["MINOR"], (int)data["PATCH"])
                                : new System.Version((int)data["MAJOR"], (int)data["MINOR"], (int)data["PATCH"], (int)data["BUILD"]);

                    default:
                        return new System.Version();
                }
            }
            catch (Exception ex)
            {
                Logger.Log(this.filename.Replace(UrlDir.ApplicationRootPath, string.Empty) + " a problem was encountered whilst parsing a version.");
                throw new Exception(ex.Message, ex);
            }
        }

        #endregion
    }
}