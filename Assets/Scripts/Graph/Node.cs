
using System.Collections.Generic;
using UnityEngine;

    public class Node {
        public GameObject gameObject;
        public List<Node> nodes = new List<Node>();

        public Node(GameObject obj) {
            gameObject = obj;
        }
    }
