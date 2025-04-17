namespace AstarApp.GameLogic;

public static class AStarPathFinder
{
    private class Node
    {
        public int x, y;
        public int g;      // Custo acumulado do início até este nó
        public int h;      // Heurística (estimativa do custo até o objetivo)
        public int f { get { return g + h; } } // f = g + h
        public Node parent; // Para reconstruir o caminho

        public Node(int x, int y)
        {
            this.x = x;
            this.y = y;
        }
    }

    private static int Heuristic(int x, int y, int goalX, int goalY) => Math.Abs(goalX - x) + Math.Abs(goalY - y);
    public static List<(int x, int y)> Search(int startX, int startY, int goalX, int goalY, MapaGrid mapa, HashSet<int> nodosPercorridos)
    {
        int gridWidth = mapa.Largura;
        int gridHeight = mapa.Altura;

        if (startX < 0 || startX >= gridWidth || startY < 0 || startY >= gridHeight ||
            goalX < 0 || goalX >= gridWidth || goalY < 0 || goalY >= gridHeight)
            return null;

        if (mapa.mapa[startY, startX] != 0 || mapa.mapa[goalY, goalX] != 0)
            return null;

        List<Node> openSet = new List<Node>();
        HashSet<(int, int)> closedSet = new HashSet<(int, int)>();

        Node startNode = new Node(startX, startY)
        {
            g = 0,
            h = Heuristic(startX, startY, goalX, goalY),
            parent = null
        };

        openSet.Add(startNode);

        while (openSet.Count > 0)
        {
            openSet.Sort((a, b) => a.f.CompareTo(b.f));
            Node current = openSet[0];

            if (current.x == goalX && current.y == goalY)
            {
                List<(int, int)> path = new List<(int, int)>();
                while (current != null)
                {
                    path.Add((current.x, current.y));
                    current = current.parent;
                }
                path.Reverse();
                return path;
            }

            openSet.RemoveAt(0);
            closedSet.Add((current.x, current.y));

            nodosPercorridos.Add(current.x + current.y * 1000);

            int[,] directions = new int[,] { { 0, 1 }, { 1, 0 }, { 0, -1 }, { -1, 0 } };

            for (int i = 0; i < directions.GetLength(0); i++)
            {
                int newX = current.x + directions[i, 0];
                int newY = current.y + directions[i, 1];

                if (newX < 0 || newX >= gridWidth || newY < 0 || newY >= gridHeight)
                    continue;

                if (mapa.mapa[newY, newX] != 0)
                    continue;

                if (closedSet.Contains((newX, newY)))
                    continue;

                int tentativeG = current.g + 1;

                Node neighbor = openSet.Find(n => n.x == newX && n.y == newY);
                if (neighbor == null)
                {
                    neighbor = new Node(newX, newY)
                    {
                        g = tentativeG,
                        h = Heuristic(newX, newY, goalX, goalY),
                        parent = current
                    };
                    openSet.Add(neighbor);
                }
                else if (tentativeG < neighbor.g)
                {
                    neighbor.g = tentativeG;
                    neighbor.parent = current;
                }
            }
        }

        return null;
    }
}