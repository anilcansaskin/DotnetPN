using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using DotnetPN.Interfaces;
using FirebaseAdmin;
using FirebaseAdmin.Messaging;
using Google.Apis.Auth.OAuth2;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace DotnetPN.Services;

public class FcmService : IFcmService
{
    private readonly ILogger<FcmService> _logger;
    private readonly bool _isInitialized;

    public FcmService(IConfiguration configuration, ILogger<FcmService> logger)
    {
        _logger = logger;
        try
        {
            var credentialPath = configuration["Firebase:CredentialFilePath"];
            if (string.IsNullOrEmpty(credentialPath))
            {
                credentialPath = "firebase-adminsdk.json";
            }

            var fullPath = Path.Combine(Directory.GetCurrentDirectory(), credentialPath);
            
            if (File.Exists(fullPath))
            {
                if (FirebaseApp.DefaultInstance == null)
                {
                    using var stream = new FileStream(fullPath, FileMode.Open, FileAccess.Read);
                    FirebaseApp.Create(new AppOptions
                    {
                        Credential = CredentialFactory.FromStream<ServiceAccountCredential>(stream).ToGoogleCredential()
                    });
                }
                _isInitialized = true;
                _logger.LogInformation("Firebase Admin SDK successfully initialized from file: {Path}", fullPath);
            }
            else
            {
                _logger.LogWarning("Firebase credential file not found at {Path}. FCM operations will be simulated or fail.", fullPath);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to initialize Firebase Admin SDK.");
        }
    }

    public async Task<bool> SendNotificationAsync(string token, string title, string body, Dictionary<string, string>? data = null)
    {
        if (!_isInitialized)
        {
            _logger.LogWarning("FCM is not initialized. Cannot send notification.");
            return false;
        }

        try
        {
            var message = new Message
            {
                Token = token,
                Notification = new Notification
                {
                    Title = title,
                    Body = body
                },
                Data = data
            };

            var response = await FirebaseMessaging.DefaultInstance.SendAsync(message);
            _logger.LogInformation("Successfully sent message: {Response}", response);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error sending FCM notification to token {Token}", token);
            return false;
        }
    }

    public async Task<int> SendNotificationToAllAsync(IEnumerable<string> tokens, string title, string body, Dictionary<string, string>? data = null)
    {
        if (!_isInitialized)
        {
            _logger.LogWarning("FCM is not initialized. Cannot send notifications.");
            return 0;
        }

        var tokenList = tokens.ToList();
        if (tokenList.Count == 0) return 0;

        int successfulSends = 0;
        // Firebase Multicast allows up to 500 tokens per request
        const int batchSize = 500;
        for (int i = 0; i < tokenList.Count; i += batchSize)
        {
            var batch = tokenList.Skip(i).Take(batchSize).ToList();
            try
            {
                var message = new MulticastMessage
                {
                    Tokens = batch,
                    Notification = new Notification
                    {
                        Title = title,
                        Body = body
                    },
                    Data = data
                };

                var response = await FirebaseMessaging.DefaultInstance.SendEachForMulticastAsync(message);
                successfulSends += response.SuccessCount;
                _logger.LogInformation("Sent batch of notifications. Success: {SuccessCount}, Failure: {FailureCount}", response.SuccessCount, response.FailureCount);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error sending batch notification.");
            }
        }

        return successfulSends;
    }
}
