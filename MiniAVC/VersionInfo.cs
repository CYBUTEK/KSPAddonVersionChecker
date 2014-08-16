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
using System.Globalization;
using System.Text.RegularExpressions;

#endregion

namespace MiniAVC
{
    public class VersionInfo : IComparable
    {
        #region IComparable Members

        public int CompareTo(object obj)
        {
            if (obj == null)
            {
                return 1;
            }

            var other = obj as VersionInfo;
            if (other == null)
            {
                throw new ArgumentException("Not a VersionInfo object.");
            }

            var major = this.Major.CompareTo(other.Major);
            if (major != 0)
            {
                return major;
            }

            var minor = this.Minor.CompareTo(other.Minor);
            if (minor != 0)
            {
                return minor;
            }

            var patch = this.Patch.CompareTo(other.Patch);
            return patch != 0 ? patch : this.Build.CompareTo(other.Build);
        }

        #endregion

        #region Constructors

        public VersionInfo()
        {
            this.SetVersion(0, 0, 0, 0);
        }

        public VersionInfo(long major)
        {
            this.SetVersion(major, 0, 0, 0);
        }

        public VersionInfo(long major, long minor)
        {
            this.SetVersion(major, minor, 0, 0);
        }

        public VersionInfo(long major, long minor, long patch)
        {
            this.SetVersion(major, minor, patch, 0);
        }

        public VersionInfo(long major, long minor, long patch, long build)
        {
            this.SetVersion(major, minor, patch, build);
        }

        public VersionInfo(string version)
        {
            var sections = Regex.Replace(version, @"[^\d\.]", string.Empty).Split('.');

            switch (sections.Length)
            {
                case 1:
                    this.SetVersion(long.Parse(sections[0]), 0, 0, 0);
                    return;

                case 2:
                    this.SetVersion(long.Parse(sections[0]), long.Parse(sections[1]), 0, 0);
                    return;

                case 3:
                    this.SetVersion(long.Parse(sections[0]), long.Parse(sections[1]), long.Parse(sections[2]), 0);
                    return;

                case 4:
                    this.SetVersion(long.Parse(sections[0]), long.Parse(sections[1]), long.Parse(sections[2]), long.Parse(sections[3]));
                    return;

                default:
                    this.SetVersion(0, 0, 0, 0);
                    return;
            }
        }

        #endregion

        #region Properties

        public long Major { get; private set; }
        public long Minor { get; private set; }
        public long Patch { get; private set; }
        public long Build { get; private set; }

        public static VersionInfo Min
        {
            get { return new VersionInfo(); }
        }

        public static VersionInfo Max
        {
            get { return new VersionInfo(long.MaxValue, long.MaxValue, long.MaxValue, long.MaxValue); }
        }

        #endregion

        #region Private Methods

        private void SetVersion(long major, long minor, long patch, long build)
        {
            this.Major = major;
            this.Minor = minor;
            this.Patch = patch;
            this.Build = build;
        }

        #endregion

        #region Public Methods

        public override string ToString()
        {
            if (this.Build > 0)
            {
                return string.Format("{0}.{1}.{2}.{3}", this.Major, this.Minor, this.Patch, this.Build);
            }
            if (this.Patch > 0)
            {
                return string.Format("{0}.{1}.{2}", this.Major, this.Minor, this.Patch);
            }

            return this.Minor > 0 ? string.Format("{0}.{1}", this.Major, this.Minor) : this.Major.ToString(CultureInfo.InvariantCulture);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = this.Major.GetHashCode();
                hashCode = (hashCode * 397) ^ this.Minor.GetHashCode();
                hashCode = (hashCode * 397) ^ this.Patch.GetHashCode();
                hashCode = (hashCode * 397) ^ this.Build.GetHashCode();
                return hashCode;
            }
        }

        protected bool Equals(VersionInfo other)
        {
            return this.Major == other.Major && this.Minor == other.Minor && this.Patch == other.Patch && this.Build == other.Build;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj))
            {
                return false;
            }
            if (ReferenceEquals(this, obj))
            {
                return true;
            }
            return obj.GetType() == this.GetType() && this.Equals((VersionInfo)obj);
        }

        #endregion

        #region Operators

        public static implicit operator System.Version(VersionInfo version)
        {
            return new System.Version((int)version.Major, (int)version.Minor, (int)version.Patch, (int)version.Build);
        }

        public static implicit operator VersionInfo(System.Version version)
        {
            return new VersionInfo(version.Major, version.Minor, version.Build, version.Revision);
        }

        public static bool operator ==(VersionInfo v1, VersionInfo v2)
        {
            return Equals(v1, v2);
        }

        public static bool operator !=(VersionInfo v1, VersionInfo v2)
        {
            return !Equals(v1, v2);
        }

        public static bool operator >(VersionInfo v1, VersionInfo v2)
        {
            return v1.CompareTo(v2) > 0;
        }

        public static bool operator >=(VersionInfo v1, VersionInfo v2)
        {
            return v1.CompareTo(v2) >= 0;
        }

        public static bool operator <(VersionInfo v1, VersionInfo v2)
        {
            return v1.CompareTo(v2) < 0;
        }

        public static bool operator <=(VersionInfo v1, VersionInfo v2)
        {
            return v1.CompareTo(v2) <= 0;
        }

        #endregion
    }
}