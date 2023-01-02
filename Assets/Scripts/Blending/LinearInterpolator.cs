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
            float interpolationDesiredTime = 2f;

            // Loop until interpolation is done
            // TODO remove the division per 10 => find why the cycle seems to be done way before the end of the timer
            while (interpolationTimer < interpolationDesiredTime / 10)
            {
                interpolationTimer += Time.deltaTime;

                
                foreach (KeyValuePair<Transform, Dictionary<string, object>> boneProperties in bonesTargetProperties)
                {
                    // Assign var for readability
                    Transform targetBone = boneProperties.Key;

                    // Foreach bone, get each property to change 
                    // Then interpolate the difference between the wanted state and the current state
                    foreach (KeyValuePair<string, object> property in boneProperties.Value)
                    {
                        // Get the property in itself
                        System.Reflection.PropertyInfo bonePropertyInfo = typeof(Transform).GetProperty(property.Key);

                        object interpolatedProperty = property.Value;
                        object currentBonePropValue = bonePropertyInfo.GetValue(targetBone);

                        float timeRatio = interpolationTimer / interpolationDesiredTime;

                        // Determine the property to call the good lerp function
                        switch (bonePropertyInfo.PropertyType.ToString()) {
                            case "UnityEngine.Vector3":
                                interpolatedProperty = Vector3.Lerp((Vector3) currentBonePropValue, (Vector3) property.Value, timeRatio);
                            break;
                            case "UnityEngine.Quaternion":
                                interpolatedProperty = Quaternion.Lerp((Quaternion) currentBonePropValue, (Quaternion) property.Value, timeRatio);
                            break;
                        }

                        // This is how we apply the change
                        bonePropertyInfo.SetValue(targetBone, interpolatedProperty);
                    }
                }

                yield return new WaitForEndOfFrame();
            }
        }
    }
}
