﻿namespace Minary.Common
{
  using Minary.Form;
  using System;
  using System.Collections.Generic;
  using System.Diagnostics;
  using System.Reflection;
  using System.Windows.Forms;


  public class MinaryProcess
  {

    #region MEMBERS
    
    #endregion


    #region PUBLIC

    /// <summary>
    /// Initializes a new instance of the <see cref="MinaryProcess"/> class.
    ///
    /// </summary>
    public MinaryProcess()
    {
    }


    /// <summary>
    ///
    /// </summary>
    public void HandleRunningInstances()
    {
      string[] processNameArray =
        {
                                    Config.ApeProcessName,
                                    Config.ApeSnifferProcessName,
                                    Config.ArpScanProcessName,
                                    Config.HttpReverseProxyName,
                                    Assembly.GetExecutingAssembly().GetName().Name
        };

      foreach (string tmpProcName in processNameArray)
      {
        try
        {
          this.HandleProcess(tmpProcName);
        }
        catch (Exception ex)
        {
          string message = string.Format(@"An error occured while handling running instance of {0}:\r\n\r\n", tmpProcName, ex.Message);
          MessageDialog.ShowInformation(string.Empty, message);
        }
      }
    }

    #endregion


    #region PRIVATE

    private void HandleProcess(string processName)
    {
      Process[] runningMinaryInstances = Process.GetProcessesByName(processName);
      int applicationPid = Process.GetCurrentProcess().Id;

      // Abort if no process matched the search string.
      if (runningMinaryInstances == null || runningMinaryInstances.Length <= 0)
      {
        return;
      }

      // Convert proccess array to process list and remove the own PID 
      // from the process list
      List<Process> processList = new List<Process>(runningMinaryInstances);
      processList = processList.FindAll(elem => elem.Id != applicationPid);

      // Abort if no process matched the search string
      if (processList == null || processList.Count <= 0)
      {
        return;
      }

      // Handle processes that matched the search string
      string message = string.Format(@"An instance of {0} instance is running. Do you want to stop that process?", processName);
      if (MessageBox.Show(message, "Information", MessageBoxButtons.YesNo, MessageBoxIcon.Information) != DialogResult.Yes)
      {
        return;
      }

      foreach (Process proc in processList)
      {
        Utils.TryExecute2(proc.Kill);
      }
    }

    #endregion

  }
}
