using System.IO;
using Rotorz.ReorderableList;
using UnityEditor;
using UnityEngine;

namespace XcodeProject.Editor
{
    [CustomEditor(typeof(XcodeProjectData))]
    internal class XcodeProjectEditor : UnityEditor.Editor
    {

        [MenuItem("Assets/XcodeProject")]
        public static void Menu()
        {
            var folder = Path.GetDirectoryName(XcodeProjectData.Location);
            if (folder != null) Directory.CreateDirectory(folder);

            var config = AssetDatabase.LoadAssetAtPath(XcodeProjectData.Location, typeof(XcodeProjectData));

            if (config == null)
            {
                config = CreateInstance<XcodeProjectData>();

                // サンプル設定を作成
                ((XcodeProjectData)config).CreateSampleData();

                AssetDatabase.CreateAsset(config, XcodeProjectData.Location);
                AssetDatabase.Refresh();
                EditorUtility.SetDirty(config);
            }

            Selection.activeObject = config;
        }


        public override void OnInspectorGUI()
        {
            var config = (XcodeProjectData)target;

            EditorGUI.BeginChangeCheck();


            ReorderableListGUI.Title("AddFramework");
            ReorderableListGUI.ListField(config.Framework, DrawFramework, 38);

            ReorderableListGUI.Title("AddLibrary");
            ReorderableListGUI.ListField(config.Library, DrawFramework, 38);

            ReorderableListGUI.Title("BuildProperty");
            ReorderableListGUI.ListField(config.BuildProperty, DrawBuildProperty, 38);

            ReorderableListGUI.Title("CompileFlags");
            ReorderableListGUI.ListField(config.CompileFlags, DrawCompileFlags, 38);

            ReorderableListGUI.Title("Info.plist");
            ReorderableListGUI.ListField(config.InfoPlistPropertyDatas, DrawBuildProperty, 38);

            config.UsePushNotification = GUILayout.Toggle(config.UsePushNotification, "UsePushNotification");

            if (EditorGUI.EndChangeCheck())
            {
                EditorUtility.SetDirty(config);
            }
        }

        private XcodeProjectData.PropertyData DrawCompileFlags(Rect position, XcodeProjectData.PropertyData entry)
        {
            const float leftWidth = 35;
            const float rowHeight = 18;

            var leftUpper = position;
            leftUpper.xMin += 4;
            leftUpper.xMax = leftUpper.xMin + leftWidth;
            leftUpper.yMax = leftUpper.yMin + rowHeight;

            var leftLower = position;
            leftLower.xMin += 4;
            leftLower.xMax = leftUpper.xMin + leftWidth;
            leftLower.yMin = leftLower.yMax - rowHeight;

            var rightUpper = position;
            rightUpper.xMin = leftUpper.xMax + 2;
            rightUpper.yMax = rightUpper.yMin + rowHeight;

            var rightLower = position;
            rightLower.yMin = rightLower.yMax - rowHeight;
            rightLower.xMin = leftLower.xMax + 2;

            if (entry == null)
            {
                entry = new XcodeProjectData.PropertyData { Key = "", Value = "" };
            }
            GUI.Label(leftUpper, "File");
            GUI.Label(leftLower, "Flags");

            if (entry.Key == null)
            {
                entry.Key = string.Empty;
            }

            entry.Key = EditorGUI.TextField(rightUpper, entry.Key);

            if (entry.Value == null)
            {
                entry.Value = string.Empty;
            }

            entry.Value = EditorGUI.TextField(rightLower, entry.Value);

            return entry;
        }

        private XcodeProjectData.PropertyData DrawBuildProperty(Rect position, XcodeProjectData.PropertyData entry)
        {
            const float leftWidth = 35;
            const float rowHeight = 18;

            var leftUpper = position;
            leftUpper.xMin += 4;
            leftUpper.xMax = leftUpper.xMin + leftWidth;
            leftUpper.yMax = leftUpper.yMin + rowHeight;

            var leftLower = position;
            leftLower.xMin += 4;
            leftLower.xMax = leftUpper.xMin + leftWidth;
            leftLower.yMin = leftLower.yMax - rowHeight;

            var rightUpper = position;
            rightUpper.xMin = leftUpper.xMax + 2;
            rightUpper.yMax = rightUpper.yMin + rowHeight;

            var rightLower = position;
            rightLower.yMin = rightLower.yMax - rowHeight;
            rightLower.xMin = leftLower.xMax + 2;

            if (entry == null)
            {
                entry = new XcodeProjectData.PropertyData { Key = "", Value = "" };
            }

            GUI.Label(leftUpper, "name");
            GUI.Label(leftLower, "Value");

            if (entry.Key == null)
            {
                entry.Key = string.Empty;
            }

            entry.Key = EditorGUI.TextField(rightUpper, entry.Key);

            if (entry.Value == null)
            {
                entry.Value = string.Empty;
            }

            entry.Value = EditorGUI.TextField(rightLower, entry.Value);

            return entry;
        }

        private static XcodeProjectData.FrameworkData DrawFramework(Rect position, XcodeProjectData.FrameworkData entry)
        {
            const float leftWidth = 35;
            const float rowHeight = 18;

            var leftUpper = position;
            leftUpper.xMin += 4;
            leftUpper.xMax = leftUpper.xMin + leftWidth;
            leftUpper.yMax = leftUpper.yMin + rowHeight;

            var leftLower = position;
            leftLower.xMin += 4;
            leftLower.xMax = leftUpper.xMin + leftWidth;
            leftLower.yMin = leftLower.yMax - rowHeight;

            var rightUpper = position;
            rightUpper.xMin = leftUpper.xMax + 2;
            rightUpper.yMax = rightUpper.yMin + rowHeight;

            var rightLower = position;
            rightLower.yMin = rightLower.yMax - rowHeight;
            rightLower.xMin = leftLower.xMax + 2;

            if (entry == null)
            {
                entry = new XcodeProjectData.FrameworkData { File = "", Type = XcodeProjectData.FrameWorkRequestType.Optional };
            }

            GUI.Label(leftUpper, "File");
            GUI.Label(leftLower, "Request");

            if (entry.File == null)
            {
                entry.File = string.Empty;
            }

            entry.File = EditorGUI.TextField(rightUpper, entry.File);
		
            entry.Type = (XcodeProjectData.FrameWorkRequestType)EditorGUI.EnumPopup(rightLower, entry.Type);

            return entry;
        }
    }
}
