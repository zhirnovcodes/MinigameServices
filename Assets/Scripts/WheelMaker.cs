using System.Linq;
using UnityEditor;
using UnityEngine;

// WheelMaker: Attach to any GameObject. Configure references and click Build in the inspector.
public class WheelMaker : MonoBehaviour
{
#if UNITY_EDITOR
	public GameObject Root;
	public GameObject Wheel;
	public GameObject Peg;
	public GameObject PlaceholderPrefab;
	public int SectionsCount = 8;
	public float PegOffset = 0.0f;
	public float PlaceholderOffset = 0.0f;

	public void Build()
	{
		Transform pegsRoot = EnsureChild(Root.transform, "Pegs");
		Transform placeholdersRoot = EnsureChild(Root.transform, "Placeholders");
		ClearChildren(pegsRoot);
		ClearChildren(placeholdersRoot);

		Bounds wheelBounds = CalculateWorldBounds(Wheel);
		float wheelRadius = 0.5f * Mathf.Min(wheelBounds.size.x, wheelBounds.size.z);
		Vector3 wheelCenter = new Vector3(wheelBounds.center.x, Wheel.transform.position.y, wheelBounds.center.z);

		float pegRadius = GetPrefabXZRadius(Peg);
		float placeholderRadius = GetPrefabXZRadius(PlaceholderPrefab);

		float step = Mathf.PI * 2f / SectionsCount;
		for (int i = 0; i < SectionsCount; i++)
		{
			float angle = i * step;
			Vector3 dir = new Vector3(Mathf.Cos(angle), 0f, Mathf.Sin(angle));

			float pegDistanceFromCenter = Mathf.Max(0f, wheelRadius - PegOffset - pegRadius);
			Vector3 pegPos = wheelCenter + dir * pegDistanceFromCenter;
			Quaternion pegRot = Quaternion.LookRotation(dir, Vector3.up);
			CreateChildFromPrefab(Peg, pegsRoot, $"Peg_{i}", pegPos, pegRot);

			float midAngle = angle + step * 0.5f;
			Vector3 midDir = new Vector3(Mathf.Cos(midAngle), 0f, Mathf.Sin(midAngle));
			float placeholderDistanceFromCenter = Mathf.Max(0f, wheelRadius - PlaceholderOffset - placeholderRadius);
			Vector3 placeholderPos = wheelCenter + midDir * placeholderDistanceFromCenter;
			Quaternion placeholderRot = Quaternion.LookRotation(midDir, Vector3.up);
			CreateChildFromPrefab(PlaceholderPrefab, placeholdersRoot, $"Placeholder_{i}", placeholderPos, placeholderRot);
		}
	}

	static Transform EnsureChild(Transform parent, string childName)
	{
		Transform existing = parent.Find(childName);
		if (existing != null) return existing;
		GameObject go = new GameObject(childName);
		go.transform.SetParent(parent, false);
		go.transform.localPosition = Vector3.zero;
		go.transform.localRotation = Quaternion.identity;
		go.transform.localScale = Vector3.one;
		return go.transform;
	}

	static void ClearChildren(Transform parent)
	{
		for (int i = parent.childCount - 1; i >= 0; i--)
		{
			Transform child = parent.GetChild(i);
		}
	}

	static void CreateChildFromPrefab(GameObject prefab, Transform parent, string name, Vector3 position, Quaternion rotation)
	{
		GameObject instance = PrefabUtility.InstantiatePrefab(prefab) as GameObject;
		if (instance == null)
		{
			// Fallback for non-prefab assets
			instance = Object.Instantiate(prefab, parent);
		}
		instance.transform.SetParent(parent, false);
		instance.name = name;
		instance.transform.position = position;
		instance.transform.rotation = rotation;
	}

	static Bounds CalculateWorldBounds(GameObject root)
	{
		Renderer[] renderers = root.GetComponentsInChildren<Renderer>(true);
		if (renderers.Length == 0)
		{
			return new Bounds(root.transform.position, Vector3.zero);
		}
		Bounds bounds = renderers[0].bounds;
		for (int i = 1; i < renderers.Length; i++)
		{
			bounds.Encapsulate(renderers[i].bounds);
		}
		return bounds;
	}

	static float GetPrefabXZRadius(GameObject prefab)
	{
		// Try to get bounds from the asset directly via a temporary instance hidden in editor
		GameObject temp = PrefabUtility.InstantiatePrefab(prefab) as GameObject;
		float radius = 0f;
		if (temp != null)
		{
			try
			{
				temp.hideFlags = HideFlags.HideAndDontSave;
				Bounds b = CalculateWorldBounds(temp);
				radius = 0.5f * Mathf.Max(b.size.x, b.size.z);
			}
			finally
			{
				Object.DestroyImmediate(temp);
			}
		}
		else
		{
			// Fallback: assume unit cube scaled by transform, which we don't have; return 0
			radius = 0f;
		}
		return Mathf.Max(0f, radius);
	}
#endif
}


