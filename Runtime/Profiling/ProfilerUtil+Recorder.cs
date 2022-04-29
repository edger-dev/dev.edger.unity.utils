using System;
using System.Text;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.Profiling;
using Unity.Profiling;

namespace Edger.Unity.Profiling {
    public enum ProfilerItemFormat {
        Counter,
        MegaBytes,
        Seconds,
        MilliSeconds,
    }

    public class ProfilerItem {
        public readonly ProfilerRecorder Recorder;
        public readonly string Prefix;
        public readonly ProfilerItemFormat Format;

        private List<ProfilerRecorderSample> _Samples = null;

        public ProfilerItem(ProfilerRecorder recorder, string prefix, ProfilerItemFormat format) {
            Recorder = recorder;
            Prefix = prefix;
            Format = format;
            _Samples = new List<ProfilerRecorderSample>(recorder.Capacity);
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

        public static void AppendLine(bool calcAverage, StringBuilder builder, string prefix, ProfilerItemFormat format, double value) {
            builder.Append(prefix);
            builder.Append(": ");
            switch (format) {
                case ProfilerItemFormat.Counter:
                    if (calcAverage) {
                        builder.Append((int)value); 
                    } else {
                        builder.Append(value);
                    }
                    break;
                case ProfilerItemFormat.MegaBytes:
                    double mb = value / 1024 / 1024;
                    if (calcAverage) {
                        builder.Append((int) mb);
                    } else {
                        builder.Append(mb);
                    }
                    builder.Append(" MB");
                    break;
                case ProfilerItemFormat.Seconds:
                    if (calcAverage) {
                        builder.Append((int)value); 
                    } else {
                        builder.Append(value);
                    }
                    builder.Append(" s");
                    break;
                case ProfilerItemFormat.MilliSeconds:
                    double ms = value * (1e-6f);
                    if (calcAverage) {
                        builder.AppendFormat("{0:F2}", ms);
                    } else {
                        builder.Append(ms);
                    }
                    builder.Append(" ms");
                    break;
            }
            builder.AppendLine();
        }
        public void AppendLine(StringBuilder builder, bool calcAverage) {
            if (calcAverage) {
                AppendLine(calcAverage, builder, Prefix, Format, CalcAverageValue());
            } else {
                AppendLine(calcAverage, builder, Prefix, Format, Recorder.LastValueAsDouble);
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

        private static ProfilerRecorder GetRecorder(string name, ProfilerMarker marker, int capacity) {
            ProfilerRecorder recorder;
            if (!_Recorders.TryGetValue(name, out recorder)) {
                recorder = ProfilerRecorder.StartNew(marker, capacity);
                _Recorders[name] = recorder;
            }
            return recorder;
        }

        private static void AddProfilerItem(ProfilerRecorder recorder, string prefix, ProfilerItemFormat format) {
            _Items.Add(new ProfilerItem(recorder, prefix, format));
        }

        public static void AddProfilerItem(ProfilerCategory category, string name, ProfilerItemFormat format, int capacity = DEFAULT_CAPACITY) {
            if (_Recorders.ContainsKey(name)) { return; }
            var recorder = GetRecorder(category, name, capacity);
            AddProfilerItem(recorder, name, format);
        }

        public static void AddProfilerItem(string name, ProfilerMarker marker, ProfilerItemFormat format, ProfilerCategory category, int capacity = DEFAULT_CAPACITY) {
            if (_Recorders.ContainsKey(name)) { return; }
            var recorder = GetRecorder(name, marker, capacity);
            AddProfilerItem(recorder, name, format);
        }

        public static void AddProfilerItem(string name, ProfilerMarker marker, ProfilerItemFormat format, int capacity = DEFAULT_CAPACITY) {
            AddProfilerItem(name, marker, format, ProfilerCategory.Scripts, capacity);
        }

        public static void AddCommonItems(int capacity = DEFAULT_CAPACITY) {
            AddProfilerItem(ProfilerCategory.Internal, "Main Thread", ProfilerItemFormat.MilliSeconds, capacity);
            AddProfilerItem(ProfilerCategory.Internal, "Wait For Target FPS", ProfilerItemFormat.MilliSeconds, capacity);
            AddProfilerItem(ProfilerCategory.Memory, "System Used Memory", ProfilerItemFormat.MegaBytes, capacity);
            AddProfilerItem(ProfilerCategory.Memory, "GC Reserved Memory", ProfilerItemFormat.MegaBytes, capacity);
            AddProfilerItem(ProfilerCategory.Render, "SetPass Calls Count", ProfilerItemFormat.Counter, capacity);
            AddProfilerItem(ProfilerCategory.Render, "Draw Calls Count", ProfilerItemFormat.Counter, capacity);
            AddProfilerItem(ProfilerCategory.Render, "Vertices Count", ProfilerItemFormat.Counter, capacity);
        }

        public static string CalcStats(bool calcAvarage) {
            var builder = _StringBuilder;
            builder.Clear();
            foreach (var item in _Items) {
                item.AppendLine(builder, calcAvarage);
            }
            ProfilerItem.AppendLine(calcAvarage, builder, "Time", ProfilerItemFormat.Seconds, Time.realtimeSinceStartupAsDouble);
            return builder.ToString();
        }
    }
}