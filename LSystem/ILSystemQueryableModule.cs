// Copyright 2016 afuzzyllama. All Rights Reserved.
namespace PixelsForGlory.ComputationalSystem
{
    /// <summary>
    /// L-System module
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface ILSystemQueryableModule<T> : ILSystemModule<T>
    {
        /// <summary>
        /// Query the system state for parameters
        /// </summary>
        /// <param name="systemState"></param>
        /// <returns></returns>
        void QueryState(LSystemState<T> systemState);
    }
}
