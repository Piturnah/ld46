using System.Collections.Generic;
using UnityEngine;

public class NodeGrid : MonoBehaviour {

    public bool onlyDisplayPathGizmos;
    public Transform testAgent;
    public LayerMask unwalkableMask;
    public Vector2 gridWorldSize;
    public float nodeRadius;
    public Collider terrainMeshCollider;
    NodeAStar[,] grid;

    float nodeDiameter;
    int gridSizeX, gridSizeY;

    private void Start() {
        nodeDiameter = nodeRadius * 2;
        gridSizeX = Mathf.RoundToInt(gridWorldSize.x / nodeDiameter);
        gridSizeY = Mathf.RoundToInt(gridWorldSize.y / nodeDiameter);
        CreateGrid();
    }

    public int MaxSize {
        get {
            return gridSizeX * gridSizeY;
        }
    }

    void CreateGrid() {
        grid = new NodeAStar[gridSizeX, gridSizeY];
        Vector3 worldBottomLeft = transform.position - Vector3.right * gridWorldSize.x / 2f - Vector3.forward * gridWorldSize.y / 2f;

        for (int y = 0; y < gridSizeY; y++) {
            for (int x = 0; x < gridSizeX; x++) {

                // Determine height of terrain mesh at pos
                RaycastHit hit;
                Vector3 worldPoint = Vector3.zero;
                Ray ray = new Ray(new Vector3(worldBottomLeft.x + (x * nodeDiameter + nodeRadius), 150, worldBottomLeft.z + (y * nodeDiameter + nodeRadius)), Vector3.down);
                if (Physics.Raycast(ray, out hit, 200)) {
                    worldPoint = hit.point;
                }
                else {
                    Debug.LogError("NO TERRAIN FOUND IN RAYCAST");
                }

                bool walkable = !Physics.CheckSphere(worldPoint, nodeRadius, unwalkableMask);
                grid[x, y] = new NodeAStar(walkable, worldPoint, x, y);
            }
        }
    }

    public List<NodeAStar> GetNeighbouringNodes(NodeAStar node) {
        List<NodeAStar> neighbours = new List<NodeAStar>();

        for (int x = -1; x <= 1; x++) {
            for (int y = -1; y <= 1; y++) {
                if (x == 0 && y == 0) {
                    continue;
                }
                int checkX = node.gridX + x;
                int checkY = node.gridY + y;

                if (checkX >= 0 && checkX < gridSizeX && checkY >= 0 && checkY < gridSizeY) {
                    neighbours.Add(grid[checkX, checkY]);
                }
            }
        }

        return neighbours;
    }

    public NodeAStar NodeFromWorldPoint(Vector3 worldPosition) {
        float percentX = Mathf.Clamp01((worldPosition.x + gridWorldSize.x / 2) / gridWorldSize.x);
        float percentY = Mathf.Clamp01((worldPosition.z + gridWorldSize.y / 2) / gridWorldSize.y);

        int x = Mathf.RoundToInt((gridSizeX - 1) * percentX);
        int y = Mathf.RoundToInt((gridSizeY - 1) * percentY);

        return grid[x, y];
    }

    public List<NodeAStar> path;
    private void OnDrawGizmos() {
        Gizmos.DrawWireCube(transform.position, new Vector3(gridWorldSize.x, 100, gridWorldSize.y));

        if (onlyDisplayPathGizmos) {
            if (path != null) {
                foreach (NodeAStar node in path) {
                    Gizmos.color = Color.black;
                    Gizmos.DrawSphere(node.worldPosition, nodeRadius - .5f);
                }
            }
        }
        else {
            if (grid != null) {
                NodeAStar agentNode = NodeFromWorldPoint(testAgent.position);

                foreach (NodeAStar node in grid) {
                    Gizmos.color = (node.walkable) ? Color.white : Color.red;
                    if (path != null) {
                        if (path.Contains(node)) {
                            Gizmos.color = Color.black;
                        }
                    }
                    Gizmos.DrawSphere(node.worldPosition, (nodeRadius - .5f));
                }
            }
        }
    }
}
