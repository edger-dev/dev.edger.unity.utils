using System.Collections;
using System.Collections.Generic;
using System.IO;
using System;

using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEditor;

using Edger.Unity;

namespace Edger.Unity.Editor {
    public static partial class EditorUtil {
        public static void ForEachRootInLoadedScenes(Action<GameObject> callback) {
            for (var i = 0; i < SceneManager.sceneCount; i++) {
                var scene = SceneManager.GetSceneAt(i);
                ForEachRootInScene(scene, callback);
            }
        }

        public static void ForEachRootInActiveScene(Action<GameObject> callback) {
            var scene = SceneManager.GetActiveScene();
            ForEachRootInScene(scene, callback);
        }

        public static void ForEachRootInScene(Scene scene, Action<GameObject> callback) {
            var roots = scene.GetRootGameObjects();
            foreach (var go in roots) {
                callback(go);
            }
        }

        private static void ForEachMonoInScene<T>(Action<T> callback, GameObject go) where T : Component {
            var transforms = go.GetComponentsInChildren(typeof(Transform), true);
            foreach (Component t in transforms) {
                var components = t.gameObject.GetComponents(typeof(T));
                foreach (Component c in components) {
                    callback((T)c);
                }
            }
        }

        public static void ForEachRootInScene<T>(Scene scene, Action<T> callback) where T : Component {
            ForEachRootInScene(scene, (go) => {
                ForEachMonoInScene(callback, go);
            });
        }

        public static string[] GetAllAssetPaths(string extension = "*.*") {
            string[] paths = System.IO.Directory.GetFiles(
                                Application.dataPath,
                                extension,
                                System.IO.SearchOption.AllDirectories);

            for (int i = 0; i < paths.Length; i++) {
                paths[i] = paths[i].Replace(@"\", "/").Replace(Application.dataPath, "Assets");
            }

            return paths;
        }

        public static List<string> GetAssetPaths(UnityEngine.Object[] objects) {
            List<string> result = new List<string>();
            foreach (UnityEngine.Object selectedObject in objects) {
                string assetPath = AssetDatabase.GetAssetPath(selectedObject);
                if ((assetPath != null) && (assetPath.Length > 0)) {
                    result.Add(assetPath);
                }
            }
            return result;
        }

        public static void DisplayOkDialog(ILogger logger, string title, string content, bool isError = false) {
            if (isError) {
                logger.Error("DisplayDialog: {0}\n{1}", title, content);
            } else {
                logger.Info("DisplayDialog: {0}\n{1}", title, content);
            }
            EditorUtility.DisplayDialog(title, content, "OK");
        }
    }
}
