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
using System.Reflection;
using System.Xml.Serialization;

#endregion

namespace KSP_AVC
{
    public class Settings
    {
        #region Instance

        private static Settings instance;

        public static Settings Instance
        {
            get { return instance ?? (instance = Load()); }
        }

        #endregion

        #region Properties

        private bool firstRun = true;
        private string version = Assembly.GetExecutingAssembly().GetName().Version.ToString();

        public bool FirstRun
        {
            get { return this.firstRun; }
            set
            {
                this.firstRun = value;
                Logger.Log("Settings->FirstRun = " + value);
            }
        }

        public string Version
        {
            get { return this.version; }
            set
            {
                this.version = value;
                Logger.Log("Settings->Version = " + value);
            }
        }

        #endregion

        #region Save and Load

        private static readonly string fileName = Path.Combine(new FileInfo(Assembly.GetExecutingAssembly().Location).Directory.ToString(), "KSP-AVC.cfg");

        public static void Save()
        {
            try
            {
                var stream = new FileStream(fileName, FileMode.Create);
                var xml = new XmlSerializer(typeof(Settings));
                xml.Serialize(stream, instance);
                stream.Close();
            }
            catch (Exception ex)
            {
                Logger.Exception(ex, "Settings->Save");
            }
        }

        public static Settings Load()
        {
            try
            {
                if (!File.Exists(fileName))
                {
                    return new Settings();
                }

                var stream = new FileStream(fileName, FileMode.Open);
                var xml = new XmlSerializer(typeof(Settings));
                var settings = xml.Deserialize(stream);
                stream.Close();
                return (Settings)settings;
            }
            catch (Exception ex)
            {
                Logger.Exception(ex, "Settings->Load");
                return new Settings();
            }
        }

        #endregion
    }
}