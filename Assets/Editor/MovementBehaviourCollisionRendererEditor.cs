using log4net.Util;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(MovementBehaviourCollisionRenderer))]
public class MovementBehaviourCollisionRendererEditor : Editor
{

    public void OnSceneGUI()
    {
        MovementBehaviour movementBehaviour = target.GetComponent<MovementBehaviour>();
        float a = movementBehaviour.radius;
        float b = movementBehaviour.height;

        Handles.color = Color.green;
        Handles.zTest = UnityEngine.Rendering.CompareFunction.Less;
        Handles.DrawWireDisc(movementBehaviour.transform.position - Vector3.up * b * movementBehaviour.collision.lossyScale.y, Vector3.up, a * movementBehaviour.collision.lossyScale.y);
        Handles.DrawWireDisc(movementBehaviour.transform.position - Vector3.up * b * movementBehaviour.collision.lossyScale.y, Vector3.right, a * movementBehaviour.collision.lossyScale.y);
        Handles.DrawWireDisc(movementBehaviour.transform.position - Vector3.up * b * movementBehaviour.collision.lossyScale.y, Vector3.forward, a * movementBehaviour.collision.lossyScale.y);

        Handles.zTest = UnityEngine.Rendering.CompareFunction.GreaterEqual;
        Handles.color = Color.green * 0.5f;
        Handles.DrawWireDisc(movementBehaviour.transform.position - Vector3.up * b * movementBehaviour.collision.lossyScale.y, Vector3.up, a * movementBehaviour.collision.lossyScale.y);
        Handles.DrawWireDisc(movementBehaviour.transform.position - Vector3.up * b * movementBehaviour.collision.lossyScale.y, Vector3.right, a * movementBehaviour.collision.lossyScale.y);
        Handles.DrawWireDisc(movementBehaviour.transform.position - Vector3.up * b * movementBehaviour.collision.lossyScale.y, Vector3.forward, a * movementBehaviour.collision.lossyScale.y);
    }
}