using System.Reflection; 
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System;

using UnityEngine;
using UnityEditor;

using Edger.Unity;

namespace Edger.Unity.Editor {
    public class EdgerMenu { 
        public static bool ClearConsoleOnMenu = true;

        private static void LogMenuItem(MenuItem menu) {
            if (!Application.isPlaying) {
                if (ClearConsoleOnMenu) {
                    ClearConsole();
                }
                Log.Init(new UnityLogProvider(false, "editor_menu"));
            }
            Log.Error("EdgerMenu: {0}", menu.menuItem);
        }

    #if UNITY_EDITOR_OSX
        [MenuItem("Edger/Fix Shaders in Scene %e")] // CMD + E
    #else
        [MenuItem("Edger/Fix Shaders in Scene &e")] // ALT + E
    #endif
        public static void FixShadersInScene() {
            LogMenuItem((MenuItem)MethodBase.GetCurrentMethod().GetCustomAttributes(typeof(MenuItem), true)[0]);
            _FixShadersInScene(false);
        }

        [MenuItem("Edger/Fix Shaders in Scene (Include Inactive)")]
        public static void FixShadersInSceneIncludeInactive() {
            LogMenuItem((MenuItem)MethodBase.GetCurrentMethod().GetCustomAttributes(typeof(MenuItem), true)[0]);
            _FixShadersInScene(true);
        }

        private static void _FixShadersInScene(bool includeInactive) {
            int goCount = 0;
            int count = 0;
            var fixers = ShaderFixer.CreateFixers();
            EditorUtil.ForEachRootInLoadedScenes((GameObject go) => {
                if (go != null) {
                    goCount++;
                    count += ShaderFixer.FixGameObject(go, includeInactive, fixers);
                }
            });
            //Log.Info("FixShadersInScene() -> GameObject count: {0}, Material count: {1}", goCount, count);
            if (count == 0) {
                EditorUtil.DisplayOkDialog(Log.Provider, "Fix Shaders in Scene", "Nothing Found in Scene", true);
            }
        }

        [MenuItem ("Edger/Clear Console Logs %&c", false, 100001)] // CTRL/CMD + ALT + C
        public static void ClearConsole() {
            EditorUtil.ClearConsole();
        }
    }
}
