#if UNITY_EDITOR

using System;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.Build.Reporting;
using UnityEngine;

namespace Editor
{
    public static class BuildActions
    {
        private static readonly int BuildNumber = int.TryParse(Environment.GetEnvironmentVariable("BUILD_NUMBER"), out var value) ? value : 0;
        private static readonly string BuildVersion = $"{Application.version}.{BuildNumber / 256}.{BuildNumber}";

        private static readonly string BuildPath = Directory.GetCurrentDirectory();

        private static readonly string BuildPathIOS = $"{BuildPath}/Builds/iOS";
        private static readonly string BuildPathAndroid = $"{BuildPath}/Builds/android-{BuildVersion}.apk";

        private static string[] BuildScenes => EditorBuildSettings.scenes.Where(scene => scene.enabled).Select(scene => scene.path).ToArray();

        [MenuItem("Build/iOS")]
        public static void BuildIOSMenu() => BuildIOS_Impl(false);
        [MenuItem("Build/Android")]
        public static void BuildAndroidMenu() => BuildAndroid_Impl(false);
    
        public static void BuildIOS() => BuildIOS_Impl(true);
        public static void BuildAndroid() => BuildAndroid_Impl(true);

        private static BuildReport BuildCommon(BuildTargetGroup targetGroup, BuildTarget target, string buildPath)
        {
            PlayerSettings.SetScriptingDefineSymbolsForGroup(targetGroup, "DEVELOPMENT"); // TODO: Change as needed
            EditorUserBuildSettings.SwitchActiveBuildTarget(targetGroup, target);
            EditorUserBuildSettings.development = true; // TODO: Change as needed
            PlayerSettings.bundleVersion = BuildVersion;
            var options = new BuildPlayerOptions
            {
                scenes = BuildScenes,
                locationPathName = buildPath,
                target = target,
                options = BuildOptions.None
            };
            return BuildPipeline.BuildPlayer(options);
        }

        private static void BuildIOS_Impl(bool reportAndExitAfterBuild)
        {
            PlayerSettings.SetScriptingBackend(BuildTargetGroup.iOS, ScriptingImplementation.IL2CPP);
            var result = BuildCommon(BuildTargetGroup.iOS, BuildTarget.iOS, BuildPathIOS);
            if (!reportAndExitAfterBuild)
            {
                return;
            }
            File.WriteAllText(@".build_summary.json", JsonUtility.ToJson(result.summary));
            EditorApplication.Exit(result.summary.result == BuildResult.Succeeded ? 0 : 1);
        }

        private static void BuildAndroid_Impl(bool reportAndExitAfterBuild)
        {
            var result = BuildCommon(BuildTargetGroup.Android, BuildTarget.Android, BuildPathAndroid);
            if (!reportAndExitAfterBuild)
            {
                return;
            }
            File.WriteAllText(@".build_summary.json", JsonUtility.ToJson(result.summary));
            EditorApplication.Exit(result.summary.result == BuildResult.Succeeded ? 0 : 1);
        }
    }
}

#endif
