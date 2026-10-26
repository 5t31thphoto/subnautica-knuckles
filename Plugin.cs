using BepInEx;
using Nautilus.Assets;
using Nautilus.Assets.Gadgets;
using Nautilus.Assets.PrefabTemplates;
using Nautilus.Crafting;
using UnityEngine;

namespace SubnauticaKnuckles
{
    [BepInPlugin(PluginInfo.PLUGIN_GUID, PluginInfo.PLUGIN_NAME, PluginInfo.PLUGIN_VERSION)]
    [BepInDependency("com.snmodding.nautilus")]
    public class Plugin : BaseUnityPlugin
    {
        public static TechType BrassKnucklesTechType { get; private set; }

        private void Awake()
        {
            Logger.LogInfo("Brass Knuckles loading...");

            var info = PrefabInfo
                .WithTechType("BrassKnuckles", "Brass Knuckles", "Put sea monsters in their place.")
                .WithIcon(SpriteManager.Get(TechType.ReinforcedGloves));

            var prefab = new CustomPrefab(info);

            prefab.SetEquipment(EquipmentType.Gloves);

            prefab.SetRecipe(new RecipeData(
                new Ingredient(TechType.Titanium, 2),
                new Ingredient(TechType.Silver, 1)
            ))
            .WithFabricatorType(CraftTree.Type.Fabricator)
            .WithStepsToFabricatorTab("Personal", "Equipment");

            // Simple world/inventory model – clone reinforced gloves
            prefab.SetGameObject(new CloneTemplate(info, TechType.ReinforcedGloves));

            prefab.Register();
            BrassKnucklesTechType = info.TechType;

            // Persistent controller that watches the glove slot
            var go = new GameObject("BrassKnucklesController");
            Object.DontDestroyOnLoad(go);
            go.AddComponent<BrassKnucklesController>();

            Logger.LogInfo("Brass Knuckles registered.");
        }
    }
}
