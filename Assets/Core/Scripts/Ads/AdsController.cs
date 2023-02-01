using System;
using UnityEngine.Advertisements;
using UnityEngine;

namespace HyperCasual.Core
{
    public class AdsController : AbstractSingleton<AdsController>, IUnityAdsInitializationListener, IUnityAdsLoadListener, IUnityAdsShowListener
    {
        private static bool s_Initialized = false;
        public bool Initialized => s_Initialized;

        
        #pragma warning disable 0414
        //NOTE: Replace the following in the prefab with your gameID from your Unity web dashboard. This will be different for each platform.
        [SerializeField]
        private string m_AndroidGameId = "4960150";
        
        [SerializeField]
        private string m_iOSGameId = "4960151";
        #pragma warning restore 0414
        
        
#if UNITY_ANDROID
        public string GameId => m_AndroidGameId;
#elif UNITY_IOS
        public string GameId => m_iOSGameId;
#else
        public string GameId => m_AndroidGameId; // For testing
#endif
        
        public bool TestMode { get; set; } = true; // Change this for production builds

        protected override void Awake()
        {
            base.Awake();
            Initialize();
        }

        public void Initialize()
        {
            if (!Advertisement.isSupported)
            {
                Debug.LogWarning(Application.platform + " is not supported by Advertisement");
            }
            else
            {
                Advertisement.Initialize(GameId, TestMode, false, this);
            }
        }

        public void SetBanner(string bannerId, BannerPosition position, bool visible, bool destroy = false)
        {
            if (visible)
            {
                Advertisement.Banner.SetPosition(position);
                Advertisement.Banner.Show(bannerId);
            }
            else
            {
                Advertisement.Banner.Hide(destroy);
            }
        }

        public void LoadAd(string adId) => LoadAd(adId, this);
        public void LoadAd(string adId, IUnityAdsLoadListener callbackListener)
        {
            Advertisement.Load(adId, callbackListener);
        }

        public void ShowAd(string adId) => ShowAd(adId, this);
        public void ShowAd(string adId, IUnityAdsShowListener callbackListener)
        {
            Advertisement.Show(adId, callbackListener);
        }

        // --- Interface Implementations (Logging if no callback listener is provided):

        public void OnInitializationComplete()
        {
            s_Initialized = true;
        }

        public void OnInitializationFailed(UnityAdsInitializationError error, string message)
        {
            Debug.Log($"Init Failed: [{error}]: {message}");
        }

        public void OnUnityAdsAdLoaded(string placementId)
        {
            Debug.Log($"Load Success: {placementId}");
        }

        public void OnUnityAdsFailedToLoad(string placementId, UnityAdsLoadError error, string message)
        {
            Debug.Log($"Load Failed: [{error}:{placementId}] {message}");
        }

        public void OnUnityAdsShowFailure(string placementId, UnityAdsShowError error, string message)
        {
            Debug.Log($"OnUnityAdsShowFailure: [{error}]: {message}");
        }

        public void OnUnityAdsShowStart(string placementId)
        {
            Debug.Log($"OnUnityAdsShowStart: {placementId}");
        }

        public void OnUnityAdsShowClick(string placementId)
        {
            Debug.Log($"OnUnityAdsShowClick: {placementId}");
        }

        public void OnUnityAdsShowComplete(string placementId, UnityAdsShowCompletionState showCompletionState)
        {
            Debug.Log($"OnUnityAdsShowComplete: [{showCompletionState}]: {placementId}");
        }
    }
}
