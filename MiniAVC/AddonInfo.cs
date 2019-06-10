// Copyright (C) 2014 CYBUTEK
//
// This program is free software: you can redistribute it and/or modify it under the terms of the GNU
// General Public License as published by the Free Software Foundation, either version 3 of the
// License, or (at your option) any later version.
//
// This program is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without
// even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU
// General Public License for more details.
//
// You should have received a copy of the GNU General Public License along with this program. If not,
// see <http://www.gnu.org/licenses/>.

using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;

using UnityEngine;

namespace MiniAVC
{
    public class AddonInfo
    {
        private static readonly VersionInfo actualKspVersion;

        private readonly string path;

        private VersionInfo kspVersion;
        private VersionInfo kspVersionMax;
        private VersionInfo kspVersionMin;
        private List<VersionInfo> kspExcludeVersions = null;
        private List<VersionInfo> kspIncludeVersions = null;

        static AddonInfo()
        {
            actualKspVersion = new VersionInfo(Versioning.version_major, Versioning.version_minor, Versioning.Revision);
        }

        public AddonInfo(string path, string json)
        {
            try
            {
                this.path = path;
                Base64String = Regex.Replace(Convert.ToBase64String(Encoding.ASCII.GetBytes(json)), @"\s+", string.Empty); ;
                Parse(json);
            }
            catch
            {
                ParseError = true;
                throw;
            }
            finally
            {
                if (ParseError)
                {
                    Logger.Log("Version file contains errors: " + path);
                }
            }
        }

        public static VersionInfo ActualKspVersion
        {
            get { return actualKspVersion; }
        }

        public string Base64String { get; private set; } = string.Empty;

        public string Download { get; private set; }

        public GitHubInfo GitHub { get; private set; }

        public bool IsCompatible
        {
            get
            {
                if (kspIncludeVersions != null && this.IsKspIncludedVersion)
                    return true;
                bool b = (this.IsCompatibleKspVersion && this.kspVersionMin == null && this.kspVersionMax == null)
                 ||
                 ((this.kspVersionMin != null || this.kspVersionMax != null) && this.IsCompatibleKspVersionMin && this.IsCompatibleKspVersionMax);
                if (b)
                {
                    if (kspExcludeVersions != null && this.IsKspExcludedVersion)
                        b = false;
                }
                return b;
            }
        }

        public bool IsCompatibleGitHubVersion
        {
            get { return GitHub == null || GitHub.Version == null || Version == GitHub.Version; }
        }

        public bool IsCompatibleKspVersion
        {
            get { return Equals(KspVersion, actualKspVersion); }
        }

        public bool IsCompatibleKspVersionMax
        {
            get { return KspVersionMax >= actualKspVersion; }
        }

        public bool IsCompatibleKspVersionMin
        {
            get { return KspVersionMin <= actualKspVersion; }
        }

        public VersionInfo KspVersion
        {
            get { return (kspVersion ?? actualKspVersion); }
        }

        public VersionInfo KspVersionMax
        {
            get { return (kspVersionMax ?? VersionInfo.MaxValue); }
        }

        public VersionInfo KspVersionMin
        {
            get { return (kspVersionMin ?? VersionInfo.MinValue); }
        }
        public bool IsKspExcludedVersion
        {
            get
            {
                bool b = this.kspExcludeVersions.Contains(actualKspVersion);
                return b;
            }
        }
        public bool IsKspIncludedVersion
        {
            get
            {
                if (this.kspIncludeVersions == null)
                    return false;
                bool b = this.kspIncludeVersions.Contains(actualKspVersion);
                return b;
            }
        }
        public bool KspExcludeVersionIsNull { get { return this.kspExcludeVersions == null; } }

        public string Name { get; private set; }

        public bool ParseError { get; private set; }

        public string Url { get; private set; }

        public VersionInfo Version { get; private set; }

        public void FetchRemoteData()
        {
            if (GitHub != null)
            {
                GitHub.FetchRemoteData();
            }
        }

        public override string ToString()
        {
            string str = path +
                   "\n\tNAME: " + (String.IsNullOrEmpty(Name) ? "NULL (required)" : Name) +
                   "\n\tURL: " + (String.IsNullOrEmpty(Url) ? "NULL" : Url) +
                   "\n\tDOWNLOAD: " + (String.IsNullOrEmpty(Download) ? "NULL" : Download) +
                   "\n\tGITHUB: " + (GitHub != null ? GitHub.ToString() : "NULL") +
                   "\n\tVERSION: " + (Version != null ? Version.ToString() : "NULL (required)") +
                   "\n\tKSP_VERSION: " + KspVersion +
                   "\n\tKSP_VERSION_MIN: " + (kspVersionMin != null ? kspVersionMin.ToString() : "NULL") +
                   "\n\tKSP_VERSION_MAX: " + (kspVersionMax != null ? kspVersionMax.ToString() : "NULL");
            if (kspExcludeVersions != null)
            {
                str += "\n\tKSP_VERSION_EXCLUDE:";
                foreach (var s in kspExcludeVersions)
                {
                    str += "\n\t\t" + s;
                }
            }
            str +=
                   "\n\tCompatibleKspVersion: " + IsCompatibleKspVersion +
                   "\n\tCompatibleKspVersionMin: " + IsCompatibleKspVersionMin +
                   "\n\tCompatibleKspVersionMax: " + IsCompatibleKspVersionMax +
                   "\n\tCompatibleGitHubVersion: " + IsCompatibleGitHubVersion;
            return str;
        }

        private static string FormatCompatibleUrl(string url)
        {
            if (!url.Contains("github.com"))
            {
                return url;
            }

            url = url.Replace("github.com", "raw.githubusercontent.com");
            url = url.Replace("/tree/", "/");
            url = url.Replace("/blob/", "/");
            return url;
        }

        private static VersionInfo GetVersion(object obj)
        {
            if (obj is Dictionary<string, object>)
            {
                return ParseVersion(obj as Dictionary<string, object>);
            }
            return new VersionInfo((string)obj);
        }

        private static VersionInfo ParseVersion(Dictionary<string, object> data)
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

        private void Parse(string json)
        {
            var data = MiniAVC.Json.Deserialize(json) as Dictionary<string, object>;
            if (data == null)
            {
                ParseError = true;
                return;
            }
            foreach (var key in data.Keys)
            {
                switch (key.ToUpper())
                {
                    case "NAME":
                        Name = (string)data[key];
                        break;

                    case "URL":
                        Url = FormatCompatibleUrl((string)data[key]);
                        break;

                    case "DOWNLOAD":
                        Download = (string)data[key];
                        break;

                    case "GITHUB":
                        GitHub = new GitHubInfo(data[key], this);
                        break;

                    case "VERSION":
                        Version = GetVersion(data[key]);
                        break;

                    case "KSP_VERSION":
                        kspVersion = GetVersion(data[key]);
                        break;

                    case "KSP_VERSION_MIN":
                        kspVersionMin = GetVersion(data[key]);
                        break;

                    case "KSP_VERSION_MAX":
                        kspVersionMax = GetVersion(data[key]);
                        break;
                    case "KSP_VERSION_EXCLUDE":
                        kspExcludeVersions = new List<VersionInfo>();
                        List<System.Object> ExcludeList = (List<System.Object>)data[key];
                        foreach (System.Object el in ExcludeList)
                        {
                            var s = GetVersion(el);
                            kspExcludeVersions.Add(s);
                        }
                        break;
                }
            }
        }

        public class GitHubInfo
        {
            private readonly AddonInfo addonInfo;

            public GitHubInfo(object obj, AddonInfo addonInfo)
            {
                this.addonInfo = addonInfo;
                ParseJson(obj);
            }

            public bool AllowPreRelease { get; private set; }

            public bool ParseError { get; private set; }

            public string Repository { get; private set; }

            public string Tag { get; private set; }

            public string Username { get; private set; }

            public VersionInfo Version { get; private set; }

            public void FetchRemoteData()
            {
                try
                {
                    using (var www = new WWW("https://api.github.com/repos/" + Username + "/" + Repository + "/releases"))
                    {
                        while (!www.isDone)
                        {
                            Thread.Sleep(100);
                        }
                        if (www.error == null)
                        {
                            ParseGitHubJson(www.text);
                        }
                    }
                }
                catch (Exception ex)
                {
                    Logger.Exception(ex);
                }
            }

            public override string ToString()
            {
                return Username + "/" + Repository +
                       "\n\t\tLatestRelease: " + (Version != null ? Version.ToString() : "NULL") +
                       "\n\t\tAllowPreRelease: " + AllowPreRelease;
            }

            private void ParseGitHubJson(string json)
            {
                try
                {
                    var obj = MiniAVC.Json.Deserialize(json) as List<object>;
                    if (obj == null || obj.Count == 0)
                    {
                        ParseError = true;
                        return;
                    }

                    foreach (Dictionary<string, object> data in obj)
                    {
                        if (!AllowPreRelease && (bool)data["prerelease"])
                        {
                            continue;
                        }

                        var tag = (string)data["tag_name"];
                        var version = GetVersion(data["tag_name"]);

                        if (version == null || version <= Version)
                        {
                            continue;
                        }

                        Version = version;
                        Tag = tag;

                        if (String.IsNullOrEmpty(addonInfo.Download))
                        {
                            addonInfo.Download = "https://github.com/" + Username + "/" + Repository + "/releases/tag/" + Tag;
                        }
                    }
                }
                catch (Exception ex)
                {
                    Logger.Exception(ex);
                }
            }

            private void ParseJson(object obj)
            {
                var data = obj as Dictionary<string, object>;
                if (data == null)
                {
                    ParseError = true;
                    return;
                }

                foreach (var key in data.Keys)
                {
                    switch (key)
                    {
                        case "USERNAME":
                            Username = (string)data[key];
                            break;

                        case "REPOSITORY":
                            Repository = (string)data[key];
                            break;

                        case "ALLOW_PRE_RELEASE":
                            AllowPreRelease = (bool)data[key];
                            break;
                    }
                }
            }
        }
    }
}