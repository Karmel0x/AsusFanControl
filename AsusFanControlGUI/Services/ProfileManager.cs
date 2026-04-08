using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Json;
using System.Text;
using AsusFanControlGUI.Models;

namespace AsusFanControlGUI.Services
{
    public class ProfileManager
    {
        public static readonly string ProfilesDir = Path.Combine(
            System.Environment.GetFolderPath(System.Environment.SpecialFolder.ApplicationData),
            "AsusFanControl", "profiles");

        /// <summary>Returns built-in profiles plus all saved user profiles.</summary>
        public List<FanCurveProfile> LoadAll()
        {
            var profiles = BuiltInProfiles.All;

            if (!Directory.Exists(ProfilesDir))
                Directory.CreateDirectory(ProfilesDir);

            foreach (var file in Directory.GetFiles(ProfilesDir, "*.json"))
            {
                try
                {
                    var profile = LoadFromFile(file);
                    if (profile != null)
                    {
                        profile.IsBuiltIn = false;
                        profiles.Add(profile);
                    }
                }
                catch { /* skip corrupt files */ }
            }

            return profiles;
        }

        /// <summary>Serializes a user-created profile to JSON and saves it.</summary>
        public void Save(FanCurveProfile profile)
        {
            if (profile.IsBuiltIn)
                throw new System.InvalidOperationException("Cannot save a built-in profile.");

            if (!Directory.Exists(ProfilesDir))
                Directory.CreateDirectory(ProfilesDir);

            var path = Path.Combine(ProfilesDir, SanitizeFileName(profile.Name) + ".json");
            var json = Serialize(profile);
            File.WriteAllText(path, json, Encoding.UTF8);
        }

        /// <summary>Deletes a user-created profile file.</summary>
        public void Delete(FanCurveProfile profile)
        {
            if (profile.IsBuiltIn)
                throw new System.InvalidOperationException("Cannot delete a built-in profile.");

            var path = Path.Combine(ProfilesDir, SanitizeFileName(profile.Name) + ".json");
            if (File.Exists(path))
                File.Delete(path);
        }

        // ── Serialization helpers using DataContractJsonSerializer ──────────────

        private static string Serialize(FanCurveProfile profile)
        {
            var serializer = new DataContractJsonSerializer(typeof(FanCurveProfile));
            using (var ms = new MemoryStream())
            {
                using (var writer = JsonReaderWriterFactory.CreateJsonWriter(ms, Encoding.UTF8, true, true, "  "))
                {
                    serializer.WriteObject(writer, profile);
                    writer.Flush();
                }
                return Encoding.UTF8.GetString(ms.ToArray());
            }
        }

        private static FanCurveProfile LoadFromFile(string path)
        {
            var serializer = new DataContractJsonSerializer(typeof(FanCurveProfile));
            using (var fs = File.OpenRead(path))
            {
                return (FanCurveProfile)serializer.ReadObject(fs);
            }
        }

        private static string SanitizeFileName(string name)
        {
            foreach (var c in Path.GetInvalidFileNameChars())
                name = name.Replace(c, '_');
            return name;
        }
    }
}
