using System;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Profiling;
using Unity.Profiling;

namespace Edger.Utils.Profiling {
    public enum ProfilerItemFormat {
        MegaBytes,
        MilliSeconds,
    }

    public class ProfilerItem {
        public readonly ProfilerRecorder Recorder;
        public readonly string Prefix;
        public readonly bool UseAvarage;
        public readonly ProfilerItemFormat Format;

        private List<ProfilerRecorderSample> _Samples = null;

        public ProfilerItem(ProfilerRecorder recorder, string prefix, bool useAvarage, ProfilerItemFormat format) {
            Recorder = recorder;
            Prefix = prefix;
            UseAvarage = useAvarage;
            Format = format;

            if (useAvarage) {
                _Samples = new List<ProfilerRecorderSample>(recorder.Capacity);
            }
        }

        private double CalcAverageValue() {
            var count = Recorder.Count;
            if (count == 0)
                return 0;

            double r = 0;
            Recorder.CopyTo(_Samples);
            for (var i = 0; i < count; i++)
                r += _Samples[i].Value;
            return r / count;
        }

        public void AppendValue(StringBuilder builder, double value) {
            switch (Format) {
                case ProfilerItemFormat.MegaBytes:
                    builder.Append($"{value / 1024 / 1024:F2} MB");
                    break;
                case ProfilerItemFormat.MilliSeconds:
                    builder.Append($"{value * (1e-6f):F2} ms");
                    break;
            }
        }
        public void Append(StringBuilder builder) {
            builder.Append(Prefix);
            if (UseAvarage) {
                AppendValue(builder, CalcAverageValue());
            } else {
                AppendValue(builder, Recorder.LastValueAsDouble);
            }
        }
    }

    public partial class ProfilerUtil {
        public const int DEFAULT_CAPACITY = 15;
        private static readonly Dictionary<string, ProfilerRecorder> _Recorders = new Dictionary<string, ProfilerRecorder>();

        private static StringBuilder _StringBuilder = new StringBuilder(1024);
        private static readonly List<ProfilerItem> _Items = new List<ProfilerItem>();

        private static ProfilerRecorder GetRecorder(ProfilerCategory category, string name, int capacity) {
            ProfilerRecorder recorder;
            if (!_Recorders.TryGetValue(name, out recorder)) {
                recorder = ProfilerRecorder.StartNew(category, name, capacity);
                _Recorders[name] = recorder;
            }
            return recorder;
        }

        private static ProfilerRecorder GetRecorder(string name, ProfilerMarker marker) {
            ProfilerRecorder recorder;
            if (!_Recorders.TryGetValue(name, out recorder)) {
                recorder = ProfilerRecorder.StartNew(marker);
                _Recorders[name] = recorder;
            }
            return recorder;
        }

        private static void AddProfilerItem(ProfilerRecorder recorder, string prefix, bool useAvarage, ProfilerItemFormat Format) {
            _Items.Add(new ProfilerItem(recorder, prefix, useAvarage, Format));
        }

        public static void AddProfilerItem(ProfilerCategory category, string name, int capacity, bool useAvarage, ProfilerItemFormat Format) {
            if (_Recorders.ContainsKey(name)) { return; }
            var recorder = GetRecorder(category, name, capacity);
            AddProfilerItem(recorder, $"{name}: ", useAvarage, Format);
        }

        public static void AddProfilerItem(string name, ProfilerMarker marker, bool useAvarage, ProfilerItemFormat Format) {
            if (_Recorders.ContainsKey(name)) { return; }
            var recorder = GetRecorder(name, marker);
            AddProfilerItem(recorder, $"{name}: ", useAvarage, Format);
        }

        public static void AddCommonItems() {
            AddProfilerItem(ProfilerCategory.Internal, "Main Thread", DEFAULT_CAPACITY, true, ProfilerItemFormat.MilliSeconds);
            AddProfilerItem(ProfilerCategory.Memory, "System Used Memory", 1, false, ProfilerItemFormat.MegaBytes);
            AddProfilerItem(ProfilerCategory.Memory, "GC Reserved Memory", 1, false, ProfilerItemFormat.MegaBytes);
        }

        public static string CalcStats() {
            var sb = _StringBuilder;
            sb.Clear();
            foreach (var item in _Items) {
                item.Append(sb);
                sb.AppendLine();
            }
            return sb.ToString();
        }
    }
}