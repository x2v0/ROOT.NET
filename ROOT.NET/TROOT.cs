// $Id: TROOT.cs 4922 2017-01-18 10:27:11Z onuchin $
// Author: Valeriy Onuchin 12.10.2011

using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Security.Cryptography;

namespace ROOT
{
   /// <summary>
   ///    Class TROOT
   /// </summary>
   public static class TROOT
   {
      #region Static fields

      /// <summary>
      ///    The fLocker
      /// </summary>
      private static readonly object fLocker = new object();

      /// <summary>
      ///    The fLoaded macros
      /// </summary>
      private static Dictionary<string, string> fLoadedMacros;

      #endregion

      #region Constructors and destructors

      /// <summary>
      ///    Static constructor
      /// </summary>
      static TROOT()
      {
         //LoadROOTdotNET();
         new TApplication();
      }

      #endregion

      #region Public methods

      /// <summary>
      ///    Executes the root macro.
      /// </summary>
      /// <param name="macro">The macro.</param>
      /// <returns>System.Int64.</returns>
      [DllImport("ROOTdotNET.dll", EntryPoint = "ExecuteRootMacro", SetLastError = true, CallingConvention = CallingConvention.Cdecl)]
      public static extern long ExecuteRootMacro(string macro);

      /// <summary>
      ///    Process data by ROOT method or function which has a signature "long MyFunc(char *data, int len)"
      /// </summary>
      /// <param name="method">ROOT function name or obj-&gt;Method to be invoked</param>
      /// <param name="data">Data to  be processed as bytes array</param>
      /// <param name="len">The length of bytes array</param>
      /// <returns>The result. Note, it can be a pointer to some object</returns>
      public static long ExportData(string method, byte[] data, int len)
      {
         long ret;

         lock (fLocker) {
            var pin = GCHandle.Alloc(data, GCHandleType.Pinned);

            try {
               var ptr = Marshal.UnsafeAddrOfPinnedArrayElement(data, 0);
               ret = ProcessRootLine(string.Format(method + "((char*){0}, {1})", ptr, len));
            } finally {
               pin.Free();
            }
         }

         return ret;
      }

      /// <summary>
      ///    This method returns an byte array of serialized ROOT object
      ///    or an array specified by it's name
      /// </summary>
      /// <param name="method">The method.</param>
      /// <param name="data">The data.</param>
      /// <returns>Returns 0 in case of success;</returns>
      public static long ImportData(string method, out byte[] data)
      {
         long ret = 3;

         lock (fLocker) {
            const string code1 = "\n" + "  static void *ptr2RootData = 0;\n" +
                                 "  static int   sizeRootData = 0;\n\n";
            const string code2 = "\n" + "   if ((ptr2RootData == 0) && (sizeRootData > 0)) {\n" +
                                 "      ptr2RootData = new char[sizeRootData];\n" + "   }\n\n";

            ProcessRootLineSync(code1);
            ProcessRootLine(string.Format(method + "(&sizeRootData, ptr2RootData)"));

            ProcessRootLineSync(code2);

            if (ProcessRootLine("ptr2RootData") == 0) {
               ret = ProcessRootLine(string.Format(method + "(&sizeRootData, ptr2RootData)"));
            }

            var sz = ProcessRootLine("sizeRootData");

            data = new byte[sz];
         }

         return ret;
      }


      /// <summary>
      ///    Process void ROOT method or function
      /// </summary>
      /// <param name="method">ROOT function name or obj-&gt;Method to be invoked</param>
      /// <returns>The result of execution</returns>
      public static long Invoke(string method)
      {
         long ret;

         lock (fLocker) {
            ret = ProcessRootLine(string.Format(method + "()"));
         }

         return ret;
      }

      /// <summary>
      ///    Process ROOT method or function with a single argument
      /// </summary>
      /// <param name="method">ROOT function name or obj-&gt;Method to be invoked</param>
      /// <param name="param1">The parameter to pass</param>
      /// <returns>The result of execution</returns>
      public static long Invoke(string method, object param1)
      {
         long ret;

         lock (fLocker) {
            var pin = GCHandle.Alloc(param1, GCHandleType.Pinned);

            try {
               ret = ProcessRootLine(string.Format(method + "({0})", param1));
            } finally {
               pin.Free();
            }
         }

         return ret;
      }

      /// <summary>
      ///    Process ROOT method or function with two arguments
      /// </summary>
      /// <param name="method">ROOT function name or obj-&gt;Method to be invoked</param>
      /// <param name="param1">The first parameter</param>
      /// <param name="param2">The second parameter</param>
      /// <returns>The result of execution</returns>
      public static long Invoke(string method, object param1, object param2)
      {
         long ret;

         lock (fLocker) {
            var pin1 = GCHandle.Alloc(param1, GCHandleType.Pinned);
            var pin2 = GCHandle.Alloc(param2, GCHandleType.Pinned);

            try {
               ret = ProcessRootLine(string.Format(method + "({0},{1})", param1, param2));
            } finally {
               pin1.Free();
               pin2.Free();
            }
         }

         return ret;
      }

      /// <summary>
      ///    Process ROOT method or funciton with three arguments
      /// </summary>
      /// <param name="method">ROOT function name or obj-&gt;Method to be invoked</param>
      /// <param name="param1">The first parameter</param>
      /// <param name="param2">The second parameter</param>
      /// <param name="param3">The third parameter</param>
      /// <returns>The result of execution</returns>
      public static long Invoke(string method, object param1, object param2, object param3)
      {
         long ret;

         lock (fLocker) {
            var pin1 = GCHandle.Alloc(param1, GCHandleType.Pinned);
            var pin2 = GCHandle.Alloc(param2, GCHandleType.Pinned);
            var pin3 = GCHandle.Alloc(param2, GCHandleType.Pinned);

            try {
               ret = ProcessRootLine(string.Format(method + "({0},{1},{2})", param1, param2, param3));
            } finally {
               pin1.Free();
               pin2.Free();
               pin3.Free();
            }
         }

         return ret;
      }


      /// <summary>
      ///    Process ROOT method or funciton which with four arguments
      /// </summary>
      /// <param name="method">ROOT function name or obj-&gt;Method to be invoked</param>
      /// <param name="param1">The first parameter</param>
      /// <param name="param2">The second parameter</param>
      /// <param name="param3">The third parameter</param>
      /// <param name="param4">The 4th parameter</param>
      /// <returns>The result of execution</returns>
      public static long Invoke(string method, object param1, object param2, object param3, object param4)
      {
         long ret;

         lock (fLocker) {
            var pin1 = GCHandle.Alloc(param1, GCHandleType.Pinned);
            var pin2 = GCHandle.Alloc(param2, GCHandleType.Pinned);
            var pin3 = GCHandle.Alloc(param2, GCHandleType.Pinned);
            var pin4 = GCHandle.Alloc(param2, GCHandleType.Pinned);

            try {
               ret = ProcessRootLine(string.Format(method + "({0},{1},{2},{3})", param1, param2, param3, param4));
            } finally {
               pin1.Free();
               pin2.Free();
               pin3.Free();
               pin4.Free();
            }
         }
         return ret;
      }

      /// <summary>
      ///    Load a shared library embedded into resource of the executable program
      /// </summary>
      /// <param name="resource">The name of embedded resource in the executable program</param>
      /// <param name="dll">The name of ROOT dll. It might be compiled macro with ACLiC</param>
      /// <returns>Returns 0 on successful loading, 1 in case lib was already loaded</returns>
      /// <exception cref="System.Exception"></exception>
      public static int LoadLib(string resource, string dll)
      {
         if (resource == null) {
            return -1;
         }

         var assembly = Assembly.GetCallingAssembly();

         byte[] ba;
         var path = resource + "." + dll;
         int ret;

         using (var strm = assembly.GetManifestResourceStream(path)) {
            if (strm == null) {
               throw new Exception(path + " is not found in embedded resources.");
            }

            ba = new byte[(int) strm.Length];
            strm.Read(ba, 0, (int) strm.Length);
         }

         var tempFile = Path.GetTempFileName() + ".dll";
         File.WriteAllBytes(tempFile, ba);

         lock (fLocker) {
            ret = LoadRootDLL(tempFile);
         }

         // delete temporary file 
         File.Delete(tempFile);

         return ret;
      }

      /// <summary>
      ///    Load a shared library
      /// </summary>
      /// <param name="dll">The DLL.</param>
      /// <returns>Returns 0 on successful loading, 1 in case lib was already loaded</returns>
      public static int LoadLib(string dll)
      {
         int ret;

         lock (fLocker) {
            var path = Path.GetDirectoryName(dll);

            if (string.IsNullOrEmpty(path)) {
               path = Path.GetDirectoryName(Assembly.GetCallingAssembly().CodeBase);
            }

            ret = LoadRootDLL(path + "\\" + dll);
         }

         return ret;
      }

      /// <summary>
      ///    Load a macro in the interpreter's memory. Equivalent to the command line
      ///    command ".L filename". If the filename has "+" or "++" appended
      ///    the macro will be compiled by ACLiC. The filename must have the format:
      ///    [path/]macro.C[+|++[g|O]].
      ///    The possible error codes are defined by TInterpreter::EErrorCode.
      ///    If check is true it will only check if filename exists and is
      ///    readable.
      /// </summary>
      /// <param name="macro">ROOT macro filename</param>
      /// <returns>
      ///    Returns 0 on successful loading and -1 in case filename does not
      ///    exist or in case of error.
      /// </returns>
      public static int LoadMacro(string macro)
      {
         int ret;

         lock (fLocker) {
            ret = LoadRootMacro(macro);
         }

         return ret;
      }

      /// <summary>
      ///    Load macro embedded into resource of the executable program
      /// </summary>
      /// <param name="resource">The name of embedded resource in the executable program</param>
      /// <param name="macro">The name of macro file</param>
      /// <returns>
      ///    Returns 0 on successful loading and -1 in case filename does not
      ///    exist or in case of error.
      /// </returns>
      /// <exception cref="System.Exception"></exception>
      public static int LoadMacro(string resource, string macro)
      {
         var path = resource + "." + macro;

         lock (fLocker) {
            if (fLoadedMacros == null) {
               fLoadedMacros = new Dictionary<string, string>();
            }

            var assembly = Assembly.GetEntryAssembly();

            int ret;
            using (var stream = assembly.GetManifestResourceStream(path)) {
               if (stream == null) {
                  throw new Exception(path + " is not found in embedded resources.");
               }

               var textStreamReader = new StreamReader(stream);
               var macroCode = textStreamReader.ReadToEnd();

               var tmpfile = Path.GetTempFileName();
               var writer = new StreamWriter(tmpfile);
               writer.Write(macroCode);
               writer.Close();

               ret = LoadRootMacro(tmpfile);
               fLoadedMacros.Add(macro, tmpfile);
            }

            return ret;
         }
      }

      /// <summary>
      ///    Loads the root DLL.
      /// </summary>
      /// <param name="lib">The lib.</param>
      /// <returns>System.Int32.</returns>
      [DllImport("ROOTdotNET.dll", EntryPoint = "LoadRootDLL", SetLastError = true, CallingConvention = CallingConvention.Cdecl)]
      public static extern int LoadRootDLL(string lib);

      /// <summary>
      ///    Loads the root macro.
      /// </summary>
      /// <param name="macro">The macro.</param>
      /// <returns>System.Int32.</returns>
      [DllImport("ROOTdotNET.dll", EntryPoint = "LoadRootMacro", SetLastError = true, CallingConvention = CallingConvention.Cdecl)]
      public static extern int LoadRootMacro(string macro);

      /// <summary>
      ///    Execute a macro in the interpreter. Equivalent to the command line
      ///    command ".x filename". If the filename has "+" or "++" appended
      ///    the macro will be compiled by ACLiC. The filename must have the format:
      ///    [path/]macro.C[+|++[g|O]][(args)].
      ///    The possible error codes are defined by TInterpreter::EErrorCode.
      ///    If padUpdate is true (default) update the current pad.
      /// </summary>
      /// <param name="macro">ROOT macro filename</param>
      /// <returns>Returns the macro return value.</returns>
      public static long Macro(string macro)
      {
         long ret;

         lock (fLocker) {
            ret = ExecuteRootMacro(macro);
         }

         return ret;
      }


      /// <summary>
      ///    Allows to execute ROOT line
      /// </summary>
      /// <param name="line">CINT line to execute</param>
      /// <returns>Returns 0 in case of success;</returns>
      public static long ProcessLine(string line)
      {
         long ret;

         lock (fLocker) {
            ret = ProcessRootLine(line);
         }
         return ret;
      }

      /// <summary>
      ///    Processes the root line.
      /// </summary>
      /// <param name="line">The line.</param>
      /// <returns>System.Int64.</returns>
      [DllImport("ROOTdotNET.dll", EntryPoint = "ProcessRootLine", SetLastError = true, CallingConvention = CallingConvention.Cdecl)]
      public static extern long ProcessRootLine(string line);

      /// <summary>
      ///    Processes the root line sync.
      /// </summary>
      /// <param name="line">The line.</param>
      /// <returns>System.Int64.</returns>
      [DllImport("ROOTdotNET.dll", EntryPoint = "ProcessRootLineSync", SetLastError = true, CallingConvention = CallingConvention.Cdecl)]
      public static extern long ProcessRootLineSync(string line);

      #endregion

      #region Private methods

      /// <summary>
      ///    Test loading embedded ROOTdotNET.dll ... not used (because unmanaged)
      /// </summary>
      private static void LoadROOTdotNET()
      {
         var assembly = Assembly.GetCallingAssembly();

         var dll = "ROOTdotNET.dll";
         bool fileOk;
         string tempFile;
         byte[] ba;
         var path = assembly.GetName().Name + "." + dll;

         using (var strm = assembly.GetManifestResourceStream(path)) {
            ba = new byte[(int) strm.Length];
            strm.Read(ba, 0, (int) strm.Length);
         }

         using (var sha1 = new SHA1CryptoServiceProvider()) {
            var fileHash = BitConverter.ToString(sha1.ComputeHash(ba)).Replace("-", string.Empty);
            ;

            tempFile = Path.GetTempPath() + dll;

            if (File.Exists(tempFile)) {
               var bb = File.ReadAllBytes(tempFile);
               var fileHash2 = BitConverter.ToString(sha1.ComputeHash(bb)).Replace("-", string.Empty);

               if (fileHash == fileHash2) {
                  fileOk = true;
               } else {
                  fileOk = false;
               }
            } else {
               fileOk = false;
            }
         }

         if (!fileOk) {
            File.WriteAllBytes(tempFile, ba);
         }

         Assembly.LoadFile(tempFile);
      }

      #endregion
   }
}