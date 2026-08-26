using UnityEngine;
using UnityEngine.SceneManagement;

namespace RehmanMughal.PostmarkProtocol
{
    public sealed class PostalApplication : MonoBehaviour
    {
        public static int SelectedLevel = 1;
        private void Awake()
        {
            Application.targetFrameRate = 60;
            Screen.sleepTimeout = SleepTimeout.SystemSetting;
            _ = SaveService.Current;
        }
        private void Start() { SceneManager.LoadScene("MainDesk"); }
        private void OnApplicationPause(bool paused) { if (paused) SaveService.Save(); }
        private void OnApplicationQuit() { SaveService.Save(); }
    }
}
