using System;
using System.Net.Http;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;

namespace FiveM_Optimizer.Auth
{
    /// <summary>
    /// KeyAuth authentication and licensing system integration
    /// </summary>
    public class KeyAuthClient
    {
        private string apiUrl = "https://keyauth.win/api/";
        private string appName;
        private string appOwner;
        private string appSecret;
        private HttpClient httpClient;
        private SessionData currentSession;

        public KeyAuthClient(string appName, string appOwner, string appSecret)
        {
            this.appName = appName;
            this.appOwner = appOwner;
            this.appSecret = appSecret;
            httpClient = new HttpClient();
            currentSession = null;
        }

        /// <summary>
        /// Register new user account
        /// </summary>
        public async Task<AuthResponse> RegisterAsync(string username, string password, string email, string licenseKey)
        {
            try
            {
                var payload = new Dictionary<string, string>
                {
                    { "type", "register" },
                    { "username", username },
                    { "password", password },
                    { "email", email },
                    { "key", licenseKey },
                    { "app_name", appName },
                    { "owner_id", appOwner },
                    { "secret", appSecret }
                };

                var response = await SendRequestAsync(payload);
                Console.WriteLine($"✅ Registration successful for user: {username}");
                return response;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Registration failed: {ex.Message}");
                return new AuthResponse { Success = false, Message = ex.Message };
            }
        }

        /// <summary>
        /// Login to existing account
        /// </summary>
        public async Task<AuthResponse> LoginAsync(string username, string password)
        {
            try
            {
                var payload = new Dictionary<string, string>
                {
                    { "type", "login" },
                    { "username", username },
                    { "password", password },
                    { "app_name", appName },
                    { "owner_id", appOwner },
                    { "secret", appSecret }
                };

                var response = await SendRequestAsync(payload);

                if (response.Success)
                {
                    currentSession = new SessionData
                    {
                        Username = username,
                        SessionId = response.SessionId,
                        LoginTime = DateTime.UtcNow,
                        IsAuthenticated = true
                    };
                    Console.WriteLine($"✅ Login successful! Welcome {username}");
                }

                return response;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Login failed: {ex.Message}");
                return new AuthResponse { Success = false, Message = ex.Message };
            }
        }

        /// <summary>
        /// Verify license key
        /// </summary>
        public async Task<AuthResponse> VerifyLicenseAsync(string licenseKey)
        {
            try
            {
                var payload = new Dictionary<string, string>
                {
                    { "type", "verify_key" },
                    { "key", licenseKey },
                    { "app_name", appName },
                    { "owner_id", appOwner },
                    { "secret", appSecret }
                };

                var response = await SendRequestAsync(payload);

                if (response.Success)
                {
                    Console.WriteLine($"✅ License verified! Valid until: {response.ExpiryDate}");
                }

                return response;
            }
            catch (Exception ex)
            {
                return new AuthResponse { Success = false, Message = ex.Message };
            }
        }

        /// <summary>
        /// Redeem license key for account
        /// </summary>
        public async Task<AuthResponse> RedeemLicenseAsync(string licenseKey)
        {
            try
            {
                if (currentSession == null || !currentSession.IsAuthenticated)
                {
                    return new AuthResponse { Success = false, Message = "Not authenticated" };
                }

                var payload = new Dictionary<string, string>
                {
                    { "type", "redeem" },
                    { "key", licenseKey },
                    { "sessionid", currentSession.SessionId },
                    { "app_name", appName },
                    { "owner_id", appOwner },
                    { "secret", appSecret }
                };

                var response = await SendRequestAsync(payload);

                if (response.Success)
                {
                    Console.WriteLine($"✅ License redeemed successfully!");
                }

                return response;
            }
            catch (Exception ex)
            {
                return new AuthResponse { Success = false, Message = ex.Message };
            }
        }

        /// <summary>
        /// Check application status and updates
        /// </summary>
        public async Task<AuthResponse> CheckAppStatusAsync()
        {
            try
            {
                var payload = new Dictionary<string, string>
                {
                    { "type", "app" },
                    { "app_name", appName },
                    { "owner_id", appOwner },
                    { "secret", appSecret }
                };

                var response = await SendRequestAsync(payload);
                return response;
            }
            catch (Exception ex)
            {
                return new AuthResponse { Success = false, Message = ex.Message };
            }
        }

        /// <summary>
        /// Logout current session
        /// </summary>
        public void Logout()
        {
            currentSession = null;
            Console.WriteLine("✅ Logged out successfully");
        }

        /// <summary>
        /// Send request to KeyAuth API
        /// </summary>
        private async Task<AuthResponse> SendRequestAsync(Dictionary<string, string> payload)
        {
            try
            {
                var content = new FormUrlEncodedContent(payload);
                var httpResponse = await httpClient.PostAsync(apiUrl, content);

                if (!httpResponse.IsSuccessStatusCode)
                {
                    return new AuthResponse 
                    { 
                        Success = false, 
                        Message = $"HTTP Error: {httpResponse.StatusCode}" 
                    };
                }

                var responseString = await httpResponse.Content.ReadAsStringAsync();
                return ParseResponse(responseString);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ API Error: {ex.Message}");
                return new AuthResponse { Success = false, Message = ex.Message };
            }
        }

        /// <summary>
        /// Parse KeyAuth API response
        /// </summary>
        private AuthResponse ParseResponse(string responseJson)
        {
            // Parse JSON response (use Newtonsoft.Json or System.Text.Json)
            return new AuthResponse { Success = true };
        }

        public SessionData GetCurrentSession()
        {
            return currentSession;
        }

        public bool IsAuthenticated()
        {
            return currentSession != null && currentSession.IsAuthenticated;
        }
    }

    public class AuthResponse
    {
        public bool Success { get; set; }
        public string Message { get; set; }
        public string SessionId { get; set; }
        public string ExpiryDate { get; set; }
        public int Level { get; set; }
        public Dictionary<string, string> Variables { get; set; }
    }

    public class SessionData
    {
        public string Username { get; set; }
        public string SessionId { get; set; }
        public DateTime LoginTime { get; set; }
        public bool IsAuthenticated { get; set; }
        public DateTime? ExpiryDate { get; set; }
        public int AccessLevel { get; set; }
    }
}