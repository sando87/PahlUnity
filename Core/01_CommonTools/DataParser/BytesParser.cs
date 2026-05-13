using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
using System.Runtime.InteropServices;
using System;

namespace PahlUnity
{
    public static class BytesParser
    {
        static public byte[] Serialize(object obj)
        {
            try
            {
                int size = Marshal.SizeOf(obj);
                byte[] arr = new byte[size];
                IntPtr ptr = Marshal.AllocHGlobal(size);
                Marshal.StructureToPtr(obj, ptr, true);
                Marshal.Copy(ptr, arr, 0, size);
                Marshal.FreeHGlobal(ptr);
                return arr;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        static public T Deserialize<T>(ref byte[] data, int off = 0) where T : new()
        {
            try
            {
                T str = new T();
                int size = Marshal.SizeOf(str);
                IntPtr ptr = Marshal.AllocHGlobal(size);
                Marshal.Copy(data, off, ptr, size);
                str = (T)Marshal.PtrToStructure(ptr, str.GetType());
                Marshal.FreeHGlobal(ptr);
                return str;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        static public int Sizeof<T>()
        {
            return Marshal.SizeOf(typeof(T));
        }
    }
}