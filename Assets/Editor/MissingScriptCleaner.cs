using System.Linq;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

public static class MissingScriptCleaner
{
    // 빌드 대상 씬 전체에서 missing script 컴포넌트 일괄 제거 (배치 모드용)
    public static void CleanBuildScenes()
    {
        var total = 0;
        foreach (var scenePath in EditorBuildSettings.scenes.Where(s => s.enabled).Select(s => s.path))
        {
            var scene = EditorSceneManager.OpenScene(scenePath, OpenSceneMode.Single);
            var removed = 0;
            foreach (var root in scene.GetRootGameObjects())
            {
                foreach (var t in root.GetComponentsInChildren<Transform>(true))
                {
                    removed += GameObjectUtility.RemoveMonoBehavioursWithMissingScript(t.gameObject);
                }
            }

            if (removed > 0)
            {
                EditorSceneManager.MarkSceneDirty(scene);
                EditorSceneManager.SaveScene(scene);
            }
            Debug.Log($"[MissingScriptCleaner] {scenePath}: {removed}개 제거");
            total += removed;
        }
        Debug.Log($"[MissingScriptCleaner] 총 {total}개 제거 완료");
        EditorApplication.Exit(0);
    }
}
