using System;
using System.Collections.Generic;
using UnityEngine;

namespace XcodeProject.Editor
{
    public class XcodeProjectData : ScriptableObject
    {
        public enum FrameWorkRequestType
        {
            Required,
            Optional
        }

        [Serializable]
        public class PropertyData
        {
            public string Key;
            public string Value;
        }

        [Serializable]
        public class FrameworkData
        {
            public string File;
            public FrameWorkRequestType Type;
        }

        private const string Path = "XcodeMod";

        public static readonly string Location = "Assets/Editor/" + Path + ".asset";

        public List<FrameworkData> Framework = new List<FrameworkData>();
        public List<PropertyData> BuildProperty = new List<PropertyData>();
        public List<PropertyData> CompileFlags = new List<PropertyData>();
        public List<FrameworkData> Library = new List<FrameworkData>();
        public List<PropertyData> InfoPlistPropertyDatas = new List<PropertyData>();
        public bool UsePushNotification;

        public void CreateSampleData()
        {
            Framework.Add(new FrameworkData() { File = "StoreKit.framework", Type = FrameWorkRequestType.Optional });
            BuildProperty.Add(new PropertyData() { Key = "SAMPLE_KEY", Value = "SAMPLE_VALUE" });
            CompileFlags.Add(new PropertyData() { Key = "file/file.mm", Value = "flag1,flag2" });
            Library.Add(new FrameworkData() { File = "libiconv.2.dylib", Type = FrameWorkRequestType.Optional });
            UsePushNotification = false;
        }
    }
}
