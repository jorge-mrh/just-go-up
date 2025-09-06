using UnityEngine;
using System;
using GoogleMobileAds;
using GoogleMobileAds.Api;
using System.Data.Common;


public class AdmobBanner : MonoBehaviour
{
    public string bannerID = "";
    BannerView bannerView;

    public void Start()
    {
        // Initialize Google Mobile Ads SDK.
        MobileAds.Initialize((InitializationStatus initStatus) =>
        {
            // This callback is called once the MobileAds SDK is initialized.
            CreateBannerView();
        });
    }

    public void CreateBannerView()
    {
        Debug.Log("Creating banner view");

        //If we already have a banner, destroy the old one
        if (bannerView != null)
        {
            DestroyAdd();
        }

        AdSize adaptiveSize =
        AdSize.GetCurrentOrientationAnchoredAdaptiveBannerAdSizeWithWidth(AdSize.FullWidth);

        // Create a 320x50 banner at top of the screen.
        bannerView = new BannerView(bannerID, adaptiveSize, AdPosition.Top);
        LoadAd();
    }

    public void LoadAd()
    {
        if (bannerView == null)
        {
            CreateBannerView();
        }

        //Create request used to load the ad
        var adRequest = new AdRequest();

        //Send the request to load the ad
        Debug.Log("Loading ad");
        bannerView.LoadAd(adRequest);
    }

    public void DestroyAdd()
    {
        if (bannerView != null)
        {
            Debug.Log("Destroying ad");
            bannerView.Destroy();
            bannerView = null;
        }
    }
}
