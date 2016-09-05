// Copyright 2016 afuzzyllama. All Rights Reserved.
using System.Collections.Generic;

namespace PixelsForGlory.ComputationalSystem
{
    /// <summary>
    /// L-System
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class LSystem<T>
    {
        /// <summary>
        /// Stores the current derivation of the l-system
        /// </summary>
        private readonly LinkedList<LSystemNode<T>> _currentDerivation;

        /// <summary>
        /// Productions to run 
        /// </summary>
        private readonly List<LSystemProduction<T>> _productions;

        /// <summary>
        /// How many times the productions have been run on the system
        /// </summary>
        private int _currentStepNumber;

        public LSystem(IList<LSystemNode<T>> axiom, IList<LSystemProduction<T>> productions)
        {
            _currentStepNumber = 0;
            _currentDerivation = new LinkedList<LSystemNode<T>>(axiom);
            _productions = new List<LSystemProduction<T>>(productions);
        }

        /// <summary>
        /// Runs the provided productions
        /// </summary>
        public void RunProduction()
        {
            _currentStepNumber += 1;
            foreach(var production in _productions)
            {
                var currentNode = _currentDerivation.First;
                while(currentNode != null)
                {
                    var newNodes = production.GetSuccessor(_currentStepNumber, currentNode);
                    if(newNodes != null)
                    {
                        // Add each new node before the current node.  This preserves order
                        foreach(var newNode in newNodes)
                        {
                            _currentDerivation.AddBefore(currentNode, newNode);
                        }

                        // Remove the current node and continue with the last new node
                        var removeNode = currentNode;
                        currentNode = currentNode.Previous;
                        _currentDerivation.Remove(removeNode);
                    }

                    if(currentNode == null)
                    {
                        break;
                    }

                    currentNode = currentNode.Next;
                }
            }
        }

        /// <summary>
        /// Gets the current derivation of the l-system
        /// </summary>
        /// <returns></returns>
        public LinkedList<LSystemNode<T>> GetCurrentDerivation()
        {
            var initialState = new LSystemState<T>(default(T));

            UpdateList(_currentDerivation.First, initialState);
            return _currentDerivation;
        }

        /// <summary>
        /// Gets the current derivation of the l-system with a starting parameter
        /// </summary>
        /// <param name="initialState"></param>
        /// <returns></returns>
        public LinkedList<LSystemNode<T>> GetCurrentDerivation(LSystemState<T> initialState)
        {
            UpdateList(_currentDerivation.First, initialState);
            return _currentDerivation;
        }

        private static void UpdateList(LinkedListNode<LSystemNode<T>> linkedListNode, LSystemState<T> state)
        {
            while(linkedListNode != null)
            {
                linkedListNode.Value.NodeModule?.ChangeState(state);

                if(linkedListNode.Value.NodeModule is LSystemQueryModule<T>)
                {
                    var queryModule = (LSystemQueryModule<T>) linkedListNode.Value.NodeModule;
                    queryModule.QueryState(state);
                }

                if(linkedListNode.Value.SupportingBranch != null)
                {
                    var currentState = new LSystemState<T>(state.CurrentState);
                    UpdateList(linkedListNode.Value.SupportingBranch.First, currentState);
                }

                linkedListNode = linkedListNode.Next;
            }
        }
    }
}
