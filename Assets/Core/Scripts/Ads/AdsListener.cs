using System;
using UnityEngine;
using UnityEngine.Advertisements;

namespace HyperCasual.Core
{
    public class AdsListener : MonoBehaviour, IUnityAdsLoadListener, IUnityAdsShowListener
    {
        [SerializeField] string m_EndOfLevelAndroidAdUnitId = "Interstitial_Android";
        [SerializeField] string m_EndOfLevelIOSAdUnitId = "Interstitial_iOS";
        [SerializeField] GenericGameEventListener[] m_EndOfLevelListeners;

        public void Awake()
        {
            foreach (GenericGameEventListener trigger in m_EndOfLevelListeners)
            {
                trigger.EventHandler += OnEndOfLevel;
                trigger.Subscribe();
            }
        }

        public void OnDestroy()
        {
            foreach (GenericGameEventListener trigger in m_EndOfLevelListeners)
            {
                trigger.EventHandler -= OnEndOfLevel;
                trigger.Unsubscribe();
            }
        }

        public void OnEndOfLevel()
        {
            AdsController adsController = AdsController.Instance;
            if (adsController.Initialized)
            {
                string adId = m_EndOfLevelAndroidAdUnitId;
                if (Application.platform == RuntimePlatform.IPhonePlayer)
                {
                    adId = m_EndOfLevelIOSAdUnitId;
                }
                
                Debug.Log($"Loading ad {adId} from an end of level event");
                adsController.ShowAd(adId, this);
            }
            else
            {
                Debug.Log($"Could not load ad because {nameof(AdsController)} has not initialized");
            }
        }
        
        // --- IUnityAdsLoadListener

        /// <summary>
        /// Handler for when an ad is successfully loaded
        /// </summary>
        /// <param name="adUnitId">The ad unit ID for the loaded ad</param>
        public void OnUnityAdsAdLoaded(string adUnitId)
        {
            // Optionally execute code if the Ad Unit successfully loads content.
            AdsController.Instance.ShowAd(adUnitId, this);
        }
     
        /// <summary>
        /// Handler for when a Unity ad fails to load
        /// </summary>
        /// <param name="adUnitId">The ad unit ID for the ad</param>
        /// <param name="error">The error that prevented the ad from loading</param>
        /// <param name="message">The message accompanying the error</param>
        public void OnUnityAdsFailedToLoad(string adUnitId, UnityAdsLoadError error, string message)
        {
            Debug.LogError($"Error loading Ad Unit: {adUnitId} - {error.ToString()} - {message}");
            // Optionally execute code if the Ad Unit fails to load, such as attempting to try again.
        }

        // --- IUnityAdsShowListener

        /// <summary>
        /// Handler for when an add finishes showing
        /// </summary>
        /// <param name="adUnitId"></param>
        /// <param name="showCompletionState"></param>
        public void OnUnityAdsShowComplete(string adUnitId, UnityAdsShowCompletionState showCompletionState) { }

        /// <summary>
        /// Handler for when showing an ad fails
        /// </summary>
        /// <param name="adUnitId">The ad unit ID for the ad</param>
        /// <param name="error">The error that prevented the ad from loading</param>
        /// <param name="message">The message accompanying the error</param>
        public void OnUnityAdsShowFailure(string adUnitId, UnityAdsShowError error, string message)
        {
            Debug.LogError($"Error showing Ad Unit {adUnitId}: {error.ToString()} - {message}");
        }
     
        /// <summary>
        /// Handler for when an ad starts showing
        /// </summary>
        /// <param name="adUnitId"></param>
        public void OnUnityAdsShowStart(string adUnitId) { }
        
        /// <summary>
        /// Handler for when the user clicks/taps on an ad
        /// </summary>
        /// <param name="adUnitId"></param>
        public void OnUnityAdsShowClick(string adUnitId) { }
    }
}
