using HarmonyLib;

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
