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
    public Transform ClickerPointer;
    public Transform Wheel;

    public int index;
    public int index0;
    public int index1;
    public int index2;
    public float dot0;
    public float dot1;
    public float dot2;
    public int GetCurrentSectionId()
    {
        float sectionAngle = (360f / placeholders.Count);

        // Get wheel Y rotation and calculate approximate index
        float wheelYRotation = Wheel.localRotation.eulerAngles.y;
        
        // Normalize angle to 0-360 range
        wheelYRotation = wheelYRotation % 360f;
        if (wheelYRotation < 0)
            wheelYRotation += 360f;
        float approximateIndex = (wheelYRotation - sectionAngle / 2f) / sectionAngle;
        
        // Round up to get the upper index
        int firstIndex = Mathf.CeilToInt(approximateIndex);
        int secondIndex = firstIndex + 1;
        int thirdIndex = firstIndex - 1;
        
        // Ensure indices are within valid range
        firstIndex = firstIndex % placeholders.Count;
        secondIndex = secondIndex % placeholders.Count;
        thirdIndex = thirdIndex % placeholders.Count;
        if (secondIndex < 0)
            secondIndex += placeholders.Count;
        if (thirdIndex < 0)
            thirdIndex += placeholders.Count;
        
        // Compare angles from wheel center to placeholder vs ClickerPointer
        Vector3 center = Wheel.position;
        Vector3 clickerDirection = (ClickerPointer.position - center).normalized;
        
        Vector3 firstPlaceholderDirection = (placeholders[firstIndex].position - center).normalized;
        Vector3 secondPlaceholderDirection = (placeholders[secondIndex].position - center).normalized;
        Vector3 thirdPlaceholderDirection = (placeholders[thirdIndex].position - center).normalized;
        
        float firstAngle = Vector3.Angle(firstPlaceholderDirection, clickerDirection);
        float secondAngle = Vector3.Angle(secondPlaceholderDirection, clickerDirection);
        float thirdAngle = Vector3.Angle(thirdPlaceholderDirection, clickerDirection);
        
        // Also keep dot products for debugging/inspection
        float firstDotProduct = Vector3.Dot(firstPlaceholderDirection, clickerDirection);
        float secondDotProduct = Vector3.Dot(secondPlaceholderDirection, clickerDirection);
        float thirdDotProduct = Vector3.Dot(thirdPlaceholderDirection, clickerDirection);

        index0 = firstIndex;
        index1 = secondIndex;
        index2 = thirdIndex;
        dot0 = firstDotProduct;
        dot1 = secondDotProduct;
        dot2 = thirdDotProduct;
        int resultIndex = firstIndex;
        float minAngle = firstAngle;
        if (secondAngle < minAngle)
        {
            resultIndex = secondIndex;
            minAngle = secondAngle;
        }
        if (thirdAngle < minAngle)
        {
            resultIndex = thirdIndex;
        }
        return resultIndex;
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
