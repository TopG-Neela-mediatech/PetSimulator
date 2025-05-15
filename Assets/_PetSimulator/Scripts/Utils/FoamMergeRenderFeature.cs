using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace TMKOC.PetSimulator
{
    public class FoamMergeRenderFeature : ScriptableRendererFeature
    {
        class FoamMergePass : ScriptableRenderPass
        {
            private Material mergeMaterial;
            private RTHandle cameraTarget;

            public FoamMergePass(Material material)
            {
                mergeMaterial = material;
                renderPassEvent = RenderPassEvent.AfterRenderingPostProcessing;
            }

            public override void OnCameraSetup(CommandBuffer cmd, ref RenderingData renderingData)
            {
                cameraTarget = renderingData.cameraData.renderer.cameraColorTargetHandle;
                ConfigureTarget(cameraTarget);
            }

            public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
            {
                if (mergeMaterial == null || cameraTarget == null || cameraTarget.rt == null)
                {
                    Debug.LogWarning("FoamMergePass: Skipped due to null target or material");
                    return;
                }

                Debug.Log("FoamMergePass: Executing blit to " + cameraTarget.name);
                Debug.Log("Material: " + mergeMaterial.name + ", Alpha: " + mergeMaterial.color.a);

                CommandBuffer cmd = CommandBufferPool.Get("Foam Merge Pass");
                Blitter.BlitCameraTexture(cmd, cameraTarget, cameraTarget, mergeMaterial, 0);
                context.ExecuteCommandBuffer(cmd);
                CommandBufferPool.Release(cmd);
            }

            public override void OnCameraCleanup(CommandBuffer cmd)
            {
                // Optional: clean up if needed
            }
        }

        [System.Serializable]
        public class FoamMergeSettings
        {
            public Material mergeMaterial;
        }

        public FoamMergeSettings settings = new FoamMergeSettings();

        private FoamMergePass foamPass;

        public override void Create()
        {
            if (settings.mergeMaterial != null)
            {
                foamPass = new FoamMergePass(settings.mergeMaterial);
            }
        }

        public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
        {
            if (foamPass != null)
            {
                renderer.EnqueuePass(foamPass);
            }
        }
    }
}
