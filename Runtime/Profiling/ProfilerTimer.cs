using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Edger.Unity;
using Edger.Unity.Remote;

namespace Edger.Unity.Profiling {
    // Note: to keep this called before all other scripts, need to put ProfilerTimer
    // as the first one in "Project Settings -> Script Execution Order"
    // http://docs.unity3d.com/Documentation/Components/class-ScriptExecution.html
    public class ProfilerTimer : BaseMono {
        public static DateTime UpdateStartTime { get; private set; }
        public static DateTime FixedUpdateStartTime { get; private set; }
        public static DateTime LateUpdateStartTime { get; private set; }

        public static double UpdateSeconds { get; private set; }
        public static double FixedUpdateSeconds { get; private set; }
        public static double LateUpdateSeconds { get; private set; }

        public static bool CalcUpdateSeconds(double threshold) {
            UpdateSeconds = UpdateStartTime.GetPassedSeconds();
            return UpdateSeconds >= threshold;
        }

        public static bool CalcFixedUpdateSeconds(double threshold) {
            FixedUpdateSeconds = FixedUpdateStartTime.GetPassedSeconds();
            return FixedUpdateSeconds >= threshold;
        }

        public static bool CalcLateUpdateSeconds(double threshold) {
            LateUpdateSeconds = LateUpdateStartTime.GetPassedSeconds();
            return LateUpdateSeconds >= threshold;
        }

        public void Update() {
            UpdateStartTime = DateTimeUtil.GetStartTime();
        }

        public void FixedUpdate() {
            FixedUpdateStartTime = DateTimeUtil.GetStartTime();
        }

        public void LateUpdate() {
            LateUpdateStartTime = DateTimeUtil.GetStartTime();
        }
    }
}