using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Profiling;
using Unity.Profiling;

namespace Edger.Unity.Dev.Profiling {
    public partial class ProfilerTool {
        private static readonly Dictionary<string, ProfilerMarker> _Markers = new Dictionary<string, ProfilerMarker>();

        public static ProfilerMarker GetMarker(ProfilerCategory category, string name) {
            ProfilerMarker marker;
            if (!_Markers.TryGetValue(name, out marker)) {
                marker = new ProfilerMarker(category, name);
                _Markers[name] = marker;
            }
            return marker;
        }

        public static ProfilerMarker GetMarker(ProfilerCategory category, string name, ProfilerItemFormat format) {
            bool isNew = !_Markers.ContainsKey(name);
            var marker = GetMarker(category, name);
            if (isNew) {
                AddProfilerItem(name, marker, format);
            }
            return marker;
        }

        public static ProfilerMarker GetMarker(string name) {
            return GetMarker(ProfilerCategory.Scripts, name);
        }

        public static ProfilerMarker GetMarker(string name, ProfilerItemFormat format) {
            return GetMarker(ProfilerCategory.Scripts, name, format);
        }
    }
}