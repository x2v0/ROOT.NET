// $Id: hsimple.NET.cxx 6 2011-07-07 05:59:41Z onuchin $

#pragma warning(disable:4244)    // conversion from
#pragma warning(disable:4702)    // unreachable code
#pragma warning(disable:4996)    // unsafe CRT
#pragma warning(disable:4800)    // forcing value to another type

#include "TCanvas.h"
#include "TVirtualX.h"
#include "TROOT.h"
#include "TFrame.h"
#include <TFile.h>
#include <TNtuple.h>
#include <TH2.h>
#include <TProfile.h>
#include <TSystem.h>
#include <TRandom.h>
#include <TBenchmark.h>
#include <TInterpreter.h>


//using namespace System;
//using namespace System::Windows::Forms;


TCanvas *c1 = 0;


//______________________________________________________________________________
void hsimple(const char *cname)
{

      Int_t get=0;
//  This program creates :
//    - a one dimensional histogram
//    - a two dimensional histogram
//    - a profile histogram
//    - a memory-resident ntuple
//
//  These objects are filled with some random numbers and saved on a file.
//  If get=1 the macro returns a pointer to the TFile of "hsimple.root"
//          if this file exists, otherwise it is created.
//  The file "hsimple.root" is created in $ROOTSYS/tutorials if the caller has
//  write access to this directory, otherwise the file is created in $PWD

   c1 = (TCanvas*)gROOT->GetListOfCanvases()->FindObject(cname);
   if (!c1) {
      return;
   }
   c1->SetFillColor(42);
   c1->GetFrame()->SetFillColor(21);
   c1->GetFrame()->SetBorderSize(6);
   c1->GetFrame()->SetBorderMode(-1);

   TString filename = "hsimple.root";
   TString dir = gSystem->UnixPathName(gInterpreter->GetCurrentMacroName());
   dir.ReplaceAll("hsimple.C","");
   dir.ReplaceAll("/./","/");
   TFile *hfile = 0;
   if (get) {
      // if the argument get =1 return the file "hsimple.root"
      // if the file does not exist, it is created
      TString fullPath = dir+"hsimple.root";
      if (!gSystem->AccessPathName(fullPath,kFileExists)) {
    hfile = TFile::Open(fullPath); //in $ROOTSYS/tutorials

         if (hfile) return;
      }

      //otherwise try $PWD/hsimple.root
      if (!gSystem->AccessPathName("hsimple.root",kFileExists)) {
         hfile = TFile::Open("hsimple.root"); //in current dir
         if (hfile) return;
      }
   }

   //no hsimple.root file found. Must generate it !
   //generate hsimple.root in $ROOTSYS/tutorials if we have write access
   if (!gSystem->AccessPathName(dir,kWritePermission)) {
      filename = dir+"hsimple.root";
   } else if (!gSystem->AccessPathName(".",kWritePermission)) {
      //otherwise generate hsimple.root in the current directory
   } else {
      printf("you must run the script in a directory with write access\n");
      return;
   }
   hfile = (TFile*)gROOT->FindObject(filename); if (hfile) hfile->Close();
   hfile = new TFile(filename,"RECREATE","Demo ROOT file with histograms");

   // Create some histograms, a profile histogram and an ntuple
   TH1F *hpx = new TH1F("hpx","This is gaus distribution",100,-4,4);
   hpx->SetFillColor(48);
   TH2F *hpxpy = new TH2F("hpxpy","py vs px",40,-4,4,40,-4,4);
   TProfile *hprof = new TProfile("hprof","Profile of pz versus px",100,-4,4,0,20);
   TNtuple *ntuple = new TNtuple("ntuple","Demo ntuple","px:py:pz:random:i");

   //gBenchmark->Start("hsimple");

   // Fill histograms randomly
   gRandom->SetSeed();
   Float_t px, py, pz;
   const Int_t kUPDATE = 1000;
   for (Int_t i = 0; i < 250000; i++) {
      gRandom->Rannor(px,py);
      pz = px*px + py*py;
      Float_t random = gRandom->Rndm(1);
      hpx->Fill(px);
      hpxpy->Fill(px/2,py/2);
      hprof->Fill(px,pz);
      ntuple->Fill(px,py,pz,random,i);
      if (i && (i%kUPDATE) == 0) {
         if (i == kUPDATE) hpx->Draw();
         c1->Modified();
         c1->Update();
         if (gSystem->ProcessEvents())
            break;
      }
   }
   //gBenchmark->Show("hsimple");

   // Save all objects in this file
   hpx->SetFillColor(0);
   hfile->Write();
   hpx->SetFillColor(48);
   c1->Modified();
   return;

// Note that the file is automatically close when application terminates
// or when the file destructor is called.
}