﻿namespace Minary.Domain.Main
{
  using Minary.Common;
  using Minary.DataTypes.Enum;
  using Minary.DataTypes.Interface;
  using Minary.Domain.State;
  using Minary.Domain.State.Service;
  using Minary.Form.Main;
  using System;
  using System.IO;
  using System.Net.NetworkInformation;


  public class SystemStateCheck
  {

    #region PUBLIC

    public static IMinaryState GetMinaryEventBase(MinaryMain minaryObj)
    {
      MinaryState state = DetermineSystemState();

      if ((state & MinaryState.NetworkMissing) == MinaryState.NetworkMissing)
      {
        return new NetworkMissing(minaryObj);
      }
      else if ((state & MinaryState.NPcapMissing) == MinaryState.NPcapMissing)
      {
        return new NPcapMissing(minaryObj);
      }
      else if ((state & MinaryState.NotAdmin) == MinaryState.NotAdmin)
      {
        return new NotAdmin(minaryObj);
      }
      else if ((state & MinaryState.ApeBinaryMissing) == MinaryState.ApeBinaryMissing)
      {
        return new ApeNotInstalled(minaryObj);
      }
      else if ((state & MinaryState.SnifferMissing) == MinaryState.SnifferMissing)
      {
        return new SnifferNotInstalled(minaryObj);
      }
      else if ((state & MinaryState.HttpProxyMissing) == MinaryState.HttpProxyMissing)
      {
        return new HttpReverseProxyNotInstalled(minaryObj);
      }

      return new StateOk(minaryObj);
    }


    public static MinaryState DetermineSystemState()
    {
      MinaryState retVal = MinaryState.StateOk;

      // Check NPcap
      if (string.IsNullOrEmpty(Utils.TryExecute(NPcap.GetNPcapVersion)) == true)
      {
        retVal |= MinaryState.NPcapMissing;
      }

      // Check interfaces
      if (NetworkInterface.GetIsNetworkAvailable() == false ||
          NetworkInterface.GetAllNetworkInterfaces().Length <= 0)
      {
        retVal |= MinaryState.NetworkMissing;
      }

      // Check SystemService: APE
      if (File.Exists(Config.ApeBinaryPath) == false)
      {
        retVal |= MinaryState.ApeBinaryMissing;
      }

      // Check SystemService: RouterIPv4
      if (File.Exists(Config.RouterIPv4BinaryPath) == false)
      {
        retVal |= MinaryState.RouterIPv4BinaryMissing;
      }

      // Check SystemService: DnsPoisoning
      if (File.Exists(Config.DnsPoisoningBinaryPath) == false)
      {
        retVal |= MinaryState.DnsPoisoningBinaryMissing;
      }
      

      // Check SystemService: Sniffer
      if (File.Exists(Config.SnifferBinaryPath) == false)
      {
        retVal |= MinaryState.SnifferMissing;
      }

      // Check SystemService: HttpProxy
      if (File.Exists(Config.HttpReverseProxyBinaryPath) == false)
      {
        retVal |= MinaryState.HttpProxyMissing;
      }

      return retVal;
    }


    public static void EvaluateMinaryState(out MinaryState minaryState)
    {
      minaryState = Minary.Domain.Main.SystemStateCheck.DetermineSystemState();

      if ((minaryState & MinaryState.NotAdmin) == MinaryState.NotAdmin)
      {
        throw new Exception("Can't start Minary because of missing admin privileges.");
      }

      if ((minaryState & MinaryState.NPcapMissing) == MinaryState.NPcapMissing)
      {
        throw new Exception("Can't start Minary because NPcap is not installed.");
      }

      if ((minaryState & MinaryState.ApeBinaryMissing) == MinaryState.ApeBinaryMissing)
      {
        throw new Exception("Can't start Minary because APE is not installed.");
      }

      if ((minaryState & MinaryState.SnifferMissing) == MinaryState.SnifferMissing)
      {
        throw new Exception("Can't scan network because Sniffer is not installed.");
      }

      if ((minaryState & MinaryState.NetworkMissing) == MinaryState.NetworkMissing)
      {
        throw new Exception("Can't start Minary because no network interface was found.");
      }
    }

    #endregion

  }
}
