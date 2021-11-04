using System;
using System.Runtime.InteropServices;
using System.Text;
using UnityEngine;

namespace BertecDevice
{

   public enum PlateState : int
   {
      BOGUS_VALUE = -1000,
      PLATE_NOT_SETUP = -1,
      INITIAL_SETUP = 0,

      // If this state is set, the program is waiting for nobody to be on the plate. The no load timer is reset when the game is over,
      // and if we are currently waiting for no load, and someone steps on it, resets the timer.
      WAITING_FOR_NO_LOAD,

      // Conversely, once we have no load, we are waiting for a valid load on the plate to be there for a certain time
      WAITING_FOR_GOOD_LOAD,

      // Once we have cycled through no load, and then a valid load for a certain time, we are ready to go.
      HAVE_GOOD_LOAD
   }


   public class COPvalues
   {
      public bool valid;
      public double x, y;
   }

   // only usable for the Dynamic systen; set to zero for the USB force plate
   public class AngleValues
   {
      public bool valid;
      public double swayAngle, rotAngle;
   }

   public class COPAngleValues
   {
      public bool valid;
      public double swayAngle, rotAngle;
      public double x, y;
   }


   ////////////////////////////////////////////////////////

   internal class BertecUSBDevice : IDisposable
   {
      const String dllname = "BertecDevice"; // Unity will pick the proper file in the Plugins/x86 or Plugins/x86_64 folder depending on the platform.
                                             // the filename must be the same! Notice the lack of .dll which is a new requirement.

      [DllImport(dllname, CallingConvention = CallingConvention.Cdecl)]
      private static extern int ForcePlateStart();

      [DllImport(dllname, CallingConvention = CallingConvention.Cdecl)]
      private static extern int ForcePlateShutdown();

      [DllImport(dllname, CallingConvention = CallingConvention.Cdecl)]
      private static extern int ForcePlateZeroPlate();

      [DllImport(dllname, CallingConvention = CallingConvention.Cdecl)]
      private static extern PlateState ForcePlateState();

      [DllImport(dllname, CallingConvention = CallingConvention.Cdecl)]
      private static extern bool ForcePlateHasCopValues();

      [DllImport(dllname, CallingConvention = CallingConvention.Cdecl)]
      private static extern double ForcePlateCopAngleRad();

      [DllImport(dllname, CallingConvention = CallingConvention.Cdecl)]
      private static extern double ForcePlateCopDistance();

      [DllImport(dllname, CallingConvention = CallingConvention.Cdecl)]
      private static extern bool ForcePlateGetLastCOPvalues(out double copX, out double copY, out int timestamp);

      [DllImport(dllname, CallingConvention = CallingConvention.Cdecl)]
      private static extern void ForcePlateSetBufferDepth(int newdepth);

      [DllImport(dllname, CallingConvention = CallingConvention.Cdecl)]
      private static extern void ForcePlateClearBuffer();

      [DllImport(dllname, CallingConvention = CallingConvention.Cdecl)]
      private static extern bool ForcePlateGetChannelNames(StringBuilder buffer, int buffersize);

      [DllImport(dllname, CallingConvention = CallingConvention.Cdecl)]
      private static extern int ForcePlateGetForceData([In, Out] double[] pDataBuffer, ref int pNumberChannelsInOut, ref int timestamp);

      //////////////////////////////////////////////////////

      public BertecUSBDevice()
      {
         Debug.Log("using the BertecUSBDevice");
         ForcePlateStart();
      }

      ~BertecUSBDevice()
      {
         Dispose(false);
      }

      public void Dispose()
      {
         Dispose(true);
         GC.SuppressFinalize(this);
      }

      private void Dispose(bool disposing)
      {
         ForcePlateShutdown();
      }

      public void ForceOOBShutdown()
      {
         ForcePlateShutdown();
      }

      public void ZeroPlate()
      {
         ForcePlateZeroPlate();
      }

      public bool HasGoodLoad
      {
         get
         {
            return (ForcePlateState() == PlateState.HAVE_GOOD_LOAD);
         }
      }

      public PlateState State
      {
         get { return ForcePlateState(); }
      }

      public bool HasCopValues
      {
         get { return ForcePlateHasCopValues(); }
      }

      public double CopAngleRad
      {
         get { return ForcePlateCopAngleRad(); }
      }

      public double CopDistance
      {
         get { return ForcePlateCopDistance(); }
      }

      public bool LastCOPvalues(out double copX, out double copY, out int timestamp)
      {
         return ForcePlateGetLastCOPvalues(out copX, out copY, out timestamp);
      }

      public COPvalues LastCOPvalues()
      {
         COPvalues result = new COPvalues();
         int timestamp;
         result.valid = LastCOPvalues(out result.x, out result.y, out timestamp);
         return result;
      }

      public string[] ChannelNames()
      {
         StringBuilder str = new StringBuilder(256);
         ForcePlateGetChannelNames(str, 256);
         return str.ToString().Split(',');
      }

      public void SetBufferDepth(int newdepth)
      {
         ForcePlateSetBufferDepth(newdepth);
      }

      public void ClearBuffer()
      {
         ForcePlateClearBuffer();
      }

      public int GetForceData(int bufferSize, ref double[] buffer, ref int timestamp)
      {
         return ForcePlateGetForceData(buffer, ref bufferSize, ref timestamp);
      }

   }

}
