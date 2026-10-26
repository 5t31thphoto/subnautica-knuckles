using System.IO;
using System.Reflection;
using UnityEngine;

namespace SubnauticaKnuckles
{
    public static class AssetLoader
    {
        private static string ModFolder => Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);

        // Loads a standard PNG texture into a Texture2D object
        public static Texture2D LoadTexture(string fileName)
        {
            string path = Path.Combine(ModFolder, "Assets", fileName);
            if (!File.Exists(path)) return null;

            byte[] fileData = File.ReadAllBytes(path);
            Texture2D tex = new Texture2D(2, 2);
            if (tex.LoadImage(fileData))
            {
                return tex;
            }
            return null;
        }

        // Sets up the PBR material properties using your loaded textures
        public static Material CreateKnucklesMaterial()
        {
            // Pull the standard PBR shader used by Subnautica tools
            Shader shader = Shader.Find("MarmosetUBER");
            if (shader == null) shader = Shader.Find("Standard");

            Material mat = new Material(shader);

            Texture2D albedo = LoadTexture("knuckles_albedo.png");
            Texture2D normal = LoadTexture("knuckles_normal.png");
            Texture2D metallicSmoothness = LoadTexture("knuckles_metallic.png");

            if (albedo != null) mat.SetTexture("_MainTex", albedo);
            if (normal != null)
            {
                mat.SetTexture("_BumpMap", normal);
                mat.EnableKeyword("_NORMALMAP");
            }
            if (metallicSmoothness != null)
            {
                mat.SetTexture("_MetallicGlossMap", metallicSmoothness);
                mat.EnableKeyword("_METALLICGLOSSMAP");
            }

            return mat;
        }
    }
}
