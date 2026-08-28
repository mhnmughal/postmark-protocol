using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;

namespace RehmanMughal.PostmarkProtocol.Editor
{
    // Fail visibly before a future player build; never ship incomplete contact details silently.
    public sealed class ReleaseCompliance : IPreprocessBuildWithReport
    {
        public int callbackOrder => -100;
        public static string[] Issues()
        {
            var issues = new List<string>();
            var info = AssetDatabase.LoadAssetAtPath<AppInformation>("Assets/Data/AppInformation.asset");
            if (info == null || !info.Ready) issues.Add("Configure a real HTTPS privacy-policy URL and a support URL/email in AppInformation.");
            var settings = new SerializedObject(AssetDatabase.LoadAllAssetsAtPath("ProjectSettings/UnityConnectSettings.asset")[0]);
            foreach (var key in new[] { "InsightsSettings.m_EngineDiagnosticsEnabled", "InsightsSettings.m_Enabled", "UnityAnalyticsSettings.m_Enabled", "UnityAnalyticsSettings.m_InitializeOnStartup", "UnityAdsSettings.m_Enabled", "CrashReportingSettings.m_EnableCloudDiagnosticsReporting", "PerformanceReportingSettings.m_Enabled" })
                if (settings.FindProperty(key)?.boolValue != false) issues.Add("Unexpected data/advertising setting: " + key);
            var flags = BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic;
            var analytics = typeof(PlayerSettings).GetProperty("submitAnalytics", flags);
            if (analytics == null || (bool)analytics.GetValue(null)) issues.Add("Player analytics reporting must be disabled.");
            return issues.ToArray();
        }
        public void OnPreprocessBuild(BuildReport report)
        {
            var issues = Issues();
            if (issues.Length != 0) throw new BuildFailedException(string.Join("\n", issues));
        }
    }
}
