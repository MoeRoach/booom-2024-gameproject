// File create date:2022/8/3
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using RoachFramework;
// Created By Yu.Liu
public class StagePathFinder : MonoSingleton<StagePathFinder> {

	private const int AsyncCallbackToMainThreadPerFrame = 32;
	public delegate void PathCallback(List<Square> path);

	private ConcurrentQueue<PathCallbackMethod> _callbacks;
	private PathNodeComparer _nodeComparer;

	protected override void OnAwake() {
		base.OnAwake();
		_callbacks = new ConcurrentQueue<PathCallbackMethod>();
		_nodeComparer = new PathNodeComparer();
	}

	private void Update() {
		if (_callbacks == null || _callbacks.Count <= 0) return;
		for (var i = 0; i < AsyncCallbackToMainThreadPerFrame; i++) {
			if(_callbacks.Count <= 0) break;
			if (!_callbacks.TryDequeue(out var method)) break;
			method.callback.Invoke(method.path);
		}
	}

	private void CallbackToMainThread(PathCallbackMethod method) {
		_callbacks.Enqueue(method);
	}

	private void SchedulePathFinder(PathSearchRequest request) {
		ThreadPool.QueueUserWorkItem(state => {
			if (request.callback == null) return;
			var path = SearchPath(request);
			var method = new PathCallbackMethod(request.callback, path);
			CallbackToMainThread(method);
		});
	}

	private List<Square> SearchPath(PathSearchRequest req) {
		if (req.src.x == req.dst.x && req.src.y == req.dst.y) {
			return new List<Square> {req.src};
		}

		var candidate = new List<Square> {req.dst};
		var secondary = CompileSecondaryTargets(req);
		candidate.AddRange(secondary);
		foreach (var target in candidate) {
			req.dst = target;
			var path = CalculateSinglePath(req);
			if (path.Count > 1) return path;
		}

		return new List<Square> {req.src};
	}

	private List<Square> CalculateSinglePath(PathSearchRequest req) {
		var path = new List<Square>();
		try {
			var record = AStar(req);
			var stack = new Stack<PathNode>();
			if (record.Count > 0) {
				var cusor = req.dst.Sid;
				if (record.ContainsKey(cusor)) {
					while (!string.IsNullOrEmpty(cusor)) {
						stack.Push(record[cusor]);
						cusor = record[cusor].ParentIdentifier;
					}

					while (stack.Count > 0) {
						var node = stack.Pop();
						path.Add(node.square);
					}
				} else {
					// 路径里没有目标点，表示可能不连通，暂时只返回起点
					path.Add(req.src);
					return path;
				}
			} else {
				// 没找到任何路径
				path.Add(req.src);
				return path;
			}
		} catch (Exception e) {
			LogUtils.LogError($"{e.Message}\n{e.StackTrace}");
			path.Add(req.src);
		}
		return path;
	}

	private Dictionary<string, PathNode> AStar(PathSearchRequest req) {
		var openSet = new Dictionary<string, PathNode>();
		var openQueue = new PriorityQueue<PathNode>(_nodeComparer);
		var closeSet = new HashSet<string>();
		var record = new Dictionary<string, PathNode>();
		var srcNode = new PathNode(req.src);
		srcNode.ClearParentSquare();
		record[srcNode.Identifier] = srcNode;
		openSet[srcNode.Identifier] = srcNode;
		openQueue.Enqueue(srcNode);
		while (openSet.Count > 0 && !openSet.ContainsKey(req.dst.Sid)) {
			var cusor = openQueue.Dequeue();
			openSet.Remove(cusor.Identifier);
			closeSet.Add(cusor.Identifier);
			for (var d = 0; d <= CoordinateUtils.SQR_DIR_LEFT_BOT; d += 2) {
				var nq = CoordinateUtils.MoveNextSquare(cusor.square, d);
				if (!req.checkWalkable(req.scid, nq, req.area))
					continue;
				if (openSet.ContainsKey(nq.Sid)) {
					var openNode = openSet[nq.Sid];
					var cost = CoordinateUtils.GetSquareDistance(cusor.square, nq);
					if(cusor.pathCost + cost >= openNode.pathCost) continue;
					openQueue.Remove(openNode);
					openNode.SetupParentSquare(cusor.parentSquare);
					openNode.pathCost = cusor.pathCost + cost;
					openNode.estCost =
						openNode.pathCost + req.getDistance(openNode.square, req.dst);
					openSet[openNode.Identifier] = openNode;
					openQueue.Enqueue(openNode);
				} else if(!closeSet.Contains(nq.Sid)) {
					var cost = CoordinateUtils.GetSquareDistance(cusor.square, nq);
					var openNode = new PathNode(nq);
					openNode.SetupParentSquare(cusor.square);
					openNode.pathCost = cusor.pathCost + cost;
					openNode.estCost =
						openNode.pathCost + req.getDistance(openNode.square, req.dst);
					openSet[openNode.Identifier] = openNode;
					openQueue.Enqueue(openNode);
					record[openNode.Identifier] = openNode;
				}
			}
		}
		return record;
	}

	private HashSet<Square> CompileSecondaryTargets(PathSearchRequest req) {
		var targets = new HashSet<Square>();
		for (var x = - req.area.x; x <= 1; x++) {
			for (var y = - req.area.y; y <= 1; y++) {
				if ((x == -req.area.x || x == 1) && (y == -req.area.y || y == 1)) continue;
				var sq = new Square(req.dst.x + x, req.dst.y + y);
				if (!req.checkWalkable(req.scid, sq, req.area)) continue;
				targets.Add(sq);
			}
		}

		return targets;
	}

	public void RequestForPath(PathSearchRequest request) {
		SchedulePathFinder(request);
	}
	
	public enum PathState {
		Wait,
		Finding,
		Done
	}

	public static PathSearchRequest CreateRequest(int id, Square src, Square dst, Square area,
		PathCallback callback) {
		return new PathSearchRequest(id, src, dst, area, callback);
	}

	public class PathSearchRequest {
		
		public int scid;
		public Square src;
		public Square dst;
		public Square area;
		public Func<Square, Square, int> getDistance;
		public Func<int, Square, Square, bool> checkWalkable;
		public PathCallback callback;

		public PathSearchRequest(int id, Square src, Square dst, Square area,
			PathCallback callback) {
			scid = id;
			this.src = src;
			this.dst = dst;
			this.area = area;
			this.callback = callback;
		}
	}

	private class PathCallbackMethod {
		
		public readonly PathCallback callback;
		public readonly List<Square> path;

		public PathCallbackMethod(PathCallback callback, List<Square> path) {
			this.callback = callback;
			this.path = path;
		}
	}

	private class PathNode : IComparable<PathNode> {
		
		public Square square;
		public string Identifier => square.Sid;
		public Square parentSquare;
		public string ParentIdentifier { get; private set; }
		public int pathCost;
		public int estCost;

		public PathNode(Square src) {
			square = src;
			pathCost = 0;
			estCost = 0;
		}

		public void SetupSquare(Square sq) {
			square = sq;
		}

		public void ClearParentSquare() {
			ParentIdentifier = string.Empty;
		}

		public void SetupParentSquare(Square coord) {
			parentSquare = coord;
			ParentIdentifier = parentSquare.Sid;
		}

		public int CompareTo(PathNode other) {
			if (estCost > other.estCost) {
				return 1;
			}

			if (estCost < other.estCost) {
				return -1;
			}

			return 0;
		}
	}

	private class PathNodeComparer : IComparer<PathNode> {
		public int Compare(PathNode x, PathNode y) {
			if (x == null || y == null)
				throw new NullReferenceException("null is not comparable!");
			if (x.estCost > y.estCost) {
				return -1;
			}

			return x.estCost < y.estCost ? 1 : 0;
		}
	}
}