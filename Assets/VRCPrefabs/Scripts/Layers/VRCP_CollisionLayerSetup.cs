// Created by CyanLaser
// 2018-07-21
//
// A script to export and import a unity project's custom layers for a given VRChat prefab.
// Adds a window option called "VRC Prefabs" with two options under it: "Setup Layers" and 
// "Export Layers". On first startup or import with the new assets, this script will 
// automatically show the Setup Layers window to prompt the user this prefab uses custom layers.

// TODO @CyanLaser: Make this more generic so that layers are added rather than replaced.

#if UNITY_EDITOR

using UnityEngine;
using UnityEditor;

namespace VRCP.Layers
{
    [InitializeOnLoad]
    public class VRCP_CollisionLayerSetup : EditorWindow
    {
        public static readonly string exportPath = "Assets/VRCPrefabs/Data/Layers/";
        public static readonly int version = 0;
        private static Vector2 layerViewScroll;
        VRCP_PhysicsLayers physicsLayers;
        VRCP_CollisionLayerSettings settings;

        static VRCP_CollisionLayerSetup()
        {
            EditorApplication.update -= DoLayerSplashScreen;
            EditorApplication.update += DoLayerSplashScreen;
        }

        public static string GetAssetPath(string fileName)
        {
            return string.Format("{0}{1}", exportPath, fileName);
        }

        private static bool ShouldShowSplashScreen()
        {
            VRCP_CollisionLayerSettings settings = VRCP_CollisionLayerSettings.LoadLayerSettings();
            if (settings == null)
            {
                settings = VRCP_CollisionLayerSettings.GetAndSetShowSetupLayers(true);
            }

            return settings.showSetupLayers;
        }

        private static void DoLayerSplashScreen()
        {
            EditorApplication.update -= DoLayerSplashScreen;
            
            if (ShouldShowSplashScreen())
            {
                InitLayerSetup();
            }
        }

        [MenuItem("VRC Prefabs/Layers/Setup Layers", false, 1)]
        static void InitLayerSetup()
        {
            GetWindow<VRCP_CollisionLayerSetup>(true);
        }

        [MenuItem("VRC Prefabs/Layers/Export Layers", false, 2)]
        static void ExportLayers()
        {
            bool continueSetup = EditorUtility.DisplayDialog(
                "Export Current Layers?",
                "This will overwrite the current exported layers with the layers in the current project. Are you sure you want to continue?",
                "Continue",
                "Cancel"
            );
            if (continueSetup)
            {
                VRCP_PhysicsLayers.SavePhysicsLayers();
            }
        }

        public void OnEnable()
        {
            settings = VRCP_CollisionLayerSettings.LoadLayerSettings();
            if (settings == null)
            {
                settings = VRCP_CollisionLayerSettings.GetAndSetShowSetupLayers(true);
            }

            titleContent = new GUIContent("VRC Prefabs Layer Setup V"+version);

            maxSize = new Vector2(350, 200);
            minSize = maxSize;

            physicsLayers = VRCP_PhysicsLayers.LoadPhysicsLayers();
        }

        void ShowMissingLayersFile()
        {
            GUILayout.Label("There was an issue in reading the layer file. Please reimport the prefab package or message the creator.\nFile path: " + GetAssetPath(VRCP_PhysicsLayers.fileName));

            GUILayout.FlexibleSpace();

            GUILayout.BeginHorizontal();
            if (GUILayout.Button("Try again"))
            {
                physicsLayers = VRCP_PhysicsLayers.LoadPhysicsLayers();
            }
            if (GUILayout.Button("Ignore"))
            {
                VRCP_CollisionLayerSettings.GetAndSetShowSetupLayers(false);
                Close();
            }

            GUILayout.EndHorizontal();
        }

        void ShowSetupLayer()
        {
            GUILayout.Label("The following layers will be added to your project:");

            layerViewScroll = GUILayout.BeginScrollView(layerViewScroll);

            SerializedObject serializedObject = new SerializedObject(AssetDatabase.LoadAllAssetsAtPath("ProjectSettings/TagManager.asset")[0]);
            SerializedProperty layerProperty = serializedObject.FindProperty("layers");

            for (int curLayer = 0; curLayer < physicsLayers.numLayers; ++curLayer)
            {
                bool isPreviousLayer = layerProperty.GetArrayElementAtIndex(physicsLayers.layerInd[curLayer]).stringValue.Trim().Length != 0;

                GUILayout.Label(string.Format("{0}: {1}{2}", physicsLayers.layerInd[curLayer], physicsLayers.layerNames[curLayer], isPreviousLayer ? "*" : ""));
            }
            GUILayout.EndScrollView();

            GUILayout.Space(4);

            GUILayout.Label("* new layer will overwrite current layer");

            GUILayout.Space(4);
            GUILayout.BeginHorizontal();

            if (GUILayout.Button("Import Layers"))
            {
                bool continueSetup = EditorUtility.DisplayDialog(
                    "Import Layers for prefab?",
                    "This will replace any custom layers you have setup for the current project with the layers needed for this prefab. Are you sure you want to continue?",
                    "Continue",
                    "Cancel"
                );
                if (continueSetup)
                {
                    physicsLayers.SetupLayers();
                    VRCP_CollisionLayerSettings.GetAndSetShowSetupLayers(false);
                    Close();
                }
            }
            if (GUILayout.Button("Ignore"))
            {
                VRCP_CollisionLayerSettings.GetAndSetShowSetupLayers(false);
                Close();
            }

            GUILayout.EndHorizontal();
        }

        void OnGUI()
        {
            GUI.skin.label.wordWrap = true;

            GUILayout.Label("This prefab package requires custom layers!", EditorStyles.boldLabel);

            GUILayout.Space(4);

            if (physicsLayers == null)
            {
                ShowMissingLayersFile();
                return;
            }

            ShowSetupLayer();
        }
    }
}

#endif
