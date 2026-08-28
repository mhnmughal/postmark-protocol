using System;
using System.Net.Mail;
using UnityEngine;

namespace RehmanMughal.PostmarkProtocol
{
    [CreateAssetMenu(menuName = "Postmark/App information")]
    public sealed class AppInformation : ScriptableObject
    {
        public string privacyPolicyUrl = "";
        public string supportUrl = "";
        public string supportEmail = "";
        public static bool IsWebUrl(string value) => Uri.TryCreate(value, UriKind.Absolute, out var uri)
            && uri.Scheme == Uri.UriSchemeHttps && !string.IsNullOrEmpty(uri.Host) && string.IsNullOrEmpty(uri.UserInfo);
        public static bool IsEmail(string value)
        {
            if (string.IsNullOrWhiteSpace(value) || value.Contains("\r") || value.Contains("\n")) return false;
            try { return new MailAddress(value).Address == value && value.Contains("."); }
            catch (FormatException) { return false; }
        }
        public bool Ready => IsWebUrl(privacyPolicyUrl) && (IsWebUrl(supportUrl) || IsEmail(supportEmail));
    }
}
