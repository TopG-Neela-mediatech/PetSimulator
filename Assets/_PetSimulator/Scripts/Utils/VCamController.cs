using UnityEngine;

namespace TMKOC.PetSimulator
{
    public class VCamController : MonoBehaviour
    {
        [SerializeField] private GameObject m_VCamMain;
        [SerializeField] private GameObject m_VCamBrush;

        private void OnEnable()
        {
            PlayerView.OnReadyToBrush += SwitchToBrush;
            PlayerView.OnBrushingCompleted += SwitchToCameraFace;
        }

        private void OnDisable()
        {
            PlayerView.OnReadyToBrush -= SwitchToBrush;
            PlayerView.OnBrushingCompleted -= SwitchToCameraFace;
        }

        private void SwitchToBrush()
        {
            m_VCamMain.SetActive(false);
            m_VCamBrush.SetActive(true);
        }

        private void SwitchToCameraFace()
        {
            m_VCamMain.SetActive(true);
            m_VCamBrush.SetActive(false);
        }
    }
}
