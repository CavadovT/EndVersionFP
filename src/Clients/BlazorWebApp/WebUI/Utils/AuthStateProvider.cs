using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components.Authorization;
using System.Security.Claims;
using WebUI.Extensions;
using WebUI.Infrastructure;

namespace WebUI.Utils
{
    public class AuthStateProvider : AuthenticationStateProvider
    {
        private readonly ILocalStorageService _localStorage;
        private readonly HttpClient _client;
        private readonly AuthenticationState _anonymous;
        private readonly AppStateManager appState;

        public AuthStateProvider(ILocalStorageService localStorage, HttpClient client, AppStateManager appState)
        {
            this.appState = appState;
            _localStorage = localStorage;
            _client = client;
            _anonymous = new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity()));
        }

        public override async Task<AuthenticationState> GetAuthenticationStateAsync()
        {
            String apiToken = await _localStorage.GetToken();

            if (String.IsNullOrEmpty(apiToken))
                return _anonymous;

            String userName = await _localStorage.GetUserName();

            var cp = new ClaimsPrincipal(new ClaimsIdentity(new[]
            {
                new Claim(ClaimTypes.Name, userName),
            }, "jwtAuthType"));

            _client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", apiToken);

            return new AuthenticationState(cp);
        }

        public void NotifyUserLogin(string userName)
        {
            var cp = new ClaimsPrincipal(new ClaimsIdentity(new[]
            {
                new Claim(ClaimTypes.Name,userName)
            }, "jwtAuthType"));

            var authState = Task.FromResult(new AuthenticationState(cp));

            NotifyAuthenticationStateChanged(authState);
            appState.LoginChanged(null);
        }

        public void NotifyUserLogout()
        {
            var authState = Task.FromResult(_anonymous);
            NotifyAuthenticationStateChanged(authState);
            appState.LoginChanged(null);
        }
    }
}