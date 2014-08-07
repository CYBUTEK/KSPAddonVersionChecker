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

#endregion

namespace MiniAVC
{
    public class AddonInfo
    {
        #region Fields

        private static readonly System.Version actualKspVersion;
        private static readonly System.Version defaultMinVersion = new System.Version();
        private static readonly System.Version defaultMaxVersion = new System.Version(int.MaxValue, int.MaxValue, int.MaxValue, int.MaxValue);
        private readonly string path;

        private System.Version kspVersion;
        private System.Version kspVersionMax;
        private System.Version kspVersionMin;

        #endregion

        #region Contructors

        static AddonInfo()
        {
            try
            {
                actualKspVersion = new System.Version(Versioning.version_major, Versioning.version_minor, Versioning.Revision);
            }
            catch (Exception ex)
            {
                Logger.Exception(ex);
            }
        }

        public AddonInfo(string path, string json)
        {
            try
            {
                this.path = path;
                this.Parse(json);
            }
            catch (Exception ex)
            {
                Logger.Exception(ex);
                this.ParseError = true;
            }

            if (this.ParseError)
            {
                Logger.Log("Version file contains errors: " + path);
            }
        }

        #endregion

        #region Properties

        public bool ParseError { get; private set; }

        public string Name { get; private set; }

        public string Url { get; private set; }

        public string Download { get; private set; }

        public System.Version Version { get; private set; }

        public System.Version KspVersion
        {
            get { return (this.kspVersion ?? actualKspVersion); }
        }

        public System.Version KspVersionMin
        {
            get { return (this.kspVersionMin ?? defaultMinVersion); }
        }

        public System.Version KspVersionMax
        {
            get { return (this.kspVersionMax ?? defaultMaxVersion); }
        }

        public bool IsCompatibleKspVersion
        {
            get { return Equals(this.KspVersion, actualKspVersion); }
        }

        public bool IsCompatibleKspVersionMin
        {
            get { return this.KspVersionMin <= actualKspVersion; }
        }

        public bool IsCompatibleKspVersionMax
        {
            get { return this.KspVersionMax >= actualKspVersion; }
        }

        public bool IsCompatible
        {
            get { return this.IsCompatibleKspVersion || ((this.kspVersionMin != null || this.kspVersionMax != null) && this.IsCompatibleKspVersionMin && this.IsCompatibleKspVersionMax); }
        }

        public static System.Version ActualKspVersion
        {
            get { return actualKspVersion; }
        }

        #endregion

        #region Parse Json

        private void Parse(string json)
        {
            try
            {
                var data = Json.Deserialize(json) as Dictionary<string, object>;
                if (data == null)
                {
                    this.ParseError = true;
                    return;
                }
                foreach (var key in data.Keys)
                {
                    switch (key)
                    {
                        case "NAME":
                            this.Name = (string)data["NAME"];
                            break;

                        case "URL":
                            this.Url = this.FormatCompatibleUrl((string)data["URL"]);
                            break;

                        case "DOWNLOAD":
                            this.Download = (string)data["DOWNLOAD"];
                            break;

                        case "VERSION":
                            this.Version = this.GetVersion(data["VERSION"]);
                            break;

                        case "KSP_VERSION":
                            this.kspVersion = this.GetVersion(data["KSP_VERSION"]);
                            break;

                        case "KSP_VERSION_MIN":
                            this.kspVersionMin = this.GetVersion(data["KSP_VERSION_MIN"]);
                            break;

                        case "KSP_VERSION_MAX":
                            this.kspVersionMax = this.GetVersion(data["KSP_VERSION_MAX"]);
                            break;
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Exception(ex);
            }
        }

        private System.Version GetVersion(object data)
        {
            try
            {
                if (data is Dictionary<string, object>)
                {
                    var dataVersion = data as Dictionary<string, object>;

                    switch (dataVersion.Count)
                    {
                        case 2:
                            return new System.Version((int)(long)dataVersion["MAJOR"], (int)(long)dataVersion["MINOR"]);

                        case 3:
                            return (int)(long)dataVersion["PATCH"] == 0
                                ? new System.Version((int)(long)dataVersion["MAJOR"], (int)(long)dataVersion["MINOR"])
                                : new System.Version((int)(long)dataVersion["MAJOR"], (int)(long)dataVersion["MINOR"], (int)(long)dataVersion["PATCH"]);

                        case 4:
                            return (int)(long)dataVersion["BUILD"] == 0 ? (int)(long)dataVersion["PATCH"] == 0
                                ? new System.Version((int)(long)dataVersion["MAJOR"], (int)(long)dataVersion["MINOR"])
                                : new System.Version((int)(long)dataVersion["MAJOR"], (int)(long)dataVersion["MINOR"], (int)(long)dataVersion["PATCH"])
                                : new System.Version((int)(long)dataVersion["MAJOR"], (int)(long)dataVersion["MINOR"], (int)(long)dataVersion["PATCH"], (int)(long)dataVersion["BUILD"]);

                        default:
                            return null;
                    }
                }
                return new System.Version((string)data);
            }
            catch (Exception ex)
            {
                Logger.Exception(ex);
                return new System.Version();
            }
        }

        private string FormatCompatibleUrl(string url)
        {
            try
            {
                if (url.Contains("github.com"))
                {
                    return url.Replace("github.com", "raw.githubusercontent.com");
                }

                return url;
            }
            catch (Exception ex)
            {
                Logger.Exception(ex);
                return url;
            }
        }

        #endregion

        #region Debugging

        public override string ToString()
        {
            return this.path +
                   "\n\tNAME: " + (string.IsNullOrEmpty(this.Name) ? "NULL (required)" : this.Name) +
                   "\n\tURL: " + (string.IsNullOrEmpty(this.Url) ? "NULL" : this.Url) +
                   "\n\tDOWNLOAD: " + (string.IsNullOrEmpty(this.Download) ? "NULL" : this.Download) +
                   "\n\tVERSION: " + (this.Version != null ? this.Version.ToString() : "NULL (required)") +
                   "\n\tKSP_VERSION: " + this.KspVersion +
                   "\n\tKSP_VERSION_MIN: " + (this.kspVersionMin != null ? this.kspVersion.ToString() : "NULL") +
                   "\n\tKSP_VERSION_MAX: " + (this.kspVersionMax != null ? this.kspVersionMax.ToString() : "NULL") +
                   "\n\tCompatibleKspVersion: " + this.IsCompatibleKspVersion +
                   "\n\tCompatibleKspVersionMin: " + this.IsCompatibleKspVersionMin +
                   "\n\tCompatibleKspVersionMax: " + this.IsCompatibleKspVersionMax;
        }

        #endregion
    }
}