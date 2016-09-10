// Copyright 2016 afuzzyllama. All Rights Reserved.
namespace PixelsForGlory.ComputationalSystem
{
    /// <summary>
    /// L-System module
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface ILSystemModule<T>
    {
        /// <summary>
        /// Function of how this modules changes the system state
        /// </summary>
        /// <param name="systemState"></param>
        void ChangeState(LSystemState<T> systemState);
    }
}
