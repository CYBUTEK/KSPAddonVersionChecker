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

using System.Collections.Generic;
using System.Linq;

#endregion

namespace AVCToolkit.Json
{
    public class JsonObject
    {
        #region Fields

        private readonly Dictionary<string, object> fields = new Dictionary<string, object>();

        #endregion

        #region Constructors

        public JsonObject(object obj)
        {
            foreach (var property in obj.GetType().GetProperties().Where(p => p.IsDefined(typeof(JsonFieldAttribute), true)).ToList().OrderBy(p => (p.GetCustomAttributes(typeof(JsonFieldAttribute), true).First() as JsonFieldAttribute).Order))
            {
                var name = (property.GetCustomAttributes(typeof(JsonFieldAttribute), true).First() as JsonFieldAttribute).Name;
                var value = property.GetValue(obj, null);

                this.fields.Add(name, value);
            }
        }

        #endregion

        #region Properties

        public int Count
        {
            get { return this.fields.Count; }
        }

        public Dictionary<string, object> Fields
        {
            get { return this.fields; }
        }

        public bool HasVisibleFields
        {
            get { return this.fields.Any(f => f.Value != null); }
        }

        #endregion
    }
}