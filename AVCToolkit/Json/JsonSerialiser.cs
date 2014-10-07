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

using AVCToolkit.Json;

#endregion

namespace AVCToolkit
{
    public static class JsonSerialiser
    {
        #region Methods: public

        public static string Serialise(object obj)
        {
            var jsonObject = new JsonObject(obj);
            var json = "{";

            var first = true;
            foreach (var field in jsonObject.Fields)
            {
                var jsonField = SerialiseField(field);
                if (jsonField == String.Empty)
                {
                    continue;
                }
                if (!first)
                {
                    json += ",";
                }
                json += jsonField;
                first = false;
            }

            return json + "}";
        }

        #endregion

        #region Methods: private

        private static bool IsPrimitive(object value)
        {
            return value is byte ||
                   value is sbyte ||
                   value is short ||
                   value is ushort ||
                   value is int ||
                   value is uint ||
                   value is long ||
                   value is ulong ||
                   value is float ||
                   value is double ||
                   value is decimal ||
                   value is bool;
        }

        private static string SerialiseField(KeyValuePair<string, object> field)
        {
            if (field.Value == null)
            {
                return String.Empty;
            }

            if (IsPrimitive(field.Value))
            {
                return "\"" + field.Key + "\":" + field.Value;
            }

            var jsonObject = new JsonObject(field.Value);
            if (jsonObject.Count > 0)
            {
                if (jsonObject.HasVisibleFields)
                {
                    return "\"" + field.Key + "\":" + Serialise(field.Value);
                }
                return String.Empty;
            }

            return "\"" + field.Key + "\":\"" + field.Value + "\"";
        }

        #endregion
    }
}