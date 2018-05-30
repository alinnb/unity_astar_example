using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class FindPath : MonoBehaviour {
	private Grid grid;

	// Use this for initialization
	void Start () {
		grid = GetComponent<Grid> ();
	}
	
	// Update is called once per frame
	void Update () {
		FindingPath (grid.player.position, grid.destPos.position);
	}

	// A*寻路
	void FindingPath(Vector3 s, Vector3 e) {
		NodeItem startNode = grid.getItem (s);
		NodeItem endNode = grid.getItem (e);

		List<NodeItem> openSet = new List<NodeItem> ();
		HashSet<NodeItem> closeSet = new HashSet<NodeItem> ();

		//先把出发点Node放入OpenSet
		openSet.Add (startNode);

		//如果OpenSet不为空，就一直查找
		while (openSet.Count > 0) {
			NodeItem curNode = openSet [0];

			//根据上一轮计算结果，选择下一个节点
			for (int i = 0, max = openSet.Count; i < max; i++) {
				//搜索策略为：在f更小或者一样的情况下，选择h小的那个
				//f 总预估路径长度
				//g 离目标路径预估长度
				if (openSet [i].fCost <= curNode.fCost &&
				    openSet [i].hCost < curNode.hCost) {
					curNode = openSet [i];
				}
			}

			openSet.Remove (curNode);
			closeSet.Add (curNode);

			// 找到的目标节点，生成路径并更新
			if (curNode == endNode) {
				generatePath (startNode, endNode);
				return;
			}

			// 判断周围节点，选择一个最优的节点
			foreach (var item in grid.getNeibourhood(curNode)) {
				// 如果是墙或者已经在关闭列表中
				if (item.isWall || closeSet.Contains (item))
					continue;
				
				// 计算当前相领节点现开始节点距离，cost单位为实际距离x10
				int newCost = curNode.gCost + getDistanceNodes (curNode, item);

				// 如果距离更小，或者原来不在开始列表中
				if (newCost < item.gCost || !openSet.Contains (item)) {
					// 更新与开始节点的距离
					item.gCost = newCost;
					// 更新与终点的距离
					item.hCost = getDistanceNodes (item, endNode);
					// 更新父节点为当前选定的节点
					item.parent = curNode;
					// 如果节点是新加入的，将它加入打开列表中
					if (!openSet.Contains (item)) {
						openSet.Add (item);
					}
				}
			}
		}

		//找不到路径，不生成
		generatePath (startNode, null);
	}

	// 生成路径
	void generatePath(NodeItem startNode, NodeItem endNode) {
		List<NodeItem> path = new List<NodeItem>();
		if (endNode != null) {
			NodeItem temp = endNode;
			while (temp != startNode) {
				path.Add (temp);
				temp = temp.parent;
			}
			// 反转路径
			path.Reverse ();
		}
		// 更新路径
		grid.updatePath(path);
	}

	// 获取两个节点之间的距离
	//采用对角线估算法
	int getDistanceNodes(NodeItem a, NodeItem b) {
		int cntX = Mathf.Abs (a.x - b.x);
		int cntY = Mathf.Abs (a.y - b.y);
		// 判断到底是那个轴相差的距离更远 ，为了简化计算，我们将代价*10变成了整数。
		if (cntX > cntY) {
			return 14 * cntY + 10 * (cntX - cntY);//应该是1.4 * cntY + (cntX - cntY)
		} else {
			return 14 * cntX + 10 * (cntY - cntX);
		}
	}


}
