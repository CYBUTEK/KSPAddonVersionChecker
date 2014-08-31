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
    public class Configuration
    {
        #region Fields

        private static readonly string fileName = Path.ChangeExtension(Assembly.GetExecutingAssembly().Location, "xml");

        #endregion

        #region Constructors

        static Configuration()
        {
            Instance = new Configuration
            {
                FirstRun = true,
                Version = Assembly.GetExecutingAssembly().GetName().Version.ToString()
            };
            Load();
        }

        #endregion

        #region Properties

        public static Configuration Instance { get; private set; }

        public bool FirstRun { get; set; }

        public string Version { get; set; }

        #endregion

        #region Methods: public

        public static bool GetFirstRun()
        {
            return Instance.FirstRun;
        }

        public static string GetVersion()
        {
            return Instance.Version;
        }

        public static void Load()
        {
            try
            {
                if (!File.Exists(fileName))
                {
                    return;
                }

                using (var stream = new FileStream(fileName, FileMode.Open))
                {
                    var xml = new XmlSerializer(Instance.GetType());
                    Instance = xml.Deserialize(stream) as Configuration;
                    stream.Close();
                }
            }
            catch (Exception ex)
            {
                Logger.Exception(ex);
            }
        }

        public static void Save()
        {
            try
            {
                using (var stream = new FileStream(fileName, FileMode.Create))
                {
                    var xml = new XmlSerializer(Instance.GetType());
                    xml.Serialize(stream, Instance);
                    stream.Close();
                }
            }
            catch (Exception ex)
            {
                Logger.Exception(ex);
            }
        }

        public static void SetFirstRun(bool value)
        {
            Instance.FirstRun = value;
            Save();
        }

        public static void SetVersion(string value)
        {
            Instance.Version = value;
            Save();
        }

        #endregion
    }
}