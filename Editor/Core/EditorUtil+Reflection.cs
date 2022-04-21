using System;
using System.Reflection;
using System.Diagnostics;

using UnityEngine;
using UnityEditor;

using Edger.Unity;

namespace Edger.Unity.Editor {
    public static partial class EditorUtil {
        //http://answers.unity3d.com/questions/707636/clear-console-window.html
        public static void ClearConsole () {
            if (!Application.isEditor) {
                Log.Error("UnityEditorHelper.ClearConsole() only works in editor");
                return;
            }
            // This simply does "LogEntries.Clear()" the long way:
            var logEntries = System.Type.GetType("UnityEditorInternal.LogEntries,UnityEditor.dll");
            if (logEntries != null) {
                var clearMethod = logEntries.GetMethod("Clear", System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.Public);
                clearMethod.Invoke(null, null);
            }
        }

        //This is for easier control from tools
        public static void SetSelectionActiveObject(object obj) {
            if (!Application.isEditor) {
                return;
            }
            // This simply does "LogEntries.Clear()" the long way:
            var selection = System.Type.GetType("UnityEditor.Selection,UnityEditor.dll");
            var activeObject = selection.GetProperty("activeObject", System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.Public);
            activeObject.SetValue(null, obj, null);
        }

        //Stop running in editor, didn't use conditional compiling, so can work in dll.
        //http://answers.unity3d.com/questions/161858/startstop-playmode-from-editor-script.html
        public static void SetEditorApplicationIsPlaying(bool isPlaying) {
            if (!Application.isEditor) {
                Log.Error("UnityEditorHelper.SetEditorApplicationIsPlaying() only works in editor");
                return;
            }
            //only works in editor
            //casts "UnityEditor.EditorApplication.isPlaying = false;" dynamically using reflection
            Type t = AssemblyUtil.GetType("UnityEditor.EditorApplication");
            if (t != null) {
                t.GetProperty("isPlaying").SetValue(null, isPlaying, null);
            }
        }

        public static void StartInEditor() {
            SetEditorApplicationIsPlaying(true);
        }

        public static void StopInEditor() {
            SetEditorApplicationIsPlaying(false);
        }

        public static string RunSystemCommand(string cwd, string script, string args) {
            ProcessStartInfo procInfo = new ProcessStartInfo(script);
            procInfo.WorkingDirectory = cwd;
            procInfo.Arguments = args;
            procInfo.UseShellExecute = false;
            procInfo.RedirectStandardOutput = true;

            Process proc = new Process();
            proc.StartInfo = procInfo;
            proc.Start();
            string output = proc.StandardOutput.ReadToEnd();
            proc.WaitForExit();
            proc.Close();

            //Can NOT use Log here, since it's not initialized if not running
            UnityEngine.Debug.Log(string.Format("RunSystemCommand: [{0}]: '{1}' '{2}' ->\n{3}",
                                        cwd, script, args, output));
            return output;
        }
    }
}
