using Cysharp.Threading.Tasks;
using Unity.Services.Authentication;
using Unity.Services.Core;
using UnityEngine;

namespace _Project.Scripts.Services.Authentication
{
    public class AuthService : IAuthService
    {
        public bool IsSignedIn => AuthenticationService.Instance.IsSignedIn;
        
        public async UniTask SignUpAsync()
        {
            await UnityServices.InitializeAsync();
            await SignUpAnonymouslyAsync();
        }

        private async UniTask SignUpAnonymouslyAsync()
        {
            try
            {
                await AuthenticationService.Instance.SignInAnonymouslyAsync();
            }
            catch (AuthenticationException exception)
            {
                Debug.LogError($"[AUTH SERVICE] Anonymous sign-in failed (auth): {exception.Message}");
                Debug.LogException(exception);
            }
            catch (RequestFailedException exception)
            {
                Debug.LogError($"[AUTH SERVICE] Anonymous sign-in failed (network): {exception.Message}");
                Debug.LogException(exception);
            }
        }
    }
}