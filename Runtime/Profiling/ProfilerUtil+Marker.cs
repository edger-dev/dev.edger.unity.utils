using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Profiling;
using Unity.Profiling;

namespace Edger.Utils.Profiling {
    public partial class ProfilerUtil {
        private static readonly Dictionary<string, ProfilerMarker> _Markers = new Dictionary<string, ProfilerMarker>();

        public static ProfilerMarker GetMarker(string name) {
            ProfilerMarker marker;
            if (!_Markers.TryGetValue(name, out marker)) {
                marker = new ProfilerMarker(name);
                _Markers[name] = marker;
            }
            return marker;
        }

        public static ProfilerMarker GetMarker(Type type) {
            return GetMarker(type.Name);
        }
    }
}