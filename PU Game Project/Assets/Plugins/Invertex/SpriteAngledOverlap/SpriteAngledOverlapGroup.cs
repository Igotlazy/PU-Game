using UnityEngine;
// For support, contact Invertex: reversenorms@gmail.com

namespace Invertex.Unity.Sprites
{
    /// <summary>
    /// This class is needed for updating the values on the Sprites-AngledOverlap shader.
    /// It should be placed on a root object, at the very bottom of your character/prop.
    /// </summary>
    public class SpriteAngledOverlapGroup : MonoBehaviour
    {
        [Range(0, 2), SerializeField] private float overlapPower = 1f;

        private Material[] materials;
        private float spriteHeightReference = 1f;

        void Awake()
        {
            Camera mainCam = Camera.main;

            //If we're not in deferred, then we have to turn on the DepthTexture;
            if (mainCam.renderingPath != RenderingPath.DeferredShading
            && mainCam.renderingPath != RenderingPath.DeferredLighting
            && mainCam.depthTextureMode == DepthTextureMode.None)
            {
                mainCam.depthTextureMode = DepthTextureMode.Depth;
            }

            SpriteRenderer[] sprites = GetComponentsInChildren<SpriteRenderer>();
            materials = new Material[sprites.Length];
            Bounds groupBounds = new Bounds(transform.position, Vector3.one);

            for (int i = 0; i < sprites.Length; i++)
            {
                materials[i] = sprites[i].material;
                groupBounds.Encapsulate(sprites[i].bounds);
            }
            spriteHeightReference = groupBounds.size.y;
        }

        void LateUpdate()
        {
            UpdateMaterialValues();
        }

        /// <summary>
        /// Should be called whenever the object the sprite group moves, or overlapPower value changed.
        /// Ensures the sprite's materials have the correct values to manage their culling with.
        /// </summary>
        public void UpdateMaterialValues()
        {
            foreach (Material mat in materials)
            {
                mat.SetVector("_CullValues", new Vector4(0, transform.position.y, overlapPower, spriteHeightReference));
            }
        }
    }
}