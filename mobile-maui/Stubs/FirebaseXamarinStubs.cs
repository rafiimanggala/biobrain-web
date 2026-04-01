using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

// Firebase Xamarin stubs replaced with working REST API implementations.
// Uses HttpClient + Newtonsoft.Json only (no additional NuGet packages).

namespace Firebase.Xamarin.Auth
{
    public class FirebaseConfig
    {
        public string ApiKey { get; }
        public FirebaseConfig(string apiKey) => ApiKey = apiKey;
    }

    public class FirebaseAuthProvider
    {
        private static readonly HttpClient _http = new HttpClient();
        private const string AuthBaseUrl = "https://identitytoolkit.googleapis.com/v1";

        public FirebaseConfig Config { get; }
        public FirebaseAuthProvider(FirebaseConfig config) => Config = config;

        public async Task<FirebaseAuthLink> SignInWithEmailAndPasswordAsync(string email, string password)
        {
            var url = $"{AuthBaseUrl}/accounts:signInWithPassword?key={Config.ApiKey}";
            var payload = JsonConvert.SerializeObject(new
            {
                email,
                password,
                returnSecureToken = true
            });

            var response = await _http.PostAsync(url, new StringContent(payload, Encoding.UTF8, "application/json"));
            var json = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                ThrowSignInException(json);
            }

            var result = JObject.Parse(json);
            return new FirebaseAuthLink
            {
                FirebaseToken = result["idToken"]?.ToString(),
                ApiKey = Config.ApiKey,
                User = new FirebaseUser
                {
                    LocalId = result["localId"]?.ToString(),
                    Email = result["email"]?.ToString()
                }
            };
        }

        public async Task<FirebaseAuthLink> CreateUserWithEmailAndPasswordAsync(string email, string password)
        {
            var url = $"{AuthBaseUrl}/accounts:signUp?key={Config.ApiKey}";
            var payload = JsonConvert.SerializeObject(new
            {
                email,
                password,
                returnSecureToken = true
            });

            var response = await _http.PostAsync(url, new StringContent(payload, Encoding.UTF8, "application/json"));
            var json = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                ThrowSignUpException(json);
            }

            var result = JObject.Parse(json);
            return new FirebaseAuthLink
            {
                FirebaseToken = result["idToken"]?.ToString(),
                ApiKey = Config.ApiKey,
                User = new FirebaseUser
                {
                    LocalId = result["localId"]?.ToString(),
                    Email = result["email"]?.ToString()
                }
            };
        }

        public async Task SendPasswordResetEmailAsync(string email)
        {
            var url = $"{AuthBaseUrl}/accounts:sendOobCode?key={Config.ApiKey}";
            var payload = JsonConvert.SerializeObject(new
            {
                requestType = "PASSWORD_RESET",
                email
            });

            var response = await _http.PostAsync(url, new StringContent(payload, Encoding.UTF8, "application/json"));

            if (!response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();
                var errorMsg = ParseFirebaseErrorMessage(json);
                throw new Exception($"Password reset failed: {errorMsg}");
            }
        }

        private static string ParseFirebaseErrorMessage(string json)
        {
            try
            {
                var obj = JObject.Parse(json);
                return obj["error"]?["message"]?.ToString() ?? "UNKNOWN_ERROR";
            }
            catch
            {
                return "UNKNOWN_ERROR";
            }
        }

        private static void ThrowSignInException(string json)
        {
            var errorMsg = ParseFirebaseErrorMessage(json);

            if (errorMsg.Contains("INVALID_PASSWORD") || errorMsg.Contains("INVALID_LOGIN_CREDENTIALS"))
                throw new FirebaseIncorrectPasswordException(errorMsg);

            if (errorMsg.Contains("INVALID_EMAIL"))
                throw new FirebaseInvalidEmailException(errorMsg);

            if (errorMsg.Contains("TOO_MANY_ATTEMPTS_TRY_LATER"))
                throw new FirebaseTooManyAttemptsException(errorMsg);

            throw new FirebaseIncorrectPasswordException(errorMsg);
        }

        private static void ThrowSignUpException(string json)
        {
            var errorMsg = ParseFirebaseErrorMessage(json);

            if (errorMsg.Contains("EMAIL_EXISTS"))
                throw new FirebaseUsedEmailException(errorMsg);

            if (errorMsg.Contains("WEAK_PASSWORD"))
                throw new FirebaseWeakPasswordException(errorMsg);

            if (errorMsg.Contains("INVALID_EMAIL"))
                throw new FirebaseInvalidEmailException(errorMsg);

            throw new FirebaseRegisterException(errorMsg);
        }
    }

    public class FirebaseAuthLink
    {
        private static readonly HttpClient _http = new HttpClient();
        private const string AuthBaseUrl = "https://identitytoolkit.googleapis.com/v1";

        public string FirebaseToken { get; set; }
        public string ApiKey { get; set; }
        public FirebaseUser User { get; set; }

        public async Task UpdatePasswordAsync(string newPassword)
        {
            var url = $"{AuthBaseUrl}/accounts:update?key={ApiKey}";
            var payload = JsonConvert.SerializeObject(new
            {
                idToken = FirebaseToken,
                password = newPassword,
                returnSecureToken = true
            });

            var response = await _http.PostAsync(url, new StringContent(payload, Encoding.UTF8, "application/json"));
            var json = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                throw new Exception($"Update password failed: {ParseErrorMessage(json)}");
            }

            // Update token with the refreshed one
            var result = JObject.Parse(json);
            var newToken = result["idToken"]?.ToString();
            if (!string.IsNullOrEmpty(newToken))
            {
                FirebaseToken = newToken;
            }
        }

        public async Task DeleteAccountAsync()
        {
            var url = $"{AuthBaseUrl}/accounts:delete?key={ApiKey}";
            var payload = JsonConvert.SerializeObject(new
            {
                idToken = FirebaseToken
            });

            var response = await _http.PostAsync(url, new StringContent(payload, Encoding.UTF8, "application/json"));

            if (!response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();
                throw new Exception($"Delete account failed: {ParseErrorMessage(json)}");
            }
        }

        private static string ParseErrorMessage(string json)
        {
            try
            {
                var obj = JObject.Parse(json);
                return obj["error"]?["message"]?.ToString() ?? "UNKNOWN_ERROR";
            }
            catch
            {
                return "UNKNOWN_ERROR";
            }
        }
    }

    public class FirebaseUser
    {
        public string LocalId { get; set; }
        public string Email { get; set; }
    }

    // Exception types used in AuthorizationRepository catch blocks
    public class FirebaseIncorrectPasswordException : Exception
    {
        public FirebaseIncorrectPasswordException() { }
        public FirebaseIncorrectPasswordException(string message) : base(message) { }
    }

    public class FirebaseInvalidEmailException : Exception
    {
        public FirebaseInvalidEmailException() { }
        public FirebaseInvalidEmailException(string message) : base(message) { }
    }

    public class FirebaseTooManyAttemptsException : Exception
    {
        public FirebaseTooManyAttemptsException() { }
        public FirebaseTooManyAttemptsException(string message) : base(message) { }
    }

    public class FirebaseUsedEmailException : Exception
    {
        public FirebaseUsedEmailException() { }
        public FirebaseUsedEmailException(string message) : base(message) { }
    }

    public class FirebaseWeakPasswordException : Exception
    {
        public FirebaseWeakPasswordException() { }
        public FirebaseWeakPasswordException(string message) : base(message) { }
    }

    public class FirebaseRegisterException : Exception
    {
        public FirebaseRegisterException() { }
        public FirebaseRegisterException(string message) : base(message) { }
    }
}

namespace Firebase.Xamarin.Database
{
    public class FirebaseClient
    {
        public string DatabaseUrl { get; }
        public FirebaseClient(string databaseUrl) => DatabaseUrl = databaseUrl.TrimEnd('/');

        public Query.ChildQuery Child(string path) => new Query.ChildQuery(this, path, null);
    }
}

namespace Firebase.Xamarin.Database.Query
{
    public class ChildQuery
    {
        private static readonly HttpClient _http = new HttpClient();

        private readonly Firebase.Xamarin.Database.FirebaseClient _client;
        private readonly string _path;
        private readonly string _authToken;

        public ChildQuery(Firebase.Xamarin.Database.FirebaseClient client, string path, string authToken)
        {
            _client = client;
            _path = path;
            _authToken = authToken;
        }

        public ChildQuery Child(string path) => new ChildQuery(_client, $"{_path}/{path}", _authToken);

        public ChildQuery WithAuth(string token) => new ChildQuery(_client, _path, token);

        private string BuildUrl()
        {
            var url = $"{_client.DatabaseUrl}/{_path}.json";
            if (!string.IsNullOrEmpty(_authToken))
            {
                url += $"?auth={_authToken}";
            }
            return url;
        }

        public async Task<T> OnceSingleAsync<T>()
        {
            var url = BuildUrl();
            var response = await _http.GetAsync(url);
            var json = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                throw new Exception($"Firebase DB read failed ({response.StatusCode}): {json}");
            }

            if (string.IsNullOrEmpty(json) || json == "null")
            {
                return default;
            }

            return JsonConvert.DeserializeObject<T>(json);
        }

        public async Task PutAsync<T>(T data)
        {
            var url = BuildUrl();
            var payload = JsonConvert.SerializeObject(data);
            var response = await _http.PutAsync(url, new StringContent(payload, Encoding.UTF8, "application/json"));

            if (!response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();
                throw new Exception($"Firebase DB write failed ({response.StatusCode}): {json}");
            }
        }

        public async Task DeleteAsync()
        {
            var url = BuildUrl();
            var response = await _http.DeleteAsync(url);

            if (!response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();
                throw new Exception($"Firebase DB delete failed ({response.StatusCode}): {json}");
            }
        }
    }
}
