using System.Collections.Generic;
using System.Linq;

namespace ShadowMaze
{

    // Creates a path of nodes to the destination from the entity's current position to the destination entity's position.
    public class Path
    {
        private List<Node> PathOfNodes; // Stores the path of nodes to follow to the destination.

        public List<Node> NodePath
        {
            get { return PathOfNodes; }
        }

        // Constructor takes the starting node, destination and the grid to generate the path.
        public Path(Node Start, Node Destination, Node[,] gridNodes)
        {
            List<Node> openNodes = new List<Node>(); // Creates a list of possible nodes for the path.
            List<Node> closedNodes = new List<Node>(); // Creates a list of nodes processed for the path.

            openNodes.Add(Start); //  Step 1: Adds the current node to the possibilities list.

            // Loops while the destination is not on the closed list and while the open nodes list is not empty.
            while (openNodes.Any() && !closedNodes.Contains(Destination))
            {
                // Sorts the open list according to f scores.
                openNodes.Sort((node, otherNode) => node.F.CompareTo(otherNode.F));
                Node nodeCurrent = openNodes[0]; // The node with the lowest F score is set as the current node.
                openNodes.Remove(nodeCurrent);
                closedNodes.Add(nodeCurrent); // The current node is moved to the closed list.

                // Creates a list containing all the nodes adjacent to the current node.
                List<Node> adjacentNodes = AddAdjacentNodes(gridNodes, nodeCurrent);

                CheckAdjacentNodes(adjacentNodes, ref closedNodes, ref openNodes, nodeCurrent, Destination, Start);
            }
            EstablishPath(closedNodes, Start, Destination);
        }

        // Adds all the adjacent nodes from above the current node turning clockwise.
        public List<Node> AddAdjacentNodes(Node[,] gridNodes, Node nodeCurrent)
        {
            int intCol = nodeCurrent.TileX / nodeCurrent.TileWidth;
            int intRow = nodeCurrent.TileY / nodeCurrent.TileHeight; // Gets the current node's indices.
            List<Node> adjacentNodes = new List<Node>(); // Stores the nodes adjacent to the current node.
            // The if statements check whether the node is within the grid before adding the node.
            if (intRow - 1 >= 0)
                adjacentNodes.Add(gridNodes[intCol, intRow - 1]); // Above
            if ((intCol + 1 < gridNodes.GetLength(0)  && intRow - 1 >= 0) && (gridNodes[intCol + 1, intRow].Traversable) && (gridNodes[intCol, intRow - 1].Traversable))
                adjacentNodes.Add(gridNodes[intCol + 1, intRow - 1]); // Diagonally Right Up
            if (intCol + 1 < gridNodes.GetLength(0))
                adjacentNodes.Add(gridNodes[intCol + 1, intRow]); // Right
            if (intCol + 1 < gridNodes.GetLength(0) && intRow + 1 < gridNodes.GetLength(1) && (gridNodes[intCol + 1, intRow].Traversable) && (gridNodes[intCol, intRow + 1].Traversable))
                adjacentNodes.Add(gridNodes[intCol + 1, intRow + 1]); // Diagonally Right Down
            if (intRow + 1 < gridNodes.GetLength(1))
                adjacentNodes.Add(gridNodes[intCol, intRow + 1]); // Below
            if (intCol - 1 >= 0 && intRow + 1 < gridNodes.GetLength(1) && (gridNodes[intCol - 1, intRow].Traversable) && (gridNodes[intCol, intRow + 1].Traversable))
                adjacentNodes.Add(gridNodes[intCol - 1, intRow + 1]); // Diagonally Left Down
            if (intCol - 1 >= 0)
                adjacentNodes.Add(gridNodes[intCol - 1, intRow]); // Left
            if (intCol - 1 >= 0 && intRow - 1 >= 0 && (gridNodes[intCol - 1, intRow].Traversable) && (gridNodes[intCol, intRow - 1].Traversable))
                adjacentNodes.Add(gridNodes[intCol - 1, intRow - 1]); // Diagonally Left Up

            return adjacentNodes;
        }

        // Checks the adjacent node list for nodes to be added to the open list.
        private void CheckAdjacentNodes(List<Node> adjacentNodes, ref List<Node> closedNodes, ref List<Node> openNodes, Node nodeCurrent, Node destinationNode, Node nodeStart)
        {
            foreach (Node node in adjacentNodes)
            { // Checks each node to see if it is traversable and not already on the closed list.
                if (node.Traversable && !closedNodes.Contains(node))
                {
                    // If the node is not on the open list, add it, set its parent as the current node and calculate its F, G, and H values.
                    if (!openNodes.Contains(node))
                    {
                        node.Parent = nodeCurrent;
                        node.CalculateG(nodeCurrent);
                        node.CalculateH(destinationNode);
                        node.CalculateF();
                        openNodes.Add(node);
                    }
                    else // If the node was already on the open list...
                    {
                        // If the G cost of the node (from the current node's parent to this node)
                        // is lower than the G cost of going through the current node's path to this node...
                        if (node.G < nodeCurrent.G)
                        {
                            // Make the node's parent the current node's parent and recalculate its values.
                            node.Parent = nodeCurrent;
                            node.CalculateG(nodeCurrent);
                            node.CalculateF();
                        }
                    }
                }
            }
        }

        // Creates the final path from the starting node to the destination.
        private void EstablishPath(List<Node> closedNodes, Node nodeStart, Node nodeDestination)
        {
            PathOfNodes = new List<Node>(); // Stores the path for the entity to follow to its target.

            // Start at the end of the path.
            Node nodeToAdd = closedNodes[closedNodes.Count - 1];
            while (!PathOfNodes.Contains(nodeStart))
            { // Add each node's parent until the start is reached.
                PathOfNodes.Add(nodeToAdd);
                nodeToAdd = nodeToAdd.Parent;
            }

            PathOfNodes.Remove(null); // Remove the final null node (as the final node had no parent).
        }
    }
}
