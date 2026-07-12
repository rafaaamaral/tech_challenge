using System;
using System.Collections.Generic;
using System.Text;

namespace tech_challenge.Infrastructure.Services.Email
{
    public sealed class EmailSettings
    {
        public const string SectionName = "Email";
        public string Host { get; init; } = string.Empty;
        public int Port { get; init; }
        public string UserName { get; init; } = string.Empty;
        public string Password { get; init; } = string.Empty;
        public string From { get; init; } = string.Empty;
        public string DisplayName { get; init; } = string.Empty;
        public bool EnableSsl { get; init; } = true;
        public string ApiBaseUrl { get; init; } = string.Empty;
    }
}
