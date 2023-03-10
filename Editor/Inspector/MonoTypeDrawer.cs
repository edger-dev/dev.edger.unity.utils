using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEditor;

using Edger.Unity;

namespace Edger.Unity.Editor {
    [CustomPropertyDrawer(typeof(MonoTypeAttribute))]
    public class MonoTypeDrawer : ReadOnlyFieldDrawer {
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label) {
            return EditorGUI.GetPropertyHeight(property, label, true);
        }

        public override void OnGUI(Rect position,
                                    SerializedProperty property,
                                    GUIContent label) {
            var obj = property.serializedObject.targetObject;
            if (obj != null && property.stringValue != obj.GetType().FullName) {
                property.stringValue = obj.GetType().FullName;
                AssetDatabase.SaveAssets();
            }
            base.OnGUI(position, property, label);
        }
    }
}
