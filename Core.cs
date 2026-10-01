using Alta;
using Alta.Caves;
using Alta.Networking;
using HarmonyLib;
using MelonLoader;
using System.Collections;
using System.Reflection;
using UnityEngine;
using CustomDistributionAPI;

[assembly: MelonInfo(typeof(DifferentCanvasMaterials.Core), "DifferentCanvasMaterials", "1.0.0", "CGNik", null)]
[assembly: MelonGame("Alta", "A Township Tale")]
[assembly: MelonPriority(-100)]

namespace DifferentCanvasMaterials
{
    public class InitializePatch
    {
        internal static void Postfix(NetworkPrefab __instance)
        {
            switch (__instance.Hash)
            {
                case 34570u: // This is Thin Cloth Medium Square
                    PhysicalMaterialPart physicalMaterialPart = __instance.gameObject.GetComponent<PhysicalMaterialPart>();
                    typeof(PhysicalMaterialPart).GetField("materialDistribution", BindingFlags.Instance | BindingFlags.NonPublic).SetValue(physicalMaterialPart, Core.canvasMaterialDistribution);
                    break;
                default:
                    break;
            }
        }
    }

    public class Core : MelonMod
    {
        public static Distribution canvasMaterialDistribution;

        public override void OnInitializeMelon()
        {
            LoggerInstance.Msg("Initialized.");
        }

        public override void OnLateInitializeMelon()
        {
            canvasMaterialDistribution = GameObject.Instantiate(Distribution.All.Where(dist => dist.Hash == 49220u).First());
            typeof(HashedGeneralValue<Distribution>).GetField("hash", BindingFlags.Instance | BindingFlags.NonPublic).SetValue(canvasMaterialDistribution, 49223);
            canvasMaterialDistribution.name = "Canvas Material Distribution";
            CustomDistributionAPI.Core.RegisterDistribution(canvasMaterialDistribution);
            Distribution.Item item_canvas = new Distribution.Item();
            typeof(Distribution.BaseItem).GetField("topic", BindingFlags.Instance | BindingFlags.NonPublic).SetValue(item_canvas, PhysicalMaterial.All.Where(mat => mat.Hash == 61790u).First());
            typeof(Distribution.BaseItem).GetField("baseValue", BindingFlags.Instance | BindingFlags.NonPublic).SetValue(item_canvas, 1f);
            typeof(Distribution.BaseItem).GetField("noAttributeValue", BindingFlags.Instance | BindingFlags.NonPublic).SetValue(item_canvas, 1f);
            typeof(Distribution.BaseItem).GetField("multipliers", BindingFlags.Instance | BindingFlags.NonPublic).SetValue(item_canvas, new AttributeCurveRange[] { });
            canvasMaterialDistribution.GetType().GetField("items", BindingFlags.Instance | BindingFlags.NonPublic).SetValue(canvasMaterialDistribution, new List<Distribution.Item> { item_canvas });
            HarmonyInstance.Patch(AccessTools.Method(typeof(NetworkPrefab), "Initialize"), postfix: new HarmonyMethod(typeof(InitializePatch), nameof(InitializePatch.Postfix)));
        }
    }
}