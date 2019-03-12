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
using System.Threading;

using UnityEngine;

#endregion

namespace KSP_AVC
{
    public class Addon
    {
        #region Constructors

        public Addon(string path)
        {
            this.RunProcessLocalInfo(path);
        }

        #endregion

        #region Properties

        public bool HasError { get; private set; }

        public bool TriggerIssueGui
        {
            get
            {
                return this.LocalInfo.TriggerIssueGui;
            }
        }

        public bool IsCompatible
        {
            get { return (this.IsLocalReady && this.LocalInfo.IsCompatible); }

        }

        public bool IsForcedCompatibleByVersion
        {
            get
            {
                return this.LocalInfo.IsForcedCompatibleByVersion;
            }
        }

        public bool IsForcedCompatibleByName
        {
            get
            {
                return this.LocalInfo.IsForcedCompatibleByName;
            }
        }

        public bool IsLockedByCreator
        {
            get { return this.LocalInfo.IsLockedByCreator; }
        }

        public bool IsLocalReady { get; private set; }

        public bool IsProcessingComplete { get; private set; }

        public bool IsRemoteReady { get; private set; }

        public bool IsUpdateAvailable
        {
            get {
                bool b = this.IsProcessingComplete && 
                    this.LocalInfo.Version != null && 
                    this.RemoteInfo.Version != null && 
                    this.RemoteInfo.Version > this.LocalInfo.Version && 
                    //this.RemoteInfo.IsCompatibleKspVersion && 
                    this.RemoteInfo.IsCompatible &&
                    this.RemoteInfo.IsCompatibleGitHubVersion;

                return b;

            }
        }

        public AddonInfo LocalInfo { get; private set; }

        public string Name
        {
            get { return this.LocalInfo.Name; }
        }

        public AddonInfo RemoteInfo { get; private set; }

        #endregion

        #region Methods: public

        public void RunProcessLocalInfo(string path)
        {
            ThreadPool.QueueUserWorkItem(this.ProcessLocalInfo, path);
        }

        public void RunProcessRemoteInfo()
        {
            ThreadPool.QueueUserWorkItem(this.ProcessRemoteInfo);
        }

        #endregion

        #region Methods: private

        private void FetchLocalInfo(string path)
        {
            using (var stream = new StreamReader(File.OpenRead(path)))
            {
                this.LocalInfo = new AddonInfo(path, stream.ReadToEnd(), AddonInfo.RemoteType.AVC);
                this.IsLocalReady = true;

                if (this.LocalInfo.ParseError)
                {
                    this.SetHasError();
                }
                Logger.Log("FetchLocalInfo, LocalInfo", this.LocalInfo);
            }
        }

        private void FetchRemoteInfo()
        {
			const float timeoutSeconds = 10.0f;
			float startTime = Time.time;
			float currentTime = startTime;

            if (string.IsNullOrEmpty(this.LocalInfo.Url) == false)
            {
				using (var www = new WWW(Uri.EscapeUriString(this.LocalInfo.Url)))
                {
					while ((!www.isDone) && ((currentTime - startTime) < timeoutSeconds))
                    {
                        Thread.Sleep(100);
						currentTime = Time.time;
                    }
					if ((www.error == null) && ((currentTime - startTime) < timeoutSeconds))
                    {
                        this.SetRemoteAvcInfo(www);
                    }
                    else
                    {
                        this.SetLocalInfoOnly();
                    }
                }
            }
            else
            {
                this.SetLocalInfoOnly();
            }
        }

        private void ProcessLocalInfo(object state)
        {
            try
            {
                var path = (string)state;
                if (File.Exists(path))
                {
                    this.FetchLocalInfo(path);
                    this.RunProcessRemoteInfo();
                }
                else
                {
                    Logger.Log("File Not Found: " + path);
                    this.SetHasError();
                }
            }
            catch (Exception ex)
            {
                Logger.Exception(ex);
                this.SetHasError();
            }
        }

        private void ProcessRemoteInfo(object state)
        {
            try
            {
                if (String.IsNullOrEmpty(this.LocalInfo.Url) && String.IsNullOrEmpty(this.LocalInfo.KerbalStuffUrl))
                {
                    this.SetLocalInfoOnly();
                    return;
                }

                this.FetchRemoteInfo();
            }
            catch (Exception ex)
            {
                Logger.Exception(ex);
                this.SetLocalInfoOnly();
            }
        }

        private void SetHasError()
        {
            this.HasError = true;
            this.IsProcessingComplete = true;
        }

        private void SetLocalInfoOnly()
        {

            this.RemoteInfo = this.LocalInfo;
            this.IsRemoteReady = true;
            this.IsProcessingComplete = true;
            
            Logger.Log("SetLocalInfoOnly, LocalInfo", this.LocalInfo);
            Logger.Blank();
        }

        private void SetRemoteAvcInfo(WWW www)
        {

            this.RemoteInfo = new AddonInfo(this.LocalInfo.Url, www.text, AddonInfo.RemoteType.AVC);
            this.RemoteInfo.FetchRemoteData();


#if true
            if (this.LocalInfo.Version == this.RemoteInfo.Version)
            {
                Logger.Log("Identical remote version found: Using remote version information only.");
                Logger.Log("SetRemoteAvcInfo, RemoteInfo", this.RemoteInfo);
                Logger.Blank();
                this.LocalInfo = this.RemoteInfo;
            }
            else
#endif
            {
                Logger.Log("SetRemoteAvcInfo, LocalInfo", this.LocalInfo);
                Logger.Log(this.RemoteInfo + "\n\tUpdateAvailable: " + this.IsUpdateAvailable);
                Logger.Blank();
            }

            this.IsRemoteReady = true;
            this.IsProcessingComplete = true;
        }

        private void SetRemoteKerbalStuffInfo(WWW www)
        {
            this.RemoteInfo = new AddonInfo(this.LocalInfo.KerbalStuffUrl, www.text, AddonInfo.RemoteType.KerbalStuff);

            if (this.LocalInfo.Version == this.RemoteInfo.Version)
            {
                Logger.Log("Identical remote version found: Using remote version information only.");
                Logger.Log("SetRemoteKerbalStuffInfo, RemoteInfo", this.RemoteInfo);
                Logger.Blank();
                this.LocalInfo = this.RemoteInfo;
            }
            else
            {
                Logger.Log("SetRemoteKerbalStuffInfo, LocalInfo", this.LocalInfo);
                Logger.Log(this.RemoteInfo + "\n\tUpdateAvailable: " + this.IsUpdateAvailable);
                Logger.Blank();
            }

            this.IsRemoteReady = true;
            this.IsProcessingComplete = true;
        }

#endregion
    }
}