using UnityEngine;

namespace SubnauticaKnuckles
{
    public class BrassKnucklesBehavior : MonoBehaviour
    {
        private float punchDamage = 250f; 
        private float punchRange = 3.0f;
        private GameObject visualModel;

        void Awake()
        {
            // 1. Anchor the object to the player's right hand transform space
            Transform handTarget = Player.main.GetHandsController().rightHandTarget;
            transform.SetParent(handTarget, false);

            // 2. Build the visual model
            // For a browser-built fallback, we generate a basic horizontal guard cylinder.
            visualModel = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            DestroyImmediate(visualModel.GetComponent<Collider>()); // Colliders on held tools break physics
            
            visualModel.transform.SetParent(this.transform, false);
            visualModel.transform.localScale = new Vector3(0.03f, 0.06f, 0.03f); // Hand scale
            visualModel.transform.localRotation = Quaternion.Euler(0, 0, 90);    // Align across knuckles
            visualModel.transform.localPosition = new Vector3(0.02f, 0.02f, 0.05f); // Position offset

            // Apply your PBR texture material mapping
            Renderer renderer = visualModel.GetComponent<Renderer>();
            if (renderer != null)
            {
                renderer.material = AssetLoader.CreateKnucklesMaterial();
            }
        }

        void Update()
        {
            // Only punch if the knuckles are currently selected/held in your hands
            if (Player.main != null && Player.main.GetLeftHandDown() && Inventory.main.GetHeldItem()?.item.GetTechType().ToString() == "BrassKnuckles")
            {
                TriggerPunch();
            }
        }

        void TriggerPunch()
        {
            // Execute the vanilla knife slash arm gesture animation
            Player.main.GetMainHandItem()?.animator?.SetTrigger("bash");

            Transform cam = MainCamera.camera.transform;
            if (Physics.Raycast(cam.position, cam.forward, out RaycastHit hit, punchRange))
            {
                LiveMixin target = hit.collider.GetComponentInParent<LiveMixin>();
                if (target != null)
                {
                    // Deal heavy damage to structural HP health pools
                    target.TakeDamage(punchDamage, hit.point, DamageType.Normal, null);
                    
                    // Plays the structural heavy impact crunch sound 
                    Utils.PlaySoundAtPosition("event:/sub/baleen/attack", hit.point); 
                }
            }
        }
    }
}
