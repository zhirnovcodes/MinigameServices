using UnityEngine;
using System.Collections.Generic;

public interface IWheelModel
{
    int GetCurrentSectionId();
    int GetSectionsCount();
    Transform GetPlaceholderTransform(int index);
}

public class WheelModel : MonoBehaviour, IWheelModel
{
    public List<Transform> placeholders = new List<Transform>();
    public Transform Clicker;
    public Transform Wheel;

    public int index;
    public int index0;
    public int index1;
    public float dot0;
    public float dot1;
    public int GetCurrentSectionId()
    {
        float sectionAngle = (360f / placeholders.Count);

        // Get wheel Y rotation and calculate approximate index
        float wheelYRotation = Wheel.eulerAngles.y;
        
        // Normalize angle to 0-360 range
        wheelYRotation = wheelYRotation % 360f;
        if (wheelYRotation < 0)
            wheelYRotation += 360f;
        float approximateIndex = (wheelYRotation - sectionAngle / 2f) / sectionAngle;
        
        // Round up to get the upper index
        int upperIndex = Mathf.CeilToInt(approximateIndex);
        int lowerIndex = upperIndex - 1;
        
        // Ensure indices are within valid range
        upperIndex = upperIndex % placeholders.Count;
        lowerIndex = lowerIndex % placeholders.Count;
        if (lowerIndex < 0)
            lowerIndex += placeholders.Count;
        
        // Get clicker direction
        Vector3 clickerDirection = Clicker.forward;
        
        // Calculate dot products for both placeholders
        Vector3 upperPlaceholderDirection = (placeholders[upperIndex].position - Clicker.position).normalized;
        Vector3 lowerPlaceholderDirection = (placeholders[lowerIndex].position - Clicker.position).normalized;
        
        float upperDotProduct = Vector3.Dot(upperPlaceholderDirection, clickerDirection);
        float lowerDotProduct = Vector3.Dot(lowerPlaceholderDirection, clickerDirection);

        // Return the index with positive dot product
        index0 = upperIndex;
        index1 = lowerIndex;
        dot0 = upperDotProduct;
        dot1 = lowerDotProduct;
        return upperIndex;
    }

    public int GetSectionsCount()
    {
        return placeholders.Count;
    }

    public Transform GetPlaceholderTransform(int index)
    {
        return placeholders[index];
    }

    private void Update()
    {
        index = GetCurrentSectionId();
    }
}
