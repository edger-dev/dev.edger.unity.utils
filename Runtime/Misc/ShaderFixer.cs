using System;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UI;
using TMPro;

using Edger.Unity;

namespace Edger.Unity {
    public interface IShaderFixer {
        int FixShaders(Component target);
    }

    public interface IShaderFixer<T> : IShaderFixer where T : Component {
        Type TargetType { get; }
    }

    public abstract class ShaderFixer {
        public static List<IShaderFixer> CreateFixers() {
            return AssemblyUtil.CreateInstances<IShaderFixer>();
        }

        public static int FixGameObject(GameObject go, bool includeInactive, List<IShaderFixer> fixers = null) {
            if (fixers == null) fixers = CreateFixers();

            int count = 0;
            foreach (var fixer in fixers) {
                Type targetType = AssemblyUtil.GetTargetType(typeof(IShaderFixer<>), fixer.GetType());
                if (targetType == null) continue;
                var targets = go.GetComponentsInChildren(targetType);
                if (targets.Length > 0) {
                    Log.InfoFrom(go, "FixGameObject() {0} {1} <{2}> - [{3}]", go.name, fixer.GetType().Name,targetType.Name, targets.Length);
                }
                foreach (var target in targets) {
                    count += fixer.FixShaders(target);
                }
            }
            return count;
            /* Note sure whether needed 
            var particles = go.GetComponentsInChildren<ParticleSystem>(true);
            foreach (var obj in particles) {
                var renderer = obj.GetComponent<Renderer>();
                if (renderer != null) FixShaderMatereials(obj, renderer.sharedMaterials);
            }
             */
        }

        protected static bool FixMatereial(Component target, Material material, int index = 0) {
            if (target == null || material == null) return false;

            var shaderName = material.shader.name;
            var shader = Shader.Find(shaderName);

            if (shader != null) {
                material.shader = shader;
                //Log.InfoFrom(target, "FixMatereial() done: {0} - {1}", index, shaderName);
                return true;
            } else {
                Log.ErrorFrom(target, "FixMatereial() failed: {0} - {1}", index, shaderName);
                return false;
            }
        }
    }

    public abstract class ShaderFixer<T> : ShaderFixer, IShaderFixer<T> where T : Component {
        public Type TargetType { get { return typeof(T); } }

        protected abstract Material[] GetMaterials(T target);

        public int FixShaders(Component _target) {
            T target = _target.As<T>();
            if (target == null) return 0;
            int count = 0;
            var materials = GetMaterials(target);
            Log.InfoFrom(target, "FixShaders(): {0} -> [{1}]", TransformUtil.GetPath(target.transform), materials.Length);
            for (int i = 0; i < materials.Length; i++) {
                if (materials != null) {
                    var ok = FixMatereial(target, materials[i], i);
                    if (ok) count++;
                }
            }
            return count;
        }
    }

    public class RendererShaderFixer : ShaderFixer<Renderer> {
        protected override Material[] GetMaterials(Renderer target) {
            return target.materials;
        }
    }

    public class RendererSharedShaderFixer : ShaderFixer<Renderer> {
        protected override Material[] GetMaterials(Renderer target) {
            return target.sharedMaterials;
        }
    }

    public class CanvasRendererShaderFixer : ShaderFixer<CanvasRenderer> {
        protected override Material[] GetMaterials(CanvasRenderer target) {
            var result = new List<Material>();
            for (var i = 0; i < target.materialCount; i++) {
                var m = target.GetMaterial(i);
                if (m != null) {
                    result.Add(m);
                }
            }
            return result.ToArray();
        }
    }


    public class TmpShaderFixer : ShaderFixer<TextMeshProUGUI> {
        protected override Material[] GetMaterials(TextMeshProUGUI target) {
            var result = new List<Material>();
            result.Add(target.material);
            result.Add(target.materialForRendering);
            return result.ToArray();
        }
    }

    public class ImageShaderFixer : ShaderFixer<Image> {
        protected override Material[] GetMaterials(Image target) {
            var result = new List<Material>();
            result.Add(target.material);
            return result.ToArray();
        }
    }
}
