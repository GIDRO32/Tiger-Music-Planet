using UnityEditor;
using UnityEditor.Callbacks;
using UnityEditor.iOS.Xcode;

namespace Editor
{
    public sealed class IOSBitcodePostprocessor
    {
        public static bool enableBitcode = true;
  
        [PostProcessBuild]
        public static void OnPostprocessBuild(BuildTarget target, string pathToBuiltProject) {
            switch(target) {
                case BuildTarget.iOS:
                    setupBitcode(pathToBuiltProject);
                    break;
                default: break;
            }
        }
 
        private static void setupBitcode(string pathToBuiltProject) {
            var project = new PBXProject();
            var pbxPath = PBXProject.GetPBXProjectPath(pathToBuiltProject);
            project.ReadFromFile(pbxPath);
            setupBitcodeFramework(project);
            setupBitcodeMain(project);
            project.WriteToFile(pbxPath);
        }
 
        private static void setupBitcodeFramework(PBXProject project) {
            setupBitcode(project, project.GetUnityFrameworkTargetGuid(), false);
        }
 
        private static void setupBitcodeMain(PBXProject project) {
            setupBitcode(project, project.GetUnityMainTargetGuid());
        }
        
        private static void setupBitcode(PBXProject project, string targetGUID, bool enable) {
            project.SetBuildProperty(targetGUID, "ENABLE_BITCODE", enable ? "YES" : "NO");
        }
 
        private static void setupBitcode(PBXProject project, string targetGUID) {
            project.SetBuildProperty(targetGUID, "ENABLE_BITCODE", enableBitcode ? "YES" : "NO");
        }
    }
}