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
using System.IO.IsolatedStorage;
using System.Xml.Serialization;

#endregion

namespace MiniAVC
{
    public class AddonSettings
    {
        #region Initialisation

        public AddonSettings() { }

        public AddonSettings(string fileName)
        {
            this.FileName = fileName;
        }

        #endregion

        #region Properties

        private bool firstRun = true;

        public bool FirstRun
        {
            get { return this.firstRun; }
            set { this.firstRun = value; }
        }

        public bool AllowCheck { get; set; }

        [XmlIgnore]
        public string FileName { get; set; }

        #endregion

        #region Save / Load

        public void Save()
        {
            try
            {
                using (var stream = new FileStream(this.FileName, FileMode.Create))
                {
                    new XmlSerializer(typeof(AddonSettings)).Serialize(stream, this);
                    stream.Close();
                }
            }
            catch (Exception ex)
            {
                Logger.Exception(ex);
            }
        }

        public static AddonSettings Load(string path)
        {
            var fileName = Path.Combine(path, "MiniAVC.xml");
            try
            {
                AddonSettings settings;
                using (var stream = new FileStream(fileName, FileMode.Open))
                {
                    settings = new XmlSerializer(typeof(AddonSettings)).Deserialize(stream) as AddonSettings;
                    settings.FileName = fileName;
                    stream.Close();
                }
                return settings;
            }
            catch (Exception ex)
            {
                if (!(ex is IsolatedStorageException))
                {
                    Logger.Exception(ex);
                }
                return new AddonSettings(fileName);
            }
        }

        #endregion
    }
}