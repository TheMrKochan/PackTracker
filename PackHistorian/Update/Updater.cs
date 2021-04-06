using Hearthstone_Deck_Tracker;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Runtime.Serialization.Json;
using System.Text.RegularExpressions;

namespace PackTracker.Update
{
    public class Updater
    {
        private static string _userAgend = "PackTracker";

        public bool? NewVersionAvailable()
        {
            try
            {
                var LatestRelease = this.GetLatestRelease();
                var LatestVersion = ParseVersion(LatestRelease.tag_name);

                return Plugin.CurrentVersion.CompareTo(LatestVersion) < 0;
            }
            catch (WebException)
            {
                return null;
            }

        }

        public static Version ParseVersion(string version)
        {
            return new Version(Regex.Match(version, @"\d+(\.\d+)*").ToString());
        }

        public bool Update()
        {
            var LatestRelease = this.GetLatestRelease();
            var Asset = LatestRelease.assets.Single(x => x.name == "PackTracker.zip");

            try
            {
                using (var client = new WebClient())
                {
                    using (var download = client.OpenRead(Asset.browser_download_url))
                    {
                        var path = Path.Combine(Config.AppDataPath, "Plugins");
                        var tempPath = Path.Combine(Path.GetTempPath(), "PackTracker");

                        if (Directory.Exists(tempPath))
                        {
                            Directory.Delete(tempPath, true);
                        }

                        var Zipper = new ZipArchive(download);
                        Zipper.ExtractToDirectory(tempPath);

                        foreach (var file in Directory.GetFiles(tempPath))
                        {
                            var target = Path.Combine(path, Path.GetFileName(file));
                            File.Copy(file, target, true);
                            File.SetLastWriteTime(target, DateTime.Now);
                        }

                        Directory.Delete(tempPath, true);

                        return true;
                    }
                }
            }
            catch (WebException)
            {
                return false;
            }
        }

        public Release GetLatestRelease()
        {
            var request = WebRequest.CreateHttp(@"https://api.github.com/repos/sgkoishi/PackTracker/releases/latest");
            request.Proxy = null;
            request.UserAgent = _userAgend;

            var Release = new Release();
            using (var response = request.GetResponse().GetResponseStream())
            {
                var ser = new DataContractJsonSerializer(Release.GetType());
                Release = (Release)ser.ReadObject(response);
            }

            return Release;
        }

        public IEnumerable<Release> GetAllReleases()
        {
            var Releases = new List<Release>();

            var request = WebRequest.CreateHttp(@"https://api.github.com/repos/sgkoishi/PackTracker/releases");
            request.Proxy = null;
            request.UserAgent = _userAgend;

            try
            {
                using (var response = request.GetResponse().GetResponseStream())
                {
                    var ser = new DataContractJsonSerializer(Releases.GetType());
                    Releases = (List<Release>)ser.ReadObject(response);
                }
            }
            catch (WebException)
            {
                return null;
            }

            return Releases.AsEnumerable();
        }
    }
}
