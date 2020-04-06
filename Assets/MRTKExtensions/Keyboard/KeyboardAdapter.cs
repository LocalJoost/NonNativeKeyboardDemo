using Microsoft.MixedReality.Toolkit;
using Microsoft.MixedReality.Toolkit.Experimental.UI;
using Microsoft.MixedReality.Toolkit.Input;
using Microsoft.MixedReality.Toolkit.Utilities;
using UnityEngine;
using UnityEngine.UI;

namespace MRTKExtensions.Keyboard
{
    public class KeyboardAdapter : MonoBehaviour
    {
        // Note: set maxscale and minscale to 1
        [SerializeField]
        private float Hl1Distance = 1.0f;
        [SerializeField]
        private float Hl1Scale = 0.225f;
        
        [SerializeField]
        private float Hl2Distance = 0.6f;
        [SerializeField]
        private float Hl2Scale = 0.16875f;

        [SerializeField]
        private float WmrHeadSetDistance = 1.2f;

        [SerializeField]
        private float WmrHeadSetScale = 0.45f;

        [SerializeField] 
        private AudioClip _clickSound;

        private AudioSource _clickSoundPlayer;

        private void Start()
        {
            _clickSoundPlayer = gameObject.AddComponent<AudioSource>();
            _clickSoundPlayer.playOnAwake = false;
            _clickSoundPlayer.spatialize = true;
            _clickSoundPlayer.clip = _clickSound;
            var buttons = GetComponentsInChildren<Button>();
            foreach (var button in buttons)
            {
                var ni = button.gameObject.AddComponent<NearInteractionTouchableUnityUI>();
                ni.EventsToReceive = TouchableEventType.Pointer;
                button.onClick.AddListener(PlayClick);
            }


        }

        private void PlayClick()
        {
            if (_clickSound != null)
            {
                _clickSoundPlayer.Play();
            }
        }

        private float Scale => GetPlatformValue(Hl1Scale, Hl2Scale, WmrHeadSetScale);
        private float Distance => GetPlatformValue(Hl1Distance, Hl2Distance, WmrHeadSetDistance);

        
        public void ShowKeyboard()
        {
            NonNativeKeyboard.Instance.PresentKeyboard();
            NonNativeKeyboard.Instance.RepositionKeyboard(CameraCache.Main.transform.position + 
                                                          CameraCache.Main.transform.forward * Distance, 0f);
            NonNativeKeyboard.Instance.gameObject.transform.localScale *= Scale;
        }

        private float GetPlatformValue(float hl1Value, float hl2Value, float wmrHeadsetValue)
        {

            if (CoreServices.CameraSystem.IsOpaque)
            {
                return wmrHeadsetValue;
            }

            var capabilityChecker = CoreServices.InputSystem as IMixedRealityCapabilityCheck;

            return capabilityChecker.CheckCapability(MixedRealityCapability.ArticulatedHand) ? hl2Value : hl1Value;
        }
    }
}
