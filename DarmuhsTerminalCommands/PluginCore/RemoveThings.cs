using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TerminalStuff.PluginCore
{
    internal class RemoveThings
    {
        internal static void DeleteAllNodes(ref Dictionary<TerminalNode, int> nodeDictionary) //viewtermnodes
        {
            List<TerminalNode> nodesToDelete = [];

            foreach (KeyValuePair<TerminalNode, int> item in nodeDictionary)
            {
                nodesToDelete.Add(item.Key);
            }

            nodeDictionary.Clear();

            for (int i = nodesToDelete.Count - 1; i >= 0; i--)
            {
                Plugin.Spam($"Deleting node: {nodesToDelete[i].name}");
                UnityEngine.Object.Destroy(nodesToDelete[i]);
            }
        }
    }
}
