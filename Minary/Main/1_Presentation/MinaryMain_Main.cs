﻿namespace Minary
{
  using Minary.ArpScan.DataTypes;
  using Minary.AttackService;
  using Minary.Common;
  using Minary.MiniBrowser;
  using MinaryLib.AttackService;
  using System;
  using System.Collections.Generic;
  using System.ComponentModel;
  using System.IO;
  using System.Linq;
  using System.Net.NetworkInformation;
  using System.Threading;
  using System.Windows.Forms;
  using TemplateTask = Minary.Template.Task;


  public partial class MinaryMain : Form
  {

    #region MEMBERS

    private static MinaryMain instance;
    private NetworkInterface[] allAttachednetworkInterfaces;
    private BindingList<PluginTableRecord> usedPlugins;
    private BindingList<string> targetList;
    private DataInput.InputModule inputModule;
    private PluginHandler pluginHandler;
    private TabPageHandler tabPageHandler;
    private TemplateTask.TemplateHandler templateTaskLayer;
    private string currentIpAddress;
    private Browser miniBrowser;
    private bool attackStarted;
    private Minary.TaskFacade minaryTaskFacade;
    private string[] commandLineArguments;
    private AttackServiceHandler attackServiceHandler;
    private Dictionary<string, PictureBox> attackServiceMap = new Dictionary<string, PictureBox>();

    #endregion


    #region PROPERTIES

    public string[] CommandLineArguments { get { return this.commandLineArguments; } }

    public string CurrentLocalIp { get { return this.currentIpAddress ?? string.Empty; } }

    public string CurrentGatewayIp { get { return this.tb_GatewayIp.Text; } }

    public string NetworkStartIp { get { return this.tb_NetworkStartIp.Text; } set { } }

    public string NetworkStopIp { get { return this.tb_NetworkStopIp.Text; } set { } }
    
    public LogConsole.Main.LogConsole GetLogConsole { get { return LogConsole.Main.LogConsole.LogInstance; } }

    public Minary.TaskFacade MinaryTaskFacade { get { return this.minaryTaskFacade; } }

    public PluginHandler PluginHandler { get { return this.pluginHandler; } }

    public BindingList<PluginTableRecord> UsedPlugins { get { return this.usedPlugins; } }

    public TabPageHandler MinaryTabPageHandler { get { return this.tabPageHandler; } }

    public AttackServiceHandler MinaryAttackServiceHandler { get { return this.attackServiceHandler; } set { } }

    #endregion


    #region PUBLIC

    /// <summary>
    ///
    /// </summary>
    /// <param name="args"></param>
    /// <returns></returns>
    public static MinaryMain GetInstance(string[] args)
    {
      return instance ?? (instance = new MinaryMain(args));
    }


    /// <summary>
    /// Initializes a new instance of the <see cref="MinaryMain"/> class.
    ///
    /// </summary>
    /// <param name="args"></param>
    private MinaryMain(string[] args)
    {
      this.InitializeComponent();

      // Init configuration
      Config.InitializeMinaryConfig();

      DataGridViewTextBoxColumn columnPluginName = new DataGridViewTextBoxColumn();
      columnPluginName.DataPropertyName = "PluginName";
      columnPluginName.Name = "PluginName";
      columnPluginName.HeaderText = "Plugin name";
      columnPluginName.ReadOnly = true;
      columnPluginName.Width = 120;
      this.dgv_MainPlugins.Columns.Add(columnPluginName);
      
      DataGridViewCheckBoxColumn columnActivated = new DataGridViewCheckBoxColumn();
      columnActivated.DataPropertyName = "Active";
      columnActivated.Name = "Active";
      columnActivated.HeaderText = "Activated";
      columnActivated.FalseValue = "0";
      columnActivated.TrueValue = "1";
      columnActivated.Visible = true;
      this.dgv_MainPlugins.Columns.Add(columnActivated);

      DataGridViewTextBoxColumn columnType = new DataGridViewTextBoxColumn();
      columnType.DataPropertyName = "PluginType";
      columnType.Name = "PluginType";
      columnType.HeaderText = "Type";
      columnType.Visible = true;
      this.dgv_MainPlugins.Columns.Add(columnType);

      DataGridViewTextBoxColumn columnPluginDescription = new DataGridViewTextBoxColumn();
      columnPluginDescription.DataPropertyName = "PluginDescription";
      columnPluginDescription.HeaderText = "Description";
      columnPluginDescription.ReadOnly = true;
      columnPluginDescription.Width = 120;
      columnPluginDescription.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
      this.dgv_MainPlugins.Columns.Add(columnPluginDescription);

      this.usedPlugins = new BindingList<PluginTableRecord>();
      this.dgv_MainPlugins.DataSource = this.usedPlugins;

      this.bgwOnStartAttack = new BackgroundWorker() { WorkerSupportsCancellation = true };
      this.bgwOnStartAttack.DoWork += new DoWorkEventHandler(this.BGW_OnStartAllPlugins);
      this.bgwOnStartAttack.RunWorkerCompleted += new RunWorkerCompletedEventHandler(this.BGW_OnStartAttackCompleted);

      this.bgwOnStopAttack = new BackgroundWorker() { WorkerSupportsCancellation = true };
      this.bgwOnStopAttack.DoWork += new DoWorkEventHandler(this.BGW_OnStopAllPlugins);
      this.bgwOnStopAttack.RunWorkerCompleted += new RunWorkerCompletedEventHandler(this.BGW_OnStopAttackCompleted);

      this.attackServiceHandler = new AttackServiceHandler(this);

      this.targetList = new BindingList<string>();
      this.pluginHandler = new PluginHandler(this);
      this.inputModule = new DataInput.InputModule(this);
      this.minaryTaskFacade = TaskFacade.GetInstance(this, this.dgv_MainPlugins);
      this.commandLineArguments = args;

      // Initialize log console
      LogConsole.Main.LogConsole.LogInstance.InitializeLogConsole();

      // Check if an other instance is running.
      MinaryProcess.GetInstance().HandleRunningInstances();

      // Set current Debugging mode in GUI
      if (Debugging.IsDebuggingOn())
      {
        this.tsmi_Debugging.Text = "Turn debugging off";
        this.SetAppTitle("Debugging");
      }
      else
      {
        this.tsmi_Debugging.Text = "Turn debugging on";
        this.SetAppTitle(string.Empty);
      }

      // Populate network interface.
      this.LoadNicSettings();
      LogConsole.Main.LogConsole.LogInstance.LogMessage("Current directory : {0}", Directory.GetCurrentDirectory());

      // Start data input thread.
      this.inputModule.StartInputThread();

      if (this.cb_Interfaces.Items.Count <= 0)
      {
        MessageBox.Show("No active network adapter found!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
      }
      else
      {
        // Init ArpScan console
        try
        {
          Minary.ArpScan.Presentation.ArpScan.InitArpScan(this, ref this.targetList);
        }
        catch (Exception ex)
        {
          LogConsole.Main.LogConsole.LogInstance.LogMessage("Main() : {0}", ex.StackTrace);
          Application.Exit();
        }

        this.attackStarted = false;
      }

      // Check if new Minary version is available
      Thread updateThread = new Thread(delegate()
      {
        Minary.Common.Updates.CheckForMinaryUpdates();
      });
      updateThread.Start();

      // Download attack pattern updates
      Thread syncThread = new Thread(delegate()
      {
        Minary.Common.Updates.SyncAttackPatterns();
      });
      syncThread.Start();

      // Set .mry file extension association
      Minary.Common.Associations.MryFiles.InstallMryFileAssociation();

      // Load template handler on the task layer. No UI needed.
      this.templateTaskLayer = Template.Task.TemplateHandler.GetInstance(this);

      // Load and initialize all plugins
      this.tabPageHandler = new TabPageHandler(this.tc_Plugins, this);
      this.pluginHandler.LoadPlugins();
    }


    /// <summary>
    ///
    /// </summary>
    /// <returns></returns>
    public string GetCurrentInterface()
    {
      string retVal = string.Empty;

      try
      {
        retVal = Config.GetNetworkInterfaceIDByIndexNumber(this.cb_Interfaces.SelectedIndex);
      }
      catch (Exception)
      {
      }

      return retVal;
    }


    /// <summary>
    ///
    /// </summary>
    public void PassNewTargetListToPlugins()
    {
      List<Tuple<string, string, string>> newTargetList = new List<Tuple<string, string, string>>();
      List<TargetRecord> reachableTargetSystems = ArpScan.Presentation.ArpScan.GetInstance().TargetList.ToList();

      if (reachableTargetSystems == null || reachableTargetSystems.Count <= 0)
      {
        return;
      }

      foreach (TargetRecord targetSystem in reachableTargetSystems)
      {
        try
        {
            newTargetList.Add(new Tuple<string, string, string>(targetSystem.IpAddress, targetSystem.MacAddress, targetSystem.Vendor));
        }
        catch (Exception)
        {
        }
      }

      // newTargetList.Add("0.0.0.0");
      foreach (string tmpKey in this.pluginHandler.TabPagesCatalog.Keys)
      {
        try
        {
          LogConsole.Main.LogConsole.LogInstance.LogMessage("Minary: Passing new target list to plugin \"{0}\"", tmpKey);
          this.pluginHandler.TabPagesCatalog[tmpKey].PluginObject.SetTargets(newTargetList.ToList());
        }
        catch (Exception ex)
        {
          LogConsole.Main.LogConsole.LogInstance.LogMessage("Minary: {0}\r\n{1}", ex.Message, ex.StackTrace);
        }
      }
    }


    public delegate void OnServiceExicedUnexpectedlyDelegate(string serviceName);
    public void OnServiceExicedUnexpectedly(string serviceName)
    {
      if (this.InvokeRequired)
      {
        this.BeginInvoke(new OnServiceExicedUnexpectedlyDelegate(this.OnServiceExicedUnexpectedly), new object[] { serviceName });
        return;
      }

      // Set service status on GUI footer
      ////      this.SetNewState(serviceName, Status.Error);
      //// this.attackServiceHandler.AttackServices[serviceName].Status = MinaryLib.AttackService.ServiceStatus.Error;

      this.Cursor = Cursors.WaitCursor;
      this.EnableGUIElements();

      if (!this.bgwOnStopAttack.IsBusy)
      {
        this.bgwOnStopAttack.RunWorkerAsync();
      }

      // Set service status
      this.attackStarted = false;
      this.bt_Attack.BackgroundImage = (System.Drawing.Image)Properties.Resources.StartBig;
      this.Cursor = Cursors.Default;

      // Report service failure
      string message = string.Format("The attack service \"{0}\" failed unexpectedly", serviceName);
      MessageBox.Show(message, "Attack service error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
    }


    public void SetNewAttackServiceState(string serviceName, ServiceStatus newStatus)
    {
      if (string.IsNullOrEmpty(serviceName))
      {
        return;
      }

      int tmpNewServiceStatus = (newStatus >= 0) ? (int)newStatus : (int)MinaryLib.AttackService.ServiceStatus.NotRunning;
      this.attackServiceMap[serviceName].Image = this.il_AttackServiceStat.Images[tmpNewServiceStatus];
    }


    public void RegisterService(string serviceName)
    {
      if (string.IsNullOrEmpty(serviceName))
      {
        return;
      }

      foreach (PictureBox guiElement in this.Controls.OfType<PictureBox>())
      {
        if (guiElement.Tag != null && guiElement.Tag.ToString() == serviceName)
        {
          this.attackServiceMap.Add(serviceName, guiElement);
          LogConsole.Main.LogConsole.LogInstance.LogMessage("AttackServiceHandler.RegisterService(): Registered attack service {0}, linked to label {1}", serviceName, guiElement.Name);
          break;
        }
      }
    }

    #endregion

  }
}