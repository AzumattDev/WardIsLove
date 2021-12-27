using System;
using System.Collections;
using System.Linq;
using System.Text.RegularExpressions;
using HarmonyLib;
using UnityEngine;
using UnityEngine.Networking;
using static WardIsLove.WardIsLovePlugin;

namespace WardIsLove.Util
{
    [HarmonyPatch]
    public class GitHubCheck
    {
        private static System.Version ParseVersion(string input)
        {
            try
            {
                System.Version ver = System.Version.Parse(input);
                WILLogger.LogDebug($"Converted {input} to {ver}.");
                return ver;
            }
            catch (ArgumentNullException)
            {
                WILLogger.LogError("Error: String to be parsed is null.");
            }
            catch (ArgumentOutOfRangeException)
            {
                WILLogger.LogError($"Error: Negative value in '{input}'.");
            }
            catch (ArgumentException)
            {
                WILLogger.LogError($"Error: Bad number of components in '{input}'.");
            }
            catch (FormatException)
            {
                WILLogger.LogError($"Error: Non-integer value in '{input}'.");
            }
            catch (OverflowException)
            {
                WILLogger.LogError($"Error: Number out of range in '{input}'.");
            }

            return System.Version.Parse(version);
        }

        internal static IEnumerator CheckForNewVersion()
        {
            System.Version currentVersion = ParseVersion(version);
            UnityWebRequest unityWebRequest =
                UnityWebRequest.Get(ApiRepositoryLatestRelease);
            yield return unityWebRequest.SendWebRequest();

            if (unityWebRequest.isNetworkError || unityWebRequest.isHttpError)
            {
                Debug.Log("Error While Sending: " + unityWebRequest.error);
            }
            else
            {
                bool coolKidVersion = false;
                string githubversion =
                    unityWebRequest.downloadHandler.text.Split(new[] { "," }, StringSplitOptions.None)[25]
                        .Trim().Replace("\"", "").Replace("tag_name: ", "");
                WILLogger.LogDebug(githubversion);

                System.Version githubParseVersion = ParseVersion(githubversion);

                if (githubParseVersion > currentVersion)
                {
                    IsUpToDate = false;
                }
                else if (githubParseVersion < currentVersion)
                {
                    WILLogger.LogWarning(
                        $"You seem to be running a test version of {ModName}, congrats...you're one of the cool kids. Remember to ask {Author} for a new version every now and then.");
                    coolKidVersion = true;
                    IsUpToDate = true;
                }
                else if (githubParseVersion != currentVersion)
                {
                    WILLogger.LogWarning(
                        $"Received GitHub version is not equal: GitHub version = {githubversion}; local version = {version}");
                }
                else if (githubParseVersion == currentVersion)
                {
                    IsUpToDate = true;
                }

                if (!IsUpToDate && !coolKidVersion)
                    WILLogger.LogWarning(
                        $"There is a newer version available of {ModName}. The latest version is v{githubversion}. Please visit the GitHub https://valheim.thunderstore.io/package/Azumatt/{ModName}/ to download the latest");
                else
                    WILLogger.LogInfo($"{ModName} [" + version + "] is up to date.");
            }
        }
    }
}