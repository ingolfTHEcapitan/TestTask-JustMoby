using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;

namespace _Project.Scripts.Services.NetworkAccessibility
{
    public class NetworkAccessibilityService: INetworkAccessibilityService, IDisposable
    {
        private const string PingUrl = "https://www.google.com";
        private const int TimeoutSeconds = 10;

        private CancellationTokenSource _cancellationTokenSource;
        

        public async UniTask<bool> CheckNetworkConnectionAsync()
        {
            if (Application.internetReachability == NetworkReachability.NotReachable)
                return false;

            try
            {
                using (UnityWebRequest request = UnityWebRequest.Head(PingUrl))
                {
                    request.timeout = TimeoutSeconds;

                    _cancellationTokenSource = new CancellationTokenSource();

                    await request.SendWebRequest().WithCancellation(_cancellationTokenSource.Token).AsUniTask();
                
                    if (request.result == UnityWebRequest.Result.Success)
                        return true;

                    Debug.LogWarning("[NETWORK ACCESS SERVICE] No internet access via URL: " + PingUrl);
                    return false;
                }
            }
            catch (OperationCanceledException)
            {
                return false;
            }
            catch (Exception e)
            {
                Debug.LogError($"[NETWORK ACCESS SERVICE] No internet access via URL: {PingUrl}, message {e.Message}");
                return  false;
            }
        }

        public void Dispose()
        {
            _cancellationTokenSource?.Cancel();
            _cancellationTokenSource?.Dispose();
            _cancellationTokenSource = null;
        }
    }
}