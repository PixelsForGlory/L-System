// Copyright 2016 afuzzyllama. All Rights Reserved.
namespace PixelsForGlory.ComputationalSystem
{
    /// <summary>
    /// Interface that has explicit types of copys to product
    /// </summary>
    /// <typeparam name="T">Return type of the copy</typeparam>
    public interface ICopy<T>
    {
        T ShallowCopy();
        T DeepCopy();
    }
}