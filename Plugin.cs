using BepInEx;
using Nautilus.Assets;
using Nautilus.Assets.Gadgets;
using Nautilus.Crafting;

namespace SubnauticaKnuckles
{
    [BepInPlugin("com.yourname.brassknuckles", "Brass Knuckles", "1.0.0")]
    [BepInDependency("com.snmodding.nautilus")]
    public class Plugin : BaseUnityPlugin
    {
        private void Awake()
        {
            // Define the item
            var prefab = new CustomPrefab("BrassKnuckles", "Brass Knuckles", "Put sea monsters in their place.");
            
            // Assign it to the Glove equipment slot
            prefab.SetEquipment(EquipmentType.Gloves);
            
            // Give it a recipe (1x Titanium)
            prefab.SetRecipe(new RecipeData(new CraftData.Ingredient(TechType.Titanium, 1)))
                  .WithFabricatorType(CraftTree.Type.Fabricator)
                  .WithStepsToFabricator(new[] { "Personal", "Tools" });

            // Attach the punch script to the item when it spawns
            prefab.SetGameObject(() => {
                var obj = UnityEngine.Object.Instantiate(CraftData.GetPrefabForTechType(TechType.Knife));
                UnityEngine.Object.DestroyImmediate(obj.GetComponent<PlayerKnife>()); // Strip knife behavior
                obj.AddComponent<BrassKnucklesBehavior>(); // Add punch behavior
                return obj;
            });

            prefab.Register();
        }
    }
}
