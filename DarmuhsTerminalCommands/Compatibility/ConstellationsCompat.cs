using LethalConstellations.PluginCore;
using System.Linq;

namespace TerminalStuff.Compatibility
{
    internal class ConstellationsCompat
    {
        internal static bool IsLevelInConstellation(SelectableLevel level)
        {
            string numberlessName = new(level.PlanetName.SkipWhile(c => !char.IsLetter(c)).ToArray());
            if (ClassMapper.TryGetConstellation(Collections.ConstellationStuff, Collections.CurrentConstellation, out ClassMapper currentConst))
            {
                if (currentConst.constelMoons.Contains(numberlessName))
                    return true;
                else
                    return false;
            }
            return false;
        }
    }
}
