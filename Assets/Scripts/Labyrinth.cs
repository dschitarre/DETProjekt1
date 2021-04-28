using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;
using UnityEditor;

public class Labyrinth : ScriptableObject
{
    public static Labyrinth Instance { get; private set; }
    private int size;
    private int cellSize;
    private float wallThickness;
    private int pathLengthRelativeToSize;
    private GameObject labyrinthWallPrefab;
    private LabyrinthGraph labyrinth;
    private System.Random rand = new System.Random();
    private Cell[][] cells;
    public Vector2 startPos {get; private set;}

    public Vector2 exitPos {get; private set;}
    
    public void Awake() {
        if (Instance != null) {
            return;
        } else {
            Instance = this;
        }

        labyrinthWallPrefab = AssetDatabase.LoadAssetAtPath("Assets/labyrinthWall.prefab", typeof(GameObject)) as GameObject;
    }

    public void SetSettings(GameSettings gameSettings) {
        size = gameSettings.size;
        cellSize = gameSettings.cellSize;
        wallThickness = gameSettings.wallThickness;
        pathLengthRelativeToSize = gameSettings.pathLengthRelativeToSize;
    }

    Vector4 GetPosAndSizeOfWall(Wall wall) {
        Cell cell1 = wall.GetCell1();
        Cell cell2 = wall.GetCell2();

        // ensure cell1 has smaller position vector
        if (cell1.GetPosX() > cell2.GetPosX() || cell1.GetPosY() > cell2.GetPosY()) {
            Cell tmp = cell1;
            cell1 = cell2;
            cell2 = tmp;
        }

        int x = cell1.GetPosX();
        int y = cell1.GetPosY();
        
        // check if wall is horizontal or vertical
        if (x == cell2.GetPosX()) {
            return new Vector4((float) (cellSize*(x+0.5)), (float) (cellSize*(y+1)), (float) cellSize+wallThickness, wallThickness);
        } else {
            return new Vector4((float) (cellSize*(x+1)), (float) (cellSize*(y+0.5)), (float) wallThickness, cellSize+wallThickness);
        }
    }

    private void GenerateRectangle(float posX, float posY, float sizeX, float sizeY) {
        GameObject wall = Instantiate(labyrinthWallPrefab, new Vector2(posX, posY), new Quaternion(0.0f, 0.0f, 0.0f, 0.0f));
        Rigidbody2D rb = wall.GetComponent<Rigidbody2D>();
        wall.transform.localScale = new Vector2(sizeX, sizeY);
    }

    public void Generate()
    {
        cells = new Cell[size][];
        List<Wall> walls = new List<Wall>();

        // add cells
        for (int i = 0; i < size; i++) {
            cells[i] = new Cell[size];
            for (int j = 0; j < size; j++) {
                cells[i][j] = new Cell(i, j);
            }
        }

        // add walls
        for (int i = 0; i < size-1; i++) {
            walls.Add(new Wall(cells[i][size-1], cells[i+1][size-1]));
            walls.Add(new Wall(cells[size-1][i], cells[size-1][i+1]));
            for (int j = 0; j < size-1; j++) {
                Cell cell = cells[i][j];
                walls.Add(new Wall(cell, cells[i+1][j]));
                walls.Add(new Wall(cell, cells[i][j+1]));
            }
        }

        // create sets of connected cells
        HashSet<Cell>[][] connectedCellsSets = new HashSet<Cell>[size][];
        for (int i = 0; i < size; i++) {
            connectedCellsSets[i] = new HashSet<Cell>[size];
            for (int j = 0; j < size; j++) {
                connectedCellsSets[i][j] = new HashSet<Cell>();
                connectedCellsSets[i][j].Add(cells[i][j]);
            }
        }

        // remove Walls randomly until all cells are connected
        List<Wall> wallsTGenerate = new List<Wall>();
        List<Wall> removedWalls = new List<Wall>();
        while (walls.Count != 0) {
            int randomIndex = rand.Next(walls.Count);
            Wall randomWall = walls[randomIndex];

            Cell cell1 = randomWall.GetCell1();
            Cell cell2 = randomWall.GetCell2();

            HashSet<Cell> set1 = connectedCellsSets[cell1.GetPosX()][cell1.GetPosY()];
            HashSet<Cell> set2 = connectedCellsSets[cell2.GetPosX()][cell2.GetPosY()];

            if (set1 != set2) {
                set1.UnionWith(set2);
                foreach (Cell cell in set2) {
                    connectedCellsSets[cell.GetPosX()][cell.GetPosY()] = set1;
                }
                removedWalls.Add(randomWall);
            } else {
                wallsTGenerate.Add(randomWall);
            }
            walls.RemoveAt(randomIndex);
        }

        // create a graph where vertices are cells and edges are removed walls
        labyrinth = new LabyrinthGraph();
        for (int i = 0; i < size; i++) {
            for (int j = 0; j < size; j++) {
                labyrinth.AddVertex(cells[i][j].GetTag());
            }
        }
        foreach (Wall wall in removedWalls) {
            labyrinth.Connect(wall.GetCell1().GetTag(), wall.GetCell2().GetTag());
        }

        // list cells at the edge of the labyrinth
        List<Cell> outerCells = new List<Cell>();
        outerCells.Add(cells[0][0]);
        outerCells.Add(cells[0][size-1]);
        outerCells.Add(cells[size-1][0]);
        outerCells.Add(cells[size-1][size-1]);
        for (int i = 1; i < size-1; i++) {
            outerCells.Add(cells[i][0]);
            outerCells.Add(cells[i][size-1]);
            outerCells.Add(cells[0][i]);
            outerCells.Add(cells[size-1][i]);
        }

        // get pair of outer cells with path deviating the least from the wanted path length
        List<Tuple<Cell,Cell>> bestCellPairs = new List<Tuple<Cell,Cell>>();
        int wantedPathLength = pathLengthRelativeToSize*size;
        int minimumDeviation = size*size;
        // consider each outer cell as entrance
        foreach (Cell cell in outerCells) {
            List<KeyValuePair<string, int>> destinations = CalculatePathLengths(labyrinth, cell.GetTag());

            // consider each destination cell as exit
            foreach (KeyValuePair<string, int> destination in destinations) {
                int xDest, yDest;
                GetIntsFromTag(destination.Key, out xDest, out yDest);
                // check if the current destination cell is outer cell
                if (xDest == 0 || xDest == size-1 || yDest == 0 || yDest == size-1) {
                    int currentDeviation = Math.Abs(destination.Value-wantedPathLength);
                    // check if its path length deviates not more from the wanted path length
                    if (currentDeviation <= minimumDeviation) {
                        if (currentDeviation < minimumDeviation) {
                            minimumDeviation = currentDeviation;
                            bestCellPairs.Clear();
                        }
                        bestCellPairs.Add(new Tuple<Cell,Cell>(cell, cells[xDest][yDest]));
                        if (minimumDeviation == 0) {
                            break;
                        }
                    }
                }
            }
            if (minimumDeviation == 0) {
                break;
            }
        }
        Tuple<Cell,Cell> bestCellPair = bestCellPairs[rand.Next(bestCellPairs.Count)];
        Cell cellEntrance = bestCellPair.Item1;
        Cell cellExit = bestCellPair.Item2;

        Vector2 startPos, exitPos;
        GetPosFromCell(cellExit.GetPosX(), cellExit.GetPosY(), out exitPos);

        // add outer walls and save entrance and exit
        foreach (Cell cell in outerCells) {
            List<Wall> wallsToAdd = new List<Wall>();
            if (cell.GetPosX() == 0) {
                wallsToAdd.Add(new Wall(new Cell(-1, cell.GetPosY()), cell));
            }
            if (cell.GetPosX() == size-1) {
                wallsToAdd.Add(new Wall(cell, new Cell(size, cell.GetPosY())));
            }
            if (cell.GetPosY() == 0) {
                wallsToAdd.Add(new Wall(new Cell(cell.GetPosX(), -1), cell));
            }
            if (cell.GetPosY() == size-1) {
                wallsToAdd.Add(new Wall(cell, new Cell(cell.GetPosX(), size)));
            }
            if (cell.Equals(cellExit)) {
                wallsToAdd.RemoveAt(0);
            }
            foreach (Wall wall in wallsToAdd) {
                wallsTGenerate.Add(wall);
            }
        }

        // generate walls
        foreach (Wall wall in wallsTGenerate) {
            Vector4 posAndSize = GetPosAndSizeOfWall(wall);
            GenerateRectangle(posAndSize.x, posAndSize.y, posAndSize.z, posAndSize.w);
        }

        GetPosFromCell(cellEntrance.GetPosX(), cellEntrance.GetPosY(), out startPos);
        GameObject.FindWithTag("Player").transform.position = startPos;
        GameObject.FindWithTag("MainCamera").transform.position = (Vector3) startPos + new Vector3(0.0f, 0.0f, -1.0f);
    }

    private List<KeyValuePair<string, int>> CalculatePathLengths(LabyrinthGraph graph, string tagFrom) {
        Dictionary<string, int> distance = new Dictionary<string, int>(graph.GetNumberOfVertices());
        HashSet<string> visited = new HashSet<string>();
        HashSet<string> visiting = new HashSet<string>();

        foreach (string tag in graph.GetVertices()) {
            distance.Add(tag, -1);
        }
        distance[tagFrom] = 0;

        visiting.Add(tagFrom);

        while(visiting.Count != 0) {
            string currentTag = visiting.First();
            int currentDistance;
            distance.TryGetValue(currentTag, out currentDistance);

            List<string> adjacentVertices;

            if (graph.GetAdjacentVertices(currentTag, out adjacentVertices)) {
                foreach (string tag in adjacentVertices) {
                    if (!visited.Contains(tag) && !visiting.Contains(tag)) {
                        visiting.Add(tag);
                        distance.Remove(tag);
                        distance.Add(tag, currentDistance+1);
                    }
                }
            }

            visiting.Remove(currentTag);
            visited.Add(currentTag);
        }

        return distance.ToList();
    }

    public bool GetRandomPosInCell(int x, int y, out Vector2 pos) {
        if (!IsCellInLabyrinth(x, y)) {
            pos = new Vector2(0, 0);
            return false;
        }
        pos = GetRandomPosInArea((float) (x + 0.5) * cellSize, (float) (y + 0.5) * cellSize, cellSize - 2 * wallThickness, cellSize - 2 * wallThickness);
        return true;
    }

    // calculates the next cell on the way from one cell to another (in a labyrinth there is only one way from one cell to another)
    public bool nextPosToMoveToOnWayFromTo(Vector2 from, Vector2 to, out Vector2 nextPos) {
        if (!GetCellFromPos(from, out int fromX, out int fromY) || !GetCellFromPos(to, out int toX, out int toY)) {
            nextPos = new Vector2(0, 0);
            Debug.Log("start and/or destination cell is/are not in the labyrinth");
            return false;
        } else {
            string tagFrom = cells[fromX][fromY].GetTag();
            string tagTo = cells[toX][toY].GetTag();
            string tagNext;

            if (tagFrom == tagTo) {
                tagNext = tagFrom;
            } else if (labyrinth.nextVertexOnWayFromTo(tagFrom, tagTo, out tagNext)) {
                int x, y;
                GetIntsFromTag(tagNext, out x, out y);
                
                GetRandomPosInCell(x, y, out nextPos);
                return true;
            }

            nextPos = new Vector2(0, 0);
            Debug.Log("no next cell found");
            return false;
        }
    }

    public Vector2 GetRandomPosInArea(float x, float y, float w, float h) {
        return new Vector2(x - w/2 + (float) rand.NextDouble() * w, y - h/2 + (float) rand.NextDouble() * h);
    }

    public bool IsCellInLabyrinth(int x, int y) {
        if (x < 0 || y < 0 || x >= size || y >= size) {
            Debug.Log("position outside of labyrinth");
            return false;
        }
        return true;
    }

    public bool GetTagFromCell(int x, int y, out string tag) {
        if (!IsCellInLabyrinth(x, y)) {
            tag = "";
            return false;
        }
        tag = cells[x][y].GetTag();
        return true;
    }

    public bool GetIntsFromTag(string tag, out int x, out int y) {
        string[] indices;
        try {
            indices = tag.Split(',');
        } catch (Exception) {
            x = 0;
            y = 0;
            return false;
        }
        x = Convert.ToInt32(indices[0]);
        y = Convert.ToInt32(indices[1]);
        return true;
    }

    public bool GetPosFromCell(int x, int y, out Vector2 pos) {
        if (!IsCellInLabyrinth(x, y)) {
            pos = new Vector2(0, 0);
            return false;
        }
        pos = new Vector2(x + 0.5f, y + 0.5f) * cellSize;
        return true;
    }

    public bool GetCellFromPos(Vector2 pos, out int x, out int y) {
        x = (int) pos.x / cellSize;
        y = (int) pos.y / cellSize;
        if (!IsCellInLabyrinth(x, y)) {
            
            return false;
        }
        return true;
    }

    public bool GetTagFromPos(Vector2 pos, out string tag) {
        int x, y;
        if (!GetCellFromPos(pos, out x, out y)) {
            tag = "";
            return false;
        }
        if (!GetTagFromCell(x, y, out tag)) {
            return false;
        }
        return true;
    }

    public bool GetRandomCellAtMaxDistance(string tagFrom, int maxDistance, out string tagRandom) {
        if (!labyrinth.GetRandomCellAtMaxDistance(tagFrom, maxDistance, out tagRandom)) {
            tagRandom = "";
            return false;
        }
        return true;
    }

    private class Cell
    {
        private int posX;
        private int posY;

        public Cell(int posX, int posY) {
            this.posX = posX;
            this.posY = posY;
        }

        public int GetPosX() {
            return posX;
        }

        public int GetPosY() {
            return posY;
        }

        public String GetTag() {
            return ToString();
        }

        public bool Equals(Cell cell) {
            return GetPosX() == cell.GetPosX() && GetPosY() == cell.GetPosY();
        }

        public override string ToString() {
            return GetPosX().ToString() + ',' +  GetPosY().ToString();
        }
    }

    private class Wall
    {
        private Cell cell1;
        private Cell cell2;

        public Wall(Cell cell1, Cell cell2) {
            this.cell1 = cell1;
            this.cell2 = cell2;
        }

        public Cell GetCell1() {
            return cell1;
        }

        public Cell GetCell2() {
            return cell2;
        }
    }
    private class LabyrinthGraph
    {
        private Dictionary<string,Vertex> vertices = new Dictionary<string,Vertex>();
        private int verticesCount = 0;
        private int edgesCount = 0;
        public LabyrinthGraph() {}

        public int GetNumberOfVertices() {
            return verticesCount;
        }

        public int GetNumberOfEdges() {
            return edgesCount;
        }

        public bool AddVertex(string tag) {
            try {
                vertices.Add(tag, new Vertex(tag));
            }
            catch (System.ArgumentException) {
                return false;
            }
            verticesCount++;
            return true;
        }

        public bool ContainsVertex(string tag) {
            return vertices.ContainsKey(tag);
        }

        public bool GetAdjacentVertices(string tag, out List<string> adjacentVertices) {
            Vertex v;
            bool containsTag = vertices.TryGetValue(tag, out v);
            if (!containsTag) {
                adjacentVertices = null;
                return false;
            }
            adjacentVertices = v.GetAdjacentVertices().Keys.ToList();
            return true;
        }

        private List<Vertex> GetAdjacentVertices(Vertex v) {
            return v.GetAdjacentVertices().Values.ToList();
        }

        public List<string> GetVertices() {
            return vertices.Keys.ToList();
        }

        public bool AreConnected(string tagV, string tagU) {
            Vertex v, u;
            return TryGetBoth(tagV, tagU, out v, out u) && AreConnected(v, u);
        }

        private bool AreConnected(Vertex v, Vertex u) {
            return v.GetAdjacentVertices().ContainsKey(u.GetTag());
        }

        public bool Connect(string tagV, string tagU) {
            Vertex v, u;
            if (!TryGetBoth(tagV, tagU, out v, out u) || v.GetAdjacentVertices().ContainsKey(tagU)) {
                return false;
            }
            Connect(v, u);
            return true;
        }

        private void Connect(Vertex v, Vertex u) {
            v.GetAdjacentVertices().Add(u.GetTag(), u);
            u.GetAdjacentVertices().Add(v.GetTag(), v);
            edgesCount++;
        }

        public bool Disconnect(string tagV, string tagU) {
            Vertex v, u;
            if (!TryGetBoth(tagV, tagU, out v, out u) || !v.GetAdjacentVertices().ContainsKey(tagU)) {
                return false;
            }
            Disconnect(v, u);
            return true;
        }

        private void Disconnect(Vertex v, Vertex u) {
            v.GetAdjacentVertices().Remove(u.GetTag());
            u.GetAdjacentVertices().Remove(v.GetTag());
            edgesCount--;
        }

        public bool ExistsWayFromTo(string tagV, string tagU) {
            Vertex v, u;
            return TryGetBoth(tagV, tagU, out v, out u) && ExistsWayFromTo(v, u);
        }

        private bool ExistsWayFromTo(Vertex v, Vertex u) {
            if (AreConnected(v, u)) {
                return true;
            }
            foreach (Vertex w in GetAdjacentVertices(v)) {
                Disconnect(v, w);
                if (ExistsWayFromTo(w, u)) {
                    Connect(v, w);
                    return true;
                }
                Connect(v, w);
            }
            return false;
        }

        public bool nextVertexOnWayFromTo(string tagFrom, string tagTo, out string tagNext) {
            Vertex from, to;

            TryGetBoth(tagFrom, tagTo, out from, out to);
            List<Vertex> candidates = GetAdjacentVertices(from);

            foreach (Vertex candidate in candidates) {
                if (candidate.GetTag() == tagTo) {
                    tagNext = tagTo;
                    return true;
                }
                Disconnect(from, candidate);
                if (ExistsWayFromTo(candidate, to)) {
                    int x, y;
                    Instance.GetIntsFromTag(candidate.GetTag(), out x, out y);
                    Connect(from, candidate);
                    Instance.GetTagFromCell(x, y, out tagNext);
                    return true;
                }
                Connect(from, candidate);
            }

            tagNext = "";
            return false;
        }

        public bool GetRandomCellAtMaxDistance(string tagFrom, int maxDistance, out string tagRandom) {
            Vertex from;

            if (!vertices.TryGetValue(tagFrom, out from)) {
                tagRandom = "";
                return false;
            }

            tagRandom = GetRandomVertexAtMaxDistance(from, maxDistance).GetTag();
            return true;
        }

        private Vertex GetRandomVertexAtMaxDistance(Vertex from, int maxDistance) {
            if (maxDistance == 0) {
                return from;
            }

            List<Vertex> adjacentVertices = GetAdjacentVertices(from);

            if (adjacentVertices.Count == 0) {
                return from;
            }

            Vertex next = adjacentVertices[Instance.rand.Next(adjacentVertices.Count)];
            Disconnect(from, next);
            Vertex dest = GetRandomVertexAtMaxDistance(next, maxDistance - 1);
            Connect(from, next);
            
            return dest;
        }

        private bool TryGetBoth(string tagV, string tagU, out Vertex v, out Vertex u) {
            u = new Vertex("");
            return vertices.TryGetValue(tagV, out v) && vertices.TryGetValue(tagU, out u);
        }

        private class Vertex
        {
            private string tag;
            private Dictionary<string,Vertex> adjacentVertices = new Dictionary<string,Vertex>();

            public Vertex(string tag) {
                this.tag = tag;
            }

            public string GetTag() {
                return tag;
            }

            public Dictionary<string,Vertex> GetAdjacentVertices() {
                return adjacentVertices;
            }

            public bool IsAdjacentTo(string tag) {
                return adjacentVertices.ContainsKey(tag);
            }
        }
    }
}
