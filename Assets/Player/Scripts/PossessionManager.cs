using UnityEngine;

public class PossessionManager : MonoBehaviour
{
    private Transform _currentPossessedObject;
    private Transform _modelRoot;

    private void Awake()
    {
        _modelRoot = transform.Find("Model");
        if (_modelRoot == null)
        {
            Debug.LogError("Error! PossessionManager could not find child with name Model");
        }


    }
}
