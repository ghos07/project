using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class BehaviorTreeEditor : EditorWindow
{
    private List<Node> nodes = new List<Node>();
    private Vector2 offset;
    private Vector2 drag;
    private GUIStyle nodeStyle;
    private GUIStyle selectedNodeStyle;
    private GUIStyle inPointStyle;
    private GUIStyle outPointStyle;
    private ConnectionPoint selectedInPoint;
    private ConnectionPoint selectedOutPoint;
    private Node selectedNode;

    [MenuItem("Window/Behavior Tree Editor")]
    private static void OpenWindow()
    {
        BehaviorTreeEditor window = GetWindow<BehaviorTreeEditor>();
        window.titleContent = new GUIContent("Behavior Tree Editor");
    }

    private void OnEnable()
    {
        nodeStyle = new GUIStyle();
        nodeStyle.normal.background = EditorGUIUtility.Load("builtin skins/darkskin/images/node1.png") as Texture2D;
        nodeStyle.border = new RectOffset(12, 12, 12, 12);

        selectedNodeStyle = new GUIStyle();
        selectedNodeStyle.normal.background = EditorGUIUtility.Load("builtin skins/darkskin/images/node1 on.png") as Texture2D;
        selectedNodeStyle.border = new RectOffset(12, 12, 12, 12);

        inPointStyle = new GUIStyle();
        inPointStyle.normal.background = EditorGUIUtility.Load("builtin skins/darkskin/images/btn left.png") as Texture2D;
        inPointStyle.active.background = EditorGUIUtility.Load("builtin skins/darkskin/images/btn left on.png") as Texture2D;
        inPointStyle.border = new RectOffset(4, 4, 12, 12);

        outPointStyle = new GUIStyle();
        outPointStyle.normal.background = EditorGUIUtility.Load("builtin skins/darkskin/images/btn right.png") as Texture2D;
        outPointStyle.active.background = EditorGUIUtility.Load("builtin skins/darkskin/images/btn right on.png") as Texture2D;
        outPointStyle.border = new RectOffset(4, 4, 12, 12);
    }

    private void OnGUI()
    {
        DrawGrid(20, 0.2f, Color.gray);
        DrawGrid(100, 0.4f, Color.gray);

        DrawNodes();
        DrawConnections();
        DrawConnectionLine(Event.current);

        ProcessNodeEvents(Event.current);
        ProcessEvents(Event.current);

        if (GUI.changed) Repaint();
    }

    private void DrawGrid(float gridSpacing, float gridOpacity, Color gridColor)
    {
        int widthDivs = Mathf.CeilToInt(position.width / gridSpacing);
        int heightDivs = Mathf.CeilToInt(position.height / gridSpacing);

        Handles.BeginGUI();
        Handles.color = new Color(gridColor.r, gridColor.g, gridColor.b, gridOpacity);

        offset += drag * 0.5f;
        Vector3 newOffset = new Vector3(offset.x % gridSpacing, offset.y % gridSpacing, 0);

        for (int i = 0; i < widthDivs; i++)
        {
            Handles.DrawLine(new Vector3(gridSpacing * i, -gridSpacing, 0) + newOffset,
                             new Vector3(gridSpacing * i, position.height, 0f) + newOffset);
        }

        for (int j = 0; j < heightDivs; j++)
        {
            Handles.DrawLine(new Vector3(-gridSpacing, gridSpacing * j, 0) + newOffset,
                             new Vector3(position.width, gridSpacing * j, 0f) + newOffset);
        }

        Handles.color = Color.white;
        Handles.EndGUI();
    }

    private void DrawNodes()
    {
        if (nodes != null)
        {
            for (int i = 0; i < nodes.Count; i++)
            {
                nodes[i].Draw();
            }
        }
    }

    private void DrawConnections()
    {
        if (nodes != null)
        {
            for (int i = 0; i < nodes.Count; i++)
            {
                nodes[i].DrawConnections();
            }
        }
    }

    private void DrawConnectionLine(Event e)
    {
        if (selectedInPoint != null && selectedOutPoint == null)
        {
            Handles.DrawBezier(
                selectedInPoint.rect.center,
                e.mousePosition,
                selectedInPoint.rect.center + Vector2.left * 50f,
                e.mousePosition - Vector2.left * 50f,
                Color.white,
                null,
                2f
            );

            GUI.changed = true;
        }

        if (selectedOutPoint != null && selectedInPoint == null)
        {
            Handles.DrawBezier(
                selectedOutPoint.rect.center,
                e.mousePosition,
                selectedOutPoint.rect.center - Vector2.left * 50f,
                e.mousePosition + Vector2.left * 50f,
                Color.white,
                null,
                2f
            );

            GUI.changed = true;
        }
    }

    private void ProcessEvents(Event e)
    {
        drag = Vector2.zero;

        switch (e.type)
        {
            case EventType.MouseDown:
                if (e.button == 1)
                {
                    ProcessContextMenu(e.mousePosition);
                }
                break;

            case EventType.MouseDrag:
                if (e.button == 0)
                {
                    OnDrag(e.delta);
                }
                break;
        }
    }

    private void ProcessNodeEvents(Event e)
    {
        if (nodes != null)
        {
            for (int i = nodes.Count - 1; i >= 0; i--)
            {
                bool guiChanged = nodes[i].ProcessEvents(e);

                if (guiChanged)
                {
                    GUI.changed = true;
                }
            }
        }
    }

    private void ProcessContextMenu(Vector2 mousePosition)
    {
        GenericMenu genericMenu = new GenericMenu();
        genericMenu.AddItem(new GUIContent("Add Action Node"), false, () => OnClickAddNode(mousePosition, NodeType.Action));
        genericMenu.AddItem(new GUIContent("Add Condition Node"), false, () => OnClickAddNode(mousePosition, NodeType.Condition));
        genericMenu.AddItem(new GUIContent("Add Selector Node"), false, () => OnClickAddNode(mousePosition, NodeType.Selector));
        genericMenu.AddItem(new GUIContent("Add Sequence Node"), false, () => OnClickAddNode(mousePosition, NodeType.Sequence));
        genericMenu.ShowAsContext();
    }

    private void OnClickAddNode(Vector2 mousePosition, NodeType nodeType)
    {
        if (nodes == null)
        {
            nodes = new List<Node>();
        }

        switch (nodeType)
        {
            case NodeType.Action:
                nodes.Add(new ActionNode(mousePosition, 200, 100, nodeStyle, selectedNodeStyle, inPointStyle, outPointStyle, OnClickInPoint, OnClickOutPoint, OnClickRemoveNode));
                break;
            case NodeType.Condition:
                nodes.Add(new ConditionNode(mousePosition, 200, 100, nodeStyle, selectedNodeStyle, inPointStyle, outPointStyle, OnClickInPoint, OnClickOutPoint, OnClickRemoveNode));
                break;
            case NodeType.Selector:
                nodes.Add(new SelectorNode(mousePosition, 200, 100, nodeStyle, selectedNodeStyle, inPointStyle, outPointStyle, OnClickInPoint, OnClickOutPoint, OnClickRemoveNode));
                break;
            case NodeType.Sequence:
                nodes.Add(new SequenceNode(mousePosition, 200, 100, nodeStyle, selectedNodeStyle, inPointStyle, outPointStyle, OnClickInPoint, OnClickOutPoint, OnClickRemoveNode));
                break;
        }
    }

    private void OnClickInPoint(ConnectionPoint inPoint)
    {
        selectedInPoint = inPoint;

        if (selectedOutPoint != null)
        {
            if (selectedOutPoint.node != selectedInPoint.node)
            {
                CreateConnection();
                ClearConnectionSelection();
            }
            else
            {
                ClearConnectionSelection();
            }
        }
    }

    private void OnClickOutPoint(ConnectionPoint outPoint)
    {
        selectedOutPoint = outPoint;

        if (selectedInPoint != null)
        {
            if (selectedOutPoint.node != selectedInPoint.node)
            {
                CreateConnection();
                ClearConnectionSelection();
            }
            else
            {
                ClearConnectionSelection();
            }
        }
    }

    private void OnClickRemoveNode(Node node)
    {
        if (nodes.Contains(node))
        {
            nodes.Remove(node);
        }
    }

    private void OnDrag(Vector2 delta)
    {
        drag = delta;

        if (nodes != null)
        {
            for (int i = 0; i < nodes.Count; i++)
            {
                nodes[i].Drag(delta);
            }
        }

        GUI.changed = true;
    }

    private void CreateConnection()
    {
        selectedInPoint.node.AddConnection(selectedInPoint, selectedOutPoint);
    }

    private void ClearConnectionSelection()
    {
        selectedInPoint = null;
        selectedOutPoint = null;
    }
}

public enum NodeType
{
    Action,
    Condition,
    Selector,
    Sequence
}

public class Connection
{
    public Node inNode;
    public Node outNode;
    public ConnectionPoint inPoint;
    public ConnectionPoint outPoint;

    public Connection(Node inNode, Node outNode, ConnectionPoint inPoint, ConnectionPoint outPoint)
    {
        this.inNode = inNode;
        this.outNode = outNode;
        this.inPoint = inPoint;
        this.outPoint = outPoint;
    }

    public void Draw()
    {
        Handles.DrawBezier(
            inPoint.rect.center,
            outPoint.rect.center,
            inPoint.rect.center + Vector2.left * 50f,
            outPoint.rect.center - Vector2.left * 50f,
            Color.white,
            null,
            2f
        );
    }
}

public class ConnectionPoint
{
    public Rect rect;
    public Node node;
    public ConnectionPointType type;
    public GUIStyle style;
    public Action<ConnectionPoint> OnClickConnectionPoint;

    public ConnectionPoint(Node node, ConnectionPointType type, GUIStyle style, Action<ConnectionPoint> OnClickConnectionPoint)
    {
        this.node = node;
        this.type = type;
        this.style = style;
        this.OnClickConnectionPoint = OnClickConnectionPoint;
        rect = new Rect(0, 0, 10f, 20f);
    }

    public void Draw()
    {
        rect.y = node.rect.y + (node.rect.height * 0.5f) - rect.height * 0.5f;

        switch (type)
        {
            case ConnectionPointType.In:
                rect.x = node.rect.x - rect.width + 8f;
                break;

            case ConnectionPointType.Out:
                rect.x = node.rect.x + node.rect.width - 8f;
                break;
        }

        if (GUI.Button(rect, "", style))
        {
            if (OnClickConnectionPoint != null)
            {
                OnClickConnectionPoint(this);
            }
        }
    }
}

public enum ConnectionPointType { In, Out }

public abstract class Node
{
    public Rect rect;
    public string title;
    public bool isDragged;
    public bool isSelected;

    public ConnectionPoint inPoint;
    public ConnectionPoint outPoint;

    protected GUIStyle style;
    protected GUIStyle defaultNodeStyle;
    protected GUIStyle selectedNodeStyle;

    public Action<Node> OnRemoveNode;
    public List<Connection> connections;

    public Node(Vector2 position, float width, float height, GUIStyle nodeStyle, GUIStyle selectedStyle, GUIStyle inPointStyle, GUIStyle outPointStyle, Action<ConnectionPoint> OnClickInPoint, Action<ConnectionPoint> OnClickOutPoint, Action<Node> OnRemoveNode)
    {
        rect = new Rect(position.x, position.y, width, height);
        style = nodeStyle;
        defaultNodeStyle = nodeStyle;
        selectedNodeStyle = selectedStyle;

        inPoint = new ConnectionPoint(this, ConnectionPointType.In, inPointStyle, OnClickInPoint);
        outPoint = new ConnectionPoint(this, ConnectionPointType.Out, outPointStyle, OnClickOutPoint);

        this.OnRemoveNode = OnRemoveNode;
        connections = new List<Connection>();
    }

    public void Drag(Vector2 delta)
    {
        rect.position += delta;
    }

    public void Draw()
    {
        inPoint.Draw();
        outPoint.Draw();
        GUI.Box(rect, title, style);
    }

    public void DrawConnections()
    {
        for (int i = 0; i < connections.Count; i++)
        {
            connections[i].Draw();
        }
    }

    public bool ProcessEvents(Event e)
    {
        switch (e.type)
        {
            case EventType.MouseDown:
                if (e.button == 0)
                {
                    if (rect.Contains(e.mousePosition))
                    {
                        isDragged = true;
                        GUI.changed = true;
                        isSelected = true;
                        style = selectedNodeStyle;
                    }
                    else
                    {
                        GUI.changed = true;
                        isSelected = false;
                        style = defaultNodeStyle;
                    }
                }

                if (e.button == 1 && isSelected && rect.Contains(e.mousePosition))
                {
                    ProcessContextMenu();
                    e.Use();
                }
                break;

            case EventType.MouseUp:
                isDragged = false;
                break;

            case EventType.MouseDrag:
                if (e.button == 0 && isDragged)
                {
                    Drag(e.delta);
                    e.Use();
                    return true;
                }
                break;
        }

        return false;
    }

    private void ProcessContextMenu()
    {
        GenericMenu genericMenu = new GenericMenu();
        genericMenu.AddItem(new GUIContent("Remove Node"), false, OnClickRemoveNode);
        genericMenu.ShowAsContext();
    }

    private void OnClickRemoveNode()
    {
        if (OnRemoveNode != null)
        {
            OnRemoveNode(this);
        }
    }

    public void AddConnection(ConnectionPoint inPoint, ConnectionPoint outPoint)
    {
        Connection connection = new Connection(inPoint.node, outPoint.node, inPoint, outPoint);
        inPoint.node.connections.Add(connection);
        outPoint.node.connections.Add(connection);
    }

    public abstract bool Execute();
    public abstract void DrawDetails();
}

public class ActionNode : Node
{
    private Action action;

    public ActionNode(Vector2 position, float width, float height, GUIStyle nodeStyle, GUIStyle selectedStyle, GUIStyle inPointStyle, GUIStyle outPointStyle, Action<ConnectionPoint> OnClickInPoint, Action<ConnectionPoint> OnClickOutPoint, Action<Node> OnRemoveNode)
        : base(position, width, height, nodeStyle, selectedStyle, inPointStyle, outPointStyle, OnClickInPoint, OnClickOutPoint, OnRemoveNode)
    {
        title = "Action Node";
        action = () => Debug.Log("Action executed");
    }

    public override bool Execute()
    {
        action();
        return true;
    }

    public override void DrawDetails()
    {
        EditorGUILayout.LabelField("Action Node Details", EditorStyles.boldLabel);
        if (GUILayout.Button("Change Action"))
        {
            action = () => Debug.Log("New action executed");
        }
    }
}

public class ConditionNode : Node
{
    private Func<bool> condition;

    public ConditionNode(Vector2 position, float width, float height, GUIStyle nodeStyle, GUIStyle selectedStyle, GUIStyle inPointStyle, GUIStyle outPointStyle, Action<ConnectionPoint> OnClickInPoint, Action<ConnectionPoint> OnClickOutPoint, Action<Node> OnRemoveNode)
        : base(position, width, height, nodeStyle, selectedStyle, inPointStyle, outPointStyle, OnClickInPoint, OnClickOutPoint, OnRemoveNode)
    {
        title = "Condition Node";
        condition = () => UnityEngine.Random.value > 0.5f;
    }

    public override bool Execute()
    {
        return condition();
    }

    public override void DrawDetails()
    {
        EditorGUILayout.LabelField("Condition Node Details", EditorStyles.boldLabel);
        if (GUILayout.Button("Change Condition"))
        {
            condition = () => UnityEngine.Random.value > 0.75f;
        }
    }
}

public class SelectorNode : Node
{
    public SelectorNode(Vector2 position, float width, float height, GUIStyle nodeStyle, GUIStyle selectedStyle, GUIStyle inPointStyle, GUIStyle outPointStyle, Action<ConnectionPoint> OnClickInPoint, Action<ConnectionPoint> OnClickOutPoint, Action<Node> OnRemoveNode)
        : base(position, width, height, nodeStyle, selectedStyle, inPointStyle, outPointStyle, OnClickInPoint, OnClickOutPoint, OnRemoveNode)
    {
        title = "Selector Node";
    }

    public override bool Execute()
    {
        foreach (var connection in connections)
        {
            if (connection.outNode.Execute())
            {
                return true;
            }
        }
        return false;
    }

    public override void DrawDetails()
    {
        EditorGUILayout.LabelField("Selector Node Details", EditorStyles.boldLabel);
    }
}

public class SequenceNode : Node
{
    public SequenceNode(Vector2 position, float width, float height, GUIStyle nodeStyle, GUIStyle selectedStyle, GUIStyle inPointStyle, GUIStyle outPointStyle, Action<ConnectionPoint> OnClickInPoint, Action<ConnectionPoint> OnClickOutPoint, Action<Node> OnRemoveNode)
        : base(position, width, height, nodeStyle, selectedStyle, inPointStyle, outPointStyle, OnClickInPoint, OnClickOutPoint, OnRemoveNode)
    {
        title = "Sequence Node";
    }

    public override bool Execute()
    {
        foreach (var connection in connections)
        {
            if (!connection.outNode.Execute())
            {
                return false;
            }
        }
        return true;
    }

    public override void DrawDetails()
    {
        EditorGUILayout.LabelField("Sequence Node Details", EditorStyles.boldLabel);
    }
}

public class BehaviorTree
{
    public Node rootNode;

    public BehaviorTree(Node rootNode)
    {
        this.rootNode = rootNode;
    }

    public void Tick()
    {
        rootNode.Execute();
    }
}

public class AIController : MonoBehaviour
{
    private BehaviorTree behaviorTree;

    void Start()
    {
        var actionNode = new ActionNode(new Vector2(0, 0), 200, 50, null, null, null, null, null, null, null);
        behaviorTree = new BehaviorTree(actionNode);
    }

    void Update()
    {
        behaviorTree.Tick();
    }
}
