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

using UnityEngine;

#endregion

namespace KSP_AVC
{
    public class AddonInfo
    {
        #region Fields

        private static readonly VersionInfo actualKspVersion;

        private readonly string path;

        private VersionInfo kspVersion;
        private VersionInfo kspVersionMax;
        private VersionInfo kspVersionMin;
        
        #endregion

        #region Contructors

        static AddonInfo()
        {
            try
            {
                actualKspVersion = new VersionInfo(Versioning.version_major, Versioning.version_minor, Versioning.Revision);
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

        public string ChangeLog { get; private set; }

        public string ChangeLogUrl { get; private set; }

        public VersionInfo Version { get; private set; }

        public VersionInfo KspVersion
        {
            get { return this.kspVersion ?? actualKspVersion; }
        }

        public VersionInfo KspVersionMin
        {
            get { return this.kspVersionMin ?? VersionInfo.Min; }
        }

        public VersionInfo KspVersionMax
        {
            get { return this.kspVersionMax ?? VersionInfo.Max; }
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

        public bool IsCompatibleGitHubVersion
        {
            get { return this.GitHub == null || this.GitHub.Version == null || this.Version == this.GitHub.Version; }
        }

        public bool IsCompatible
        {
            get { return this.IsCompatibleKspVersion || ((this.kspVersionMin != null || this.kspVersionMax != null) && this.IsCompatibleKspVersionMin && this.IsCompatibleKspVersionMax); }
        }

        public GitHubInfo GitHub { get; private set; }

        public static VersionInfo ActualKspVersion
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
                    switch (key.ToUpper())
                    {
                        case "NAME":
                            this.Name = (string)data[key];
                            break;

                        case "URL":
                            this.Url = FormatCompatibleUrl((string)data[key]);
                            break;

                        case "DOWNLOAD":
                            this.Download = (string)data[key];
                            break;

                        case "CHANGE_LOG":
                            this.ChangeLog = (string)data[key];
                            break;

                        case "CHANGE_LOG_URL":
                            this.ChangeLogUrl = (string)data[key];
                            break;

                        case "GITHUB":
                            this.GitHub = new GitHubInfo(data[key], this);
                            break;

                        case "VERSION":
                            this.Version = GetVersion(data[key]);
                            break;

                        case "KSP_VERSION":
                            this.kspVersion = GetVersion(data[key]);
                            break;

                        case "KSP_VERSION_MIN":
                            this.kspVersionMin = GetVersion(data[key]);
                            break;

                        case "KSP_VERSION_MAX":
                            this.kspVersionMax = GetVersion(data[key]);
                            break;
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Exception(ex);
            }
        }

        private static VersionInfo GetVersion(object obj)
        {
            try
            {
                if (obj is Dictionary<string, object>)
                {
                    return ParseVersion(obj as Dictionary<string, object>);
                }
                return new VersionInfo((string)obj);
            }
            catch (Exception ex)
            {
                Logger.Exception(ex);
                return new VersionInfo();
            }
        }

        private static VersionInfo ParseVersion(Dictionary<string, object> data)
        {
            try
            {
                var version = new VersionInfo();

                foreach (var key in data.Keys)
                {
                    switch (key.ToUpper())
                    {
                        case "MAJOR":
                            version.Major = (long)data[key];
                            break;

                        case "MINOR":
                            version.Minor = (long)data[key];
                            break;

                        case "PATCH":
                            version.Patch = (long)data[key];
                            break;

                        case "BUILD":
                            version.Build = (long)data[key];
                            break;
                    }
                }

                return version;
            }
            catch (Exception ex)
            {
                Logger.Exception(ex);
                return new VersionInfo();
            }
        }

        private static string FormatCompatibleUrl(string url)
        {
            try
            {
                if (url.Contains("github.com"))
                {
                    return url.Replace("github.com", "raw.githubusercontent.com").Replace("/tree/", "/");
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

        #region Fetch Remote Data

        public void FetchRemoteData()
        {
            try
            {
                if (this.GitHub != null)
                {
                    this.GitHub.FetchRemoteData();
                }

                if (this.ChangeLogUrl != null)
                {
                    this.FetchChangeLog();
                }
            }
            catch (Exception ex)
            {
                Logger.Exception(ex);
            }
        }

        private void FetchChangeLog()
        {
            try
            {
                using (var www = new WWW(this.ChangeLogUrl))
                {
                    while (!www.isDone) { }
                    if (www.error == null)
                    {
                        this.ChangeLog = www.text;
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Exception(ex);
            }
        }

        #endregion

        #region Debugging

        public override string ToString()
        {
            try
            {
                return this.path +
                       "\n\tNAME: " + (string.IsNullOrEmpty(this.Name) ? "NULL (required)" : this.Name) +
                       "\n\tURL: " + (string.IsNullOrEmpty(this.Url) ? "NULL" : this.Url) +
                       "\n\tDOWNLOAD: " + (string.IsNullOrEmpty(this.Download) ? "NULL" : this.Download) +
                       "\n\tGITHUB: " + (this.GitHub != null ? this.GitHub.ToString() : "NULL") +
                       "\n\tVERSION: " + (this.Version != null ? this.Version.ToString() : "NULL (required)") +
                       "\n\tKSP_VERSION: " + this.KspVersion +
                       "\n\tKSP_VERSION_MIN: " + (this.kspVersionMin != null ? this.kspVersion.ToString() : "NULL") +
                       "\n\tKSP_VERSION_MAX: " + (this.kspVersionMax != null ? this.kspVersionMax.ToString() : "NULL") +
                       "\n\tCompatibleKspVersion: " + this.IsCompatibleKspVersion +
                       "\n\tCompatibleKspVersionMin: " + this.IsCompatibleKspVersionMin +
                       "\n\tCompatibleKspVersionMax: " + this.IsCompatibleKspVersionMax +
                       "\n\tCompatibleGitHubVersion: " + this.IsCompatibleGitHubVersion;
            }
            catch (Exception ex)
            {
                Logger.Exception(ex);
                return ex.Message;
            }
        }

        #endregion

        #region Nested type: GitHubInfo

        public class GitHubInfo
        {
            #region Fields

            private readonly AddonInfo addonInfo;

            #endregion

            #region Constructors

            public GitHubInfo(object obj, AddonInfo addonInfo)
            {
                try
                {
                    this.addonInfo = addonInfo;

                    var data = obj as Dictionary<string, object>;
                    if (data == null)
                    {
                        this.ParseError = true;
                        return;
                    }

                    foreach (var key in data.Keys)
                    {
                        switch (key)
                        {
                            case "USERNAME":
                                this.Username = (string)data[key];
                                break;

                            case "REPOSITORY":
                                this.Repository = (string)data[key];
                                break;

                            case "ALLOW_PRE_RELEASE":
                                this.AllowPreRelease = (bool)data[key];
                                break;
                        }
                    }
                }
                catch (Exception ex)
                {
                    Logger.Exception(ex);
                }
            }

            #endregion

            #region Properties

            public bool ParseError { get; private set; }
            public string Username { get; private set; }
            public string Repository { get; private set; }
            public string Tag { get; private set; }
            public bool AllowPreRelease { get; private set; }
            public VersionInfo Version { get; private set; }

            #endregion

            #region Private Methods

            private void Parse(string json)
            {
                try
                {
                    var obj = Json.Deserialize(json) as List<object>;
                    if (obj == null || obj.Count == 0)
                    {
                        this.ParseError = true;
                        return;
                    }

                    foreach (Dictionary<string, object> data in obj)
                    {
                        if (!this.AllowPreRelease && (bool)data["prerelease"])
                        {
                            continue;
                        }

                        var tag = (string)data["tag_name"];
                        var version = GetVersion(tag);

                        if (version == null || version <= this.Version)
                        {
                            continue;
                        }

                        this.Version = version;
                        this.Tag = tag;

                        if (string.IsNullOrEmpty(this.addonInfo.Download))
                        {
                            this.addonInfo.Download = "https://github.com/" + this.Username + "/" + this.Repository + "/releases/tag/" + this.Tag;
                        }
                    }
                }
                catch (Exception ex)
                {
                    Logger.Exception(ex);
                }
            }

            #endregion

            #region Public Methods

            public void FetchRemoteData()
            {
                try
                {
                    using (var www = new WWW("https://api.github.com/repos/" + this.Username + "/" + this.Repository + "/releases"))
                    {
                        while (!www.isDone) { }
                        if (www.error == null)
                        {
                            this.Parse(www.text);
                        }
                    }
                }
                catch (Exception ex)
                {
                    Logger.Exception(ex);
                }
            }

            #endregion

            #region Debug

            public override string ToString()
            {
                return this.Username + "/" + this.Repository +
                       "\n\t\tLatestRelease: " + (this.Version != null ? this.Version.ToString() : "NULL") +
                       "\n\t\tAllowPreRelease: " + this.AllowPreRelease;
            }

            #endregion
        }

        #endregion
    }
}