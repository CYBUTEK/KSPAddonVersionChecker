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

using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;
using UnityEngine;

namespace MiniAVC
{
    public class AddonSettings
    {
        private bool firstRun = true;

        public AddonSettings() { }

        public AddonSettings(string fileName)
        {
            FileName = fileName;
        }

        public bool AllowCheck { get; set; }

        [XmlIgnore]
        public string FileName { get; set; }

        public bool FirstRun
        {
            get { return firstRun; }
            set { firstRun = value; }
        }

        public List<string> IgnoredUpdates { get; set; } = new List<string>();

        public static AddonSettings Load(string rootPath)
        {
            var filePath = Path.Combine(rootPath, "MiniAVC.xml");

            if (!File.Exists(filePath))
            {
                return new AddonSettings(filePath);
            }

            AddonSettings settings;
            using (var stream = new FileStream(filePath, FileMode.Open))
            {
                settings = new XmlSerializer(typeof(AddonSettings)).Deserialize(stream) as AddonSettings;
                settings.FileName = filePath;
                stream.Close();
            }
            return settings;
        }

        public void Save()
        {
            using (var stream = new FileStream(FileName, FileMode.Create))
            {
                new XmlSerializer(typeof(AddonSettings)).Serialize(stream, this);
                stream.Close();
            }
        }
    }
}