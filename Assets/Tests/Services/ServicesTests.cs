using System;
using System.IO;
using NUnit.Framework;
using UnityEngine;

namespace RehmanMughal.PostmarkProtocol.Tests
{
    public sealed class ServicesTests
    {
        private string folder;
        private string path;
        [SetUp] public void Setup()
        {
            folder = Path.Combine(Path.GetTempPath(), "PostmarkServicesTests-" + Guid.NewGuid().ToString("N"));
            Directory.CreateDirectory(folder);
            path = Path.Combine(folder, "save.json");
        }
        [TearDown] public void Cleanup() { if (Directory.Exists(folder)) Directory.Delete(folder, true); }
        private SaveRepository Load() { var repo = new SaveRepository(path); repo.Load(); return repo; }

        [Test] public void NewSaveHasPlayableDefaults()
        {
            var repo = Load();
            Assert.That(repo.Data.version, Is.EqualTo(2));
            Assert.That(repo.Data.unlockedLevel, Is.EqualTo(1));
            Assert.That(repo.Data.unlockedChapter, Is.EqualTo(1));
            Assert.That(repo.Data.haptics, Is.True);
            Assert.That(repo.GetLevel(1).delivered, Is.False);
        }
        [Test] public void WriteReadRestoresSettingsAndProgress()
        {
            var repo = Load();
            repo.Data.musicVolume = 0.25f;
            repo.Data.soundVolume = 0.6f;
            repo.Data.haptics = false;
            repo.Data.tutorialSeen.Add("first-dispatch");
            repo.RecordDelivery(1, 1, 4, 2, 4);
            var restored = Load();
            Assert.That(restored.Data.musicVolume, Is.EqualTo(0.25f));
            Assert.That(restored.Data.soundVolume, Is.EqualTo(0.6f));
            Assert.That(restored.Data.haptics, Is.False);
            Assert.That(restored.Data.tutorialSeen, Does.Contain("first-dispatch"));
            Assert.That(restored.GetLevel(1).perfect, Is.True);
        }
        [Test] public void VersionOneUpgradesWithoutLosingKnownFields()
        {
            File.WriteAllText(path, "{\"version\":1,\"unlockedLevel\":8,\"musicVolume\":0.3,\"levels\":[{\"id\":7,\"delivered\":true,\"bestCommandCount\":3}]}");
            var repo = Load();
            Assert.That(repo.Data.version, Is.EqualTo(2));
            Assert.That(repo.Data.unlockedLevel, Is.EqualTo(8));
            Assert.That(repo.Data.unlockedChapter, Is.EqualTo(2));
            Assert.That(repo.GetLevel(7).bestCommandCount, Is.EqualTo(3));
            Assert.That(repo.Data.haptics, Is.True);
            Assert.That(repo.Save(), Is.True);
            Assert.That(Load().GetLevel(7).delivered, Is.True);
        }
        [Test] public void FutureVersionIsNotOverwritten()
        {
            const string original = "{\"version\":99,\"unlockedLevel\":12,\"futureField\":\"retain me\"}";
            File.WriteAllText(path, original);
            var repo = Load();
            Assert.That(repo.Data.unlockedLevel, Is.EqualTo(12));
            Assert.That(repo.Save(), Is.False);
            Assert.That(File.ReadAllText(path), Is.EqualTo(original));
        }
        [Test] public void CorruptPrimaryRecoversBackup()
        {
            var repo = Load();
            repo.RecordDelivery(1, 1, 5, 1, 5);
            repo.RecordDelivery(2, 2, 8, 2, 8);
            File.WriteAllText(path, "{broken");
            var restored = Load();
            Assert.That(restored.GetLevel(1).delivered, Is.True);
            Assert.That(restored.LastIssue, Does.Contain("Recovered"));
            Assert.That(restored.Save(), Is.True);
            File.WriteAllText(path, "corrupt again");
            Assert.That(Load().GetLevel(1).delivered, Is.True);
        }
        [Test] public void ChecksumRejectsTampering()
        {
            var repo = Load();
            repo.RecordDelivery(1, 1, 5, 1, 5);
            repo.Save();
            var original = File.ReadAllText(path);
            var modified = original.Replace("unlockedLevel\\\":2", "unlockedLevel\\\":35");
            Assert.That(modified, Is.Not.EqualTo(original));
            File.WriteAllText(path, modified);
            Assert.That(Load().Data.unlockedLevel, Is.EqualTo(2));
        }
        [Test] public void MissingOrCorruptFilesRemainPlayable()
        {
            File.WriteAllText(path, "{}");
            File.WriteAllText(path + ".bak", "not json");
            var repo = Load();
            Assert.That(repo.Data.unlockedLevel, Is.EqualTo(1));
            Assert.That(repo.Save(), Is.True);
        }
        [Test] public void UnavailableStorageKeepsSessionProgress()
        {
            var blocker = Path.Combine(folder, "file-not-directory");
            File.WriteAllText(blocker, "occupied");
            var repo = new SaveRepository(Path.Combine(blocker, "save.json"));
            repo.Load();
            repo.RecordDelivery(1, 1, 5, 1, 5);
            Assert.That(repo.Save(), Is.False);
            Assert.That(repo.GetLevel(1).delivered, Is.True);
            Assert.That(repo.LastIssue, Is.Not.Empty);
        }
        [Test] public void ResetKeepsSettingsAndClearsRecoverableProgress()
        {
            var repo = Load();
            repo.Data.musicVolume = 0.2f;
            repo.Data.soundVolume = 0.3f;
            repo.Data.haptics = false;
            repo.Data.tutorialSeen.Add("intro");
            repo.RecordDelivery(12, 3, 14, 3, 14);
            Assert.That(repo.ResetProgress(), Is.True);
            File.WriteAllText(path, "corrupt");
            var restored = Load();
            Assert.That(restored.Data.unlockedLevel, Is.EqualTo(1));
            Assert.That(restored.Data.levels, Is.Empty);
            Assert.That(restored.Data.tutorialSeen, Is.Empty);
            Assert.That(restored.Data.musicVolume, Is.EqualTo(0.2f));
            Assert.That(restored.Data.soundVolume, Is.EqualTo(0.3f));
            Assert.That(restored.Data.haptics, Is.False);
        }
        [Test] public void FailedResetPreservesSessionAndCanBeRetried()
        {
            var repo = Load();
            repo.RecordDelivery(12, 3, 14, 3, 14);
            Directory.CreateDirectory(path + ".tmp"); // Deterministically prevents the atomic write.
            Assert.That(repo.ResetProgress(), Is.False);
            Assert.That(repo.GetLevel(12).delivered, Is.True);
            Assert.That(Load().GetLevel(12).delivered, Is.True);
            Assert.That(repo.LastIssue, Does.Contain("Reset was not saved"));
            Directory.Delete(path + ".tmp");
            Assert.That(repo.ResetProgress(), Is.True);
            Assert.That(Load().Data.levels, Is.Empty);
        }
        [Test] public void AppInformationRejectsUnsafeLinksAndIncompleteContact()
        {
            Assert.That(AppInformation.IsWebUrl("https://example.com/privacy"), Is.True);
            Assert.That(AppInformation.IsWebUrl("javascript:alert(1)"), Is.False);
            Assert.That(AppInformation.IsWebUrl("https://user:password@example.com"), Is.False);
            Assert.That(AppInformation.IsEmail("support@example.com"), Is.True);
            Assert.That(AppInformation.IsEmail("support@example.com\r\nBcc:other@example.com"), Is.False);
            var info = ScriptableObject.CreateInstance<AppInformation>();
            try { Assert.That(info.Ready, Is.False); }
            finally { UnityEngine.Object.DestroyImmediate(info); }
        }
        [Test] public void ProgressionUnlocksNextChapterAndStopsAt36()
        {
            var repo = Load();
            for (int i = 1; i <= 36; i++)
            {
                repo.RecordDelivery(i, 2, 8, 2, 8);
                Assert.That(repo.Data.unlockedLevel, Is.EqualTo(Math.Min(36, i + 1)));
                Assert.That(repo.Data.unlockedChapter, Is.EqualTo((repo.Data.unlockedLevel - 1) / 6 + 1));
            }
        }
        [Test] public void ReplayNeverDowngradesSealsOrBestScores()
        {
            var repo = Load();
            repo.RecordDelivery(1, 1, 5, 1, 5);
            repo.RecordDelivery(1, 4, 12, 1, 5);
            var progress = Load().GetLevel(1);
            Assert.That(progress.delivered && progress.efficient && progress.perfect, Is.True);
            Assert.That(progress.bestCommandCount, Is.EqualTo(1));
            Assert.That(progress.bestSteps, Is.EqualTo(5));
        }
        [Test] public void SealRequirementsAreIndependentAndEarnedOnlyWhenMet()
        {
            var repo = Load();
            repo.RecordDelivery(1, 4, 10, 3, 8);
            Assert.That(repo.GetLevel(1).efficient, Is.False);
            Assert.That(repo.GetLevel(1).perfect, Is.False);
            repo.RecordDelivery(1, 3, 10, 3, 8);
            Assert.That(repo.GetLevel(1).efficient, Is.True);
            Assert.That(repo.GetLevel(1).perfect, Is.False);
            repo.RecordDelivery(1, 3, 8, 3, 8);
            Assert.That(repo.GetLevel(1).perfect, Is.True);
        }
        [Test] public void InvalidDataIsSanitized()
        {
            File.WriteAllText(path, "{\"version\":1,\"musicVolume\":9,\"soundVolume\":-2,\"unlockedLevel\":99,\"levels\":[null,{\"id\":-1},{\"id\":2},{\"id\":2}],\"tutorialSeen\":null}");
            var repo = Load();
            Assert.That(repo.Data.musicVolume, Is.EqualTo(1));
            Assert.That(repo.Data.soundVolume, Is.EqualTo(0));
            Assert.That(repo.Data.unlockedLevel, Is.EqualTo(36));
            Assert.That(repo.Data.levels.Count, Is.EqualTo(1));
            Assert.That(repo.Data.tutorialSeen, Is.Not.Null);
        }
        [Test] public void InvalidLevelIdsAreRejected()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => Load().GetLevel(0));
            Assert.Throws<ArgumentOutOfRangeException>(() => Load().GetLevel(37));
        }
        [Test] public void MixerVolumeMappingIsRealDecibelScale()
        {
            Assert.That(AudioService.Decibels(0), Is.EqualTo(-80));
            Assert.That(AudioService.Decibels(1), Is.EqualTo(0));
            Assert.That(AudioService.Decibels(0.5f), Is.EqualTo(-6.0206f).Within(0.001f));
        }

        // The same NUnit checks can run synchronously through MCP, with every fixture isolated.
        public static string RunAll()
        {
            var fixture = new ServicesTests();
            int count = 0;
            foreach (var method in typeof(ServicesTests).GetMethods())
            {
                if (!Attribute.IsDefined(method, typeof(TestAttribute))) continue;
                fixture.Setup();
                try { method.Invoke(fixture, null); count++; }
                finally { fixture.Cleanup(); }
            }
            string message = "POSTMARK SERVICES QA PASS: " + count + " NUnit checks.";
            Debug.Log(message);
            return message;
        }
    }
}
