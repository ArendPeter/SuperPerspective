using UnityEngine;
using System.Collections;
using UnityEditor;
using UnityEditor.Callbacks;

public class Build
{
    [MenuItem( "Build/All" )]
    public static void RunBuilds()
    {
        var levels = new[] { "Assets/Test.unity" };

        BuildPipeline.BuildPlayer( levels, "Build/Win32/fpsw_w32.exe", BuildTarget.StandaloneWindows, BuildOptions.AllowDebugging );
        BuildPipeline.BuildPlayer( levels, "Build/Win64/fpsw_w64.exe", BuildTarget.StandaloneWindows64, BuildOptions.AllowDebugging );
    }

    [PostProcessBuild(1)]
    public static void OnPostProcessBuild(BuildTarget target, string pathToBuiltProject)
    {
        //
        // Only steam
        //
        if ( !target.ToString().StartsWith( "Standalone" ) )
            return;

        //
        // You only need a steam_appid.txt if you're launching outside of Steam
        //
        FileUtil.ReplaceFile("steam_appid.txt", System.IO.Path.GetDirectoryName(pathToBuiltProject) + "/steam_appid.txt");

        //
        // You only need these dlls if you're launching outside of Steam
        //
        if (target == BuildTarget.StandaloneWindows)
            FileUtil.ReplaceFile("steam_api.dll", System.IO.Path.GetDirectoryName(pathToBuiltProject) + "/steam_api.dll");

        if (target == BuildTarget.StandaloneWindows64)
            FileUtil.ReplaceFile("steam_api64.dll", System.IO.Path.GetDirectoryName(pathToBuiltProject) + "/steam_api64.dll");
    }

}
