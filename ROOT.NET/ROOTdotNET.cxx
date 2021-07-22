// $Id: ROOTdotNET.cxx 4923 2017-01-18 11:52:04Z onuchin $
// Author: Valeriy Onuchin 11.10.11

#pragma warning(disable:4244)    // conversion from
#pragma warning(disable:4702)    // unreachable code
#pragma warning(disable:4996)    // unsafe CRT
#pragma warning(disable:4800)    // forcing value to another type

#include <windows.h>

#include "Windows4Root.h"
#include "TSystem.h"
#include "TApplication.h"
#include "TROOT.h"
#include "TList.h"
#include "TCanvas.h"
#include "TVirtualX.h"
#include "TAxis.h"
#include "TImage.h"
#include "TEnv.h"
#include "TError.h"

#include <stdio.h>

//using namespace System;
//using namespace System::Windows::Forms;

//______________________________________________________________________________
bool newTApplication()
{
   // Creates ROOT Application

   if (!gApplication) {
      TApplication *gMyRootApp = new TApplication("ROOT Application", 0, 0);

      // tell application to return from run
      gMyRootApp->SetReturnFromRun(true);
   }

   return kTRUE;
}

//______________________________________________________________________________
void TApplicationRefresh()
{
   // Process Root events when a timer message is received

   gApplication->StartIdleing();
   gSystem->InnerLoop();
   gApplication->StopIdleing();

   return;
}

//______________________________________________________________________________
long ExecuteRootMacro(const char *macro)
{
   // execute ROOT C++ macro.  Returns 0 in case of success;

   return gROOT ? gROOT->Macro(macro) : -1;
}

//______________________________________________________________________________
long ProcessRootLine(const char *line)
{
   // Process interpreter command directly via CINT interpreter.
   // Only executable statements are allowed (no variable declarations),
   // In all other cases use TROOT::ProcessLine().  Returns 0 in case of success;

   return gROOT ? gROOT->ProcessLineFast(line) : -1;
}

//______________________________________________________________________________
long ProcessRootLineSync(const char *line)
{
   // Process interpreter command via TApplication::ProcessLine().
   // The line will be processed synchronously (i.e. it will
   // only return when the CINT interpreter thread has finished executing the line
   // Variable declarations allowed. Returns 0 in case of success;

   return gROOT ? gROOT->ProcessLineSync(line) : -1;
}

//______________________________________________________________________________
int LoadRootMacro(const char *macro)
{
   // Load ROOT macro.  Returns 0 in case of success;

   return gROOT ? gROOT->LoadMacro(macro) : -1;
}

//______________________________________________________________________________
int LoadRootDLL(const char *dll)
{
   // Load ROOT DLL.  Returns 0 in case of success;

   if (!dll || !strlen(dll)) {
      return -2;
   }

   TString dir = dll;

   if (!dir.BeginsWith("file:\\")) {
      return gSystem ? gSystem->Load(dir) : -1;
   }

   dir.ReplaceAll("file:\\", "");
   TString name = gSystem->BaseName(dll);
   dir.Remove(dir.Length() - name.Length());
   gSystem->AddDynamicPath(dir);
   ::Warning("LoadRootDLL", "dll - %s dynamic path %s\n", dll, gSystem->GetDynamicPath());

   return gSystem ? gSystem->Load(name) : -1;
}

//
// obligatory implementation of TCanvas functions
//______________________________________________________________________________
void LMouseDownCanvas(void *ptr, Long64_t pos)
{
   // Handle Mouse Left button down event

   TCanvas *canvas = (TCanvas*)ptr;

   if (canvas) {
      int x = int(pos & 0xffff);
      int y = int(pos >> 16);

      canvas->HandleInput(kButton1Down, x, y);
   }
}

//______________________________________________________________________________
void LMouseUpCanvas(void *ptr, Long64_t pos)
{
   // Handle Mouse Left button uo event

   TCanvas *canvas = (TCanvas*)ptr;

   if (canvas) {
      int x = int(pos & 0xffff);
      int y = int(pos >> 16);

      canvas->HandleInput(kButton1Up, x, y);
   }
}

//______________________________________________________________________________
void RMouseDownCanvas(void *ptr, Long64_t pos)
{
   // Handle Mouse Right button down event

   TCanvas *canvas = (TCanvas*)ptr;

   if (canvas) {
      int x = int(pos & 0xffff);
      int y = int(pos >> 16);

      canvas->HandleInput(kButton3Down, x, y);
   }
}

//______________________________________________________________________________
void DoubleClickCanvas(void *ptr)
{
   // handle double click in canvas

   TCanvas *canvas = (TCanvas*)ptr;

   if (canvas) {
      TObject *o = canvas->GetSelected();

      if (o && o->InheritsFrom(TAxis::Class())) {
         TAxis *axis = (TAxis*)o;
         axis->UnZoom();
         canvas->Modified();
         canvas->Update();
      } else if (o && o->InheritsFrom(TImage::Class())) {
         TImage *img = (TImage*)o;
         img->UnZoom();
         canvas->Modified();
         canvas->Update();
      }  
   }
}

//______________________________________________________________________________
void RMouseUpCanvas(void *ptr, Long64_t pos)
{
   // Handle Mouse Right button up event

   TCanvas *canvas = (TCanvas*)ptr;

   if (canvas) {
      int x = int(pos & 0xffff);
      int y = int(pos >> 16);

      canvas->HandleInput(kButton3Up, x, y);
   }
}

//______________________________________________________________________________
void MouseMoveCanvas(void *ptr, Long64_t pos)
{
   // Handle Mouse move event

   TCanvas *canvas = (TCanvas*)ptr;

   if (!canvas) {
      return;
   }

   Bool_t pressed = Bool_t(pos & 0x10000000);

   int x = int(pos & 0xffff);
   pos &= 0x0fffffff;
   int y = int(pos >> 16);

   if (pressed) {
      canvas->HandleInput(kButton1Motion, x, y);
   } else {
      canvas->HandleInput(kMouseMotion, x, y);
   }
}

//______________________________________________________________________________
void EnterCanvas(void *ptr)
{
   // change the current canvas

   TCanvas *canvas = (TCanvas*)ptr;

   if (canvas) {
      canvas->cd();
   }
}

//______________________________________________________________________________
void UpdateCanvas(void *ptr)
{
   // Handle Paint events

   TCanvas *canvas = (TCanvas*)ptr;

   if (canvas) {
      canvas->Modified();
      canvas->Update();
   }
}

//______________________________________________________________________________
void ResizeCanvas(void *ptr)
{
   // Handle Resize  events

   TCanvas *canvas = (TCanvas*)ptr;

   if (canvas) {
      canvas->Resize();
   }
}

//______________________________________________________________________________
void *GetCanvasByHandle(void *ptr)
{
   // Returns canvas HWND handle

   if (!ptr || !gVirtualX) {
      return 0;
   }

   return (void*)gVirtualX->GetWindowID(((TCanvas*)ptr)->GetCanvasID());
}

//______________________________________________________________________________
void *GetCanvasByName(char *name)
{
   // Returns pointer to canvas by canvas name

   if (!gROOT) {
      return 0;
   }

   return gROOT->GetListOfCanvases()->FindObject(name);
}

//______________________________________________________________________________
void CreateTCanvas(char *name, void *hwd, int w, int h)
{
   // Create new embedded ROOT canvas

   if (!gVirtualX) {
      return;
   }

   int wid = gVirtualX->AddWindow((ULong_t)hwd, w, h);
   TCanvas *c1 = new TCanvas(name, w, h, wid);
}

//______________________________________________________________________________
void SaveCanvasAs(void *ptr, const char *name)
{
   // Save canvas as image, macro or root file

   TCanvas *canvas = (TCanvas*)ptr;

   if (canvas) {
      canvas->SaveAs(name);
   }
}
