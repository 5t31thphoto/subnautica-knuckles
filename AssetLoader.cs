using System.IO;
using System.Reflection;
using UnityEngine;

namespace SubnauticaKnuckles
{
    public static class AssetLoader
    {
        private static string ModFolder => Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);

        /// <summary>
        /// Tries to load "knuckles" prefab from Assets/knuckles.bundle.
        /// Returns null if the bundle is missing (placeholder will be used).
        /// </summary>
        public static GameObject TryLoadKnucklesModel()
        {
            string bundlePath = Path.Combine(ModFolder, "Assets", "knuckles.bundle");
            if (!File.Exists(bundlePath))
                return null;

            var bundle = AssetBundle.LoadFromFile(bundlePath);
            if (bundle == null)
                return null;

            // Expect the prefab inside the bundle to be named "Knuckles" (or change this string)
            GameObject prefab = bundle.LoadAsset<GameObject>("Knuckles");
            if (prefab == null)
            {
                // Fallback: take the first GameObject in the bundle
                var all = bundle.LoadAllAssets<GameObject>();
                if (all != null && all.Length > 0)
                    prefab = all[0];
            }

            if (prefab == null)
                return null;

            return Object.Instantiate(prefab);
        }

        public static Material CreatePlaceholderMaterial()
        {
            Shader shader = Shader.Find("MarmosetUBER") ?? Shader.Find("Standard");
            Material mat = new Material(shader);
            mat.color = new Color(0.55f, 0.45f, 0.25f); // dull brass
            mat.SetFloat("_Metallic", 0.85f);
            mat.SetFloat("_Glossiness", 0.6f);
            return mat;
        }
    }
}
