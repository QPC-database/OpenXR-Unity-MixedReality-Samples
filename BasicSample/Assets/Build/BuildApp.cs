﻿// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Linq;
using UnityEditor;
using UnityEditor.Build.Reporting;
using UnityEngine;

namespace Microsoft.MixedReality.OpenXR.BasicSample.Build
{
    public static class BuildApp
    {
        private static string[] scenes = { };
        private static string buildPath = "build";

        public static void StartCommandLineBuild()
        {
            ParseBuildCommandLine();

            // We don't need stack traces on all our logs. Makes things a lot easier to read.
            Application.SetStackTraceLogType(LogType.Log, StackTraceLogType.None);
            Debug.Log($"Starting command line build for {EditorUserBuildSettings.activeBuildTarget}...");

            BuildPlayerOptions options = new BuildPlayerOptions()
            {
                scenes = scenes.Length > 0 ? scenes : EditorBuildSettings.scenes.Where(scene => scene.enabled).Select(scene => scene.path).ToArray(),
                locationPathName = buildPath,
                targetGroup = EditorUserBuildSettings.selectedBuildTargetGroup,
                target = EditorUserBuildSettings.activeBuildTarget,
            };

            bool success;
            try
            {
                BuildReport buildReport = BuildPipeline.BuildPlayer(options);
                success = buildReport != null && buildReport.summary.result == BuildResult.Succeeded;
            }
            catch (Exception e)
            {
                Debug.LogError($"Build Failed!\n{e.Message}\n{e.StackTrace}");
                success = false;
            }

            Debug.Log($"Finished build... Build success? {success}");
        }

        private static void ParseBuildCommandLine()
        {
            string[] arguments = Environment.GetCommandLineArgs();

            for (int i = 0; i < arguments.Length; ++i)
            {
                switch (arguments[i])
                {
                    case "-sceneList":
                        scenes = SplitSceneList(arguments[++i]);
                        break;
                    case "-buildOutput":
                        buildPath = arguments[++i];
                        break;
                }
            }
        }

        private static string[] SplitSceneList(string sceneList)
        {
            return (from scene in sceneList.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries)
                    select scene.Trim()).ToArray();
        }
    }
}
