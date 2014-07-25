using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using UnityEngine;

namespace AddonVersionChecker.Extensions
{
    public static class RectExtensions
    {
        public static Rect CentreWindow(this Rect value)
        {
            value.x = (Screen.width * 0.5f) - (value.width * 0.5f);
            value.y = (Screen.height * 0.5f) - (value.height * 0.5f);
            return value;
        }
    }
}
