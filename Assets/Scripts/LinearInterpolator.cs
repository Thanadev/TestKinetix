using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TestKinetix
{
    public class LinearInterpolator
    {
        public static IEnumerator BlendTo(Dictionary<Transform, Dictionary<string, object>> bonesTargetProperties)
        {
            float interpolationTimer = 0;
            float interpolationDesiredTime = 0.5f;

            while (interpolationTimer < interpolationDesiredTime)
            {
                interpolationTimer += Time.deltaTime;

                foreach (KeyValuePair<Transform, Dictionary<string, object>> boneProperties in bonesTargetProperties)
                {
                    // Assign var for readability
                    Transform targetBone = boneProperties.Key;

                    foreach (KeyValuePair<string, object> property in boneProperties.Value)
                    {
                        System.Reflection.PropertyInfo bonePropertyInfo = typeof(Transform).GetProperty(property.Key);

                        object interpolatedProperty = property.Value;
                        object currentBonePropValue = bonePropertyInfo.GetValue(targetBone);

                        float timeRatio = interpolationTimer / interpolationDesiredTime;


                        switch (bonePropertyInfo.PropertyType.ToString()) {
                            case "UnityEngine.Vector3":
                                interpolatedProperty = Vector3.Slerp((Vector3) currentBonePropValue, (Vector3) property.Value, timeRatio);
                            break;
                            case "UnityEngine.Quaternion":
                                interpolatedProperty = Quaternion.Slerp((Quaternion) currentBonePropValue, (Quaternion) property.Value, timeRatio);
                            break;
                        }

                        bonePropertyInfo.SetValue(targetBone, interpolatedProperty);
                    }
                }

                yield return new WaitForEndOfFrame();
            }
        }
    }
}
