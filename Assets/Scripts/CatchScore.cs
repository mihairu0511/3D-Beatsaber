using UnityEngine;
using UnityEngine.Events;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

public class CatchScore : MonoBehaviour
{
    XRGrabInteractable _grab;

    void Awake()
    {
        _grab = GetComponent<XRGrabInteractable>();
    }

    void OnEnable()
    {
        if (_grab == null) return;

        _grab.selectEntered.AddListener(
            new UnityAction<UnityEngine.XR.Interaction.Toolkit.SelectEnterEventArgs>(OnGrabbed)
        );
    }

    void OnDisable()
    {
        if (_grab == null) return;

        _grab.selectEntered.RemoveListener(
            new UnityAction<UnityEngine.XR.Interaction.Toolkit.SelectEnterEventArgs>(OnGrabbed)
        );
    }

    void OnGrabbed(UnityEngine.XR.Interaction.Toolkit.SelectEnterEventArgs args)
    {
        Debug.Log("Caught!");
    }
}