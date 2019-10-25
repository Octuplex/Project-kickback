// Created by CyanLaser
// 2018-07-21
//
// ScriptableObject to save non VRC project layers.

#if UNITY_EDITOR

using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace VRCP.Layers
{
    [System.Serializable]
    public class VRCP_BoolArrayContainer
    {
        public bool[] bools;
        public VRCP_BoolArrayContainer(int size)
        {
            bools = new bool[size];
        }
    }

    public class VRCP_PhysicsLayers : ScriptableObject
    {
        public int numLayers;
        public int[] layerInd;
        public string[] layerNames;
        public List<VRCP_BoolArrayContainer> layerCollisions;
        
        public static readonly string fileName = "PhysicsLayers.asset";

        public void SetupLayers()
        {
            Debug.Log("Importing prefab layers");

            SerializedObject serializedObject = new SerializedObject(AssetDatabase.LoadAllAssetsAtPath("ProjectSettings/TagManager.asset")[0]);
            SerializedProperty property = serializedObject.FindProperty("layers");
            
            for (int layer = 0; layer < numLayers; ++layer)
            {
                if (property.GetArrayElementAtIndex(layerInd[layer]).stringValue.Trim().Length != 0)
                {
                    Debug.LogWarning("Layer " + layerInd[layer] + " " + property.GetArrayElementAtIndex(layerInd[layer]).stringValue + " was overwritten by " + layerNames[layer]);
                }

                property.GetArrayElementAtIndex(layerInd[layer]).stringValue = layerNames[layer];
            }
            serializedObject.ApplyModifiedProperties();

            for (int layer = 0; layer < numLayers; ++layer)
            {
                for (int otherLayer = 0; otherLayer < 32; ++otherLayer)
                {
                    Physics.IgnoreLayerCollision(layerInd[layer], otherLayer, !layerCollisions[layer].bools[otherLayer]);
                }
            }

            Debug.Log("The following layers were successfully imported:");
            for (int layer = 0; layer < layerNames.Length; ++layer)
            {
                Debug.Log(string.Format("{0} {1}", layerInd[layer], layerNames[layer]));
            }
        }

        public static void SavePhysicsLayers()
        {
            VRCP_PhysicsLayers layers = ScriptableObject.CreateInstance<VRCP_PhysicsLayers>();

            SerializedObject serializedObject = new SerializedObject(AssetDatabase.LoadAllAssetsAtPath("ProjectSettings/TagManager.asset")[0]);
            SerializedProperty property = serializedObject.FindProperty("layers");
            List<string> layerNames = new List<string>();
            List<int> layerInd = new List<int>();

            // Unity and VRChat uses layers 0 - 21, custom layers start at 22.
            for (int layer = 22; layer < 32; ++layer)
            {
                SerializedProperty layerProperty = property.GetArrayElementAtIndex(layer);
                if (layerProperty != null && layerProperty.stringValue.Trim().Length != 0)
                {
                    layerInd.Add(layer);
                    layerNames.Add(layerProperty.stringValue);
                }
            }

            if (layerInd.Count == 0)
            {
                Debug.LogWarning("No custom layers were found.");
                return;
            }

            layers.numLayers = layerInd.Count;
            layers.layerInd = layerInd.ToArray();
            layers.layerNames = layerNames.ToArray();
            layers.layerCollisions = new List<VRCP_BoolArrayContainer>();

            for (int customLayerInd = 0; customLayerInd < layerInd.Count; ++customLayerInd)
            {
                int customLayer = layerInd[customLayerInd];
                layers.layerCollisions.Add(new VRCP_BoolArrayContainer(32));

                for (int layer = 0; layer < 32; ++layer)
                {
                    layers.layerCollisions[customLayerInd].bools[layer] = !Physics.GetIgnoreLayerCollision(customLayer, layer);
                }
            }

            Debug.Log("The following layers were successfully exported:");
            for (int layer = 0; layer < layerInd.Count; ++layer)
            {
                Debug.Log(string.Format("{0} {1}", layerInd[layer], layerNames[layer]));
            }

            AssetDatabase.CreateAsset(layers, VRCP_CollisionLayerSetup.GetAssetPath(fileName));
            AssetDatabase.SaveAssets();
        }

        public static VRCP_PhysicsLayers LoadPhysicsLayers()
        {
            return (VRCP_PhysicsLayers)AssetDatabase.LoadAssetAtPath(VRCP_CollisionLayerSetup.GetAssetPath(fileName), typeof(VRCP_PhysicsLayers));
        }
    }
}

#endif