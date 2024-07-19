/*
 * Coded by Antonio J. Fernández Leiva
 * Máster Creación videojuegos UMA (https://www.mastervideojuegos.uma.es/)
 * 2024, january
 * Behavior Trees  Implementation. Generic Architecture
 * Reference: https://www.youtube.com/watch?v=aR6wt5BlE-E
*/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BehaviorTree
{
    public abstract class BTree : MonoBehaviour
    {
        private Node _root = null;

        // Start is called before the first frame update
        protected virtual void Start()
        {
            _root = SetUpTree();
        }

        // Update is called once per frame
        protected virtual void Update()
        {
            if (_root != null)
            {
                _root.Evaluate();
            }

        }

        protected abstract Node SetUpTree();  //La deben implementar sus classe derivadas, sí o sí.
    }
}



