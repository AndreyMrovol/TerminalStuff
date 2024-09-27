using BepInEx;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Video;
using static TerminalStuff.AllMyTerminalPatches;
using static TerminalStuff.ViewCommands;
using Random = System.Random;

namespace TerminalStuff
{
    internal static class VideoManager //reworked this bit of code from TVLoader by Rattenbonkers, credit to them
    {
        public static List<string> Videos = [];
        private static int lastPlayedIndex = -1;
        internal static string currentlyPlaying;
        internal static TerminalNode videoPlayerNode;
        internal static bool uniqueShuffled = false;

        public static void Load()
        {
            foreach (string directory in Directory.GetDirectories(Paths.PluginPath))
            {
                string path = Path.Combine(Paths.PluginPath, directory, $"{ConfigSettings.VideoFolderPath.Value}");
                if (Directory.Exists(path))
                {
                    string[] files = Directory.GetFiles(path, "*.mp4");
                    Videos.AddRange(files);
                    Plugin.Log.LogInfo(string.Format("{0} has {1} videos.", directory, files.Length));
                    return;
                }
                else if (directory.ToLower().Contains(ConfigSettings.VideoFolderPath.Value.ToLower()))
                {
                    string path2 = Path.Combine(Paths.PluginPath, directory);
                    string[] files = Directory.GetFiles(path2, "*.mp4");
                    Videos.AddRange(files);
                    Plugin.Log.LogInfo(string.Format("{0} has {1} videos.", directory, files.Length));
                    return;
                }
            }

            Plugin.WARNING($"Unable to load video files from path configuration: {ConfigSettings.VideoFolderPath.Value}");
        }

        internal static void PlaySyncedVideo()
        {
            Plugin.MoreLogs("Start of synced LolEvent");

            TerminalNode node = Plugin.instance.Terminal.currentNode;

            if (node == null)
            {
                Plugin.WARNING("Attempted to play video on NULL node!");
                return;
            }

            node.clearPreviousText = true;
            FixVideoPatch.sanityCheckLOL = true;

            SplitViewChecks.CheckForSplitView("neither"); // Disables split view components if enabled
            if (!isVideoPlaying)
            {
                SetVideoToPlay(Plugin.instance.Terminal.videoPlayer, currentlyPlaying);
                SetupVideoPlayer(Plugin.instance.Terminal.videoPlayer, Plugin.instance.Terminal);
                Plugin.instance.Terminal.videoPlayer.Play();
                Plugin.MoreLogs("Synced video should be playing");
            }
            else
            {
                Plugin.MoreLogs("Video detected already playing, trying to stop it");
                FixVideoPatch.OnVideoEnd(Plugin.instance.Terminal);
            }

        }

        internal static string PickVideoToPlay(VideoPlayer termVP)
        {
            string displayText;

            if (!isVideoPlaying)
            {
                Plugin.MoreLogs("Video not playing, running LolEvents");
                DetermineVideoCount();
                Plugin.MoreLogs($"Random Clip: {lastPlayedIndex} - {Videos[lastPlayedIndex]}");

                // Set up the video player
                GetVideoToPlay(termVP, lastPlayedIndex);
                SetupVideoPlayer(termVP, Plugin.instance.Terminal);

                termVP.Play();
                Plugin.MoreLogs("Video should be playing");

                displayText = $"{ConfigSettings.VideoStartString.Value}\n";
                return displayText;
            }
            else if (isVideoPlaying)
            {
                Plugin.MoreLogs("Video detected playing, trying to stop it");
                FixVideoPatch.OnVideoEnd(Plugin.instance.Terminal);
                displayText = $"{ConfigSettings.VideoStopString.Value}\n";
                Plugin.MoreLogs("Lol stop detected");
                return displayText;
            }

            displayText = "Unexpected Error with displaying video... \r\n\r\n\r\n";
            return displayText;
        }

        private static void DetermineVideoCount()
        {
            // Play the next video if not playing
            if (Videos.Count == 0)
            {
                Plugin.ERROR("ERROR: No videos found, video player failure.");
                return;
            }
            else if (Videos.Count <= 2)
            {
                lastPlayedIndex = 0;
                Plugin.Spam("2 or less videos detected, no shuffle");
            }
            else if (ConfigSettings.AlwaysUniqueVideo.Value)
            {
                if (lastPlayedIndex < 0 || lastPlayedIndex >= Videos.Count - 1)
                {
                    Plugin.Spam("AlwaysUniqueVideo, shuffling");
                    // Shuffle the list of videos to get a random order
                    ShuffleList(Videos);
                    lastPlayedIndex = 0;
                    uniqueShuffled = true;
                }
                else
                {
                    lastPlayedIndex++;
                    Plugin.Spam($"set to {lastPlayedIndex} of {Videos.Count - 1}");
                }
            }
            else
            {
                Plugin.MoreLogs("More than 2 videos detected, shuffling");
                // Shuffle the list of videos to get a random order
                ShuffleList(Videos);

                // Always select the first video (except when there are only 1 or 2 videos available)
                lastPlayedIndex = Mathf.Min(lastPlayedIndex + 1, Videos.Count - 1);
                Plugin.Spam($"{lastPlayedIndex} - random video selected");
            }
        }

        private static void ShuffleList(List<string> list)
        {
            Random rand = new();
            for (int i = 0; i < list.Count; i++)
            {
                int randomIndex = rand.Next(i, list.Count);
                (list[i], list[randomIndex]) = (list[randomIndex], list[i]);
            }
        }

        private static void GetVideoToPlay(VideoPlayer termVP, int randomIndex)
        {
            termVP.clip = null;
            termVP.url = "file://" + Videos[randomIndex];
            currentlyPlaying = Videos[randomIndex];
            Plugin.MoreLogs("URL:" + termVP.url);

            if (ConfigSettings.VideoSync.Value && ConfigSettings.ModNetworking.Value && ConfigSettings.NetworkedNodes.Value)
            {
                NetHandler.SyncMyVideoChoiceToEveryone(currentlyPlaying);
                Plugin.MoreLogs("Video picked and sent to clients");
            }
        }

        private static void SetVideoToPlay(VideoPlayer termVP, string VideoName)
        {
            termVP.clip = null;
            termVP.url = "file://" + VideoName;
            currentlyPlaying = VideoName;
            Plugin.MoreLogs("URL:" + termVP.url);
        }

        private static void SetupVideoPlayer(VideoPlayer termVP, Terminal getTerm)
        {
            termVP.Stop(); // Stop for setup
            getTerm.terminalAudio.Stop(); // Fix audio

            termVP.renderMode = VideoRenderMode.RenderTexture;
            termVP.aspectRatio = VideoAspectRatio.Stretch;
            termVP.isLooping = false;
            termVP.playOnAwake = false;

            getTerm.terminalImage.texture = getTerm.videoTexture;
            termVP.targetTexture = getTerm.videoTexture;

            termVP.audioOutputMode = VideoAudioOutputMode.AudioSource;
            termVP.controlledAudioTrackCount = 1;

            termVP.SetTargetAudioSource(0, getTerm.terminalAudio);
            termVP.source = VideoSource.Url;
            termVP.enabled = true;
            Plugin.MoreLogs("Videoplayer setup complete");
        }
    }
}