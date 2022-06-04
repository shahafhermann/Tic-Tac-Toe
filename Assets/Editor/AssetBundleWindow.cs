using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;

class AssetBundleWindow : EditorWindow 
{
    // string myString = "Hello World";
    // bool groupEnabled;
    // bool myBool = true;
    // float myFloat = 1.23f;

    private const string WindowTitle = "Create Asset Bundle";
    private Sprite _xSymbol;
    private Sprite _oSymbol;
    private Sprite _background;
    private string _assetBundleName;
    
    [MenuItem ("Window/Asset Bundle Window")]
    public static void  ShowWindow () 
    {
        //Show existing window instance. If one doesn't exist, make one.
        GetWindow(typeof(AssetBundleWindow));
    }
    
    void OnGUI () 
    {
        
        GUILayout.Label (WindowTitle, EditorStyles.boldLabel);
        _xSymbol = (Sprite) EditorGUILayout.ObjectField("X Symbol", _xSymbol, typeof(Sprite), true);
        _oSymbol = (Sprite) EditorGUILayout.ObjectField("O Symbol", _oSymbol, typeof(Sprite), true);
        _background = (Sprite) EditorGUILayout.ObjectField("Background", _background, typeof(Sprite), true);
        _assetBundleName = EditorGUILayout.TextField ("Asset Bundle Name", _assetBundleName);
        if (GUILayout.Button("Build Asset Bundle"))
        {
            // Create the array of bundle build details.
            AssetBundleBuild[] buildMap = new AssetBundleBuild[1];

            buildMap[0].assetBundleName = _assetBundleName;

            string[] graphicAssets = new string[3];
            graphicAssets[0] = AssetDatabase.GetAssetPath(_xSymbol);
            graphicAssets[1] = AssetDatabase.GetAssetPath(_oSymbol);
            graphicAssets[2] = AssetDatabase.GetAssetPath(_background);

            buildMap[0].assetNames = graphicAssets;

            System.IO.Directory.CreateDirectory(Application.streamingAssetsPath);
            BuildPipeline.BuildAssetBundles(Application.streamingAssetsPath, buildMap, BuildAssetBundleOptions.None, BuildTarget.StandaloneWindows);
            
        }
        
        
    }
}

#endif