using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace sailadex
{
    internal static class Helper
    {
        public static T GetPrivateField<T>(this object obj, string field)
        {
            return (T)Traverse.Create(obj).Field(field).GetValue();
        }
    }
}
