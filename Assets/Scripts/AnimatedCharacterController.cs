using UnityEngine;
using UnityEditor;
using System;
using System.Collections;
using System.Collections.Generic;

namespace TestKinetix {

    [RequireComponent(typeof(CharacterController))]
    public class AnimatedCharacterController : MonoBehaviour
    {
        [Header("Emotes")]
        [SerializeField] private Animation animation;
        [SerializeField] private Animator animator;
        [SerializeField] private AnimationClip legAnim;

        
        [Header("Movement")]
        [SerializeField] private Transform cameraOrbiter; // Used to get the camera forward and move relative to that direction
        [SerializeField] private float walkSpeed = 1;
        [Tooltip("Turn speed for the character. The less speed the longer the character will take to turn")]
        [SerializeField] private float faceDirectionRatio = 0.01f;
        private Vector3 movementDirection;

        private CharacterController characterController;

        private void Awake()
        {
            characterController = GetComponent<CharacterController>();
        }

        private void Update()
        {
            Move();

            if (Input.GetKeyDown(KeyCode.R)) {
                animator.ResetTrigger("CancelEmotes");

                PlayHumanoidEmote();
            } else if (Input.GetKeyDown(KeyCode.T)) {
                CancelEmotes();

                StartCoroutine(PlayLegacyAnim());
            }
        }

        private void Move()
        {
            movementDirection = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical")).normalized;

            animator.SetFloat("MoveMagnitude", movementDirection.magnitude);

            if (movementDirection.magnitude > 0) {
                movementDirection = cameraOrbiter.TransformDirection(movementDirection) * -1;
                
                Quaternion wantedRotation = Quaternion.LookRotation(movementDirection, transform.up);
                transform.rotation = Quaternion.Slerp(transform.rotation, wantedRotation, faceDirectionRatio);

                CancelEmotes();

                characterController.Move(movementDirection * walkSpeed * Time.deltaTime);   
            }            
        }



        #region Emotes

        private void PlayHumanoidEmote()
        {
            animator.SetTrigger("PlayHumanoidEmote");
        }

        private void CancelEmotes()
        {
            animator.SetTrigger("CancelEmotes");
            
            animator.enabled = true;
            StopCoroutine(PlayLegacyAnim());
        }

        private IEnumerator PlayLegacyAnim()
        {
            animator.enabled = false;

            float animTimer = 0f;
           

            Dictionary<Transform, Dictionary<string, object>> bonesTargetProperties = new Dictionary<Transform, Dictionary<string, object>>();
            Dictionary<Transform, Dictionary<string, object>> bonesOriginalProperties = new Dictionary<Transform, Dictionary<string, object>>();


            foreach (var binding in AnimationUtility.GetCurveBindings(legAnim))
            {
                AnimationCurve curve = AnimationUtility.GetEditorCurve(legAnim, binding);
                Transform targetBone = transform.Find(binding.path);

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



                System.Reflection.PropertyInfo? targetBoneProp = typeof(Transform).GetProperty(mainPropertyName);

                // If the property does not exist yet, initialize it with the current value for the bone
                if (!bonesTargetProperties[targetBone].ContainsKey(mainPropertyName)) {
                    bonesTargetProperties[targetBone].Add(mainPropertyName, targetBoneProp.GetValue(targetBone));
                    bonesOriginalProperties[targetBone].Add(mainPropertyName, targetBoneProp.GetValue(targetBone));
                }

                if (subPropertyName != null) {
                    System.Reflection.FieldInfo? targetBoneSubProp = targetBoneProp.PropertyType.GetField(subPropertyName);
                    object cProp = bonesTargetProperties[targetBone][mainPropertyName];

                    targetBoneSubProp.SetValue(cProp, (float) curve.Evaluate(0));
                    
                    bonesTargetProperties[targetBone][mainPropertyName] = cProp;
                }                
            }


            yield return BlendTo(bonesTargetProperties);


            while (animTimer < legAnim.length)
            {
                animTimer += Time.deltaTime;

                legAnim.SampleAnimation(gameObject, animTimer);

                yield return new WaitForEndOfFrame();                
            }

            
            yield return BlendTo(bonesOriginalProperties);

            animator.enabled = true;
        }

        private IEnumerator BlendTo(Dictionary<Transform, Dictionary<string, object>> bonesTargetProperties)
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
                        System.Reflection.PropertyInfo? bonePropertyInfo = typeof(Transform).GetProperty(property.Key);

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

        #endregion
    }


    
}
