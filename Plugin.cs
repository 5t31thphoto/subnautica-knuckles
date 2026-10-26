using BepInEx;
using Nautilus.Assets;
using Nautilus.Assets.Gadgets;
using Nautilus.Assets.PrefabTemplates;
using Nautilus.Crafting;
using Nautilus.Handlers;
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

            PrefabInfo info = PrefabInfo
                .WithTechType("BrassKnuckles", "Brass Knuckles", "Put sea monsters in their place.")
                .WithIcon(SpriteManager.Get(TechType.ReinforcedGloves));

            CustomPrefab prefab = new CustomPrefab(info);

            // Glove slot
            prefab.SetEquipment(EquipmentType.Gloves);

            // Recipe
            prefab.SetRecipe(new RecipeData(
                new Ingredient(TechType.Titanium, 2),
                new Ingredient(TechType.Silver, 1)
            ))
            .WithFabricatorType(CraftTree.Type.Fabricator)
            .WithStepsToFabricatorMenu("Personal", "Equipment");

            // Simple world prefab (cloned from Reinforced Gloves)
            prefab.SetGameObject(new CloneTemplate(info, TechType.ReinforcedGloves));

            prefab.Register();
            BrassKnucklesTechType = info.TechType;

            // Persistent controller
            var go = new GameObject("BrassKnucklesController");
            Object.DontDestroyOnLoad(go);
            go.AddComponent<BrassKnucklesController>();

            Logger.LogInfo("Brass Knuckles registered.");
        }
    }
}
