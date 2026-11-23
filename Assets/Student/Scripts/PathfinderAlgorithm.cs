using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class PathfindingAlgorithm
{
	public static List<Vector2Int> FindShortestPath(Vector2Int start, Vector2Int goal, IMapData mapData)
	{
		List<Vector2Int> vertecies = new();
		foreach (int x in Enumerable.Range(0, mapData.Width))
			foreach (int y in Enumerable.Range(0, mapData.Height))
				vertecies.Add(new Vector2Int(x, y));

		WeightedDigraph<Vector2Int> graph = new(vertecies);
		foreach (Vector2Int vertex in vertecies)
			CreateEdges(graph, vertex, mapData);

		int startVertex = graph.GetVertex(start);
		int goalVertex = graph.GetVertex(goal);

		var paths = new Dijkstras(graph, startVertex).Paths;
		var connectingEdge = paths[goalVertex];
		if (connectingEdge.IsNull) return null;

		List<Vector2Int> finalPath = new();

		int currentVertex = goalVertex;
		while (currentVertex != startVertex)
		{
			Edge e = paths[currentVertex];
			finalPath.Add(graph.VertexData[e.W]);
			currentVertex = e.V;
		}
		finalPath.Reverse();

		return finalPath;
	}

	private static void CreateEdges(WeightedDigraph<Vector2Int> graph, Vector2Int pos, IMapData mapData)
	{
		Vector2Int up, down, left, right;
		up = new(pos.x, pos.y + 1);
		down = new(pos.x, pos.y - 1);
		left = new(pos.x - 1, pos.y);
		right = new(pos.x + 1, pos.y);

		if (!IsMovementBlocked(pos, up, mapData))
			graph.AddEdge(pos, up, mapData.GetHorizontalWallCost(pos.x, pos.y));

		if (!IsMovementBlocked(pos, down, mapData))
			graph.AddEdge(pos, down, mapData.GetHorizontalWallCost(pos.x, pos.y));

		if (!IsMovementBlocked(pos, left, mapData))
			graph.AddEdge(pos, left, mapData.GetVerticalWallCost(pos.x, pos.y));

		if (!IsMovementBlocked(pos, right, mapData))
			graph.AddEdge(pos, right, mapData.GetVerticalWallCost(pos.x, pos.y));

		var jumps = mapData.GetJumpsFrom(pos);
		foreach (var jump in jumps)
			graph.AddEdge(pos, jump.To, jump.Cost);
	}
	public static bool IsMovementBlocked(Vector2Int from, Vector2Int to, IMapData mapData)
	{
		if (mapData.TryGetJumpCost(from, to, out _))
			return false;

		int deltaX = to.x - from.x;
		int deltaY = to.y - from.y;

		if (Mathf.Abs(deltaX) + Mathf.Abs(deltaY) != 1)
			return true;

		if (deltaX != 0)
		{
			int wallX = deltaX > 0 ? to.x : from.x;
			float wallCost = mapData.GetVerticalWallCost(wallX, from.y);
			return float.IsPositiveInfinity(wallCost) || wallCost >= float.MaxValue;
		}

		int wallY = deltaY > 0 ? to.y : from.y;
		float horizontalWallCost = mapData.GetHorizontalWallCost(from.x, wallY);
		return float.IsPositiveInfinity(horizontalWallCost) || horizontalWallCost >= float.MaxValue;
	}
}
