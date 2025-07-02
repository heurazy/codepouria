using System;
using TMPro;
using UnityEngine;

// Token: 0x020001A9 RID: 425
[ExecuteInEditMode]
[RequireComponent(typeof(TextMeshProUGUI))]
public class CircularTextMeshPro : MonoBehaviour
{
	// Token: 0x170000AA RID: 170
	// (get) Token: 0x06000BCF RID: 3023 RVA: 0x0003B58A File Offset: 0x0003978A
	// (set) Token: 0x06000BD0 RID: 3024 RVA: 0x0003B592 File Offset: 0x00039792
	[Tooltip("The radius of the text circle arc")]
	public float Radius
	{
		get
		{
			return this.m_radius;
		}
		set
		{
			this.m_radius = value;
			this.OnCurvePropertyChanged();
		}
	}

	// Token: 0x06000BD1 RID: 3025 RVA: 0x0003B5A1 File Offset: 0x000397A1
	private void Awake()
	{
		this.m_TextComponent = base.gameObject.GetComponent<TextMeshProUGUI>();
	}

	// Token: 0x06000BD2 RID: 3026 RVA: 0x0003B5B4 File Offset: 0x000397B4
	private void OnEnable()
	{
		this.m_TextComponent.OnPreRenderText += this.UpdateTextCurve;
		this.OnCurvePropertyChanged();
	}

	// Token: 0x06000BD3 RID: 3027 RVA: 0x0003B5D3 File Offset: 0x000397D3
	private void OnDisable()
	{
		this.m_TextComponent.OnPreRenderText -= this.UpdateTextCurve;
	}

	// Token: 0x06000BD4 RID: 3028 RVA: 0x0003B5EC File Offset: 0x000397EC
	protected void OnCurvePropertyChanged()
	{
		this.UpdateTextCurve(this.m_TextComponent.textInfo);
		this.m_TextComponent.ForceMeshUpdate(false, false);
	}

	// Token: 0x06000BD5 RID: 3029 RVA: 0x0003B60C File Offset: 0x0003980C
	protected void UpdateTextCurve(TMP_TextInfo textInfo)
	{
		for (int i = 0; i < textInfo.characterInfo.Length; i++)
		{
			if (textInfo.characterInfo[i].isVisible)
			{
				int vertexIndex = textInfo.characterInfo[i].vertexIndex;
				int materialReferenceIndex = textInfo.characterInfo[i].materialReferenceIndex;
				Vector3[] vertices = textInfo.meshInfo[materialReferenceIndex].vertices;
				Vector3 vector = new Vector2((vertices[vertexIndex].x + vertices[vertexIndex + 2].x) / 2f, textInfo.characterInfo[i].baseLine);
				vertices[vertexIndex] += -vector;
				vertices[vertexIndex + 1] += -vector;
				vertices[vertexIndex + 2] += -vector;
				vertices[vertexIndex + 3] += -vector;
				Matrix4x4 matrix4x = this.ComputeTransformationMatrix(vector, textInfo, i);
				vertices[vertexIndex] = matrix4x.MultiplyPoint3x4(vertices[vertexIndex]);
				vertices[vertexIndex + 1] = matrix4x.MultiplyPoint3x4(vertices[vertexIndex + 1]);
				vertices[vertexIndex + 2] = matrix4x.MultiplyPoint3x4(vertices[vertexIndex + 2]);
				vertices[vertexIndex + 3] = matrix4x.MultiplyPoint3x4(vertices[vertexIndex + 3]);
			}
		}
	}

	// Token: 0x06000BD6 RID: 3030 RVA: 0x0003B7A8 File Offset: 0x000399A8
	protected Matrix4x4 ComputeTransformationMatrix(Vector3 charMidBaselinePos, TMP_TextInfo textInfo, int charIdx)
	{
		float num = this.m_radius + textInfo.lineInfo[textInfo.characterInfo[charIdx].lineNumber].baseline;
		float num2 = 2f * num * 3.1415927f;
		float num3 = ((charMidBaselinePos.x / num2 - 0.5f) * 360f + 90f) * 0.017453292f;
		float num4 = Mathf.Cos(num3);
		float num5 = Mathf.Sin(num3);
		Vector2 vector = new Vector2(num4 * num, -num5 * num);
		float num6 = -Mathf.Atan2(num5, num4) * 57.29578f - 90f;
		return Matrix4x4.TRS(new Vector3(vector.x, vector.y, 0f), Quaternion.AngleAxis(num6, Vector3.forward), Vector3.one);
	}

	// Token: 0x04000AAD RID: 2733
	private TextMeshProUGUI m_TextComponent;

	// Token: 0x04000AAE RID: 2734
	[SerializeField]
	[HideInInspector]
	private float m_radius = 10f;
}
