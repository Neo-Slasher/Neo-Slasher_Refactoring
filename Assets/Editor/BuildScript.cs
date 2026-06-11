using System.Linq;
using UnityEditor;
using UnityEngine;

public static class BuildScript
{
    // 커맨드라인 검증용 개발 빌드 (디버그 서명, 키스토어 불필요)
    public static void BuildAndroidDev()
    {
        PlayerSettings.Android.useCustomKeystore = false;
        EditorUserBuildSettings.buildAppBundle = false;

        var options = new BuildPlayerOptions
        {
            scenes = EditorBuildSettings.scenes.Where(s => s.enabled).Select(s => s.path).ToArray(),
            locationPathName = "Builds/NeoSlasher-dev.apk",
            target = BuildTarget.Android,
            options = BuildOptions.Development,
        };

        var report = BuildPipeline.BuildPlayer(options);
        Debug.Log($"[BuildScript] result={report.summary.result} errors={report.summary.totalErrors} " +
                  $"size={report.summary.totalSize} time={report.summary.totalTime}");
        EditorApplication.Exit(report.summary.result == UnityEditor.Build.Reporting.BuildResult.Succeeded ? 0 : 1);
    }
}
