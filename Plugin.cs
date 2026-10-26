using BepInEx;
using Nautilus.Assets;
using Nautilus.Assets.Gadgets;
using Nautilus.Crafting;
using Nautilus.Handlers;
using Nautilus.Utility;
using UnityEngine;
using Ingredient = CraftData.Ingredient;

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

            // Create the item
            PrefabInfo info = PrefabInfo
                .WithTechType("BrassKnuckles", "Brass Knuckles", "Put sea monsters in their place.")
                .WithIcon(SpriteManager.Get(TechType.ReinforcedGloves)); // placeholder icon

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

            // Simple prefab (we don't need a complex world object for pure equipment)
            prefab.SetGameObject(new CloneTemplate(info, TechType.ReinforcedGloves));

            prefab.Register();
            BrassKnucklesTechType = info.TechType;

            // Start the player-side controller once the player exists
            var go = new GameObject("BrassKnucklesController");
            DontDestroyOnLoad(go);
            go.AddComponent<BrassKnucklesController>();

            Logger.LogInfo("Brass Knuckles registered.");
        }
    }

    public static class PluginInfo
    {
        public const string PLUGIN_GUID = "com.yourname.brassknuckles";
        public const string PLUGIN_NAME = "Brass Knuckles";
        public const string PLUGIN_VERSION = "1.0.0";
    }
}
