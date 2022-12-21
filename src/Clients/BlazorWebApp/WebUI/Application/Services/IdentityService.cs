using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components.Authorization;
using WebUI.Application.Interfaces;
using WebUI.Domain.Models.UserModels;
using WebUI.Extensions;
using WebUI.Utils;

namespace WebUI.Application.Services
{
    public class IdentityService : IIdentityService
    {
        private readonly HttpClient _client;
        private readonly ISyncLocalStorageService _syncLocalStorageService;
        private readonly AuthenticationStateProvider _authenticationStateProvider;

        public IdentityService(HttpClient httpClient, ISyncLocalStorageService syncLocalStorageService, AuthenticationStateProvider authenticationStateProvider)
        {
            _client = httpClient;
            _syncLocalStorageService = syncLocalStorageService;
            _authenticationStateProvider = authenticationStateProvider;
        }

        public bool IsLoggedIn => !string.IsNullOrEmpty(GetUserToken());

        public string GetUserName()
        {
            return _syncLocalStorageService.GetUserName();
        }

        public string GetUserToken()
        {
            return _syncLocalStorageService.GetToken();
        }

        public async Task<bool> Login(string userName, string password)
        {
            var request = new UserLoginRequest(userName, password);

            var response = await _client.PostGetResponseAsync<UserLoginResponse, UserLoginRequest>("auth", request);

            if (!string.IsNullOrEmpty(response.UserToken))//login success
            {
                _syncLocalStorageService.SetToken(response.UserToken);
                _syncLocalStorageService.SetUserName(response.UserName);

                ((AuthStateProvider)_authenticationStateProvider).NotifyUserLogin(response.UserName);

                _client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("bearer", response.UserToken);
                return true;
            }
            return false;
        }

        public void Logout()
        {
            _syncLocalStorageService.RemoveItem("token");
            _syncLocalStorageService.RemoveItem("username");

            ((AuthStateProvider)_authenticationStateProvider).NotifyUserLogout();

            _client.DefaultRequestHeaders.Authorization = null;
        }
    }
}