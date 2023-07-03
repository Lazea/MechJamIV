using UnityEditor;
using UnityEngine;

public class AnimationClipEditor : EditorWindow
{
    [MenuItem("Window/Animation Clip Editor")]
    private static void OpenWindow()
    {
        AnimationClipEditor window = GetWindow<AnimationClipEditor>();
        window.titleContent = new GUIContent("Animation Clip Editor");
        window.Show();
    }

    private string fbxPath = "Assets/Models/Player/PlayerMecha.fbx";

    private void OnGUI()
    {
        GUILayout.Label("FBX File Path:", EditorStyles.boldLabel);
        fbxPath = EditorGUILayout.TextField(fbxPath);

        GUILayout.Space(20);

        if (GUILayout.Button("Correct Scales to 1.0"))
        {
            SetScaleToOne(fbxPath);
        }
    }

    private void SetScaleToOne(string fbxPath)
    {
        var assetRepresentationsAtPath = AssetDatabase.LoadAllAssetRepresentationsAtPath(fbxPath);
        foreach (var assetRepresentation in assetRepresentationsAtPath)
        {
            AnimationClip animClip = assetRepresentation as AnimationClip;
            if (animClip != null)
            {
                Debug.Log("Animation Clip Name: " + animClip.name);

                EditorCurveBinding[] curveBindings = AnimationUtility.GetCurveBindings(animClip);
                foreach (EditorCurveBinding curveBinding in curveBindings)
                {
                    if (curveBinding.propertyName == "m_LocalScale.x" ||
                        curveBinding.propertyName == "m_LocalScale.y" ||
                        curveBinding.propertyName == "m_LocalScale.z")
                    {
                        AnimationCurve curve = AnimationUtility.GetEditorCurve(animClip, curveBinding);
                        if (curve != null)
                        {
                            Keyframe[] newKeyframes = new Keyframe[curve.length];
                            for (int i = 0; i < curve.length; i++)
                            {
                                float time = curve.keys[i].time;
                                newKeyframes[i] = new Keyframe(time, 1.0f);
                            }
                            AnimationCurve newCurve = new AnimationCurve(newKeyframes);
                            AnimationUtility.SetEditorCurve(animClip, curveBinding, newCurve);
                        }
                    }
                }

                EditorUtility.SetDirty(animClip);
            }
        }

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        Debug.Log("Scale set to 1.0 for all animation clips.");
    }
}
