using UnityEngine;

namespace SubnauticaKnuckles
{
    public class BrassKnucklesBehavior : MonoBehaviour
    {
        private float punchDamage = 250f; // High damage to punch leviathans
        private float punchRange = 3.0f;

        void Update()
        {
            if (Player.main != null && Player.main.GetLeftHandDown() && Inventory.main.GetHeldItem()?.item.GetTechType().ToString() == "BrassKnuckles")
            {
                TriggerPunch();
            }
        }

        void TriggerPunch()
        {
            // Reuse knife slash animation for the arm movement
            Player.main.GetMainHandItem()?.animator?.SetTrigger("bash");

            Transform cam = MainCamera.camera.transform;
            if (Physics.Raycast(cam.position, cam.forward, out RaycastHit hit, punchRange))
            {
                LiveMixin target = hit.collider.GetComponentInParent<LiveMixin>();
                if (target != null)
                {
                    target.TakeDamage(punchDamage, hit.point, DamageType.Normal, null);
                    Utils.PlaySoundAtPosition("event:/sub/baleen/attack", hit.point); // Prawn punch sound
                }
            }
        }
    }
}
