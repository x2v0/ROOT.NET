// $Id: TCanvas.cs 4922 2017-01-18 10:27:11Z onuchin $
// Author: Valeriy Onuchin 12.10.2011

using System;
using System.ComponentModel;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows.Forms;

namespace ROOT
{
   [ToolboxItem(true), Description("ROOT canvas embedded into .NET application")]
   public class TCanvas : UserControl
   {
      #region Constants

      private const int GWL_STYLE = -16;
      private const int SWP_ASYNCWINDOWPOS = 0x4000;
      private const int SWP_FRAMECHANGED = 0x20;
      private const int SWP_NOACTIVATE = 0x10;
      private const int SWP_NOMOVE = 0x2;

      private const int SWP_NOOWNERZORDER = 0x200;
      private const int SWP_NOREDRAW = 0x8;
      private const int SWP_NOSIZE = 0x1;
      private const int SWP_NOZORDER = 0x4;
      private const int SWP_SHOWWINDOW = 0x0040;
      private const int WM_CLOSE = 0x10;
      private const int WS_CHILD = 0x40000000;
      private const int WS_EX_MDICHILD = 0x40;
      private const int WS_VISIBLE = 0x10000000;

      #endregion

      #region Constructors and destructors

      /// <summary>
      ///    Constructor
      /// </summary>
      public TCanvas()
      {
         Name = "c1";
      }

      #endregion

      #region  Fields

      /// <summary>
      ///    Pointer to TCanvas object
      /// </summary>
      private IntPtr fCanvas;

      /// <summary>
      ///    Track if the application has been created
      /// </summary>
      private bool fCreated;

      /// <summary>
      ///    Handle to the Canvas Window
      /// </summary>
      private IntPtr fHandle;

      #endregion

      #region  Delegates

      public delegate void CdCanvasHandler(object canvas, CanvasCdArgs args);

      public delegate void CreateCanvasHandler(object canvas, CanvasCreateEventArgs args);

      public delegate void UpdateCanvasHandler(object canvas, CanvasUpdateArgs args);

      #endregion

      #region Public events

      public event CreateCanvasHandler OnCanvasCreated;
      public event CdCanvasHandler OnCd;
      public event UpdateCanvasHandler OnUpdate;

      #endregion

      #region Public properties

      public IntPtr CanvasHandle
      {
         get { return fHandle; }
      }

      #endregion

      #region Public methods

      [DllImport("ROOTdotNET.dll", EntryPoint = "CreateTCanvas", SetLastError = true, CallingConvention = CallingConvention.Cdecl)]
      public static extern void CreateTCanvas(string name, IntPtr canvas, int w, int h);

      [DllImport("ROOTdotNET.dll", EntryPoint = "DoubleClickCanvas", SetLastError = true, CallingConvention = CallingConvention.Cdecl)]
      public static extern void DoubleClickCanvas(IntPtr canvas);

      [DllImport("ROOTdotNET.dll", EntryPoint = "EnterCanvas", SetLastError = true, CallingConvention = CallingConvention.Cdecl)]
      public static extern void EnterCanvas(IntPtr canvas);

      [DllImport("ROOTdotNET.dll", EntryPoint = "GetCanvasByHandle", SetLastError = true, CallingConvention = CallingConvention.Cdecl)]
      public static extern IntPtr GetCanvasByHandle(IntPtr canvas);

      [DllImport("ROOTdotNET.dll", EntryPoint = "GetCanvasByName", SetLastError = true, CallingConvention = CallingConvention.Cdecl)]
      public static extern IntPtr GetCanvasByName(string name);

      [DllImport("ROOTdotNET.dll", EntryPoint = "LMouseDownCanvas", SetLastError = true, CallingConvention = CallingConvention.Cdecl)]
      public static extern void LMouseDownCanvas(IntPtr canvas, long pos);

      [DllImport("ROOTdotNET.dll", EntryPoint = "LMouseUpCanvas", SetLastError = true, CallingConvention = CallingConvention.Cdecl)]
      public static extern void LMouseUpCanvas(IntPtr canvas, long pos);

      [DllImport("ROOTdotNET.dll", EntryPoint = "MouseMoveCanvas", SetLastError = true, CallingConvention = CallingConvention.Cdecl)]
      public static extern void MouseMoveCanvas(IntPtr canvas, long pos);

      [DllImport("ROOTdotNET.dll", EntryPoint = "ResizeCanvas", SetLastError = true, CallingConvention = CallingConvention.Cdecl)]
      public static extern void ResizeCanvas(IntPtr canvas);

      [DllImport("ROOTdotNET.dll", EntryPoint = "RMouseDownCanvas", SetLastError = true, CallingConvention = CallingConvention.Cdecl)]
      public static extern void RMouseDownCanvas(IntPtr canvas, long pos);

      [DllImport("ROOTdotNET.dll", EntryPoint = "RMouseUpCanvas", SetLastError = true, CallingConvention = CallingConvention.Cdecl)]
      public static extern void RMouseUpCanvas(IntPtr canvas, long pos);

      [DllImport("ROOTdotNET.dll", EntryPoint = "SaveCanvasAs", SetLastError = true, CallingConvention = CallingConvention.Cdecl)]
      public static extern void SaveAs(IntPtr canvas, string name);

      [DllImport("ROOTdotNET.dll", EntryPoint = "UpdateCanvas", SetLastError = true, CallingConvention = CallingConvention.Cdecl)]
      public static extern void UpdateCanvas(IntPtr canvas);

      public virtual void cd()
      {
         EnterCanvas(fCanvas);

         if (OnCd != null) {
            OnCd(this, new CanvasCdArgs());
         }
      }

      /// <summary>
      ///    Save canvas in different image formats:
      ///    if filename is "", the file produced is padname.ps
      ///    if name starts with a dot, the padname is added in front
      ///    if name contains .eps, an Encapsulated Postscript file is produced
      ///    if name contains .pdf, a PDF file is produced
      ///    if name contains .svg, a SVG file is produced
      ///    if name contains .gif, a GIF file is produced
      ///    if name contains .gif+NN, an animated GIF file is produced
      ///    if name contains .xpm, a XPM file is produced
      ///    if name contains .png, a PNG file is produced
      ///    if name contains .jpg, a JPEG file is produced
      ///    NOTE: JPEG's lossy compression will make all sharp edges fuzzy.
      ///    if name contains .C or .cxx, a C++ macro file is produced
      ///    if name contains .root, a Root file is produced
      ///    if name contains .xml, a XML file is produced
      /// </summary>
      /// <param name="name">file name</param>
      public virtual void SaveAs(string name)
      {
         SaveAs(fHandle, name);
      }

      public virtual void UpdateCanvas()
      {
         UpdateCanvas(fCanvas);

         if (OnUpdate != null) {
            OnUpdate(this, new CanvasUpdateArgs());
         }
      }

      #endregion

      #region Protected methods

      /// <summary>
      ///    Handle mouse left button double click events
      /// </summary>
      /// <param name="e">Not used</param>
      protected override void OnDoubleClick(EventArgs e)
      {
         if (!DesignMode) {
            DoubleClickCanvas(fCanvas);
         }
         base.OnDoubleClick(e);
      }

      protected override void OnEnter(EventArgs e)
      {
         base.OnEnter(e);

         if (!DesignMode) {
            EnterCanvas(fCanvas);
            UpdateCanvas(fCanvas);
            //MessageBox.Show("OnEnter - "  + Name);
         }
      }

      /// <summary>
      ///    Called when the main form is closed
      /// </summary>
      /// <param name="e"></param>
      protected override void OnHandleDestroyed(EventArgs e)
      {
         if (!DesignMode) {
            // Stop the application
            if (fHandle != IntPtr.Zero) {
               // Post a close message
               TApplication.PostMessage(fHandle, WM_CLOSE, 0, 0);

               // Delay for it to get the message
               Thread.Sleep(1000);

               // Clear internal handle
               fHandle = IntPtr.Zero;
            }
         }
         base.OnHandleDestroyed(e);
      }

      /// <summary>
      ///    Handle mouse left button down events
      /// </summary>
      /// <param name="e">Not used</param>
      protected override void OnMouseDown(MouseEventArgs e)
      {
         long y = e.Y << 16;
         var pos = e.X + y;

         if (e.Button == MouseButtons.Left) {
            if (!DesignMode) {
               LMouseDownCanvas(fCanvas, pos);
            }
         } else if (e.Button == MouseButtons.Right) {
            if (!DesignMode) {
               //RMouseDownCanvas(fCanvas, pos);
            }
         }
         base.OnMouseDown(e);
      }


      /// <summary>
      ///    Handle mouse moving events
      /// </summary>
      /// <param name="e">Not used</param>
      protected override void OnMouseMove(MouseEventArgs e)
      {
         long y = e.Y << 16;
         var pos = e.X + y;

         if (e.Button == MouseButtons.Left) {
            pos ^= 0x10000000; // left mouse pressed - set the higher bit 
         }
         if (!DesignMode) {
            MouseMoveCanvas(fCanvas, pos);
         }
         base.OnMouseMove(e);
      }

      /// <summary>
      ///    Handle mouse left button up events
      /// </summary>
      /// <param name="e">Not used</param>
      protected override void OnMouseUp(MouseEventArgs e)
      {
         long y = e.Y << 16;
         var pos = e.X + y;

         if (e.Button == MouseButtons.Left) {
            if (!DesignMode) {
               //if ((this.ModifierKeys  & Keys.Shift) == Keys.Shift) {
               //} else {
               LMouseUpCanvas(fCanvas, pos);
               //}
            }
         } else if (e.Button == MouseButtons.Right) {
            if (!DesignMode) {
               //RMouseUpCanvas(fCanvas, pos);
            }
         }
         base.OnMouseUp(e);
      }

      /// <summary>
      ///    Handle Paint events
      /// </summary>
      /// <param name="e">Not used</param>
      protected override void OnPaint(PaintEventArgs e)
      {
         base.OnPaint(e);

         if (!DesignMode) {
            UpdateCanvas(fCanvas);
         }
      }

      /// <summary>
      ///    Update display of the executable
      /// </summary>
      /// <param name="e">Not used</param>
      protected override void OnResize(EventArgs e)
      {
         base.OnResize(e);

         if (!DesignMode) {
            ResizeCanvas(fCanvas);
         }
      }

      /// <summary>
      ///    Force redraw of control when size changes
      /// </summary>
      /// <param name="e">Not used</param>
      protected override void OnSizeChanged(EventArgs e)
      {
         base.OnSizeChanged(e);

         if (!DesignMode) {
            ResizeCanvas(fCanvas);
            UpdateCanvas(fCanvas);
         }
      }

      /// <summary>
      ///    Create control when visibility changes
      /// </summary>
      /// <param name="e">Not used</param>
      protected override void OnVisibleChanged(EventArgs e)
      {
         if (DesignMode) {
            base.OnVisibleChanged(e);
            return;
         }

         if (TApplication.Initialized == false) {
            new TApplication();
         }

         // If control needs to be initialized/created
         if (fCreated == false) {
            // Mark that control is created
            fCreated = true;

            // Initialize handle value to invalid
            fHandle = IntPtr.Zero;

            // Initialize canvas  ptr to invalid
            fCanvas = IntPtr.Zero;
            CreateTCanvas(Name, Handle, Width, Height);
            fCanvas = GetCanvasByName(Name);

            if (fCanvas == IntPtr.Zero) {
               throw new Exception("TCanvas: failed to create ROOT canvas - " + Name);
            }

            if (OnCanvasCreated != null) {
               OnCanvasCreated(this, new CanvasCreateEventArgs(Name, Handle, Width, Height - 20));
            }
         } else {
            if (Visible) {
               EnterCanvas(fCanvas);
               UpdateCanvas(fCanvas);
            }
         }

         base.OnVisibleChanged(e);
      }

      #endregion

      #region Nested classes

      /// <summary>
      ///    Canvas cd args
      /// </summary>
      public class CanvasCdArgs : EventArgs
      {
      }


      /// <summary>
      ///    Canvas events args
      /// </summary>
      public class CanvasCreateEventArgs : EventArgs
      {
         #region Constructors and destructors

         public CanvasCreateEventArgs(string name, IntPtr handle, int w, int h)
         {
            Name = name;
            Handle = handle;
            Width = w;
            Height = h;
         }

         #endregion

         #region  Fields

         public readonly IntPtr Handle;
         public readonly int Height;
         public readonly string Name;
         public readonly int Width;

         #endregion
      }

      /// <summary>
      ///    Canvas Update args
      /// </summary>
      public class CanvasUpdateArgs : EventArgs
      {
      }

      #endregion
   }
}