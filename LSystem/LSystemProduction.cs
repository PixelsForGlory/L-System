// Copyright 2016 afuzzyllama. All Rights Reserved.
using System;
using System.Collections.Generic;

namespace PixelsForGlory.ComputationalSystem
{
    /// <summary>
    /// Context sensitive production
    /// </summary>
    public abstract class LSystemProduction<T>
    {
        /// <summary>
        /// Module type for the left context.  Null if no left context.
        /// </summary>
        public readonly Type LeftContext;

        /// <summary>
        /// Module type for the predecessor.
        /// </summary>
        public readonly Type StrictPredecessor;

        /// <summary>
        /// Module type for the right context.  Null if no right context.
        /// </summary>
        public readonly Type RightContext;

        protected LSystemProduction(Type leftContext, Type strictPredecessor, Type rightContext)
        {
            LeftContext = leftContext;
            StrictPredecessor = strictPredecessor;
            RightContext = rightContext;
        }

        /// <summary>
        /// Gets the successor of the current node
        /// </summary>
        /// <param name="stepNumber"></param>
        /// <param name="currentNode"></param>
        /// <returns>Returns new node if one exists, null otherwise</returns>
        public List<LSystemNode<T>> GetSuccessor(int stepNumber, LinkedListNode<LSystemNode<T>> currentNode)
        {
            if(currentNode == null)
            {
                return null;
            }

            if(currentNode.Value.CreatedStepNumber == stepNumber)
            {
                return null;
            }

            if(currentNode.Value.NodeModule.GetType() != StrictPredecessor)
            {
                return null;
            }

            if(LeftContext != null && currentNode.Previous != null && currentNode.Previous.Value.NodeModule.GetType() != LeftContext)
            {
                return null;
            }

            if(RightContext != null && currentNode.Next != null && currentNode.Next.Value.NodeModule.GetType() != RightContext)
            {
                return null;
            }

            if(Condition(currentNode) == false)
            {
                return null;
            }

            if(Probability(currentNode) == false)
            {
                return null;
            }

            return CreateSuccessor(stepNumber, currentNode);
        }

        /// <summary>
        /// Determines if the conditions are correct for replacing the predecessor with the successor
        /// </summary>
        /// <param name="predecessor"></param>
        /// <returns></returns>
        protected abstract bool Condition(LinkedListNode<LSystemNode<T>> predecessor);

        /// <summary>
        /// Determines the probabilty that the predecessor will replace the successor
        /// </summary>
        /// <param name="predecessor"></param>
        /// <returns></returns>
        protected abstract bool Probability(LinkedListNode<LSystemNode<T>> predecessor);

        /// <summary>
        /// Creates a new successor for the predecessor
        /// </summary>
        /// <param name="stepNumber">Step that successor is being created in</param>
        /// <param name="predecessor"></param>
        /// <returns></returns>
        protected abstract List<LSystemNode<T>> CreateSuccessor(int stepNumber, LinkedListNode<LSystemNode<T>> predecessor);

    }
}
