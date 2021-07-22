// $Id: TApplication.cs 4922 2017-01-18 10:27:11Z onuchin $
// Author: Valeriy Onuchin 12.10.2011

using System;
using System.Runtime.InteropServices;
using System.Threading;

namespace ROOT
{
   /// <summary>
   ///    Class TApplication
   /// </summary>
   public class TApplication
   {
      #region Static fields

      /// <summary>
      ///    The locker
      /// </summary>
      private static readonly object fLocker = new object();

      /// <summary>
      ///    The application init
      /// </summary>
      private static bool gAppInit;

      #endregion

      #region Constructors and destructors

      /// <summary>
      ///    Initializes a new instance of the <see cref="TApplication" /> class.
      /// </summary>
      public TApplication()
      {
         // constructor

         gAppInit = newTApplication();
         fTimer = new Timer(Refresh, null, 0, 20);
      }

      #endregion

      #region  Fields

      /// <summary>
      ///    Refresh timer
      /// </summary>
      private Timer fTimer;

      #endregion

      #region  Delegates

      /// <summary>
      ///    Delegate RootEventHandler
      /// </summary>
      /// <param name="sender">The sender.</param>
      /// <param name="e">The <see cref="RootEventArgs" /> instance containing the event data.</param>
      public delegate void RootEventHandler(object sender, RootEventArgs e);

      #endregion

      #region Public events

      /// <summary>
      ///    Occurs when [on root event].
      /// </summary>
      public static event RootEventHandler OnRootEvent;

      #endregion

      #region Public properties

      /// <summary>
      ///    Gets a value indicating whether this <see cref="TApplication" /> is initialized.
      /// </summary>
      /// <value><c>true</c> if initialized; otherwise, <c>false</c>.</value>
      public static bool Initialized
      {
         get { return gAppInit; }
      }

      #endregion

      #region Public methods

      /// <summary>
      ///    Finds the window.
      /// </summary>
      /// <param name="lpClassName">Name of the lp class.</param>
      /// <param name="lpWindowName">Name of the lp window.</param>
      /// <returns>IntPtr.</returns>
      [DllImport("user32.dll", SetLastError = true)]
      public static extern IntPtr FindWindow(string lpClassName, string lpWindowName);

      /// <summary>
      ///    Gets the window long.
      /// </summary>
      /// <param name="hwnd">The HWND.</param>
      /// <param name="nIndex">Index of the n.</param>
      /// <returns>System.Int64.</returns>
      [DllImport("user32.dll", EntryPoint = "GetWindowLongA", SetLastError = true)]
      public static extern long GetWindowLong(IntPtr hwnd, int nIndex);

      /// <summary>
      ///    Gets the window thread process id.
      /// </summary>
      /// <param name="hWnd">The h WND.</param>
      /// <param name="lpdwProcessId">The LPDW process id.</param>
      /// <returns>System.Int64.</returns>
      [DllImport("user32.dll", EntryPoint = "GetWindowThreadProcessId", SetLastError = true,
         CharSet = CharSet.Unicode, ExactSpelling = true, CallingConvention = CallingConvention.StdCall)]
      public static extern long GetWindowThreadProcessId(long hWnd, long lpdwProcessId);

      /// <summary>
      ///    Moves the window.
      /// </summary>
      /// <param name="hwnd">The HWND.</param>
      /// <param name="x">The x.</param>
      /// <param name="y">The y.</param>
      /// <param name="cx">The cx.</param>
      /// <param name="cy">The cy.</param>
      /// <param name="repaint">if set to <c>true</c> [repaint].</param>
      /// <returns><c>true</c> if XXXX, <c>false</c> otherwise</returns>
      [DllImport("user32.dll", SetLastError = true)]
      public static extern bool MoveWindow(IntPtr hwnd, int x, int y, int cx, int cy, bool repaint);

      /// <summary>
      ///    News the T application.
      /// </summary>
      /// <returns><c>true</c> if XXXX, <c>false</c> otherwise</returns>
      [DllImport("ROOTdotNET.dll", EntryPoint = "newTApplication", SetLastError = true, CallingConvention = CallingConvention.Cdecl)]
      public static extern bool newTApplication();

      /// <summary>
      ///    Posts the message.
      /// </summary>
      /// <param name="hwnd">The HWND.</param>
      /// <param name="msg">The MSG.</param>
      /// <param name="wParam">The w param.</param>
      /// <param name="lParam">The l param.</param>
      /// <returns><c>true</c> if XXXX, <c>false</c> otherwise</returns>
      [DllImport("user32.dll", EntryPoint = "PostMessageA", SetLastError = true)]
      public static extern bool PostMessage(IntPtr hwnd, uint msg, long wParam, long lParam);

      /// <summary>
      ///    The main ROOT processing procedure
      /// </summary>
      /// <param name="state">not used</param>
      public static void Refresh(object state)
      {
         lock (fLocker) {
            TApplicationRefresh();

            if (OnRootEvent != null) {
               OnRootEvent(gAppInit, new RootEventArgs("qq"));
            }
         }
      }

      /// <summary>
      ///    Sets the parent.
      /// </summary>
      /// <param name="hWndChild">The h WND child.</param>
      /// <param name="hWndNewParent">The h WND new parent.</param>
      /// <returns>System.Int64.</returns>
      [DllImport("user32.dll", SetLastError = true)]
      public static extern long SetParent(IntPtr hWndChild, IntPtr hWndNewParent);

      /// <summary>
      ///    Sets the window long.
      /// </summary>
      /// <param name="hwnd">The HWND.</param>
      /// <param name="nIndex">Index of the n.</param>
      /// <param name="dwNewLong">The dw new long.</param>
      /// <returns>System.Int64.</returns>
      [DllImport("user32.dll", EntryPoint = "SetWindowLongA", SetLastError = true)]
      public static extern long SetWindowLong(IntPtr hwnd, int nIndex, long dwNewLong);

      /// <summary>
      ///    Sets the window pos.
      /// </summary>
      /// <param name="hwnd">The HWND.</param>
      /// <param name="hWndInsertAfter">The h WND insert after.</param>
      /// <param name="x">The x.</param>
      /// <param name="y">The y.</param>
      /// <param name="cx">The cx.</param>
      /// <param name="cy">The cy.</param>
      /// <param name="wFlags">The w flags.</param>
      /// <returns>System.Int64.</returns>
      [DllImport("user32.dll", SetLastError = true)]
      public static extern long SetWindowPos(IntPtr hwnd, long hWndInsertAfter, long x, long y, long cx, long cy,
         long wFlags);

      /// <summary>
      ///    Ts the application refresh.
      /// </summary>
      /// <returns>System.Int32.</returns>
      [DllImport("ROOTdotNET.dll", EntryPoint = "TApplicationRefresh", SetLastError = true)]
      public static extern int TApplicationRefresh();

      #endregion

      #region Nested classes

      /// <summary>
      ///    Class RootEventArgs
      /// </summary>
      public class RootEventArgs : EventArgs
      {
         #region Constructors and destructors

         /// <summary>
         ///    Initializes a new instance of the <see cref="RootEventArgs" /> class.
         /// </summary>
         /// <param name="value">The value.</param>
         public RootEventArgs(string value)
         {
            Value = value;
         }

         #endregion

         #region  Fields

         /// <summary>
         ///    The value
         /// </summary>
         public readonly string Value;

         #endregion
      }

      #endregion
   }
}