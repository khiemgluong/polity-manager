using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace KL
{
    public class PolityFamilyGraph : EditorWindow
    {
        /* ---------------------------- POLITY VARIABLES ---------------------------- */
        enum CurveAnchor
        {
            Top,
            Right,
            Left,
            Bottom,
            Child
        }
        enum RelationType
        {
            Root,
            Parent,
            Partner,
            Children,
        }
        List<PolityMember> polityMembers = new();

        /* ----------------------------- NODE RENDERERS ----------------------------- */
        struct Node
        {
            public Rect Rect;
            public RelationType Relation;
            public Node(Rect rect, RelationType relationType)
            {
                Rect = rect;
                Relation = relationType;
            }
        }
        class Link
        {
            public Node Node;
            public int Index;
            public CurveAnchor Anchor;
            public Link(Node node, int index, CurveAnchor anchorPoint)
            {
                Node = node;
                Index = index;
                Anchor = anchorPoint;
            }
            public void ChangeIndex(int index) => Index = index;

        }
        SerializedObject serializedObject;
        GUIStyle parentNode, partnerNode, childNode;
        List<Node> nodes = new();
        Dictionary<Link, Link> links = new();
        readonly Vector2 nodeSize = new(140, 65);
        bool isRootGenerated;

        /* ------------------------------ PAN CONTROLS ------------------------------ */
        float panX = 0, panY = 0;
        bool isDragging = false;
        Vector2 initialMousePosition;
        Vector2 dragStartPosition;

        void OnEnable()
        {
            serializedObject = new SerializedObject(this);

            parentNode = new GUIStyle(GUI.skin.window);
            parentNode.normal.background = EditorGUIUtility.Load("builtin skins/darkskin/images/node6.png") as Texture2D;
            partnerNode = new GUIStyle(GUI.skin.window);
            partnerNode.normal.background = EditorGUIUtility.Load("builtin skins/darkskin/images/node3.png") as Texture2D;
            childNode = new GUIStyle(GUI.skin.window);
            childNode.normal.background = EditorGUIUtility.Load("builtin skins/darkskin/images/node1.png") as Texture2D;

            // nodes.Clear();
            nodes.Clear();
            polityMembers.Clear();
            // linkedNodes.Clear();

            // Calculate the center of the groupRect
            Vector2 windowCenter = new(position.width / 2, position.height / 2);
            Rect rootNodeRect = new(windowCenter.x - nodeSize.x / 2,
                                    (windowCenter.y / 2) + nodeSize.y / 2, nodeSize.x, 110);
            nodes.Add(new Node(rootNodeRect, RelationType.Root));
        }

        public static void ShowWindow()
        {
            var window = GetWindow<PolityFamilyGraph>("Family Graph");
            window.minSize = new Vector2(400, 400);
            var screenResolution = new Vector2(Screen.currentResolution.width, Screen.currentResolution.height);
            var windowSize = screenResolution * .15f;
            var windowPosition = (screenResolution - windowSize) * .15f;
            window.position = new Rect(windowPosition.x, windowPosition.y, windowSize.x * 3, windowSize.y);
            window.Show();
        }
        #region  OnGUI
        void OnGUI()
        {
            /* -------------------------------------------------------------------------- */
            /*                                  SIDEBAR                                   */
            /* -------------------------------------------------------------------------- */
            float topBarHeight = position.height * .1f;
            float topBarWidth = position.width;
            GUILayout.BeginArea(new Rect(0, 0, topBarWidth, topBarHeight), "", GUI.skin.window);
            EditorGUILayout.BeginHorizontal();
            // if (GUILayout.Button("Add Node", GUILayout.ExpandWidth(false)))
            // {
            //     if (polityMembers[0] != null) nodes.Add(NewNode(nodes[0].x, nodes[0].y - 100));
            //     else Debug.LogWarning("You must assign a root PolityMember first");
            // }
            EditorGUILayout.EndHorizontal();
            GUILayout.EndArea();
            /* ------------------------------- SIDEBAR END ------------------------------ */


            /* -------------------------------------------------------------------------- */
            /*                                 MAIN PANEL                                 */
            /* -------------------------------------------------------------------------- */
            float mainPanelWidth = position.width;
            float mainPanelHeight = position.height - topBarHeight;
            GUILayout.BeginArea(new Rect(0, topBarHeight, mainPanelWidth, mainPanelHeight), "", GUI.skin.window);
            Rect groupRect = new(panX, panY, 5000, 5000);
            GUI.BeginGroup(groupRect);
            BeginWindows();

            foreach (var pair in links)
                DrawNodeCurve(pair.Key, pair.Value, pair.Key.Anchor);
            int index = 0;
            foreach (var node in nodes)
            {
                Rect nodeRect = node.Rect;
                switch (node.Relation)
                {
                    case RelationType.Root:
                        nodeRect = GUI.Window(index, nodeRect, DrawNode, "Root");
                        break;
                    case RelationType.Parent:
                        nodeRect = GUI.Window(index, nodeRect, DrawNode, "Parent " + index, parentNode);
                        break;
                    case RelationType.Partner:
                        nodeRect = GUI.Window(index, nodeRect, DrawNode, "Partner " + index, partnerNode);
                        break;
                    case RelationType.Children:
                        nodeRect = GUI.Window(index, nodeRect, DrawNode, "Child " + index, childNode);
                        break;
                }
                index++;
            }
            EndWindows();
            GUI.EndGroup();
            GUILayout.EndArea();
            /* ----------------------------- MAIN PANEL END ----------------------------- */

            /* ------------------------------ PAN CONTROLS ------------------------------ */
            if (Event.current.type == EventType.MouseDown && groupRect.Contains(Event.current.mousePosition))
            {
                isDragging = true;
                initialMousePosition = Event.current.mousePosition;
                dragStartPosition = new Vector2(panX, panY);
                Event.current.Use();  // Consume the event so no other GUI elements use it
            }

            if (isDragging)
            {
                if (Event.current.type == EventType.MouseDrag)
                {
                    Vector2 currentMousePosition = Event.current.mousePosition;
                    Vector2 delta = currentMousePosition - initialMousePosition;

                    panX = dragStartPosition.x + delta.x;
                    panY = dragStartPosition.y + delta.y;
                    Repaint();
                }
                else if (Event.current.type == EventType.MouseUp)
                {
                    isDragging = false;
                }
            }
        }
        #endregion

        #region Init
        void InitializeNodes()
        {
            if (polityMembers[0] == null) return;
            PolityMember root = polityMembers[0];
            root.family.parents = root.family.parents.Where(item => item != null).ToList();
            root.family.partners = root.family.partners.Where(item => item != null).ToList();

            // root.family.partners = root.family.partners.Where(item => item.partner != null).ToList();
            // Clean up children for each partner
            for (int i = 0; i < root.family.partners.Count; i++)
            {
                var partner = root.family.partners[i];
                // partner.children = partner.children.Where(item => item != null).ToArray();
                root.family.partners[i] = partner;
            }


            Rect rootRect = nodes[0].Rect;
            float currentXOffset = 0;
            /* ------------------------- Initialize Parent Nodes ------------------------ */
            for (int i = 0; i < root.family.parents.Count; i++)
            {
                polityMembers.Add(root.family.parents[i]);
                AddParentNode();
            }
            /* ------------------------ Initialize Partner Nodes ------------------------ */
            for (int i = 0; i < root.family.partners.Count; i++)
            {
                polityMembers.Add(root.family.partners[i]);
                AddPartnerNode();
            }
            // for (int i = 0; i < root.children.Count; i++)
            // {
            //     polityMembers.Add(root.children[i]);
            //     if (i == 0)
            //         nodes.Add(new Rect(rootNode.x + currentXOffset * 2f, rootNode.y + nodeSize.y * 2f, nodeSize.x, nodeSize.y));
            //     else
            //     {
            //         currentXOffset += nodeSize.x * 2f;
            //         nodes.Add(new Rect(rootNode.x + currentXOffset, rootNode.y + nodeSize.y * 2f, nodeSize.x, nodeSize.y));
            //     }
            //     relationType = RelationType.Children;

            //     int _i = polityMembers.IndexOf(root.children[i]);
            //     AttachCurveToRootNode(_i);
            //     for (int x = 0; x < partnersIds.Count; x++)
            //         if (polityMembers[partnersIds[x]].children.Contains(polityMembers[_i]))
            //         {
            //             childNodeId = _i;
            //             AttachCurveToParentNode(partnersIds[x]);
            //         }
            // }
            isRootGenerated = true;
        }
        #endregion

        #region Nodes
        Node NewNode(float x, float y, RelationType type)
        {
            Node node = new(new Rect(x, y, nodeSize.x, nodeSize.y), type);
            nodes.Add(node);
            return node;
        }
        void MoveNode(Node node, float x, float y)
        {
            int nodeIndex = nodes.IndexOf(node);
            if (nodeIndex < 0 || nodeIndex >= nodes.Count)
            {
                Debug.LogWarning("Invalid node index " + nodeIndex + " count " + nodes.Count);
                return;
            }
            Node oldNode = nodes[nodeIndex];
            Rect newRect = new(x, y, oldNode.Rect.width, oldNode.Rect.height);
            Node newNode = new(newRect, oldNode.Relation);
            nodes[nodeIndex] = new(newRect, oldNode.Relation);

            // Transfer the polityMembers reference if it exists
            if (nodeIndex < polityMembers.Count && polityMembers[nodeIndex] != null)
                polityMembers[nodeIndex] = polityMembers[nodeIndex];
            MoveLink(nodeIndex, newNode);
        }

        void RemoveNode(Node node)
        {
            polityMembers.RemoveAt(nodes.IndexOf(node));
            nodes.Remove(node);
        }
        #endregion

        #region Parent

        void AddParentNode()
        {
            int parentCount = 0;
            foreach (var node in nodes)
                if (node.Relation == RelationType.Parent)
                    parentCount++;
            Debug.LogError("parent node count " + parentCount);
            switch (parentCount)
            {
                case 0:
                    Node node = NewNode(nodes[0].Rect.x, nodes[0].Rect.y - nodeSize.y * 2f, RelationType.Parent);
                    CreateLink(node, nodes[0], CurveAnchor.Bottom);
                    break;
                case 1:
                    Node node1 = NewNode(nodes[0].Rect.x + nodeSize.x * 1.5f,
                             nodes[0].Rect.y - nodeSize.y * 2f, RelationType.Parent);
                    CreateLink(node1, nodes[0], CurveAnchor.Bottom);
                    break;
            }
        }
        #endregion

        #region Partner
        void AddPartnerNode()
        {
            byte partnerCount = 0;
            foreach (var node in nodes)
                if (node.Relation == RelationType.Partner)
                    partnerCount++;
            float xPos = nodes[0].Rect.x + nodeSize.x * (1.5f + partnerCount * 1.5f);
            float yPos = nodes[0].Rect.y + 45 / 2;
            Node partnerNode = NewNode(xPos, yPos, RelationType.Partner);
            CreateLink(partnerNode, nodes[0], CurveAnchor.Left);
        }
        #endregion

        #region Children
        void AddChildrenNode(Node parentNode)
        {
            byte linkCount = 0;
            foreach (var pair in links)
                if (pair.Value.Node.Equals(parentNode))
                    if (pair.Key.Node.Relation == RelationType.Children)
                        linkCount++;

            float xPos = parentNode.Rect.x;
            float yPos = nodes[0].Rect.y + nodeSize.y * (2f + linkCount * 2f);
            Node childNode = NewNode(xPos, yPos, RelationType.Children);
            CreateLink(childNode, parentNode, CurveAnchor.Top);
        }
        #endregion
        #region  DrawNode
        void DrawNode(int index)
        {
            while (polityMembers.Count <= index) polityMembers.Add(null);
            EditorGUI.BeginChangeCheck();
            polityMembers[index] = (PolityMember)EditorGUILayout.ObjectField(label: "",
                                                                obj: polityMembers[index],
                                                            objType: typeof(PolityMember),
                                                                allowSceneObjects: false);
            if (EditorGUI.EndChangeCheck())
            {
                Debug.Log($"PolityMember {polityMembers[index].name} has been referenced.");
                LinkFamily(index);
                serializedObject.ApplyModifiedProperties();
            }
            if (index == 0)
            {
                if (polityMembers[0] != null && PrefabUtility.IsPartOfPrefabAsset(polityMembers[0]))
                    if (!CheckForDuplicateNode(index))
                    {
                        List<Node> parentNodes = new();
                        foreach (var node in nodes)
                            if (node.Relation == RelationType.Parent)
                                parentNodes.Add(node);
                        if (parentNodes.Count < 2)
                        {
                            if (GUILayout.Button(RelationType.Parent.ToString()))
                                AddParentNode();
                        }
                        if (GUILayout.Button(RelationType.Partner.ToString()))
                            AddPartnerNode();
                        if (GUILayout.Button(RelationType.Children.ToString()))
                            AddChildrenNode(nodes[0]);
                        if (!isRootGenerated) InitializeNodes();
                    }
            }
            else
            {
                // if (!CheckForDuplicateNode(id))
                EditorGUILayout.BeginHorizontal();
                if (nodes[index].Relation == RelationType.Partner)
                {
                    if (GUILayout.Button("Add Child", GUILayout.ExpandWidth(true)))
                        AddChildrenNode(nodes[index]);
                }
                if (GUILayout.Button("Remove", GUILayout.ExpandWidth(true)))
                {
                    RelationType deletedNodeRelation = nodes[index].Relation;
                    List<Node> nodesToMove = new();

                    DeleteLink(index);
                    List<Link> sortedLinks = new();

                    foreach (var pair in links)
                    {
                        sortedLinks.Add(pair.Key);
                        Debug.Log("Added link " + pair.Key);
                    }

                    for (int i = 0; i < sortedLinks.Count; i++)
                    {
                        if (sortedLinks[i].Index > index)
                        {
                            sortedLinks[i].ChangeIndex(nodes.IndexOf(sortedLinks[i].Node));
                            nodesToMove.Add(sortedLinks[i].Node);
                            Debug.Log("Updated link index " + sortedLinks[i].Index);
                        }
                    }
                    foreach (var node in nodesToMove)
                        if (node.Relation == deletedNodeRelation)
                        {
                            if (node.Relation == RelationType.Partner)
                            {
                                float newX = node.Rect.x - nodeSize.x * 1.5f;
                                MoveNode(node, newX, node.Rect.y);
                            }
                            else if (node.Relation == RelationType.Children)
                            {
                                float newY = node.Rect.y - nodeSize.y * 2f;
                                MoveNode(node, node.Rect.x, newY);
                            }
                        }

                }
                EditorGUILayout.EndHorizontal();
            }
            // GUI.DragWindow();
        }
        #endregion

        #region  Links
        void CreateLink(Node startNode, Node endNode, CurveAnchor anchor)
        {
            CurveAnchor endAnchor = GetOppositeAnchor(anchor);
            int startIndex = nodes.IndexOf(startNode);
            int endIndex = nodes.IndexOf(endNode);
            links.Add(new(startNode, startIndex, anchor), new(endNode, endIndex, endAnchor));
            Debug.Log("start Index " + startIndex + " end Index " + endIndex);
        }

        void MoveLink(int nodeIndex, Node newNode)
        {
            List<Link> keysToUpdate = new();
            foreach (var pair in links)
                if (pair.Key.Index == nodeIndex)
                    keysToUpdate.Add(pair.Key);
            Debug.Log("Keys to update " + keysToUpdate.Count);
            foreach (var key in keysToUpdate)
            {
                Link value = links[key];
                links.Remove(key);
                links.Add(new Link(newNode, key.Index, key.Anchor), value);
            }
        }

        void LinkFamily(int id)
        {
            PolityMember member = polityMembers[id];
            PolityMember valueMember = null;
            // if (polityMembers[id] != null)
            // {
            //     Debug.LogError("PolityMember " + key.name + " is null");
            //     return;
            // }
            foreach (var pair in links)
            {
                if (pair.Key.Index == id)
                {
                    valueMember = polityMembers[pair.Value.Index];
                    break;
                }
            }
            if (valueMember != null)
            {
                Debug.Log("polity Member and Key " + member + " " + valueMember);
                switch (nodes[id].Relation)
                {
                    case RelationType.Parent:
                        if (!valueMember.family.parents.Contains(member))
                            valueMember.family.parents.Add(member);
                        if (!member.family.children.Contains(valueMember))
                            member.family.children.Add(valueMember);
                        break;
                    case RelationType.Partner:
                        if (!valueMember.family.partners.Contains(member))
                            valueMember.family.partners.Add(member);
                        if (!member.family.partners.Contains(valueMember))
                            member.family.partners.Add(valueMember);
                        break;
                }
            }

        }
        void DeleteLink(int index, bool deleteFamily = true)
        {
            Link linkToRemoveKey = null;
            Link linkToRemoveValue = null;
            foreach (var pair in links)
                if (pair.Key.Index == index)
                {
                    linkToRemoveKey = pair.Key;
                    linkToRemoveValue = pair.Value;
                    break;
                }
            Debug.Log("Deleting link with ID " + index);
            if (linkToRemoveKey == null || linkToRemoveValue == null)
            {
                Debug.LogError("Link to remove is null");
                return;
            }
            Debug.Log("link key " + linkToRemoveKey.Index
            + " link value " + linkToRemoveValue.Index);

            if (deleteFamily)
            {
                int keyIndex = linkToRemoveKey.Index;
                PolityMember key = polityMembers[keyIndex];
                int valueIndex = linkToRemoveValue.Index;
                PolityMember value = polityMembers[valueIndex];
                if (key != null && value != null)
                {
                    if (key.family.parents.Contains(value))
                        key.family.parents.Remove(value);
                    if (value.family.parents.Contains(key))
                        value.family.parents.Remove(key);
                }

            }
            links.Remove(linkToRemoveKey);
            RemoveNode(linkToRemoveKey.Node);
            // foreach (var key in keysToRemove)
            //     linkedNodes.Remove(key);
            // if (keysToRemove.Count > 0)
            // Debug.Log("Removed " + keysToRemove.Count + " connections with ID " + id);
        }
        #endregion



        bool CheckForDuplicateNode(int id)
        {
            bool isDuplicate = false;
            int i; for (i = 0; i < polityMembers.Count; i++)
                if (i != id && polityMembers[i] == polityMembers[id])
                { isDuplicate = true; break; }

            if (isDuplicate)
            {
                EditorUtility.DisplayDialog(
                               "Duplicate PolityMember Detected",
                               $"This PolityMember has already been assigned to node {i}.",
                               "OK"
                           );
                polityMembers[id] = null;
            }
            return isDuplicate;
        }

        #region Curves
        /* -------------------------------------------------------------------------- */
        /*                            Bezier Curve Drawers                            */
        /* -------------------------------------------------------------------------- */
        CurveAnchor GetOppositeAnchor(CurveAnchor anchor)
            => anchor switch
            {
                CurveAnchor.Top => CurveAnchor.Bottom,
                CurveAnchor.Right => CurveAnchor.Left,
                CurveAnchor.Left => CurveAnchor.Right,
                CurveAnchor.Bottom => CurveAnchor.Top,
                _ => CurveAnchor.Child,
            };

        Vector2 GetNodeAnchorVector(CurveAnchor point)
            => point switch
            {
                CurveAnchor.Top => new(0.5f, 0f),
                CurveAnchor.Right => new(1.0f, 0.5f),
                CurveAnchor.Left => new(0.0f, 0.5f),
                CurveAnchor.Bottom => new(0.5f, 1f),
                CurveAnchor.Child => new(0.5f, 1f),
                _ => new(0.5f, 0.5f),
            };

        void DrawNodeCurve(Link start, Link end, CurveAnchor startAnchor)
        {
            CurveAnchor endAnchor = GetOppositeAnchor(startAnchor);
            Vector2 startPos = GetNodeAnchorVector(startAnchor);
            Vector2 endPos = GetNodeAnchorVector(endAnchor);
            Color lineColor = GetAnchorLineColor(startAnchor, start.Index);
            DrawNodeCurve(start.Node.Rect, end.Node.Rect, startPos, endPos, lineColor);
        }

        Color GetAnchorLineColor(CurveAnchor startAnchor, int endIndex)
        {
            if (endIndex < 0 || endIndex >= polityMembers.Count || polityMembers[endIndex] == null)
                return Color.white;
            return startAnchor switch
            {
                CurveAnchor.Top => Color.blue,
                CurveAnchor.Right => Color.green,
                CurveAnchor.Left => Color.green,
                CurveAnchor.Bottom => Color.red,
                CurveAnchor.Child => Color.cyan,
                _ => Color.black,// Default to center if unknown for some reason
            };
        }
        void DrawNodeCurve(Rect start, Rect end, Vector2 startVector, Vector2 endVector, Color lineColor)
        {
            Vector3 startPos = new(start.x + start.width * startVector.x, start.y + start.height * startVector.y, 0);
            Vector3 endPos = new(end.x + end.width * endVector.x, end.y + end.height * endVector.y, 0);
            Vector3 startTan = startPos + Vector3.right * (-50 + 100 * startVector.x) + Vector3.up * (-50 + 100 * startVector.y);
            Vector3 endTan = endPos + Vector3.right * (-50 + 100 * endVector.x) + Vector3.up * (-50 + 100 * endVector.y);
            // Color shadowCol = new(200, 200, 200, .25f);
            // for (int i = 0; i < 3; i++) // Draw a shadow
            //     Handles.DrawBezier(startPos, endPos, startTan, endTan, shadowCol, null, (i + 1) * 5);
            Handles.DrawBezier(startPos, endPos, startTan, endTan, lineColor, null, 2);
        }
    }
    #endregion
}