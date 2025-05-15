using UnityEngine;

namespace TMKOC.PetSimulator
{
    public class FoamSurfaceType : MonoBehaviour
    {
        [SerializeField] private SurfaceType surfaceType;


        public SurfaceType SurfaceType { get { return surfaceType; } }
    }
}
