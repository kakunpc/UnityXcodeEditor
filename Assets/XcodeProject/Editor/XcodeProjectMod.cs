#if UNITY_IOS

using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEditor.iOS.Xcode;
using UnityEngine;

#pragma warning disable CS0436 

namespace XcodeProject.Editor
{
    public class XcodeProjectMod
    {
        [PostProcessBuild(200)]
        public static void OnPostprocessBuild(BuildTarget buildTarget, string path)
        {
            //if (buildTarget == BuildTarget.iOS)
            {
                Debug.Log("[XcodeProjectMod]OnPostprocessBuild");
                if (!OnPostprocessBuildForiOS(path))
                {
                    EditorApplication.Exit(1);  // エラー終了
                }
                else
                {
                    Debug.Log("[XcodeProjectMod]Success");
                }
            }
        }

        private static bool OnPostprocessBuildForiOS(string path)
        {
            var xcodeData = AssetDatabase.LoadAssetAtPath<XcodeProjectData>(XcodeProjectData.Location);
        
            string projPath = PBXProject.GetPBXProjectPath(path);
            PBXProject proj = new PBXProject();
            proj.ReadFromFile(projPath);

            string targetName = "Unity-iPhone";
            string targetGuid = proj.TargetGuidByName(targetName);
            if (targetGuid == null)
            {
                Debug.LogError("[XcodeProjectMod]Cannot find " + targetName);
                return false;
            }

            // plistの変更
            var plistpath = path + "/Info.plist";
            Debug.Log("Info.plist Path : " + plistpath);
            PlistDocument plist = new PlistDocument();
            plist.ReadFromFile(plistpath);
            // iOS10対策
            foreach (var infoPlistPropertyData in xcodeData.InfoPlistPropertyDatas)
            {
                plist.root.SetString(infoPlistPropertyData.Key, infoPlistPropertyData.Value);// "NSCameraUsageDescription", "Unity");
            }
            plist.WriteToFile(plistpath);

            // libiconvの修正
            proj.RemoveLibraryFromProject(targetGuid, "libiconv.2.dylib");
            proj.AddLibraryToProject(targetGuid, "libiconv.2.tbd", false);

            // AddFramework
            foreach (var framework in xcodeData.Framework)
            {
                proj.AddFrameworkToProject(targetGuid, framework.File, (framework.Type == XcodeProjectData.FrameWorkRequestType.Optional));
            }

            // Addlibrary
            foreach (var library in xcodeData.Library)
            {
                proj.AddLibraryToProject(targetGuid, library.File, (library.Type == XcodeProjectData.FrameWorkRequestType.Optional));
            }

            // Set BuildProperty
            foreach (var BuildProperty in xcodeData.BuildProperty)
            {
                proj.SetBuildProperty(targetGuid, BuildProperty.Key, BuildProperty.Value);
            }

            // Set CompileFlags
            foreach (var CompileFlags in xcodeData.CompileFlags)
            {
                string fileGuid = proj.FindFileGuidByProjectPath(CompileFlags.Key);
                if (fileGuid == null)
                {
                    Debug.LogError("[XcodeProjectMod]Cannot find " + CompileFlags.Key);
                    return false;
                }

                List<string> flags = CompileFlags.Value.Split(',')
                    .Select(x =>
                    {
                        string returnString = x;
                        while (true)
                        {
                            if (returnString.IndexOf(" ") != 0)
                                break;
                            returnString = returnString.Substring(1);

                            if (string.IsNullOrEmpty(returnString))
                                return x;
                        }

                        while (true)
                        {
                            int spaceIndex = returnString.LastIndexOf(" ");
                            if ((returnString.Length - 1) != spaceIndex)
                                break;
                            returnString = returnString.Substring(0, (returnString.Length - 1));

                            if (string.IsNullOrEmpty(returnString))
                                return x;
                        }
                        return returnString;
                    })
                    .ToList();

                proj.SetCompileFlagsForFile(targetGuid, fileGuid, flags);
            }

            // Push通知の処理
            if (xcodeData.UsePushNotification)
            {
                string target = proj.TargetGuidByName("Unity-iPhone");

                var debug = proj.BuildConfigByName(target, "Debug");
                var release = proj.BuildConfigByName(target, "Release");

                var fileName = "production.entitlements";
                var src = Application.dataPath + "/Editor/" + fileName;
                var projectFolder = System.IO.Directory.GetCurrentDirectory() + "/";
                var dest = Path.Combine(projectFolder, path);
                dest = Path.Combine(dest, fileName);
                Debug.Log("entitlements Path : " + src);
                Debug.Log("to Path : " + src);
                File.Copy(src, dest);
                proj.AddFileToBuild(target, proj.AddFile(dest, fileName, PBXSourceTree.Source));
                proj.SetBuildPropertyForConfig(debug, "CODE_SIGN_ENTITLEMENTS", fileName);
                proj.SetBuildPropertyForConfig(release, "CODE_SIGN_ENTITLEMENTS", fileName);
            }

            proj.WriteToFile(projPath);

            // 書き出し
            // File.WriteAllText(projPath, proj.WriteToString());

            return true;
        }
    }
}

#endif
