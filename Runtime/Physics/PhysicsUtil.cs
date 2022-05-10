using System;
using System.IO;
using System.Security.Cryptography;

using UnityEngine;

using Edger.Unity;

namespace Edger.Unity {
    public static class PhysicsUtil {
        private static RaycastHit[] _RaycastHits = new RaycastHit[1024];

        public static T RaycastClosest<T>(Camera cam, Vector2 screenPos, Func<RaycastHit, T> calc,
                    float maxDistance = Mathf.Infinity,
                    int layerMask = Physics.DefaultRaycastLayers,
                    QueryTriggerInteraction queryTriggerInteraction = QueryTriggerInteraction.UseGlobal
                ) where T : class {
            var ray = cam.ScreenPointToRay(screenPos);
            var count = Physics.RaycastNonAlloc(ray, _RaycastHits, maxDistance, layerMask, queryTriggerInteraction);
            T result = null;
            if (count > 0) {
                var closestDistance = float.PositiveInfinity;

                for (var i = 0; i < count; i++) {
                    var hit = _RaycastHits[i];
                    var distance = hit.distance;

                    if (distance < closestDistance) {
                        closestDistance = distance;
                        var v = calc(hit);
                        if (v != null) {
                            result = v;
                        }
                    }
                }
            }
            return result;
        }

        public static T RaycastClosest<T>(Camera cam, Vector2 screenPos,
                    float maxDistance = Mathf.Infinity,
                    int layerMask = Physics.DefaultRaycastLayers,
                    QueryTriggerInteraction queryTriggerInteraction = QueryTriggerInteraction.UseGlobal
                ) where T : MonoBehaviour {
            return RaycastClosest<T>(cam, screenPos, (hit) => {
                return hit.collider.GetComponent<T>();
            }, maxDistance, layerMask, queryTriggerInteraction);
        }

        public static bool Raycast(Camera cam, Vector2 screenPos, out RaycastHit hit,
                    float maxDistance = Mathf.Infinity,
                    int layerMask = Physics.DefaultRaycastLayers,
                    QueryTriggerInteraction queryTriggerInteraction = QueryTriggerInteraction.UseGlobal
                ) {
            var ray = cam.ScreenPointToRay(screenPos);
            return Physics.Raycast(ray, out hit, maxDistance, layerMask, queryTriggerInteraction);
        }

        public static T Raycast<T>(Camera cam, Vector2 screenPos,
                    float maxDistance = Mathf.Infinity,
                    int layerMask = Physics.DefaultRaycastLayers,
                    QueryTriggerInteraction queryTriggerInteraction = QueryTriggerInteraction.UseGlobal
                ) where T : MonoBehaviour {
            RaycastHit hit;
            if (Raycast(cam, screenPos, out hit, maxDistance, layerMask, queryTriggerInteraction)) {
                return hit.collider.GetComponent<T>();
            }
            return null;
        }
    }
}
