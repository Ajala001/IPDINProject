﻿namespace App.Core.DTOs.Responses
{
    public class AuthResponse
    {
        public string AccessToken { get; set; } = string.Empty;
        public string RefreshToken { get; set; } = string.Empty;
    }
}
