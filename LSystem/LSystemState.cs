// Copyright 2016 afuzzyllama. All Rights Reserved.
namespace PixelsForGlory.ComputationalSystem
{
    /// <summary>
    /// L-System State.  Passed between nodes as the current state of the system.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class LSystemState<T>
    {
        /// <summary>
        /// Current state of the L-System 
        /// </summary>
        public readonly T CurrentState;

        /// <summary>
        /// Creates an l-system state and deep copies the state passed in
        /// </summary>
        /// <param name="initialState"></param>
        public LSystemState(T initialState)
        {
            var copy = initialState as ICopy<T>;
            if(copy != null)
            {
                var iCopyState = copy;
                CurrentState = iCopyState.DeepCopy();
            }
            else
            {
                CurrentState = initialState;
            }
        }
    }
}
