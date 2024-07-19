/*
 * Coded by Antonio J. Fernández Leiva
 * Máster Creación videojuegos UMA (https://www.mastervideojuegos.uma.es/)
 * 2024, january
 * Behavior Trees  Implementation. Generic Architecture
 * Reference: https://www.youtube.com/watch?v=aR6wt5BlE-E
*/

using System.Collections;
using System.Collections.Generic;

namespace BehaviorTree 
{ 
    public enum NodeState
    {
        RUNNING, SUCCESS, FAILURE
    }

    public class Node
    {
        protected NodeState state;     //Lo declaramos 'protected' para que solo als clases derivadas puedas acceder a este y actualizarlo
        public Node parent; // Cada nodo tiene un nodo padre
        protected List<Node> children; //Cada nodo tiene 0 o más hijos 

        public Node()
        {
            parent = null;
        }

        public Node(List<Node> mychildren)
        {
            children = new List<Node>();
            foreach (Node child in mychildren)
            {
                child.parent = this;
                children.Add(child);
            }
        }

        public virtual NodeState Evaluate() => NodeState.FAILURE;  //La declaramos 'virtual' para que cada Nodo luego implemente su propia evaluación


        //Manejo de datos compartidos entre nodos en el árbol

        private Dictionary<string, object> _dataContext = new Dictionary<string, object>();  //Estructura para compartir datos entre los nodos. Diferencia con FSMs

        public void SetData(string key, object value)  //Para asociar un string a un valor que qeramos y se almacene ene l doccionario.
        {
            _dataContext[key] = value;

            // Propagar el dato hacia el nodo padre
            if (parent != null)
            {
                parent.SetData(key, value);
            }
        }

        public object GetData(string key) {
            object value = null;

            //if (_dataContext[key]!= null)) 
            if (_dataContext.TryGetValue(key, out value))
                return value;
            else    //Lo intentamos buscar recursivamente en los nodos superiores
            {
                Node node = parent;
                while (node != null)
                {
                    value = node.GetData(key);
                    if (value != null)
                        return value;

                    node = node.parent;
                }
                return null;
            }
        }

        public bool ClearData(string key)  //Para borra una asociación key-value
        {
            if (_dataContext.ContainsValue(key))
            {
                _dataContext.Remove(key);
                return true;
            }
            else    //Lo intentamos buscar recursivamente en los nodos superiores
            {
                Node node = parent;
                while (node != null)
                {
                    bool cleared = node.ClearData(key);
                    if (cleared)
                        return true;

                    node = node.parent;
                }
                return false;
            }
        }
    } 
} 
