using System.Collections.Generic;
using UnityEngine;

public class Pathfinding : MonoBehaviour {

    public Transform seeker, target;

    NodeGrid grid;

    private void Awake() {
        grid = FindObjectOfType<NodeGrid>().GetComponent<NodeGrid>();
    }

    private void Update() {
        FindPath(seeker.position, target.position);
    }

    void FindPath(Vector3 startPos, Vector3 targetPos) {

        NodeAStar startNode = grid.NodeFromWorldPoint(startPos);
        NodeAStar targetNode = grid.NodeFromWorldPoint(targetPos);

        Heap<NodeAStar> openSet = new Heap<NodeAStar>(grid.MaxSize);
        HashSet<NodeAStar> closedSet = new HashSet<NodeAStar>();
        openSet.Add(startNode);

        while (openSet.Count > 0) {
            NodeAStar currentNode = openSet.PopFirst();
            closedSet.Add(currentNode);

            if (currentNode == targetNode) {
                RetracePath(startNode, targetNode);
                return;
            }

            foreach (NodeAStar neighbourNode in grid.GetNeighbouringNodes(currentNode)) {
                if (!neighbourNode.walkable || closedSet.Contains(neighbourNode)) {
                    continue;
                }

                int newMovementCostToNeighbour = currentNode.gCost + GetDistance(currentNode, neighbourNode);
                if (newMovementCostToNeighbour < neighbourNode.gCost || !openSet.Contains(neighbourNode)) {
                    neighbourNode.gCost = newMovementCostToNeighbour;
                    neighbourNode.hCost = GetDistance(neighbourNode, targetNode);
                    neighbourNode.parentNode = currentNode;

                    if (!openSet.Contains(neighbourNode)) {
                        openSet.Add(neighbourNode);
                    }
                }
            }
        }
    }

    void RetracePath(NodeAStar startNode, NodeAStar endNode) {
        List<NodeAStar> path = new List<NodeAStar>();
        NodeAStar currentNode = endNode;

        while (currentNode != startNode) {
            path.Add(currentNode);
            currentNode = currentNode.parentNode;
        }
        path.Reverse();

        grid.path = path;
    }

    int GetDistance(NodeAStar nodeA, NodeAStar nodeB) {
        int dstX = Mathf.Abs(nodeA.gridX - nodeB.gridX);
        int dstY = Mathf.Abs(nodeA.gridY - nodeB.gridY);

        if (dstX > dstY) {
            return 14 * dstY + 10 * (dstX - dstY);
        }
        else {
            return 14 * dstX + 10 * (dstY - dstX);
        }
    }
}
