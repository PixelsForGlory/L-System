// Copyright 2016 afuzzyllama. All Rights Reserved.
using System.Collections.Generic;

namespace PixelsForGlory.ComputationalSystem
{
    /// <summary>
    /// L-System node
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class LSystemNode<T>
    {
        /// <summary>
        /// Module that node represents
        /// </summary>
        public ILSystemModule<T> NodeModule;

        /// <summary>
        /// Optional supporting branch off this node.  This is what is usually represented in brackets.
        /// </summary>
        public LinkedList<LSystemNode<T>> SupportingBranch;

        /// <summary>
        /// What step number was this node created in
        /// </summary>
        public int CreatedStepNumber { get; private set; }

        public LSystemNode(int stepNumber, ILSystemModule<T> nodeModule, LinkedList<LSystemNode<T>> supportingBranch = null)
        {
            CreatedStepNumber = stepNumber;
            NodeModule = nodeModule;
            SupportingBranch = supportingBranch;
        }
    }
}
