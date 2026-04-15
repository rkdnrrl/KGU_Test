using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class DoorOutline : MonoBehaviour
{
    public Color outlineColor = Color.red;
    [Range(1f, 1.2f)] public float outlineScale = 1.03f;
    public bool enableOnStart = false;

    private readonly List<Renderer> _outlineRenderers = new List<Renderer>();
    private Material _outlineMaterial;

    private static readonly int ColorId = Shader.PropertyToID("_Color");
    private static readonly int BaseColorId = Shader.PropertyToID("_BaseColor");
    private static readonly int CullId = Shader.PropertyToID("_Cull");

    private void Awake()
    {
        BuildOutlineObjectsIfNeeded();
        SetEnabled(enableOnStart);
    }

    public void SetColor(Color c)
    {
        outlineColor = c;
        EnsureMaterial();

        if (_outlineMaterial.HasProperty(ColorId)) _outlineMaterial.SetColor(ColorId, outlineColor);
        if (_outlineMaterial.HasProperty(BaseColorId)) _outlineMaterial.SetColor(BaseColorId, outlineColor);
    }

    public void SetEnabled(bool on)
    {
        for (int i = 0; i < _outlineRenderers.Count; i++)
        {
            if (_outlineRenderers[i] != null)
                _outlineRenderers[i].enabled = on;
        }
    }

    private void BuildOutlineObjectsIfNeeded()
    {
        EnsureMaterial();

        // 이미 만들어져 있으면 다시 만들지 않음
        _outlineRenderers.Clear();
        Transform existing = transform.Find("__OUTLINE__");
        if (existing != null)
        {
            _outlineRenderers.AddRange(existing.GetComponentsInChildren<Renderer>(true));
            return;
        }

        GameObject root = new GameObject("__OUTLINE__");
        root.transform.SetParent(transform, false);
        root.transform.localPosition = Vector3.zero;
        root.transform.localRotation = Quaternion.identity;
        root.transform.localScale = Vector3.one;

        MeshRenderer[] meshRenderers = GetComponentsInChildren<MeshRenderer>(true);
        for (int i = 0; i < meshRenderers.Length; i++)
        {
            MeshRenderer src = meshRenderers[i];
            MeshFilter srcFilter = src.GetComponent<MeshFilter>();
            if (srcFilter == null || srcFilter.sharedMesh == null) continue;

            GameObject go = new GameObject(src.gameObject.name + "_Outline");
            go.transform.SetParent(root.transform, false);

            // 원본과 똑같이 위치/회전/스케일 복사
            CopyLocalTransform(src.transform, go.transform);
            go.transform.localScale = go.transform.localScale * outlineScale;

            MeshFilter mf = go.AddComponent<MeshFilter>();
            mf.sharedMesh = srcFilter.sharedMesh;

            MeshRenderer mr = go.AddComponent<MeshRenderer>();
            mr.sharedMaterial = _outlineMaterial;
            mr.shadowCastingMode = ShadowCastingMode.Off;
            mr.receiveShadows = false;
            mr.lightProbeUsage = LightProbeUsage.Off;
            mr.reflectionProbeUsage = ReflectionProbeUsage.Off;
            mr.motionVectorGenerationMode = MotionVectorGenerationMode.ForceNoMotion;

            _outlineRenderers.Add(mr);
        }

        SkinnedMeshRenderer[] skinned = GetComponentsInChildren<SkinnedMeshRenderer>(true);
        for (int i = 0; i < skinned.Length; i++)
        {
            SkinnedMeshRenderer src = skinned[i];
            if (src.sharedMesh == null) continue;

            GameObject go = new GameObject(src.gameObject.name + "_Outline");
            go.transform.SetParent(root.transform, false);
            CopyLocalTransform(src.transform, go.transform);
            go.transform.localScale = go.transform.localScale * outlineScale;

            SkinnedMeshRenderer smr = go.AddComponent<SkinnedMeshRenderer>();
            smr.sharedMesh = src.sharedMesh;
            smr.bones = src.bones;
            smr.rootBone = src.rootBone;
            smr.sharedMaterial = _outlineMaterial;
            smr.shadowCastingMode = ShadowCastingMode.Off;
            smr.receiveShadows = false;
            smr.lightProbeUsage = LightProbeUsage.Off;
            smr.reflectionProbeUsage = ReflectionProbeUsage.Off;
            smr.motionVectorGenerationMode = MotionVectorGenerationMode.ForceNoMotion;

            _outlineRenderers.Add(smr);
        }
    }

    private void EnsureMaterial()
    {
        if (_outlineMaterial != null)
            return;

        Shader shader =
            Shader.Find("Custom/DoorOutlineURP") ??
            Shader.Find("Unlit/Color") ??
            Shader.Find("Universal Render Pipeline/Unlit") ??
            Shader.Find("Sprites/Default");

        _outlineMaterial = new Material(shader);
        _outlineMaterial.name = "DoorOutline_Runtime";

        // 색상 지정
        if (_outlineMaterial.HasProperty(ColorId)) _outlineMaterial.SetColor(ColorId, outlineColor);
        if (_outlineMaterial.HasProperty(BaseColorId)) _outlineMaterial.SetColor(BaseColorId, outlineColor);

        // 외곽선 느낌: "앞면을 안 그리기(Front Cull)"가 가능하면 적용
        if (_outlineMaterial.HasProperty(CullId))
        {
            _outlineMaterial.SetInt(CullId, (int)CullMode.Front);
        }

        // 다른 물체보다 위에 보이도록 조금 뒤로(큰 값)
        _outlineMaterial.renderQueue = 3000;
    }

    private static void CopyLocalTransform(Transform src, Transform dst)
    {
        dst.localPosition = src.localPosition;
        dst.localRotation = src.localRotation;
        dst.localScale = src.localScale;
    }
}

