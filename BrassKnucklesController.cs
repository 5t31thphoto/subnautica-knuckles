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

            if (GameInput.GetButtonDown(GameInput.Button.LeftHand) &&
                Time.time >= lastPunchTime + Cooldown)
            {
                TriggerPunch();
                lastPunchTime = Time.time;
            }
        }

        private bool IsBrassKnucklesEquipped()
        {
            return Inventory.main.equipment.GetTechTypeInSlot("Gloves") == Plugin.BrassKnucklesTechType;
        }

        private void SpawnVisual()
        {
            if (visualRoot != null)
                return;

            // Parent to player for now – no hand-bone API dependency
            visualRoot = new GameObject("BrassKnucklesVisual");
            visualRoot.transform.SetParent(Player.main.transform, false);
            visualRoot.transform.localPosition = new Vector3(0.25f, -0.1f, 0.35f);

            GameObject model = AssetLoader.TryLoadKnucklesModel();
            if (model != null)
            {
                model.transform.SetParent(visualRoot.transform, false);
            }
            else
            {
                var cyl = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
                Object.DestroyImmediate(cyl.GetComponent<Collider>());
                cyl.transform.SetParent(visualRoot.transform, false);
                cyl.transform.localScale = new Vector3(0.04f, 0.08f, 0.04f);

                var r = cyl.GetComponent<Renderer>();
                if (r != null)
                    r.material = AssetLoader.CreatePlaceholderMaterial();
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
            if (MainCamera.camera == null)
                return;

            var cam = MainCamera.camera.transform;
            if (Physics.Raycast(cam.position, cam.forward, out RaycastHit hit, PunchRange))
            {
                var target = hit.collider.GetComponentInParent<LiveMixin>();
                if (target != null && target != Player.main.liveMixin)
                {
                    target.TakeDamage(PunchDamage, hit.point, DamageType.Normal, Player.main.gameObject);
                }
            }
        }

        private void OnDestroy()
        {
            DestroyVisual();
        }
    }
}
