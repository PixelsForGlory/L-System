// Copyright 2016 afuzzyllama. All Rights Reserved.
namespace PixelsForGlory.ComputationalSystem
{
    /// <summary>
    /// L-System module
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class LSystemModule<T>
    {
        /// <summary>
        /// Data that module contains
        /// </summary>
        public T Data;

        protected LSystemModule(T data)
        {
            Data = data;
        }

        /// <summary>
        /// Function of how this modules changes the system state
        /// </summary>
        /// <param name="systemState"></param>
        public abstract void ChangeState(LSystemState<T> systemState);
    }

    /// <summary>
    /// L-System query module
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class LSystemQueryModule<T> : LSystemModule<T>
    {
        protected LSystemQueryModule(T data) : base(data) { }

        /// <summary>
        /// Query the system state for parameters
        /// </summary>
        /// <param name="systemState"></param>
        /// <returns></returns>
        public abstract void QueryState(LSystemState<T> systemState);
    }
}
