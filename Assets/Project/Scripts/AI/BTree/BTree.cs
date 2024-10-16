using UnityEngine;

namespace BehaviorTree
{
    public abstract class BTree : MonoBehaviour
    {
        private Node _root = null;

        protected virtual void Start()
        {
            _root = SetUpTree();
        }

        protected virtual void Update()
        {
            if (_root != null)
            {
                _root.Evaluate();
            }

        }

        protected abstract Node SetUpTree();
    }
}



