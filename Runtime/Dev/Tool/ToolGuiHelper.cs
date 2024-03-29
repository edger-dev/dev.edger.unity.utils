using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Edger.Unity.Dev {
    public static class ToolGuiHelper {
        public const int MinScreenWidth = 800;
        public const int DefaultFontSize = 16;

        private static int _LastScreenWidth;
        private static int _LastScreenHeight;

        private static int _ScreenWidth = MinScreenWidth;
        public static int ScreenWidth {
            get {
                if (Screen.width != _LastScreenWidth
                        || Screen.height != _LastScreenHeight) {
                    CheckScale();
                }
                return _ScreenWidth;
            }
        }
        private static int _ScreenHeight;
        public static int ScreenHeight {
            get {
                if (Screen.width != _LastScreenWidth
                        || Screen.height != _LastScreenHeight) {
                    CheckScale();
                }
                return _ScreenHeight;
            }
        }

        public static Rect _ScreenRect = Rect.zero;
        public static Rect ScreenRect {
            get {
                if (Screen.width != _LastScreenWidth
                        || Screen.height != _LastScreenHeight) {
                    CheckScale();
                }
                return _ScreenRect;
            }
        }

        private static float _Scale;
        public static float Scale {
            get {
                if (Screen.width != _LastScreenWidth
                        || Screen.height != _LastScreenHeight) {
                    CheckScale();
                }
                return _Scale;
            }
        }

        public static bool CheckScale() {
            if (Screen.width != _LastScreenWidth
                    || Screen.height != _LastScreenHeight) {
                SetScreenWidth(_ScreenWidth);
                return true;
            }
            return false;
        }

        public static void SetScreenWidth(int screenWidth) {
            screenWidth = Math.Max(screenWidth, MinScreenWidth);
            _ScreenWidth = screenWidth;

            _LastScreenWidth = Screen.width;
            _LastScreenHeight = Screen.height;

            if (_LastScreenWidth > 0) {
                _Scale = (float)_LastScreenWidth / (float)_ScreenWidth;
                _ScreenHeight = Mathf.RoundToInt(_LastScreenHeight / _Scale);
            } else {
                _Scale = 1f;
            }
            _ScreenRect = NewRect(0, 0, _ScreenWidth, _ScreenHeight);

            Log.Info("ToolGuiHelper.SetScreenWidth: {0}, {1} -> {2}, {3} -> {4}",
                    Screen.width, Screen.height, ScreenWidth, ScreenHeight, Scale);
        }

        public static Rect NewRect(int x, int y, int width, int height) {
            return new Rect(x * Scale, y * Scale, width * Scale, height * Scale);
        }

        public static GUIStyle NewTextStyle(Color color, int fontSize = DefaultFontSize) {
            var style = new GUIStyle(GUI.skin.label);
            style.fontSize = Mathf.RoundToInt(fontSize * Scale);
            style.normal.textColor = color;
            return style;
        }

        public static GUIStyle NewButtonStyle(Color color, int fontSize = DefaultFontSize) {
            var style = new GUIStyle(GUI.skin.button);
            style.fontSize = Mathf.RoundToInt(fontSize * Scale);
            style.normal.textColor = color;
            style.hover.textColor = color;
            return style;
        }

        public static GUIStyle NewTextFieldStyle(Color color, int fontSize = DefaultFontSize) {
            var style = new GUIStyle(GUI.skin.textField);
            style.fontSize = Mathf.RoundToInt(fontSize * Scale);
            style.normal.textColor = color;
            return style;
        }
    }
}
