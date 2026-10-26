using UnityEngine;

namespace SubnauticaKnuckles
{
    public class BrassKnucklesController : MonoBehaviour
    {
        private const float PunchDamage = 5001f;
        private const float PunchRange = 4.0f;
        private const float Cooldown = 0.6f;

        private GameObject visualRoot;
        private float lastPunchTime;
        private bool isEquipped;

        private void Update()
        {
            if (Player.main == null || Inventory.main == null)
                return;

            bool nowEquipped = IsBrassKnucklesEquipped();

            if (nowEquipped != isEquipped)
            {
                isEquipped = nowEquipped;
                if (isEquipped)
                    SpawnVisual();
                else
                    DestroyVisual();
            }

            if (!isEquipped)
                return;

            // Left mouse / LeftHand button
            if (GameInput.GetButtonDown(GameInput.Button.LeftHand) && Time.time >= lastPunchTime + Cooldown)
            {
                TriggerPunch();
                lastPunchTime = Time.time;
            }
        }

        private bool IsBrassKnucklesEquipped()
        {
            // Works on current Nautilus / Subnautica
            return Inventory.main.equipment.GetTechTypeInSlot("Gloves") == Plugin.BrassKnucklesTechType;
        }

        private void SpawnVisual()
        {
            if (visualRoot != null)
                return;

            Transform hand = Player.main.GetHandsController()?.rightHandTarget;
            if (hand == null)
                return;

            visualRoot = new GameObject("BrassKnucklesVisual");
            visualRoot.transform.SetParent(hand, false);
            visualRoot.transform.localPosition = new Vector3(0.02f, 0.02f, 0.05f);
            visualRoot.transform.localRotation = Quaternion.Euler(0, 0, 90);

            // Try AssetBundle first, fall back to cylinder
            GameObject model = AssetLoader.TryLoadKnucklesModel();
            if (model != null)
            {
                model.transform.SetParent(visualRoot.transform, false);
            }
            else
            {
                // Placeholder cylinder
                var cyl = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
                Object.DestroyImmediate(cyl.GetComponent<Collider>());
                cyl.transform.SetParent(visualRoot.transform, false);
                cyl.transform.localScale = new Vector3(0.035f, 0.07f, 0.035f);
                cyl.transform.localPosition = Vector3.zero;
                cyl.transform.localRotation = Quaternion.identity;

                var renderer = cyl.GetComponent<Renderer>();
                if (renderer != null)
                    renderer.material = AssetLoader.CreatePlaceholderMaterial();
            }
        }

        private void DestroyVisual()
        {
            if (visualRoot != null)
            {
                Object.Destroy(visualRoot);
                visualRoot = null;
            }
        }

        private void TriggerPunch()
        {
            Transform cam = MainCamera.camera.transform;
            if (Physics.Raycast(cam.position, cam.forward, out RaycastHit hit, PunchRange))
            {
                LiveMixin target = hit.collider.GetComponentInParent<LiveMixin>();
                if (target != null && target != Player.main.liveMixin)
                {
                    target.TakeDamage(PunchDamage, hit.point, DamageType.Normal, Player.main.gameObject);

                    // Feel
                    MainCameraControl.main.PunchShake(4.5f, 0.4f);

                    // Sound (Prawn Suit heavy hit is the closest vanilla punch sound)
                    FMODUWE.PlayOneShot("event:/sub/exosuit/impact_solid_hard", hit.point);
                }
            }
        }

        private void OnDestroy()
        {
            DestroyVisual();
        }
    }
}
