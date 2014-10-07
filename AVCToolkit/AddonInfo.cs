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

using AVCToolkit.Json;

#endregion

namespace AVCToolkit
{
    public class AddonInfo
    {
        #region Fields

        private GitHubInfo gitHub = new GitHubInfo();

        #endregion

        #region Properties

        [JsonField("CHANGE_LOG", Order = 4)]
        public string ChangeLog { get; set; }

        [JsonField("CHANGE_LOG_URL", Order = 5)]
        public string ChangeLogUrl { get; set; }

        [JsonField("DOWNLOAD", Order = 3)]
        public string Download { get; set; }

        [JsonField("GITHUB", Order = 6)]
        public GitHubInfo GitHub
        {
            get { return this.gitHub; }
            set { this.gitHub = value; }
        }

        [JsonField("KSP_VERSION", Order = 8)]
        public VersionInfo KspVersion { get; set; }

        [JsonField("KSP_VERSION_MAX", Order = 10)]
        public VersionInfo KspVersionMax { get; set; }

        [JsonField("KSP_VERSION_MIN", Order = 9)]
        public VersionInfo KspVersionMin { get; set; }

        [JsonField("NAME", Order = 1)]
        public string Name { get; set; }

        [JsonField("URL", Order = 2)]
        public string Url { get; set; }

        [JsonField("VERSION", Order = 7)]
        public VersionInfo Version { get; set; }

        #endregion
    }
}