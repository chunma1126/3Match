using UnityEngine;
using GoogleMobileAds.Api;
using UnityEngine.UI;

public class AddEnergyButton : MonoBehaviour
{
    private RewardedAd rewardedAd;

#if UNITY_ANDROID
    private string adUnitId = "ca-app-pub-3940256099942544/5224354917"; // 테스트용
#elif UNITY_IOS
    private string adUnitId = "ca-app-pub-3940256099942544/1712485313"; // 테스트용
#else
    private string adUnitId = "unexpected_platform";
#endif

    private void Start()
    {
        // 1. AdMob 초기화 (콜백 방식으로 변경)
        MobileAds.Initialize((InitializationStatus initStatus) =>
        {
            Debug.Log("AdMob 초기화 완료");
            // 2. 광고 로드
            LoadRewardedAd();
        });
        
        // 3. 버튼에 리스너 연결
        GetComponent<UnityEngine.UI.Button>()?.onClick.AddListener(ShowRewardedAd);
    }
    
    private void LoadRewardedAd()
    {
        // 이전 광고 객체 제거
        if (rewardedAd != null)
        {
            rewardedAd.Destroy();
            rewardedAd = null;
        }

        var adRequest = new AdRequest();

        // 4. 콜백 방식으로 광고 로드
        RewardedAd.Load(adUnitId, adRequest, (RewardedAd ad, LoadAdError error) =>
        {
            if (error != null || ad == null)
            {
                Debug.LogError("보상 광고 로딩 실패: " + error?.GetMessage());
                return;
            }

            Debug.Log("보상 광고 로딩 완료");
            rewardedAd = ad;

            // 5. 이벤트 핸들러 설정
            RegisterEventHandlers(rewardedAd);
        });
    }

    private void RegisterEventHandlers(RewardedAd ad)
    {
        // 광고가 전체 화면 콘텐츠를 열 때
        ad.OnAdFullScreenContentOpened += () =>
        {
            Debug.Log("보상 광고 전체 화면 열림");
        };
        
        // 광고가 전체 화면 콘텐츠를 닫을 때
        ad.OnAdFullScreenContentClosed += () =>
        {
            Debug.Log("보상 광고 전체 화면 닫힘");
            
            // 다음 광고를 미리 로드
            LoadRewardedAd();
        };

        // 광고 표시 실패
        ad.OnAdFullScreenContentFailed += (AdError error) =>
        {
            Debug.LogError("보상 광고 전체 화면 표시 실패: " + error.GetMessage());
            
            // 실패했을 때도 재로드 시도
            LoadRewardedAd();
        };

        // 광고 수익 발생
        ad.OnAdPaid += (AdValue adValue) =>
        {
            Debug.Log($"광고 수익 발생: {adValue.Value} {adValue.CurrencyCode}");
        };

        // 광고 노출 기록
        ad.OnAdImpressionRecorded += () =>
        {
            Debug.Log("보상 광고 노출 기록됨");
        };

        // 광고 클릭
        ad.OnAdClicked += () =>
        {
            Debug.Log("보상 광고 클릭됨");
        };
    }

    private void ShowRewardedAd()
    {
        if (rewardedAd != null && rewardedAd.CanShowAd())
        {
            rewardedAd.Show((Reward reward) =>
            {
                Debug.Log($"보상 획득: {reward.Type}, {reward.Amount}");
                AddEnergy();
                TitleUIManager.Instance.PopUp(PopupType.Add);
            });
        }
        else
        {
            Debug.Log("보상 광고가 준비되지 않음. 재로딩 시도...");
            LoadRewardedAd();
        }
    }

    private void AddEnergy()
    {
        TitleUIManager.Instance.AddEnergy(1);        
    }
    
    private void OnDestroy()
    {
        if (rewardedAd != null)
        {
            rewardedAd.Destroy();
            rewardedAd = null;
        }
    }
}