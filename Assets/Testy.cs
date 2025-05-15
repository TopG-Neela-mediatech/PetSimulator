using DG.Tweening;
using UnityEngine;

namespace TMKOC.PetSimulator
{
    public class Testy : MonoBehaviour
    {
        private void Start()
        {
            RotateAroundY();
        }


        public void RotateAroundY()
        {
            // Rotates 360 degrees around the local Y axis over 5 seconds
            transform.DORotate(new Vector3(0, 360, 0), 5f, RotateMode.FastBeyond360)
                     .SetEase(Ease.Linear)
                     .SetLoops(-1, LoopType.Incremental);
        }

        // Update is called once per frame
        void Update()
        {
        
        }
    }
}
