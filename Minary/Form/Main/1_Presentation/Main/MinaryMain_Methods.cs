﻿namespace Minary.Form.Main
{
  using Minary.Common;
  using Minary.DataTypes.Enum;
  using Minary.LogConsole.Main;
  using MinaryLib.AttackService.Class;
  using MinaryLib.AttackService.Enum;
  using System;
  using System.Collections.Generic;
  using System.ComponentModel;
  using System.Diagnostics;
  using System.IO;
  using System.Linq;
  using System.Threading.Tasks;
  using System.Windows.Forms;


  public partial class MinaryMain
  {

    #region MEMBERS

    private BackgroundWorker bgw_OnStartAttack;

    #endregion


    #region PROPERTIES

    // Proxy properties
    public TabPage TabPagePluginCatalog { get { return this.tp_MinaryPluginCatalog; } }

    #endregion


    #region PUBLIC

    public delegate void StartAttacksOnBackgroundDelegate();
    public void StartAttacksOnBackground()
    {
      if (this.InvokeRequired)
      {
        this.BeginInvoke(new StartAttacksOnBackgroundDelegate(this.StartAttacksOnBackground), new object[] { });
        return;
      }

      // Another OnStartAttack instance is running
      if (this.bgw_OnStartAttack.IsBusy == true)
      {
        LogCons.Inst.Write(LogLevel.Warning, "Another instance of the OnStartAttack back ground worker is already running.");

      // Fail if selected interface is invalid
      }
      else if (this.cb_Interfaces.SelectedIndex < 0)
      {
        string message = "No network interface selected";
        MessageDialog.Inst.ShowWarning(string.Empty, message, this);

      // Notify user to select at least one target system
      }
      else if (Debugging.IsDebuggingOn == false &&
               this.arpScanHandler.TargetList.Where(elem => elem.Attack == true).Count() <= 0)
      {
        string message = "You must select at least one target system.";
        MessageDialog.Inst.ShowWarning(string.Empty, message, this);


      // Stop the attack
      }
      else if (this.attackStarted == false)
      {
        this.bgw_OnStartAttack.RunWorkerAsync();
        // Set TSMI Attack Status note
        this.tsmi_Attack.Text = "Attack (started)";

        // In any other case stop a running attack
      }
      else
      {
        this.Cursor = Cursors.WaitCursor;
        this.StopAttack();
        this.Cursor = Cursors.Default;

        // Set TSMI Attack Status note
        this.tsmi_Attack.Text = "Attack (stopped)";
      }
    }


    public delegate void ClearCurrentNetworkStateDelegate();
    public void ClearCurrentNetworkState()
    {
      if (this.InvokeRequired == true)
      {
        this.BeginInvoke(new ClearCurrentNetworkStateDelegate(this.ClearCurrentNetworkState), new object[] { });
        return;
      }

      this.cb_Interfaces.Items.Clear();
      this.tb_GatewayIp.Text = string.Empty;
      this.tb_GatewayMac.Text = string.Empty;
      this.tb_NetworkStartIp.Text = string.Empty;
      this.tb_NetworkStopIp.Text = string.Empty;
      this.tb_Vendor.Text = string.Empty;
    }

    
    /// <summary>
    ///
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    public void StopAttack()
    {
      // Enable GUI elements
      Utils.TryExecute2(this.EnableGuiElements);

      // Stop all plugins
      Utils.TryExecute2(this.pluginHandler.StopAllPlugins);

      // Stop all services
      Utils.TryExecute2(this.attackServiceHandler.StopAllServices);

      this.attackStarted = false;
    }


    /// <summary>
    /// 
    /// </summary>
    public void PrepareAttackAllPlugins()
    {
      // Clear all the plugins parameter dict.
      this.pluginParams2AttackServices.Clear();

      foreach (var key in this.pluginHandler.TabPagesCatalog.Keys)
      {
        if (this.pluginHandler.IsPluginActive(key) == false)
        {
          continue;
        }

        try
        {
          var tmpKey = key?.Trim()?.ToLower()?.Replace(" ", "");
          var pluginDataObj = (List<object>)this.pluginHandler.TabPagesCatalog[key].PluginObject.OnPrepareAttack();
          this.pluginParams2AttackServices.Add(tmpKey, pluginDataObj);
        }
        catch (Exception ex)
        {
          LogCons.Inst.Write(LogLevel.Error, "Minary.PrepareAllPlugins(EXCEPTION): PluginName:{0}, Error:{1}\r\n{2}", key, ex.Message, ex.StackTrace);
        }
      }
    }

    #endregion


    #region PRIVATE

    private void DisableGuiElements()
    {
      this.bt_Attack.BackgroundImage = Properties.Resources.FA_Stop;
      this.bt_ScanLan.Enabled = false;
      this.cb_Interfaces.Enabled = false;
      this.dgv_MainPlugins.Enabled = false;
      this.tb_NetworkStartIp.Enabled = false;
      this.tb_NetworkStopIp.Enabled = false;
      this.tsmi_LoadTemplate.Enabled = false;
      this.tsmi_GetUpdates.Enabled = false;
      this.tsmi_Exit.Enabled = false;
      this.tsmi_ResetMinary.Enabled = false;
      this.tsmi_DetectInterfaces.Enabled = false;
      this.tsmi_Debugging.Enabled = false;
      this.tsmi_LoadTemplate.Enabled = false;
      this.tsmi_CreateTemplate.Enabled = false;
      this.tsmi_UnloadTemplate.Enabled = false;
      this.tsmi_SimpleGUI.Enabled = false;
      this.tsmi_TlsSslCertificates.Enabled = false;
    }

   
    private void EnableGuiElements()
    {
      this.bt_Attack.BackgroundImage = Properties.Resources.FA_Play;
      this.bt_ScanLan.Enabled = true;
      this.cb_Interfaces.Enabled = true;
      this.dgv_MainPlugins.Enabled = true;
      this.tb_NetworkStartIp.Enabled = true;
      this.tb_NetworkStopIp.Enabled = true;
      this.tsmi_LoadTemplate.Enabled = true;
      this.tsmi_GetUpdates.Enabled = true;
      this.tsmi_Exit.Enabled = true;
      this.tsmi_ResetMinary.Enabled = true;
      this.tsmi_DetectInterfaces.Enabled = true;
      this.tsmi_Debugging.Enabled = true;
      this.tsmi_LoadTemplate.Enabled = true;
      this.tsmi_CreateTemplate.Enabled = true;
      this.tsmi_UnloadTemplate.Enabled = true;
      this.tsmi_SimpleGUI.Enabled = true;
      this.tsmi_TlsSslCertificates.Enabled = true;
    }


    private MinaryFileType DetermineFileType(string filePath)
    {
      if (this.templateTaskLayer.IsFileATemplate(filePath))
      {
        return MinaryFileType.TemplateFile;
      }

      return MinaryFileType.Undetermined;
    }


    private void LoadUserTemplate(string cmdLineArgument)
    {
      MinaryFileType fileType = this.DetermineFileType(cmdLineArgument);
      if (fileType != MinaryFileType.TemplateFile)
      {
        this.pluginHandler.RestoreLastPluginLoadState();
      }

      Template.Presentation.LoadTemplate loadTemplatePresentation = null;
      try
      {
        loadTemplatePresentation = new Template.Presentation.LoadTemplate(this, cmdLineArgument);
      }
      catch (Exception ex)
      {
        var message = $"Error 1 occurred while loading template file \"{Path.GetFileName(cmdLineArgument)}\".\r\n\r\n{ex.Message}";
        this.LogAndShowMessage(message, LogLevel.Error);
      }

      try
      {
        loadTemplatePresentation.ShowDialog();
      }
      catch (Exception ex)
      {
        var message = $"Error 2 occurred while loading template file \"{Path.GetFileName(cmdLineArgument)}\".\r\n\r\n{ex.Message}";
        this.LogAndShowMessage(message, LogLevel.Error);
      }

      try
      {
        if (loadTemplatePresentation != null &&
            loadTemplatePresentation.TemplateData != null &&
            string.IsNullOrEmpty(loadTemplatePresentation.TemplateData.TemplateConfig.Name) == false)
        {
          this.tb_TemplateName.Text = loadTemplatePresentation.TemplateData.TemplateConfig.Name;
        }
      }
      catch (Exception ex)
      {
        var message = $"Error 3 occurred while loading template file \"{Path.GetFileName(cmdLineArgument)}\".\r\n\r\n{ex.Message}";
        this.LogAndShowMessage(message, LogLevel.Error);
        this.pluginHandler.RestoreLastPluginLoadState();
      }
    }


    private void LogAndShowMessage(string message, LogLevel level)
    {
      LogCons.Inst.Write(level, message);
      MessageDialog.Inst.ShowWarning(string.Empty, message, this);
    }


    private void ShutDownMinary()
    {
      /*
       * 1. Stop data input thread (named pipe)
       * 2. Stop poisoning thread
       * 3. Stop sniffing thread
       * 4. Shut down all plugins.
       *
       */

      // Set the Wait cursor.
      this.Cursor = Cursors.WaitCursor;

      if (this.bgw_OnStartAttack.IsBusy)
      {
        this.StopAttack();
      }

      // Remove all static ARP entries
      var procStartInfo = new ProcessStartInfo("arp", "-d *");
      procStartInfo.WindowStyle = Debugging.IsDebuggingOn ? ProcessWindowStyle.Normal : ProcessWindowStyle.Hidden;
      var procClearArpCache = new Process();
      procClearArpCache.StartInfo = procStartInfo;
      procClearArpCache.Start();
      procClearArpCache.WaitForExit(3000);
      procClearArpCache.Close();

      // Set the default cursor
      this.Cursor = Cursors.Default;

      // Terminate process
      Environment.Exit(0);
      base.Dispose();
    }


    private void StartAllPlugins()
    {
      foreach (var key in this.pluginHandler.TabPagesCatalog.Keys)
      {
        LogCons.Inst.Write(LogLevel.Info, $"Minary.StartAllPlugins(): PluginName:{key}, IsPluginActive:{this.pluginHandler.IsPluginActive(key)}");

        try
        {
          if (this.pluginHandler.IsPluginActive(key))
          {
            this.pluginHandler.TabPagesCatalog[key].PluginObject.OnStartAttack();
          }
        }
        catch (Exception ex)
        {
          LogCons.Inst.Write(LogLevel.Error, "Minary.StartAllPlugins(EXCEPTION): PluginName:{0}, Error:{1}\r\n{2}", key, ex.Message, ex.StackTrace);
        }
      }
    }


    public void StartAttackAllServices(StartServiceParameters serviceParameters)
    {
      foreach (var tmpKey in this.attackServiceHandler.AttackServices.Keys)
      {
        try
        {
          LogCons.Inst.Write(LogLevel.Info, "Minary.StartAllServices(): Starting {0}/{1}", tmpKey, this.attackServiceHandler.AttackServices[tmpKey].ServiceName);
          ServiceStatus newServiceStatus = this.attackServiceHandler.AttackServices[tmpKey].StartService(serviceParameters, this.pluginParams2AttackServices);
          this.SetNewAttackServiceState(tmpKey, newServiceStatus);
        }
        catch (Exception)
        {
          this.SetNewAttackServiceState(tmpKey, ServiceStatus.Error);
          throw;
        }
      }
    }


    private async void FadeIn(Form o, int interval = 80)
    {
      //Object is not fully invisible. Fade it in
      while (o.Opacity < 1.0)
      {
        await Task.Delay(interval);
        o.Opacity += 0.05;
      }
      o.Opacity = 1; //make fully visible       
    }


    private async void FadeOut(Form o, int interval = 80)
    {
      //Object is fully visible. Fade it out
      while (o.Opacity > 0.0)
      {
        await Task.Delay(interval);
        o.Opacity -= 0.05;
      }
      o.Opacity = 0; //make fully invisible       
    }


    private void SimpleGuiDisable()
    {
      this.gb_TargetRange.Visible = true;
      this.gb_Interfaces.Visible = true;
      this.ms_MainWindow.Visible = true;
      this.bt_Attack.Visible = true;
      this.bt_ScanLan.Visible = true;
      this.tc_Plugins.Visible = true;
      this.simpleGui.Visible = false;
    }


    private void SimpleGuiEnable()
    {
      this.gb_TargetRange.Visible = false;
      this.gb_Interfaces.Visible = false;
      this.ms_MainWindow.Visible = false;
      this.bt_Attack.Visible = false;
      this.bt_ScanLan.Visible = false;
      this.tc_Plugins.Visible = false;
      this.simpleGui.Visible = true;
    }


    private void SimpleGuiStartScanning()
    {
      //var arpScanConf = this.GetArpScanConfig();
      //Minary.Domain.ArpScan.ArpScanner.Inst.

    }


    private void SimpleGuiStopScanning()
    {
    }


    private StartServiceParameters GetCurrentServiceParameters()
    {
      var currentServiceParams = new StartServiceParameters()
      {
        SelectedIfcIndex = this.currentInterfaceIndex,
        SelectedIfcId = this.nicHandler.GetNetworkInterfaceIdByIndex(this.currentInterfaceIndex),
        TargetList = (from target in this.arpScanHandler.TargetList
                      where target.Attack == true
                      select new { target.MacAddress, target.IpAddress }).
                    ToDictionary(elem => elem.MacAddress, elem => elem.IpAddress)
      };

      return currentServiceParams;
    }

    #endregion

  }
}
