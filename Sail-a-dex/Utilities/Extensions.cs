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

        public static void SetPrivateField(this object obj, string field, object value)
        {
            Traverse.Create(obj).Field(field).SetValue(value);
        }

        public static T GetPrivateProperty<T>(this object obj, string property)
        {
            return (T)Traverse.Create(obj).Property(property).GetValue();
        }

        public static T GetStaticProperty<T>(this object obj, string property)
        {
            return (T)Traverse.Create(obj.GetType()).Property(property).GetValue();
        }
    }
}
