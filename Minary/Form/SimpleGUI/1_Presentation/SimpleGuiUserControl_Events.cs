﻿namespace Minary.Form.SimpleGUI.Presentation
{
  using Minary.DataTypes.Enum;
  using Minary.Domain.ArpScan;
  using Minary.Form.ArpScan.DataTypes;
  using Minary.LogConsole.Main;
  using MinaryLib.AttackService.Class;
  using System;
  using System.ComponentModel;
  using System.Linq;
  using System.Windows.Forms;


  public partial class SimpleGuiUserControl
  {

    #region MEMBERS
    
    private ArpScanConfig arpScanConfig = null;
    private ArpScanner arpScanner = null;
    public ReplyListener replyListener = null;

    #endregion


    #region EVENTS

    private void DgvSimpleGui_CellContentClick(object sender, DataGridViewCellEventArgs e)
    {
      if (e.ColumnIndex != this.dgv_SimpleGui.Columns["Attack"].Index)
      {
        return;
      }
      
      var targetIp = this.dgv_SimpleGui.Rows[e.RowIndex].Cells["IpAddress"].Value;
      var targetMac = this.dgv_SimpleGui.Rows[e.RowIndex].Cells["MacAddress"].Value;

      this.dgv_SimpleGui.EndEdit();
      var startAttack = (bool)this.dgv_SimpleGui.Rows[e.RowIndex].Cells[e.ColumnIndex].Value;
      var action = startAttack == true ? "Starting" : "Stopping";

      LogCons.Inst.Write(LogLevel.Info, $"{action} attack on {targetIp}");      
    }


    private void SimpleGuiUserControl_VisibleChanged(object sender, EventArgs e)
    {
      if (this.Visible == true &&
          this.Disposing == false)
      {
        // Configure ARP scan object
        //this.arpScanner.Config = this.GetArpScanConfig();
        // Instanciate ARP scanner object
        try
        {
          this.arpScanConfig = this.GetArpScanConfig();
          this.arpScanner = new ArpScanner(this.arpScanConfig);
        }
        catch (Exception ex)
        {
          LogCons.Inst.Write(LogLevel.Error, $"SimpleGuiUserControl/SimpleGuiUserControl_VisibleChanged(EXCEPTION): {ex.Message}\r\n{ex.StackTrace}");
        }

        try
        {
          this.replyListener = new ReplyListener(this.arpScanConfig);
        }
        catch (Exception ex)
        {
          LogCons.Inst.Write(LogLevel.Error, $"SimpleGuiUserControl/BGW_ArpScanListener(EXCEPTION1): {ex.Message}");
          return;
        }

        try
        {
          this.replyListener.AddObserver(this);
        }
        catch (Exception ex)
        {
          LogCons.Inst.Write(LogLevel.Error, $"SimpleGuiUserControl/BGW_ArpScanListener(EXCEPTION2): {ex.Message}");
          return;
        }

        this.StartArpScanListener();
        this.StartArpScanSender();
        
        // Start attack services
        // First let all plugins prepare their environment before 
        // the actual attack begins.
        this.minaryObj.PrepareAttackAllPlugins();

        // After the plugins were prepared start all
        // attack services.
        var currentServiceParams = new StartServiceParameters()
        {
          SelectedIfcIndex = this.minaryObj.CurrentInterfaceIndex,
          SelectedIfcId = this.minaryObj.NetworkHandler.GetNetworkInterfaceIdByIndex(this.minaryObj.CurrentInterfaceIndex),
          TargetList = (from target in this.minaryObj.ArpScanHandler.TargetList
                        where target.Attack == true
                        select new { target.MacAddress, target.IpAddress }).
                            ToDictionary(elem => elem.MacAddress, elem => elem.IpAddress)
        };

        this.minaryObj.StartAttackAllServices(currentServiceParams);
        LogCons.Inst.Write(LogLevel.Info, "SimpleGuiUserControl/SimpleGuiUserControl_VisibleChanged: SimpleGUI/ARPScan/AttackServices started");
      }
      else
      {
        this.bgw_ArpScanSender.CancelAsync();
        this.bgw_ArpScanListener.CancelAsync();
        this.minaryObj?.MinaryAttackServiceHandler?.StopAllServices();
        LogCons.Inst.Write(LogLevel.Info, "SimpleGuiUserControl/SimpleGuiUserControl_VisibleChanged: SimpleGUI/ARPScan/AttackServices  stopped");
      }
    }


    #region EVENTS: BGW_ArpScanSender

    private void BGW_ArpScanSender_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
    {
      if (e.Error != null)
      {
        LogCons.Inst.Write(LogLevel.Error, "SimpleGuiUserControl/BGW_ArpScanSender(): Completed with error");
      }
      else if (e.Cancelled == true)
      {
        LogCons.Inst.Write(LogLevel.Info, "SimpleGuiUserControl/BGW_ArpScanSender(): Completed by cancellation");
      }
      else
      {
        LogCons.Inst.Write(LogLevel.Info, $"SimpleGuiUserControl/BGW_ArpScanSender(): Completed successfully");
      }
    }


    private void BGW_ArpScanSender_DoWork(object sender, DoWorkEventArgs e)
    {
      // In an endless loop scan all MAC addresses within the 
      // subnet until the BGW is cancelled.
      int roundCounter = 0;
      int sleepSecondsOnError = 5;

      while (true)
      {
        LogCons.Inst.Write(LogLevel.Info, $"SimpleGuiUserControl/BGW_ArpScanSender(): ARP scan round {roundCounter} started");
        if (this.bgw_ArpScanSender.CancellationPending == true)
        {
          LogCons.Inst.Write(LogLevel.Info, $"SimpleGuiUserControl/BGW_ArpScanSender(): ARP scan cancelled");
          break;
        }

        try
        {
          this.arpScanner.StartScanning();
        }
        catch (Exception ex)
        {
          LogCons.Inst.Write(LogLevel.Error, $"SimpleGuiUserControl/BGW_ArpScanSender(EXCEPTION2.0): {ex.Message}\r\n{ex.StackTrace}");
          LogCons.Inst.Write(LogLevel.Error, $"SimpleGuiUserControl/BGW_ArpScanSender(EXCEPTION2.1): Sleeping for {sleepSecondsOnError} seconds");
          System.Threading.Thread.Sleep(sleepSecondsOnError * 1000);
        }

        LogCons.Inst.Write(LogLevel.Info, $"SimpleGuiUserControl/BGW_ArpScanSender(): ARP scan round {roundCounter} finished");
        roundCounter++;
        System.Threading.Thread.Sleep(3000);
      }

      LogCons.Inst.Write(LogLevel.Info, $"SimpleGuiUserControl/BGW_ArpScanSender(): ARP scan stopped after {roundCounter} rounds");
    }
    
    #endregion


    #region EVENTS: BGW_ArpScanListener

    private void BGW_ArpScanListener_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
    {
      if (e.Error != null)
      {
        LogCons.Inst.Write(LogLevel.Error, "SimpleGuiUserControl/BGW_ArpScanListener(): Completed with error");
      }
      else if (e.Cancelled == true)
      {
        LogCons.Inst.Write(LogLevel.Info, "SimpleGuiUserControl/BGW_ArpScanListener(): Completed by cancellation");
      }
      else
      {
        LogCons.Inst.Write(LogLevel.Info, "SimpleGuiUserControl/BGW_ArpScanListener(): Completed successfully");
      }
    }


    private void BGW_ArpScanListener_DoWork(object sender, DoWorkEventArgs e)
    {
      LogCons.Inst.Write(LogLevel.Info, "SimpleGuiUserControl/BGW_ArpScanListener(): Background worker started");

      try
      {
        this.replyListener.StartReceivingArpPackets();
      }
      catch (Exception ex)
      {
        LogCons.Inst.Write(LogLevel.Error, $"BGW_ArpScanListener(EXCEPTION2): {ex.Message}");
        return;
      }

      LogCons.Inst.Write(LogLevel.Info, "SimpleGuiUserControl/BGW_ArpScanListener(): Background worker stopped");
    }

    #endregion
    
    #endregion


    #region PRIVATE

    private void StartArpScanListener(Action onScanDoneCallback = null)
    {
      // Initiate ARP scan cancellation
      if (this.bgw_ArpScanListener.IsBusy == true)
      {
        LogCons.Inst.Write(LogLevel.Info, "SimpleGuiUserControl/ArpScan: ArpScanListener is already running");
      }
      else
      {
        LogCons.Inst.Write(LogLevel.Info, "SimpleGuiUserControl/ArpScan: ArpScanListener started");
        this.bgw_ArpScanListener.RunWorkerAsync();
      }
    }


    private void StartArpScanSender(Action onScanDoneCallback = null)
    {
      // Initiate ARP scan cancellation
      if (this.bgw_ArpScanSender.IsBusy == false)
      {
        LogCons.Inst.Write(LogLevel.Info, "SimpleGuiUserControl/ArpScan: ArpScanSender started");
        this.bgw_ArpScanSender.RunWorkerAsync();
      }
      else
      {
        LogCons.Inst.Write(LogLevel.Info, "SimpleGuiUserControl/ArpScan: ArpScanSender can not be started");
      }
    }


    private ArpScanConfig GetArpScanConfig()
    {
      DataTypes.Struct.MinaryConfig minaryConfig = this.minaryObj.MinaryTaskFacade.GetCurrentMinaryConfig();
      this.communicator = PcapHandler.Inst.OpenPcapDevice(this.minaryObj.CurrentInterfaceId, 1);

      // ArpScanner
      ArpScanConfig arpScanConfig = new ArpScanConfig()
      {
        InterfaceId = minaryConfig.InterfaceId,
        GatewayIp = minaryConfig.GatewayIp,
        LocalIp = minaryConfig.LocalIp,
        LocalMac = minaryConfig.LocalMac?.Replace('-', ':'),
        NetworkStartIp = minaryConfig.StartIp,
        NetworkStopIp = minaryConfig.StopIp,
        MaxNumberSystemsToScan = -1,
        ObserverClass = this,
        Communicator = this.communicator
      };

      return arpScanConfig;
    }

    #endregion

  }
}