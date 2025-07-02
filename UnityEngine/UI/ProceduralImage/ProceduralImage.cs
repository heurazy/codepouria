using System;
using UnityEngine.Events;

namespace UnityEngine.UI.ProceduralImage
{
	// Token: 0x020002A4 RID: 676
	[ExecuteInEditMode]
	[AddComponentMenu("UI/Procedural Image")]
	public class ProceduralImage : Image
	{
		// Token: 0x170000C2 RID: 194
		// (get) Token: 0x06001009 RID: 4105 RVA: 0x00051499 File Offset: 0x0004F699
		// (set) Token: 0x0600100A RID: 4106 RVA: 0x000514C1 File Offset: 0x0004F6C1
		private static Material DefaultProceduralImageMaterial
		{
			get
			{
				if (ProceduralImage.materialInstance == null)
				{
					ProceduralImage.materialInstance = new Material(Shader.Find("UI/Procedural UI Image"));
				}
				return ProceduralImage.materialInstance;
			}
			set
			{
				ProceduralImage.materialInstance = value;
			}
		}

		// Token: 0x170000C3 RID: 195
		// (get) Token: 0x0600100B RID: 4107 RVA: 0x000514C9 File Offset: 0x0004F6C9
		// (set) Token: 0x0600100C RID: 4108 RVA: 0x000514D1 File Offset: 0x0004F6D1
		public float BorderWidth
		{
			get
			{
				return this.borderWidth;
			}
			set
			{
				this.borderWidth = value;
				this.SetVerticesDirty();
			}
		}

		// Token: 0x170000C4 RID: 196
		// (get) Token: 0x0600100D RID: 4109 RVA: 0x000514E0 File Offset: 0x0004F6E0
		// (set) Token: 0x0600100E RID: 4110 RVA: 0x000514E8 File Offset: 0x0004F6E8
		public float FalloffDistance
		{
			get
			{
				return this.falloffDistance;
			}
			set
			{
				this.falloffDistance = value;
				this.SetVerticesDirty();
			}
		}

		// Token: 0x170000C5 RID: 197
		// (get) Token: 0x0600100F RID: 4111 RVA: 0x000514F7 File Offset: 0x0004F6F7
		// (set) Token: 0x06001010 RID: 4112 RVA: 0x00051537 File Offset: 0x0004F737
		protected ProceduralImageModifier Modifier
		{
			get
			{
				if (this.modifier == null)
				{
					this.modifier = base.GetComponent<ProceduralImageModifier>();
					if (this.modifier == null)
					{
						this.ModifierType = typeof(FreeModifier);
					}
				}
				return this.modifier;
			}
			set
			{
				this.modifier = value;
			}
		}

		// Token: 0x170000C6 RID: 198
		// (get) Token: 0x06001011 RID: 4113 RVA: 0x00051540 File Offset: 0x0004F740
		// (set) Token: 0x06001012 RID: 4114 RVA: 0x00051550 File Offset: 0x0004F750
		public global::System.Type ModifierType
		{
			get
			{
				return this.Modifier.GetType();
			}
			set
			{
				if (this.modifier != null && this.modifier.GetType() != value)
				{
					if (base.GetComponent<ProceduralImageModifier>() != null)
					{
						Object.DestroyImmediate(base.GetComponent<ProceduralImageModifier>());
					}
					base.gameObject.AddComponent(value);
					this.Modifier = base.GetComponent<ProceduralImageModifier>();
					this.SetAllDirty();
					return;
				}
				if (this.modifier == null)
				{
					base.gameObject.AddComponent(value);
					this.Modifier = base.GetComponent<ProceduralImageModifier>();
					this.SetAllDirty();
				}
			}
		}

		// Token: 0x06001013 RID: 4115 RVA: 0x000515E4 File Offset: 0x0004F7E4
		protected override void OnEnable()
		{
			base.OnEnable();
			this.Init();
		}

		// Token: 0x06001014 RID: 4116 RVA: 0x000515F2 File Offset: 0x0004F7F2
		protected override void OnDisable()
		{
			base.OnDisable();
			this.m_OnDirtyVertsCallback = (UnityAction)Delegate.Remove(this.m_OnDirtyVertsCallback, new UnityAction(this.OnVerticesDirty));
		}

		// Token: 0x06001015 RID: 4117 RVA: 0x0005161C File Offset: 0x0004F81C
		private void Init()
		{
			this.FixTexCoordsInCanvas();
			this.m_OnDirtyVertsCallback = (UnityAction)Delegate.Combine(this.m_OnDirtyVertsCallback, new UnityAction(this.OnVerticesDirty));
			base.preserveAspect = false;
			this.material = null;
			if (base.sprite == null)
			{
				base.sprite = EmptySprite.Get();
			}
		}

		// Token: 0x06001016 RID: 4118 RVA: 0x00051678 File Offset: 0x0004F878
		protected void OnVerticesDirty()
		{
			if (base.sprite == null)
			{
				base.sprite = EmptySprite.Get();
			}
		}

		// Token: 0x06001017 RID: 4119 RVA: 0x00051694 File Offset: 0x0004F894
		protected void FixTexCoordsInCanvas()
		{
			Canvas componentInParent = base.GetComponentInParent<Canvas>();
			if (componentInParent != null)
			{
				this.FixTexCoordsInCanvas(componentInParent);
			}
		}

		// Token: 0x06001018 RID: 4120 RVA: 0x000516B8 File Offset: 0x0004F8B8
		protected void FixTexCoordsInCanvas(Canvas c)
		{
			c.additionalShaderChannels |= AdditionalCanvasShaderChannels.TexCoord1 | AdditionalCanvasShaderChannels.TexCoord2 | AdditionalCanvasShaderChannels.TexCoord3;
		}

		// Token: 0x06001019 RID: 4121 RVA: 0x000516C8 File Offset: 0x0004F8C8
		private Vector4 FixRadius(Vector4 vec)
		{
			Rect rect = base.rectTransform.rect;
			vec = new Vector4(Mathf.Max(vec.x, 0f), Mathf.Max(vec.y, 0f), Mathf.Max(vec.z, 0f), Mathf.Max(vec.w, 0f));
			float num = Mathf.Min(Mathf.Min(Mathf.Min(Mathf.Min(rect.width / (vec.x + vec.y), rect.width / (vec.z + vec.w)), rect.height / (vec.x + vec.w)), rect.height / (vec.z + vec.y)), 1f);
			return vec * num;
		}

		// Token: 0x0600101A RID: 4122 RVA: 0x0005179D File Offset: 0x0004F99D
		protected override void OnPopulateMesh(VertexHelper toFill)
		{
			base.OnPopulateMesh(toFill);
			this.EncodeAllInfoIntoVertices(toFill, this.CalculateInfo());
		}

		// Token: 0x0600101B RID: 4123 RVA: 0x000517B3 File Offset: 0x0004F9B3
		protected override void OnTransformParentChanged()
		{
			base.OnTransformParentChanged();
			this.FixTexCoordsInCanvas();
		}

		// Token: 0x0600101C RID: 4124 RVA: 0x000517C4 File Offset: 0x0004F9C4
		private ProceduralImageInfo CalculateInfo()
		{
			Rect pixelAdjustedRect = base.GetPixelAdjustedRect();
			float num = 1f / Mathf.Max(0f, this.falloffDistance);
			Vector4 vector = this.FixRadius(this.Modifier.CalculateRadius(pixelAdjustedRect));
			float num2 = Mathf.Min(pixelAdjustedRect.width, pixelAdjustedRect.height);
			return new ProceduralImageInfo(pixelAdjustedRect.width + this.falloffDistance, pixelAdjustedRect.height + this.falloffDistance, this.falloffDistance, num, vector / num2, this.borderWidth / num2 * 2f);
		}

		// Token: 0x0600101D RID: 4125 RVA: 0x00051854 File Offset: 0x0004FA54
		private void EncodeAllInfoIntoVertices(VertexHelper vh, ProceduralImageInfo info)
		{
			UIVertex uivertex = default(UIVertex);
			Vector2 vector = new Vector2(info.width, info.height);
			Vector2 vector2 = new Vector2(this.EncodeFloats_0_1_16_16(info.radius.x, info.radius.y), this.EncodeFloats_0_1_16_16(info.radius.z, info.radius.w));
			Vector2 vector3 = new Vector2((info.borderWidth == 0f) ? 1f : Mathf.Clamp01(info.borderWidth), info.pixelSize);
			for (int i = 0; i < vh.currentVertCount; i++)
			{
				vh.PopulateUIVertex(ref uivertex, i);
				uivertex.position += (uivertex.uv0 - new Vector3(0.5f, 0.5f)) * info.fallOffDistance;
				uivertex.uv1 = vector;
				uivertex.uv2 = vector2;
				uivertex.uv3 = vector3;
				vh.SetUIVertex(uivertex, i);
			}
		}

		// Token: 0x0600101E RID: 4126 RVA: 0x00051980 File Offset: 0x0004FB80
		private float EncodeFloats_0_1_16_16(float a, float b)
		{
			Vector2 vector = new Vector2(1f, 1.5259022E-05f);
			return Vector2.Dot(new Vector2(Mathf.Floor(a * 65534f) / 65535f, Mathf.Floor(b * 65534f) / 65535f), vector);
		}

		// Token: 0x170000C7 RID: 199
		// (get) Token: 0x0600101F RID: 4127 RVA: 0x000519CD File Offset: 0x0004FBCD
		// (set) Token: 0x06001020 RID: 4128 RVA: 0x000519E9 File Offset: 0x0004FBE9
		public override Material material
		{
			get
			{
				if (this.m_Material == null)
				{
					return ProceduralImage.DefaultProceduralImageMaterial;
				}
				return base.material;
			}
			set
			{
				base.material = value;
			}
		}

		// Token: 0x04000F15 RID: 3861
		[SerializeField]
		private float borderWidth;

		// Token: 0x04000F16 RID: 3862
		private ProceduralImageModifier modifier;

		// Token: 0x04000F17 RID: 3863
		private static Material materialInstance;

		// Token: 0x04000F18 RID: 3864
		[SerializeField]
		private float falloffDistance = 1f;
	}
}
