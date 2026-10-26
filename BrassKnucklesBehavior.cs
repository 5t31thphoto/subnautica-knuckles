using UnityEngine;

namespace SubnauticaKnuckles
{
    public class BrassKnucklesBehavior : MonoBehaviour
    {
        // 5001 damage completely overflows a Reaper Leviathan's 5000 HP pool in one hit
        private float punchDamage = 5001f; 
        private float punchRange = 4.0f; // Slightly extended range to help hit massive moving leviathan colliders
        private GameObject visualModel;

        void Awake()
        {
            Transform handTarget = Player.main.GetHandsController().rightHandTarget;
            transform.SetParent(handTarget, false);

            visualModel = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            DestroyImmediate(visualModel.GetComponent<Collider>());
            
            visualModel.transform.SetParent(this.transform, false);
            visualModel.transform.localScale = new Vector3(0.03f, 0.06f, 0.03f); 
            visualModel.transform.localRotation = Quaternion.Euler(0, 0, 90);    
            visualModel.transform.localPosition = new Vector3(0.02f, 0.02f, 0.05f); 

            Renderer renderer = visualModel.GetComponent<Renderer>();
            if (renderer != null)
            {
                renderer.material = AssetLoader.CreateKnucklesMaterial();
            }
        }

        void Update()
        {
            if (Player.main != null && Player.main.GetLeftHandDown() && Inventory.main.GetHeldItem()?.item.GetTechType().ToString() == "BrassKnuckles")
            {
                TriggerPunch();
            }
        }

        void TriggerPunch()
        {
            Player.main.GetMainHandItem()?.animator?.SetTrigger("bash");

            Transform cam = MainCamera.camera.transform;
            if (Physics.Raycast(cam.position, cam.forward, out RaycastHit hit, punchRange))
            {
                LiveMixin target = hit.collider.GetComponentInParent<LiveMixin>();
                if (target != null)
                {
                    // Unleash instant-kill damage
                    target.TakeDamage(punchDamage, hit.point, DamageType.Normal, null);
                    
                    // Native camera shake to mimic the shockwave of a massive punch
                    MainCameraControl.main.PunchShake(5f, 0.5f);

                    // Explode the Prawn Suit heavy punch impact sound right on target contact
                    Utils.PlaySoundAtPosition("event:/sub/baleen/attack", hit.point); 
                }
            }
        }
    }
}
