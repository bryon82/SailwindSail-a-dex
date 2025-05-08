using HarmonyLib;
using System;

namespace sailadex
{
    internal static class Extensions
    {
        public static T GetPrivateField<T>(this object obj, string field)
        {
            return (T)Traverse.Create(obj).Field(field).GetValue();
        }

        public static T GetPrivateProperty<T>(this object obj, string property)
        {
            return (T)Traverse.Create(obj).Property(property).GetValue();
        }
    }
}
