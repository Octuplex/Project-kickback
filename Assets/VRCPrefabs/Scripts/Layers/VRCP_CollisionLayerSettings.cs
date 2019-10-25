// Created by CyanLaser
// 2018-07-21
//
// An object to save that this project has seen the setup layers window.

#if UNITY_EDITOR

using UnityEngine;
using UnityEditor;

namespace VRCP.Layers
{
    
    public class VRCP_CollisionLayerSettings : ScriptableObject
    {
        public static readonly string fileName = "CollisionLayerSettings.asset";
        public bool showSetupLayers;

        public static VRCP_CollisionLayerSettings LoadLayerSettings()
        {
            return (VRCP_CollisionLayerSettings)AssetDatabase.LoadAssetAtPath(VRCP_CollisionLayerSetup.GetAssetPath(fileName), typeof(VRCP_CollisionLayerSettings));
        }

        public static VRCP_CollisionLayerSettings GetAndSetShowSetupLayers(bool show)
        {
            VRCP_CollisionLayerSettings settings = LoadLayerSettings();
            bool shouldSave = false;
            if (settings == null)
            {
                settings = ScriptableObject.CreateInstance<VRCP_CollisionLayerSettings>();
                shouldSave = true;
            }

            settings.showSetupLayers = show;
            EditorUtility.SetDirty(settings);
            if (shouldSave)
            {
                AssetDatabase.CreateAsset(settings, VRCP_CollisionLayerSetup.GetAssetPath(fileName));
            }
            
            AssetDatabase.SaveAssets();

            return settings;
        }
    }
}

#endif
