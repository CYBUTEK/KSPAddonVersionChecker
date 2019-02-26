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
using System.Linq;
using System.Threading;

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

        #region Constructors

        public AddonInfo(string path, string json, RemoteType remoteType)
        {
            // Following because some files are returning a few gibberish chars when downloading from Github
            json = DeleteCharsPrecedingBrace(json);

            try
            {
                this.path = path;
                switch (remoteType)
                {
                    case RemoteType.AVC:
                        this.ParseAvc(path, json);
                        break;

                    case RemoteType.KerbalStuff:
                        this.ParseKerbalStuff(json);
                        break;
                }
                
                ValidateKspMinMax();
            }
            catch
            {
                this.ParseError = true;
                this.AddParseErrorMsg = "Error parsing: " + path;
                throw;
            }
            finally
            {
                if (this.ParseError)
                {
                    Logger.Log("Version file contains errors: " + path);
                    foreach (var s in this.ParseErrorMsgs)
                        Logger.Log("Error: " + s);
                    throw new ArgumentException("Not a valid JSON object.");
                }
            }
        }

        string DeleteCharsPrecedingBrace(string json)
        {
            int i = json.IndexOf('{');
            if (i == 0)
                return json;
            if (i == -1)
                return "";
            return json.Substring(i);

        }
        static AddonInfo()
        {
            actualKspVersion = new VersionInfo(Versioning.version_major, Versioning.version_minor, Versioning.Revision);
        }

        #endregion

        #region Enums

        public enum RemoteType
        {
            AVC,
            KerbalStuff
        }

        #endregion

        #region Properties

        public static VersionInfo ActualKspVersion
        {
            get { return actualKspVersion; }
        }

        public AssemblyLoader.LoadedAssembly Assembly { get; private set; }

        public string ChangeLog { get; private set; }

        public string ChangeLogUrl { get; private set; }

        public string Download { get; private set; }

        public GitHubInfo GitHub { get; private set; }

        public bool IsCompatible
        {
            get {
                bool b = (this.IsCompatibleKspVersion && this.kspVersionMin == null && this.kspVersionMax == null)
                    || 
                    ((this.kspVersionMin != null || this.kspVersionMax != null) && this.IsCompatibleKspVersionMin && this.IsCompatibleKspVersionMax);
                
                return b;
            }
        }

        //Handles the Compatibility Override by versionnumbers, set in the AVC.cfg
        public bool IsForcedCompatible
        {
            get
            {
                if (this.IsCompatible || this.DisableOverrideGlobal || this.IgnoreOverrideSingle)
                    return false;

                bool compatible = (this.IsForcedCompatibleKspVersion && this.kspVersionMin == null && this.kspVersionMax == null)
                    ||
                    ((!this.KspVersionMinIsNull || !this.KspVersionMaxIsNull) && this.IsForcedCompatibleKspVersionMin && this.IsForcedCompatibleKspVersionMax)
                    ||
                    IsForcedCompatibleByName;
                return compatible;
            }
            //set
            //{
            //    IsForcedCompatible = value;
            //}
        }

        //Handles the Compatibility Override GUI settings
        public bool IsForcedCompatibleByName
        {
            get
            {
                bool isForcedCompatible = (from d in OverrideSettings.Instance.OverrideModCompatibility
                                           where d == this.Name
                                           select d).Any();

                return isForcedCompatible;
            }
            set
            {
                IsForcedCompatibleByName = value;
            }
        }

        public bool IsForcedCompatibleKspVersionMin
        {
            get
            {
                bool isForcedCompatible = (from d in Configuration.CompatibleVersions
                                     where KspVersionMin <= new VersionInfo(d.Key)
                                     select d.Value.compatWithVersion.Where(x => x == actualKspVersion)).Any();

                return isForcedCompatible;
            }
        }

        public bool IsForcedCompatibleKspVersionMax
        {
            get
            {
                bool isForcedCompatible = (from d in Configuration.CompatibleVersions
                                     where KspVersionMax >= new VersionInfo(d.Key)
                                     select d.Value.compatWithVersion.Where(x => x == actualKspVersion)).Any();

                return isForcedCompatible;
            }
        }

        public bool IsForcedCompatibleKspVersion
        {
            get
            {
                bool isForcedCompatible = (from d in Configuration.CompatibleVersions
                                     where new VersionInfo(d.Key) == KspVersion
                                     select d.Value.compatWithVersion.Where(x => x == actualKspVersion)).Any();

                return isForcedCompatible;
            }
        }

        //Checks for mods which need to ignore the Compatibility Override, this should be Kopernicus by default (set in AVC.cfg)
        public bool IgnoreOverrideSingle
        {
            get
            {
                bool onIgnoreList = false;
                foreach (var modName in Configuration.modsIgnoreOverride)
                {
                    if (modName == this.Name)
                        return true;
                }
                return onIgnoreList;
            }
        }

        public bool IsCompatibleGitHubVersion
        {
            get { return this.GitHub == null || this.GitHub.Version == null || this.Version == this.GitHub.Version; }
        }

        public bool IsCompatibleKspVersion
        {
            get
            {
                var b = Equals(this.KspVersion, actualKspVersion);
                
                if (!b)
                {
                    CompatVersions cv;
                    if (!Configuration.CompatibleVersions.TryGetValue(this.KspVersion.Version, out cv))
                        return false;
                    b = cv.compatibleWithVersion.Contains(actualKspVersion.Version);
                }
                return b;
            }
        }

        public bool IsCompatibleKspVersionMax
        {
            get { bool b = this.KspVersionMax >= actualKspVersion;
                return b; }
        }

        public bool IsCompatibleKspVersionMin
        {
            get { bool b = this.KspVersionMin <= actualKspVersion;
                return b;
            }
        }

        public string KerbalStuffUrl { get; private set; }

        public VersionInfo KspVersion
        {
            get { return this.kspVersion ?? VersionInfo.AnyValue; }
        }

        public bool KspVersionMaxIsNull
        {
            get { return this.kspVersionMax == null; }
        }

        public VersionInfo KspVersionMax
        {
            get { return this.kspVersionMax ?? actualKspVersion; }
        }

        public bool KspVersionMinIsNull {
            get { return this.kspVersionMin == null; }
        }

        public VersionInfo KspVersionMin
        {
            get { return this.kspVersionMin ?? VersionInfo.MinValue; }
        }

        public string Name { get; private set; }
        
        public LocalRemotePriority Priority { get; private set; }

        public bool DisableOverrideGlobal { get; private set; } //Enable/Disable the Compatibility Override feature, set in the AVC.cfg

        public bool ParseError { get; private set; }

        private List<string> parseErrorMsgs = new List<string>();
        public string AddParseErrorMsg { set { parseErrorMsgs.Add(value); } }
        internal List<string> ParseErrorMsgs { get { return parseErrorMsgs; } }

        public string Url { get; private set; }

        public VersionInfo Version { get; private set; }

        #endregion

        #region Methods: public

        public void FetchRemoteData()
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

        public override string ToString()
        {
            return this.path +
                   "\n\tNAME: " + (String.IsNullOrEmpty(this.Name) ? "NULL (required)" : this.Name) +
                   "\n\tURL: " + (String.IsNullOrEmpty(this.Url) ? "NULL" : this.Url) +
                   "\n\tDOWNLOAD: " + (String.IsNullOrEmpty(this.Download) ? "NULL" : this.Download) +
                   "\n\tGITHUB: " + (this.GitHub != null ? this.GitHub.ToString() : "NULL") +
                   "\n\tVERSION: " + (this.Version != null ? this.Version.ToString() : "NULL (required)") +
                   "\n\tKSP_VERSION: " + this.KspVersion +
                   "\n\tKSP_VERSION_MIN: " + (this.kspVersionMin != null ? this.kspVersionMin.ToString() : "NULL") +
                   "\n\tKSP_VERSION_MAX: " + (this.kspVersionMax != null ? this.kspVersionMax.ToString() : "NULL") +
                   "\n\tCompatibleKspVersion: " + this.IsCompatibleKspVersion +
                   "\n\tCompatibleKspVersionMin: " + this.IsCompatibleKspVersionMin +
                   "\n\tCompatibleKspVersionMax: " + this.IsCompatibleKspVersionMax +
                   "\n\tCompatibleGitHubVersion: " + this.IsCompatibleGitHubVersion;
        }

        #endregion

        #region Methods: private

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

        private void FetchChangeLog()
        {
            using (var www = new WWW(this.ChangeLogUrl))
            {
                while (!www.isDone)
                {
                    Thread.Sleep(100);
                }
                if (www.error == null)
                {
                    this.ChangeLog = www.text;
                }
            }
        }

        private void ParseAvc(string path, string json)
        {
            var data = Json.Deserialize(json) as Dictionary<string, object>;
            if (data == null)
            {
                this.ParseError = true;
                this.AddParseErrorMsg = "Error in Json.Deserialize, file: " + path;

                throw new ArgumentException("Not a valid JSON object.");
            }
            foreach (var key in data.Keys)
            {
                switch (key.ToUpper())
                {
 
                    case "LOCAL_HAS_PRIORITY":
                        {
                            string s = (string)data[key];
                            switch (s.ToUpper())
                            {
                                case "TRUE":
                                    this.Priority = LocalRemotePriority.local;
                                    break;
                                case "FALSE":
                                    this.Priority = LocalRemotePriority.remote;
                                    break;
                            }
                        }
                        break;

                    case "REMOTE_HAS_PRIORITY":
                        {
                            string s = (string)data[key];
                            switch (s.ToUpper())
                            {
                                case "TRUE":
                                    this.Priority = LocalRemotePriority.remote;
                                    break;
                                case "FALSE":
                                    this.Priority = LocalRemotePriority.local;
                                    break;
                            }
                        }
                        break;

                    case "DISABLE_COMPATIBLE_VERSION_OVERRIDE":
                        this.DisableOverrideGlobal = (bool)data[key];
                        break;

                    case "NAME":
                        this.Name = (string)data[key];
                        break;

                    case "KERBAL_STUFF_URL":
                        this.KerbalStuffUrl = (string)data[key];
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

                    case "ASSEMBLY_NAME":
                        this.Assembly = AssemblyLoader.loadedAssemblies.FirstOrDefault(a => a.name == (string)data[key]);
                        if (this.Assembly != null)
                        {
                            this.Version = new VersionInfo(this.Assembly.assembly.GetName().Version);
                        }
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

        private void ValidateKspMinMax()
        {
            Debug.Log("ValidateKspMinMax, KspVersionMin: " + KspVersionMin + ", KspVersionMax: " + KspVersionMax);
            if ( KspVersionMin > KspVersionMax)
            {
                this.ParseError = true;
                this.AddParseErrorMsg = "KSP_VERSION_MIN greater than KSP_VERSION_MAX";
                throw new ArgumentException("KSP_VERSION_MIN greater than KSP_VERSION_MAX");
            }

            
        }

        private void ParseKerbalStuff(string json)
        {
            var data = Json.Deserialize(json) as Dictionary<string, object>;
            if (data == null)
            {
                this.ParseError = true;
                this.AddParseErrorMsg = "No data from Json (kerbalstuff)";
                throw new ArgumentException("No data from Json (kerbalstuff)");
            }

            this.Name = (string)data["name"];
        }

        private void ParseKerbalStuffVersion(Dictionary<string, object> data)
        {
            foreach (var key in data.Keys)
            {
                switch (key.ToUpper())
                {
                    case "FRIENDLY_VERSION":
                        this.Version = GetVersion(data[key]);
                        break;

                    case "KSP_VERSION":
                        this.kspVersion = GetVersion(data[key]);
                        break;
                }
            }
        }

        #endregion

        #region Nested Type: GitHubInfo

        public class GitHubInfo
        {
            #region Fields

            private readonly AddonInfo addonInfo;

            #endregion

            #region Constructors

            public GitHubInfo(object obj, AddonInfo addonInfo)
            {
                this.addonInfo = addonInfo;
                this.ParseJson(obj);
            }

            #endregion

            #region Properties

            public bool AllowPreRelease { get; private set; }

            public bool ParseError { get; private set; }
            private List<string> parseErrorMsgs = new List<string>();
            public string AddParseErrorMsg { set { parseErrorMsgs.Add(value); } }
            internal List<string> ParseErrorMsgs { get { return parseErrorMsgs; } }

            public string Repository { get; private set; }

            public string Tag { get; private set; }

            public string Username { get; private set; }

            public VersionInfo Version { get; private set; }

            #endregion

            #region Methods: public

            public void FetchRemoteData()
            {
                try
                {
                    using (var www = new WWW("https://api.github.com/repos/" + this.Username + "/" + this.Repository + "/releases"))
                    {
                        while (!www.isDone)
                        {
                            Thread.Sleep(100);
                        }
                        if (www.error == null)
                        {
                            this.ParseGitHubJson(www.text);
                        }
                    }
                }
                catch (Exception ex)
                {
                    Logger.Exception(ex);
                    this.AddParseErrorMsg = "Error fetching data from Github: " + "https://api.github.com/repos/" + this.Username + "/" + this.Repository + "/releases";
                }
            }

            public override string ToString()
            {
                return this.Username + "/" + this.Repository +
                       "\n\t\tLatestRelease: " + (this.Version != null ? this.Version.ToString() : "NULL") +
                       "\n\t\tAllowPreRelease: " + this.AllowPreRelease;
            }

            #endregion

            #region Methods: private

            private void ParseGitHubJson(string json)
            {
                var obj = Json.Deserialize(json) as List<object>;
                if (obj == null || obj.Count == 0)
                {
                    this.ParseError = true;
                    this.AddParseErrorMsg = "No data after parsing Github Json";
                    throw new ArgumentException("No data after parsing Github Json");
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

                    if (String.IsNullOrEmpty(this.addonInfo.Download))
                    {
                        this.addonInfo.Download = "https://github.com/" + this.Username + "/" + this.Repository + "/releases/tag/" + this.Tag;
                    }
                }
            }

            private void ParseJson(object obj)
            {
                var data = obj as Dictionary<string, object>;
                if (data == null)
                {
                    this.ParseError = true;
                    this.AddParseErrorMsg = "No data in dictionary (ParseJson)";
                    throw new ArgumentException("No data in dictionary (ParseJson)");
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

            #endregion
        }

        #endregion
    }
}