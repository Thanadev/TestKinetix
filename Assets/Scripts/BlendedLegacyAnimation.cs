using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace TestKinetix
{
    public class BlendedLegacyAnimation
    {
        private AnimationClip clip;
        private AnimationContext context;

        private Dictionary<Transform, Dictionary<string, object>> bonesTargetProperties;
        private Dictionary<Transform, Dictionary<string, object>> bonesOriginalProperties;

        public BlendedLegacyAnimation(AnimationClip clip, AnimationContext context)
        {
            this.clip = clip;
            this.context = context;

            bonesTargetProperties = new Dictionary<Transform, Dictionary<string, object>>();
            bonesOriginalProperties = new Dictionary<Transform, Dictionary<string, object>>();

            DetermineTargetBoneProperties();
        }

        public IEnumerator Play()
        {
            context.Animator.enabled = false;

            float animTimer = 0f;
           
            
            yield return LinearInterpolator.BlendTo(bonesTargetProperties);


            while (animTimer < clip.length)
            {
                animTimer += Time.deltaTime;

                clip.SampleAnimation(context.AnimatedCharacter.gameObject, animTimer);

                yield return new WaitForEndOfFrame();                
            }

            
            yield return LinearInterpolator.BlendTo(bonesOriginalProperties);

            context.Animator.enabled = true;
        }

        private void DetermineTargetBoneProperties()
        {

            foreach (var binding in AnimationUtility.GetCurveBindings(clip))
            {
                AnimationCurve curve = AnimationUtility.GetEditorCurve(clip, binding);
                Transform targetBone = context.AnimatedCharacter.Find(binding.path);

                if (!bonesTargetProperties.ContainsKey(targetBone)) {
                    bonesTargetProperties.Add(targetBone, new Dictionary<string, object>());
                    bonesOriginalProperties.Add(targetBone, new Dictionary<string, object>());
                }



                string mainPropertyName = binding.propertyName;
                string subPropertyName = null;

                if (mainPropertyName.Contains("_")) {
                    mainPropertyName = mainPropertyName.Replace("m_", "");
                    mainPropertyName = Char.ToLowerInvariant(mainPropertyName[0]) + mainPropertyName.Substring(1);
                }

                if (mainPropertyName.Contains(".")) {
                    string[] splitProp = mainPropertyName.Split('.');

                    mainPropertyName = splitProp[0];
                    subPropertyName = splitProp[1];
                }



                System.Reflection.PropertyInfo targetBoneProp = typeof(Transform).GetProperty(mainPropertyName);

                // If the property does not exist yet, initialize it with the current value for the bone
                if (!bonesTargetProperties[targetBone].ContainsKey(mainPropertyName)) {
                    bonesTargetProperties[targetBone].Add(mainPropertyName, targetBoneProp.GetValue(targetBone));
                    bonesOriginalProperties[targetBone].Add(mainPropertyName, targetBoneProp.GetValue(targetBone));
                }

                if (subPropertyName != null) {
                    System.Reflection.FieldInfo targetBoneSubProp = targetBoneProp.PropertyType.GetField(subPropertyName);
                    object cProp = bonesTargetProperties[targetBone][mainPropertyName];

                    targetBoneSubProp.SetValue(cProp, (float) curve.Evaluate(0));
                    
                    bonesTargetProperties[targetBone][mainPropertyName] = cProp;
                }
            }
        }
    }
}
