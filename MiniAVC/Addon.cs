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

namespace MiniAVC
{
    public class Addon
    {
        #region Fields

        private readonly AddonSettings settings;
        private AddonInfo localInfo;
        private AddonInfo remoteInfo;

        #endregion

        #region Contructors

        public Addon(string path, AddonSettings settings)
        {
            try
            {
                this.settings = settings;
                this.RunProcessLocalInfo(path);
            }
            catch (Exception ex)
            {
                Logger.Exception(ex);
            }
        }

        #endregion

        #region Properties

        public AddonSettings Settings
        {
            get { return this.settings; }
        }

        public AddonInfo LocalInfo
        {
            get { return this.localInfo; }
        }

        public AddonInfo RemoteInfo
        {
            get { return this.remoteInfo; }
        }

        public bool HasError { get; private set; }

        public bool IsLocalReady { get; private set; }

        public bool IsRemoteReady { get; private set; }

        public bool IsProcessingComplete { get; private set; }

        public bool IsUpdateAvailable
        {
            get { return this.IsProcessingComplete && this.LocalInfo.Version != null && this.RemoteInfo.Version != null && this.RemoteInfo.Version > this.LocalInfo.Version && this.RemoteInfo.IsCompatibleKspVersion; }
        }

        public bool IsCompatible
        {
            get { return this.IsLocalReady && this.LocalInfo.IsCompatible; }
        }

        public string Name
        {
            get { return this.LocalInfo.Name; }
        }

        #endregion

        #region Public Methods

        public void RunProcessLocalInfo(string file)
        {
            try
            {
                ThreadPool.QueueUserWorkItem(this.ProcessLocalInfo, file);
            }
            catch (Exception ex)
            {
                Logger.Exception(ex);
            }
        }

        public void RunProcessRemoteInfo()
        {
            try
            {
                ThreadPool.QueueUserWorkItem(this.ProcessRemoteInfo);
            }
            catch (Exception ex)
            {
                Logger.Exception(ex);
            }
        }

        #endregion

        #region Private Methods

        private void ProcessLocalInfo(object state)
        {
            try
            {
                var path = (string)state;
                if (File.Exists(path))
                {
                    using (var stream = new StreamReader(File.OpenRead(path)))
                    {
                        this.localInfo = new AddonInfo(path, stream.ReadToEnd());
                        this.IsLocalReady = true;
                    }
                    if (!this.settings.FirstRun || string.IsNullOrEmpty(this.localInfo.Url))
                    {
                        this.RunProcessRemoteInfo();
                    }
                }
                else
                {
                    Logger.Log("File Not Found: " + path);
                    this.HasError = true;
                    this.IsProcessingComplete = true;
                }
            }
            catch (Exception ex)
            {
                Logger.Exception(ex);
                this.HasError = true;
                this.IsProcessingComplete = true;
            }
        }

        private void ProcessRemoteInfo(object state)
        {
            try
            {
                if (!this.settings.AllowCheck || string.IsNullOrEmpty(this.localInfo.Url))
                {
                    this.remoteInfo = this.localInfo;
                    this.IsRemoteReady = true;
                    this.IsProcessingComplete = true;
                    Logger.Log(this.localInfo);
                    Logger.Blank();
                    return;
                }

                using (var www = new WWW(this.localInfo.Url))
                {
                    while (!www.isDone) { }
                    if (www.error == null)
                    {
                        this.remoteInfo = new AddonInfo(this.localInfo.Url, www.text);
                        this.IsRemoteReady = true;
                        this.IsProcessingComplete = true;
                        Logger.Log(this.localInfo);
                        Logger.Log(this.remoteInfo);
                    }
                    else
                    {
                        this.remoteInfo = this.localInfo;
                        this.IsRemoteReady = true;
                        this.IsProcessingComplete = true;
                        Logger.Log(this.localInfo);
                    }
                }
                Logger.Blank();
            }
            catch (Exception ex)
            {
                Logger.Exception(ex);
                this.remoteInfo = this.localInfo;
                this.IsRemoteReady = true;
                this.IsProcessingComplete = true;
                Logger.Log(this.localInfo);
            }
        }

        #endregion
    }
}