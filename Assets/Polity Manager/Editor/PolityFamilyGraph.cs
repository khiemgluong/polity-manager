using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace KL
{
    public class PolityFamilyGraph : EditorWindow
    {
        /* ---------------------------- POLITY VARIABLES ---------------------------- */
        enum NodeAnchor
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
        RelationType relationType;
        List<PolityMember> polityMembers = new();

        /* ----------------------------- NODE RENDERERS ----------------------------- */
        struct Node
        {
            public Rect Rect;
            // public int NodeId;
            public NodeAnchor Anchor;
            public Node(Rect rect, NodeAnchor anchorPoint)
            {
                Rect = rect;
                // NodeId = nodeId;
                Anchor = anchorPoint;
            }
        }
        struct Link
        {
            public Rect Rect;
            public RelationType Relation;
            // public int NodeId;
            public NodeAnchor Anchor;
            public Link(Rect rect, RelationType relation, NodeAnchor anchorPoint)
            {
                Rect = rect;
                Relation = relation;
                // NodeId = nodeId;
                Anchor = anchorPoint;
            }
            // public Curve(Rect rect, int nodeId, NodeAnchor anchorPoint)
            // {
            //     Rect = rect;
            //     NodeId = nodeId;
            //     Anchor = anchorPoint;
            // }
        }
        SerializedObject serializedObject;
        GUIStyle parentNode, partnerNode, childNode;
        List<Node> nodes2 = new();
        Dictionary<Rect, RelationType> nodes = new();
        Dictionary<Node, Node> links = new();
        Vector2 nodeSize = new(140, 65);
        /// <summary>
        /// This nodeId is referenced only in a node which is a child of the root node
        /// </summary>
        int childNodeId = -1;
        bool isRootGenerated;
        // Dictionary<Curve, Curve> linkedNodes = new(), linkedChildNodes = new();
        Dictionary<int, RelationType> linkedRelationType = new();

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

            nodes.Clear();
            polityMembers.Clear();
            // linkedNodes.Clear();
            linkedRelationType.Clear();

            // Calculate the center of the groupRect
            Vector2 windowCenter = new(position.width / 2, position.height / 2);
            Rect rootNodeRect = new(windowCenter.x + nodeSize.x,
                                    (windowCenter.y / 2) + nodeSize.y / 2, nodeSize.x, 105);
            nodes.Add(rootNodeRect, RelationType.Root);
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

            // foreach (var pair in linkedNodes)
            //     DrawNodeCurve(nodeRects.ElementAt(pair.Key.NodeId).Key, nodeRects.ElementAt(pair.Value.NodeId).Key, pair.Key.Point, pair.Value.Point);
            // foreach (var pair in linkedChildNodes)
            //     DrawNodeCurve(nodeRects.ElementAt(pair.Value.NodeId).Key, nodeRects.ElementAt(pair.Key.NodeId).Key, pair.Value.Point, pair.Key.Point);
            int index = 0;
            foreach (var node in nodes)
            {
                Rect nodeRect = node.Key;
                if (node.Value == RelationType.Root)//Root node
                {
                    if (polityMembers.Any() && polityMembers[0] != null)
                    {
                        if (polityMembers[0].family.parents.Count < 2)
                            nodeRect = new Rect(node.Key.x, node.Key.y, nodeSize.x, 115);
                        else nodeRect = new Rect(node.Key.x, node.Key.y, nodeSize.x, 90);
                    }
                    nodeRect = GUI.Window(index, node.Key, DrawNode, "Root " + index);
                }
                else
                {
                    switch (node.Value)
                    {
                        case RelationType.Parent:
                            DrawNodeCurve(nodes.ElementAt(0).Key, node.Key, NodeAnchor.Top, NodeAnchor.Bottom);
                            nodeRect = GUI.Window(index, node.Key, DrawNode, "Parent " + index, parentNode);
                            break;
                        case RelationType.Partner:
                            nodeRect = GUI.Window(index, node.Key, DrawNode, "Partner " + index, partnerNode);
                            break;
                        case RelationType.Children:
                            if (polityMembers[index] != null)
                            {
                                if (polityMembers[index].family.parents.Count == 1)
                                    nodeRect = new Rect(node.Key.x, node.Key.y, nodeSize.x, 90);
                                else nodeRect = new Rect(node.Key.x, node.Key.y, nodeSize.x, 65);
                            }
                            nodeRect = GUI.Window(index, node.Key, DrawNode, "Child " + index, childNode);
                            break;
                    }
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
        void LinkNodes(Rect rect, NodeAnchor nodeAnchor)
        {
            // links.Add(nodes.ElementAt(0).Key, new(rect, nodeAnchor));
        }
        void NewNode(float x, float y, RelationType type)
        {
            nodes.Add(new Rect(x, y, nodeSize.x, nodeSize.y), type);
        }
        Rect MoveNode(Rect node, float x, float y)
        {
            return new Rect(x, y, node.width, node.height);
        }
        void AddParentNode()
        {
            List<Rect> parentNodes = new();
            foreach (var node in nodes)
            {
                if (node.Value == RelationType.Parent)
                    parentNodes.Add(node.Key);
            }

            if (polityMembers[0].family.parents.Count < 2 &&
                parentNodes.Count < 2)
            {
                relationType = RelationType.Parent;
                switch (parentNodes.Count)
                {
                    case 0:
                        NewNode(nodes.ElementAt(0).Key.x, nodes.ElementAt(0).Key.y - nodeSize.y * 2f, RelationType.Parent);
                        break;
                    case 1:
                        Debug.Log("Parent node b4 " + parentNodes[0].position);
                        Rect movedNode = MoveNode(parentNodes[0], nodes.ElementAt(0).Key.x - nodeSize.x,
                                                                nodes.ElementAt(0).Key.y - nodeSize.y * 2f);
                        Debug.Log("Parent node moved to " + parentNodes[0].position);
                        nodes.Remove(parentNodes[0]);
                        nodes.Add(movedNode, RelationType.Parent);
                        NewNode(nodes.ElementAt(0).Key.x + nodeSize.x,
                                nodes.ElementAt(0).Key.y - nodeSize.y * 2f, RelationType.Parent);
                        break;
                }
            }
        }
        void DrawNode(int id)
        {
            while (polityMembers.Count <= id) polityMembers.Add(null);
            EditorGUI.BeginChangeCheck();
            polityMembers[id] = EditorGUILayout.ObjectField("", polityMembers[id], typeof(PolityMember), false) as PolityMember;
            if (EditorGUI.EndChangeCheck()) serializedObject.ApplyModifiedProperties();
            if (id == 0)
            {
                if (polityMembers[0] != null && PrefabUtility.IsPartOfPrefabAsset(polityMembers[0]))
                    if (!CheckForDuplicateNode(id))
                    {
                        if (GUILayout.Button(RelationType.Parent.ToString()))
                            AddParentNode();
                        if (GUILayout.Button(RelationType.Partner.ToString()))
                            relationType = RelationType.Partner;
                        if (GUILayout.Button(RelationType.Children.ToString()))
                            relationType = RelationType.Children;
                        if (!isRootGenerated) DrawRootNode();
                    }
            }
            else
            {
                if (polityMembers[id] != null && PrefabUtility.IsPartOfPrefabAsset(polityMembers[id]))
                {
                    if (!CheckForDuplicateNode(id))
                    {
                        SetRootNodeRelationTypes(id);
                        EditorGUILayout.BeginHorizontal();
                        if (linkedRelationType.ContainsKey(id))
                        {
                            if (linkedRelationType[id] == RelationType.Partner)
                            {
                                if (GUILayout.Button("Attach", GUILayout.ExpandWidth(true)))
                                    EstablishNodeConnection(id);
                                if (GUILayout.Button("X", GUILayout.ExpandWidth(false)))
                                { ClearRootNodeRelations(id); DeleteCurveToRootNode(id); }
                            }
                            else
                            {
                                if (GUILayout.Button("Detach", GUILayout.ExpandWidth(true)))
                                {
                                    DeleteCurveToRootNode(id);
                                    if (linkedRelationType[id] == RelationType.Children)
                                    {
                                        PolityMember root = polityMembers[0];
                                        for (int i = 0; i < root.family.partners.Count; i++)
                                            for (int x = 0; x < root.family.partners[i].children.Length; x++)
                                                if (root.family.partners[i].children[x] == polityMembers[id])
                                                {
                                                    childNodeId = polityMembers.IndexOf(root.family.partners[i].partner);
                                                    DeleteCurveToParentNode(id);
                                                    ClearChildNodeRelation(id);
                                                    break;
                                                }
                                    }
                                    ClearRootNodeRelations(id);
                                }
                            }
                        }
                        else
                        {
                            if (GUILayout.Button("Attach", GUILayout.ExpandWidth(true)))
                                EstablishNodeConnection(id);
                        }
                        EditorGUILayout.EndHorizontal();
                        if (polityMembers[id] != null && nodes.ElementAt(id).Key != null)
                            if (polityMembers[id].family.parents.Contains(polityMembers[0]) && polityMembers[id].family.parents.Count < 2)
                                if (GUILayout.Button("Parent")) childNodeId = id;
                    }
                }
            }
            // GUI.DragWindow();
        }

        void EstablishNodeConnection(int id)
        {
            if (childNodeId == -1) { AttachCurveToRootNode(id); SetRootNodeRelationTypes(id); }
            else
            { AttachCurveToParentNode(id); SetNodeRelationTypes(childNodeId, id); childNodeId = -1; }
        }

        /* -------------------------------------------------------------------------- */
        /*                             NODE INITIALIZATION                            */
        /* -------------------------------------------------------------------------- */
        void DrawRootNode()
        {
            if (polityMembers[0] == null) return;
            PolityMember root = polityMembers[0];
            root.family.parents = root.family.parents.Where(item => item != null).ToList();
            root.family.partners = root.family.partners.Where(item => item.partner != null).ToList();
            // Clean up children for each partner
            for (int i = 0; i < root.family.partners.Count; i++)
            {
                var partner = root.family.partners[i];
                partner.children = partner.children.Where(item => item != null).ToArray();
                root.family.partners[i] = partner;
            }


            Rect rootNode = nodes.ElementAt(0).Key;
            float currentXOffset;
            #region Parents
            /* -------------------------- Building Parent Nodes ------------------------- */
            for (int i = 0; i < root.family.parents.Count; i++)
            {
                currentXOffset = -nodeSize.x;
                polityMembers.Add(root.family.parents[i]);
                if (i != 0) currentXOffset += nodeSize.x * 2f;
                NewNode(rootNode.x + currentXOffset, rootNode.y - nodeSize.y * 2f, RelationType.Parent);
                relationType = RelationType.Parent;
                AttachCurveToRootNode(i + 1);
            }
            currentXOffset = nodeSize.x * 2f;
            #endregion

            #region Partners
            /* ------------------------- Building Partner Nodes ------------------------- */
            List<int> partnersIds = new();
            for (int i = 0; i < root.family.partners.Count; i++)
            {
                polityMembers.Add(root.family.partners[i].partner);
                if (i != 0) currentXOffset += nodeSize.x * 2f;
                // nodes.Add(NewNode(rootNode.x + currentXOffset, rootNode.y * 1.05f), RelationType.Partners);
                NewNode(rootNode.x + currentXOffset, rootNode.y * 1.05f, RelationType.Partner);
                relationType = RelationType.Partner;
                AttachCurveToRootNode(polityMembers.Count - 1);
                partnersIds.Add(polityMembers.Count - 1);
            }
            currentXOffset = nodeSize.x / 2;
            #endregion
            /* ------------------------- Building Children Nodes ------------------------ */
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

        /* -------------------------------------------------------------------------- */
        /*                              NODE CONNECTIONS                              */
        /* -------------------------------------------------------------------------- */

        /* -------------------------- Node Curve Attachment ------------------------- */
        void AttachCurveToRootNode(int id) => AttachCurveToNode(0, id);
        void AttachCurveToNode(int rootId, int id)
        {
            if (linkedRelationType.ContainsKey(id)) return;
            // Curve root, target;
            switch (relationType)
            {
                case RelationType.Parent:
                default:
                    // root = new Node(rootId, NodeAnchor.Top);
                    // target = new Node(id, NodeAnchor.Bottom);
                    break;
                case RelationType.Partner:
                    // root = new Node(rootId, NodeAnchor.Right);
                    // target = new Node(id, NodeAnchor.Left);
                    break;
                case RelationType.Children:
                    // root = new Node(rootId, NodeAnchor.Bottom);
                    // target = new Node(id, NodeAnchor.Top);
                    break;
            }
            // if (linkedNodes.ContainsKey(target))
            //     linkedNodes[target] = root;
            // else
            //     linkedNodes.Add(target, root);
            // if (linkedRelationType.ContainsKey(id))
            //     linkedRelationType[id] = relationType;
            // else
            //     linkedRelationType.Add(id, relationType);
        }
        void AttachCurveToParentNode(int id)
        {
            // Node root = new(childNodeId, NodeAnchor.Top), target = new(id, NodeAnchor.Child);
            // if (linkedRelationType.ContainsKey(id))
            //     if (linkedRelationType[id] == RelationType.Partner)
            //     {
            //         if (linkedChildNodes.TryGetValue(target, out Node _target) && _target.Equals(target))
            //         { Debug.LogWarning("PolityMember pair already exists."); return; }
            //         else linkedChildNodes.Add(root, target);
            //     }
            //     else Debug.LogWarning("A child relation can only be made to a Partner.");
        }

        /* -------------------------- Node Curve Detachment ------------------------- */
        void DeleteCurveToNode(int rootId, int id)
        {
            // List<Curve> keysToRemove = new();
            // foreach (var pair in linkedNodes)
            //     if (pair.Key.NodeId == id && pair.Value.NodeId == rootId)
            //         keysToRemove.Add(pair.Key);

            // foreach (var key in keysToRemove)
            //     linkedNodes.Remove(key);
            // if (keysToRemove.Count > 0)
            // Debug.Log("Removed " + keysToRemove.Count + " connections with ID " + id);
        }
        void DeleteCurveToRootNode(int id) => DeleteCurveToNode(0, id);
        void DeleteCurveToParentNode(int id)
        {
            // List<Curve> keysToRemove = new();
            // foreach (var pair in linkedChildNodes)
            //     if (pair.Key.NodeId == id && pair.Value.NodeId == childNodeId)
            //         keysToRemove.Add(pair.Key);

            // foreach (var key in keysToRemove)
            //     linkedChildNodes.Remove(key);
            // if (keysToRemove.Count > 0)
            // Debug.Log("Removed " + keysToRemove.Count + " child connections with ID " + id);
        }

        /* ------------------------- Set Node Relation Type ------------------------- */
        void SetNodeRelationTypes(int rootId, int id)
        {
            if (polityMembers[rootId] == null)
            {
                EditorUtility.DisplayDialog(
               "Root Polity Member not assigned",
               $"Please assign a Polity Member at Node {rootId}.",
               "OK"
               );
                return;
            }
            if (polityMembers[id] == null) return;
            if (linkedRelationType.TryGetValue(id, out RelationType relation))
            {
                switch (relation)
                {
                    case RelationType.Parent:
                        if (!polityMembers[rootId].family.parents.Contains(polityMembers[id]))
                            polityMembers[rootId].family.parents.Add(polityMembers[id]);
                        // if (!polityMembers[id].children.Contains(polityMembers[rootId]))
                        //     polityMembers[id].children.Add(polityMembers[rootId]);
                        break;
                    case RelationType.Partner:
                        if (rootId != 0)//This is a child to partner, i.e child to parent 
                        {
                            if (!polityMembers[rootId].family.parents.Contains(polityMembers[id]))
                                polityMembers[rootId].family.parents.Add(polityMembers[id]);
                            // if (!polityMembers[id].children.Contains(polityMembers[rootId]))
                            //     polityMembers[id].children.Add(polityMembers[rootId]);
                        }
                        else
                        {
                            // if (!polityMembers[rootId].family.partners.Contains(polityMembers[id]))
                            //     polityMembers[rootId].family.partners.Add(polityMembers[id]);
                            // if (!polityMembers[id].partners.Contains(polityMembers[rootId]))
                            //     polityMembers[id].partners.Add(polityMembers[rootId]);
                        }
                        break;
                    case RelationType.Children:
                        // if (!polityMembers[rootId].children.Contains(polityMembers[id]))
                        //     polityMembers[rootId].children.Add(polityMembers[id]);
                        // if (!polityMembers[id].parents.Contains(polityMembers[rootId]))
                        //     polityMembers[id].parents.Add(polityMembers[rootId]);
                        break;
                    default:
                        Debug.Log("Unknown relationship.");
                        break;
                }
            }
            // else Debug.LogWarning("No relation found for ID: " + id);
        }

        void SetRootNodeRelationTypes(int id) => SetNodeRelationTypes(0, id);
        /// <summary>
        /// Clears relations from the start polity Member to its linked counterpart
        /// </summary>
        void ClearChildNodeRelation(int id)
        {
            if (polityMembers[id].family.parents.Contains(polityMembers[childNodeId]))
                polityMembers[id].family.parents.Remove(polityMembers[childNodeId]);
            // if (polityMembers[childNodeId].children.Contains(polityMembers[id]))
            //     polityMembers[childNodeId].children.Remove(polityMembers[id]);

        }
        void ClearLinkedNodeRelations(int rootId, int id)
        {
            if (linkedRelationType.ContainsKey(id))
            {
                switch (linkedRelationType[id])
                {
                    case RelationType.Partner:
                        // if (polityMembers[rootId].partners.Contains(polityMembers[id]))
                        //     polityMembers[rootId].partners.Remove(polityMembers[id]);
                        // if (polityMembers[id].partners.Contains(polityMembers[rootId]))
                        //     polityMembers[id].partners.Remove(polityMembers[rootId]);
                        break;
                    case RelationType.Parent:
                        if (polityMembers[rootId].family.parents.Contains(polityMembers[id]))
                            polityMembers[rootId].family.parents.Remove(polityMembers[id]);
                        // if (polityMembers[id].children.Contains(polityMembers[rootId]))
                        //     polityMembers[id].children.Remove(polityMembers[rootId]);
                        break;
                    case RelationType.Children:
                        // if (polityMembers[rootId].children.Contains(polityMembers[id]))
                        //     polityMembers[rootId].children.Remove(polityMembers[id]);
                        if (polityMembers[id].family.parents.Contains(polityMembers[rootId]))
                            polityMembers[id].family.parents.Remove(polityMembers[rootId]);
                        break;
                }
                linkedRelationType.Remove(id);
            }
        }
        void ClearRootNodeRelations(int id) => ClearLinkedNodeRelations(0, id);

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

        /* -------------------------------------------------------------------------- */
        /*                            Bezier Curve Drawers                            */
        /* -------------------------------------------------------------------------- */
        void DrawNodeCurve(Rect start, Rect end, NodeAnchor startConnection, NodeAnchor endConnection)
        {
            Vector2 startPercentage = GetPercentageFromConnectionPoint(startConnection);
            Vector2 endPercentage = GetPercentageFromConnectionPoint(endConnection);
            Color lineColor = GetStartConnectionLineColor(startConnection);
            DrawNodeCurve(start, end, startPercentage, endPercentage, lineColor);
        }
        Vector2 GetPercentageFromConnectionPoint(NodeAnchor point)
        {
            return point switch
            {
                NodeAnchor.Top => new Vector2(0.5f, 0f),
                NodeAnchor.Right => new Vector2(1.0f, 0.5f),
                NodeAnchor.Left => new Vector2(0.0f, 0.5f),
                NodeAnchor.Bottom => new Vector2(0.5f, 1f),
                NodeAnchor.Child => new Vector2(0.5f, 1f),
                _ => new Vector2(0.5f, 0.5f),// Default to center if unknown for some reason
            };
        }
        Color GetStartConnectionLineColor(NodeAnchor startConnection)
        {
            return startConnection switch
            {
                NodeAnchor.Top => Color.blue,
                NodeAnchor.Right => Color.green,
                NodeAnchor.Left => Color.green,
                NodeAnchor.Bottom => Color.red,
                NodeAnchor.Child => Color.cyan,
                _ => Color.black,// Default to center if unknown for some reason
            };
        }
        void DrawNodeCurve(Rect start, Rect end, Vector2 vStartPercentage, Vector2 vEndPercentage, Color lineColor)
        {
            Vector3 startPos = new(start.x + start.width * vStartPercentage.x, start.y + start.height * vStartPercentage.y, 0);
            Vector3 endPos = new(end.x + end.width * vEndPercentage.x, end.y + end.height * vEndPercentage.y, 0);
            Vector3 startTan = startPos + Vector3.right * (-50 + 100 * vStartPercentage.x) + Vector3.up * (-50 + 100 * vStartPercentage.y);
            Vector3 endTan = endPos + Vector3.right * (-50 + 100 * vEndPercentage.x) + Vector3.up * (-50 + 100 * vEndPercentage.y);
            Color shadowCol = new(200, 200, 200, .25f);
            for (int i = 0; i < 3; i++) // Draw a shadow
                Handles.DrawBezier(startPos, endPos, startTan, endTan, shadowCol, null, (i + 1) * 5);
            Handles.DrawBezier(startPos, endPos, startTan, endTan, lineColor, null, 2);
        }
    }
}