using System.Runtime.CompilerServices;
using static sailadex.SAD_Plugin;

namespace sailadex
{
    internal class REConfigs
    {
        private static bool Get([CallerMemberName] string property = "") 
            => RE_PluginInstance.GetStaticProperty<bool>(property);

        internal static bool IsSeaLifeModEnabled => Get();
        internal static bool IsFlotsamEnabled => Get();
        internal static bool IsDenseFogEnabled => Get();
        internal static bool IsFishingBonanzaEnabled => Get();
        internal static bool IsIntenseStormEnabled => Get();
    }
}
