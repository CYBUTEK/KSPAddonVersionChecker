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

using System;
using System.IO;
using System.Threading;

using UnityEngine;

namespace KSP_AVC
{
    public class Addon
    {
        public Addon(string path)
        {
            this.RunProcessLocalInfo(path);
        }

        public bool HasError { get; private set; }

        public bool IsCompatible
        {
            get { return this.IsLocalReady && this.LocalInfo.IsCompatible; }
        }

        public bool IsLocalReady { get; private set; }

        public bool IsProcessingComplete { get; private set; }

        public bool IsRemoteReady { get; private set; }

        public bool IsUpdateAvailable
        {
            get { return this.IsProcessingComplete && this.LocalInfo.Version != null && this.RemoteInfo.Version != null && this.RemoteInfo.Version > this.LocalInfo.Version && this.RemoteInfo.IsCompatibleKspVersion && this.RemoteInfo.IsCompatibleGitHubVersion; }
        }

        public AddonInfo LocalInfo { get; private set; }

        public string Name
        {
            get { return this.LocalInfo.Name; }
        }

        public AddonInfo RemoteInfo { get; private set; }

        public void RunProcessLocalInfo(string path)
        {
            ThreadPool.QueueUserWorkItem(this.ProcessLocalInfo, path);
        }

        public void RunProcessRemoteInfo()
        {
            ThreadPool.QueueUserWorkItem(this.ProcessRemoteInfo);
        }

        private void FetchLocalInfo(string path)
        {
            using (var stream = new StreamReader(File.OpenRead(path)))
            {
                this.LocalInfo = new AddonInfo(path, stream.ReadToEnd());
                this.IsLocalReady = true;
                this.RunProcessRemoteInfo();
            }
        }

        private void FetchRemoteInfo()
        {
            using (var www = new WWW(this.LocalInfo.Url))
            {
                while (!www.isDone)
                {
                    Thread.Sleep(100);
                }

                if (www.error != null)
                {
                    this.SetLocalOnly();
                    return;
                }

                this.RemoteInfo = new AddonInfo(this.LocalInfo.Url, www.text);
                this.RemoteInfo.FetchRemoteData();
                this.SetRemoteReady();
                Logger.Log(this.RemoteInfo + "\n\tUpdateAvailable: " + this.IsUpdateAvailable);
            }
        }

        private void ProcessLocalInfo(object state)
        {
            try
            {
                var path = (string)state;
                if (!File.Exists(path))
                {
                    Logger.Log("File Not Found: " + path);
                    this.SetHasError();
                    return;
                }
                this.FetchLocalInfo(path);
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
                if (string.IsNullOrEmpty(this.LocalInfo.Url))
                {
                    this.SetLocalOnly();
                    Logger.Blank();
                    return;
                }

                this.FetchRemoteInfo();
                Logger.Blank();
            }
            catch (Exception ex)
            {
                Logger.Exception(ex);
                this.SetLocalOnly();
            }
        }

        private void SetHasError()
        {
            this.HasError = true;
            this.IsProcessingComplete = true;
        }

        private void SetLocalOnly()
        {
            this.RemoteInfo = this.LocalInfo;
            this.SetRemoteReady();
        }

        private void SetRemoteReady()
        {
            this.IsRemoteReady = true;
            this.IsProcessingComplete = true;
            Logger.Log(this.LocalInfo);
        }
    }
}