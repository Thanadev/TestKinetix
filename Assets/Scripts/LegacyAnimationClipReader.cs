using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace TestKinetix
{
    public class LegacyAnimationClipReader
    {
        public static Dictionary<Transform, Dictionary<string, object>> DetermineTargetBoneProperties(AnimationClip clip, Transform animatedCharacter, float clipTimeSample = 0)
        {
            Dictionary<Transform, Dictionary<string, object>> bonesTargetProperties = new Dictionary<Transform, Dictionary<string, object>>();

            foreach (EditorCurveBinding binding in AnimationUtility.GetCurveBindings(clip))
            {
                AnimationCurve curve = AnimationUtility.GetEditorCurve(clip, binding);
                Transform targetBone = animatedCharacter.Find(binding.path);
                
                if (!bonesTargetProperties.ContainsKey(targetBone)) {
                    bonesTargetProperties.Add(targetBone, new Dictionary<string, object>());
                }

                // Reading property name (ex: localPosition) and the subproperty name (ex: x in localPosition.x)
                string subPropertyName = null;
                string mainPropertyName = GetCurvePropertyName(binding.propertyName, out subPropertyName);

                // Get the property from the transform, ex: "localRotation"
                System.Reflection.PropertyInfo targetBoneProp = typeof(Transform).GetProperty(mainPropertyName);

                if (targetBoneProp == null) {
                    Debug.LogWarning("Unrecognized property: " + mainPropertyName + " on Transform");
                    continue;
                }

                // If the property does not exist yet, initialize it with the current value for the bone
                if (!bonesTargetProperties[targetBone].ContainsKey(mainPropertyName)) {
                    bonesTargetProperties[targetBone].Add(mainPropertyName, targetBoneProp.GetValue(targetBone));
                }

                // TODO eventually: handle properties without subproperties

                // If the property has fields in it, like Vectors, we need to set them as they are separated curves in the keyframe
                if (subPropertyName != null) {
                    // Getting the subproperty for the wanted property, ex: localRotation.x
                    System.Reflection.FieldInfo targetBoneSubProp = targetBoneProp.PropertyType.GetField(subPropertyName);

                    // Edit the property in our dictionary for future usage in interpolation phase
                    targetBoneSubProp.SetValue(bonesTargetProperties[targetBone][mainPropertyName], (float) curve.Evaluate(clipTimeSample));
                }
            }

            return bonesTargetProperties;
        }

        public static Dictionary<Transform, Dictionary<string, object>> GetBonesPropertiesBackup(Dictionary<Transform, Dictionary<string, object>> bonesTargetProperties, Transform animatedCharacter)
        {
            Dictionary<Transform, Dictionary<string, object>> boneBackUpProperties = new Dictionary<Transform, Dictionary<string, object>>();

            foreach (KeyValuePair<Transform, Dictionary<string, object>> targetBoneProps in bonesTargetProperties)
            {
                // Assign var for readability
                Transform targetBone = targetBoneProps.Key;
                Dictionary<string, object> boneProperties = new Dictionary<string, object>();

                foreach (KeyValuePair<string, object> property in targetBoneProps.Value)
                {
                    string propertyName = property.Key;

                    System.Reflection.PropertyInfo bonePropertyInfo = typeof(Transform).GetProperty(propertyName);

                    boneProperties.Add(propertyName, bonePropertyInfo.GetValue(targetBone));
                }

                boneBackUpProperties.Add(targetBone, boneProperties);
            }

            return boneBackUpProperties;
        }

        

        private static string GetCurvePropertyName(string bindingPropertyName, out string subPropertyName)
        {
            string mainPropertyName = bindingPropertyName;
            subPropertyName = null;

            // Just checking for public variant of private properties
            if (mainPropertyName.Contains("_")) {
                mainPropertyName = mainPropertyName.Replace("m_", "");
                mainPropertyName = Char.ToLowerInvariant(mainPropertyName[0]) + mainPropertyName.Substring(1);
            }

            // Ahaaaa found a subproperty?
            if (mainPropertyName.Contains(".")) {
                string[] splitProp = mainPropertyName.Split('.');

                mainPropertyName = splitProp[0];
                subPropertyName = splitProp[1];
            }

            return mainPropertyName;
        }
    }
}
