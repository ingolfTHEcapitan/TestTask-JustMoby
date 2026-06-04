using System;
using System.Threading.Tasks;
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

        private async Task SignUpAnonymouslyAsync()
        {
            try
            {
                await AuthenticationService.Instance.SignInAnonymouslyAsync();
                Debug.Log("Sign in anonymously succeeded! is IsSignedIn");
                Debug.Log("Player ID " + AuthenticationService.Instance.PlayerId);
            }
            catch (AuthenticationException exception)
            {
                Debug.LogException(exception);
            }
            catch (RequestFailedException exception)
            {
                Debug.LogException(exception);
            }
        }
    }
}