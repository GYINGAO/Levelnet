
using LevelnetAdjustment.form;
using LevelnetAdjustment.model;
using LevelnetAdjustment.utils;
using Newtonsoft.Json;
using SplashScreenDemo;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace LevelnetAdjustment {
  public partial class MainForm : Form {

    // 输出文件格式
    public readonly string split = new string('-', 80);
    public readonly string space = new string(' ', 30);

    public ClevelingAdjust ClAdj { get; set; }

    public ProjectInfo Project { get; set; }

    public static bool flag = true;

    public bool IsImport { get; set; } = false;

    public string StartProj { get; set; } = "";

    public bool DirectExit { get; set; } = false;



    /// <summary>
    /// 构造函数
    /// </summary>
    public MainForm(string path) {
      InitializeComponent();
      this.DoubleBuffered = true;//设置本窗体
      this.StartProj = path;
      SetStyle(ControlStyles.UserPaint, true);
      SetStyle(ControlStyles.AllPaintingInWmPaint, true); // 禁止擦除背景.
      SetStyle(ControlStyles.DoubleBuffer, true); // 双缓冲
      SetStyle(ControlStyles.ResizeRedraw, true);
      SetStyle(ControlStyles.OptimizedDoubleBuffer, true);

      this.UpdateStyles();
    }
    private void MainForm_Load(object sender, EventArgs e) {
      tabControl1.TabPages.Clear();
      tabControl1.Visible = false;    // 没有元素的时候隐藏自己

      //修改其显示为当前时间
      this.toolStripStatusLabel3.Text = "系统当前时间：" + DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss");
      //对timer1进行相关设置
      this.timer1.Interval = 1000;
      this.timer1.Start();

      //设置菜单图标
      menuStrip1.RenderMode = ToolStripRenderMode.Professional;
      NewDropItem.Image = Properties.Resources._new;
      toolStripMenuItem_open.Image = Properties.Resources.open;
      ExitDropItem.Image = Properties.Resources.close2;
      生成平差文件ToolStripMenuItem.Image = Properties.Resources.TXT;
      生成观测手簿ToolStripMenuItem.Image = Properties.Resources.excel_01;
      AboutDropItem.Image = Properties.Resources.about2;
      使用说明ToolStripMenuItem.Image = Properties.Resources.帮助中心编辑;
      设置处理参数ToolStripMenuItem.Image = Properties.Resources.设置;
      导入观测文件ToolStripMenuItem.Image = Properties.Resources.import11;
      导入已知点ToolStripMenuItem.Image = Properties.Resources.坐标;
      观测数据检核ToolStripMenuItem.Image = Properties.Resources.辅助检查;
      选择平差文件ToolStripMenuItem.Image = Properties.Resources.选择文件;
      往返测高差较差ToolStripMenuItem.Image = Properties.Resources.比较图;
      ClosureErrorDropItem.Image = Properties.Resources.班级圈;
      GrossErrorDropItem.Image = Properties.Resources.搜索;
      ConstraintNetworkDropItem.Image = Properties.Resources.约束;
      RankDefectNetworkDropItem.Image = Properties.Resources.自由;
      检查更新ToolStripMenuItem.Image = Properties.Resources.更新2;
      清空数据ToolStripMenuItem.Image = Properties.Resources.清空;
      高程平差报表ToolStripMenuItem.Image = Properties.Resources.txt2;
      高差平差报表ToolStripMenuItem.Image = Properties.Resources.txt2;
      多期对比ToolStripMenuItem.Image = Properties.Resources.comparison;
      eyesToolStripMenuItem.Image = Properties.Resources.eye_open;

      //部分按钮禁用
      水准仪数据预处理ToolStripMenuItem.Enabled = false;
      AdjToolStripMenuItem.Enabled = false;
      高程平差报表ToolStripMenuItem.Enabled = false;
      高差平差报表ToolStripMenuItem.Enabled = false;


      //添加最近打开的项目
      Configuration config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
      var settings = config.AppSettings.Settings;
      if (settings.Count != 0) {
        ToolStripMenuItem terMenu;
        foreach (var key in ConfigurationManager.AppSettings.AllKeys) {
          terMenu = new ToolStripMenuItem {
            Name = key,
            Text = settings[key].Value
          };
          //注册事件
          terMenu.Click += new EventHandler(terMenu_Click);
          ((ToolStripDropDownItem)((ToolStripDropDownItem)menuStrip1.Items["FileToolStripMenuItem"]).DropDownItems["toolStripMenuItem_open"]).DropDownItems.Add(terMenu);
        };
      }

      this.BackgroundImage = Properties.Resources.backgroundimage;
      this.BackgroundImageLayout = ImageLayout.Stretch;
      Update();
      if (StartProj != "") {
        OpenProj(StartProj);
      }
    }

    /// <summary>
    /// 动态更新菜单栏
    /// </summary>
    /// <param name="value"></param>
    public void UpDateMenu(string value) {
      string key = ConfigHelper.AddAppSetting(value);
      if (key == "") {
        return;
      }
      ToolStripMenuItem terMenu = new ToolStripMenuItem {
        Name = key,
        Text = value
      };
      //注册事件
      terMenu.Click += new EventHandler(terMenu_Click);
      ((ToolStripDropDownItem)((ToolStripDropDownItem)menuStrip1.Items["FileToolStripMenuItem"]).DropDownItems["toolStripMenuItem_open"])
          .DropDownItems.Add(terMenu);
    }

    /// <summary>
    /// 动态添加子菜单事件
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    public void terMenu_Click(object sender, EventArgs e) {
      ToolStripMenuItem downItem = sender as ToolStripMenuItem;
      string path = Path.Combine(downItem.Text, Path.GetFileName(downItem.Text) + ".laproj");
      if (!File.Exists(path)) {
        MessageBox.Show("该项目已被删除！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Error);
        ConfigHelper.DeleteAppSettings(downItem.Name);
        ((ToolStripDropDownItem)((ToolStripDropDownItem)menuStrip1.Items["FileToolStripMenuItem"]).DropDownItems["toolStripMenuItem_open"]).DropDownItems.Remove(downItem);
        return;
      }

      OpenProj(path);
    }


    #region 文件管理
    /// <summary>
    /// 设置与选项
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>





    /// <summary>
    /// 关闭所有的子窗体
    /// </summary>
    void ClearForms() {
      FormCollection childCollection = Application.OpenForms;
      for (int i = childCollection.Count; i-- > 0;) {
        if (childCollection[i].Name != Name) childCollection[i].Close();
      }
      tabControl1.TabPages.Clear();
      tabControl1.Visible = false;    // 没有元素的时候隐藏自己
    }

    /// <summary>
    /// 创建项目
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void NewDropItem_Click(object sender, EventArgs e) {
      CreatePrj prj = new CreatePrj(Project);
      prj.TransfEvent += UpDateProject;
      prj.ShowDialog();
    }

    /// <summary>
    /// 更新项目信息
    /// </summary>
    /// <param name="project"></param>
    public void UpDateProject(ProjectInfo project) {
      水准仪数据预处理ToolStripMenuItem.Enabled = true;
      AdjToolStripMenuItem.Enabled = true;

      this.ClAdj = new ClevelingAdjust();
      this.Project = project;
      ClAdj.Options = project.Options;
      var projname = Path.Combine(project.Path, project.Name);
      UpDateMenu(projname);
      toolStripStatusLabel2.Text = "当前项目：" + projname;
      ClearForms();
    }

    /// <summary>
    /// 找到已打开文件tab索引
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    int FindIndexFromTabControl(string value) {
      var pages = tabControl1.TabPages;
      for (int i = 0; i < pages.Count; i++) {
        if (pages[i].Text == Path.GetFileName(value)) {
          //(tabControl1.TabPages[i].Tag as Form).Focus();
          return i;
        }
      }
      return -1;
    }

    /// <summary>
    /// 打开项目
    /// </summary>
    void OpenProj(string projname) {
      if (Project != null) {
        if (toolStripStatusLabel2.Text == "当前项目：" + Path.GetDirectoryName(projname)) {
          MessageBox.Show("打开了相同的项目！", "警告", MessageBoxButtons.OK, MessageBoxIcon.Warning);
          return;
        }
        // 打开项目保存上一个项目
        else {
          SaveProject();
        }
      }
      ClAdj = new ClevelingAdjust();
      UpDateMenu(Path.GetDirectoryName(projname));
      ClearForms();
      // 读取文件信息
      Project = JsonHelper.ReadJson(projname);
      var dic = Path.GetDirectoryName(projname);
      Project.Path = dic.Substring(0, dic.LastIndexOf(@"\"));
      Project.Name = dic.Substring(dic.LastIndexOf(@"\") + 1);
      ClAdj.Options = Project.Options;
      ClAdj.RawDatas = Project.RawDatas;
      ClAdj.ObservedDatas = Project.ObservedDatas;
      ClAdj.KnownPoints = Project.KnownPoints;
      ClAdj.UnknownPoints = Project.UnknownPoints;
      ClAdj.Calc_Params(false);
      // 获取所有文件
      FileInfo[] files = new DirectoryInfo(Path.Combine(Path.GetDirectoryName(projname), "ExportFiles")).GetFiles();
      foreach (var file in files) {
        if (!file.Extension.ToLower().Contains("txt")) {
          continue;
        }
        AddTabPage(file.FullName);  // 新建窗体同时新建一个标签
      }
      toolStripStatusLabel2.Text = "当前项目：" + Path.GetDirectoryName(projname);
      MessageBox.Show("打开成功", "提示", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
      水准仪数据预处理ToolStripMenuItem.Enabled = true;
      AdjToolStripMenuItem.Enabled = true;
    }

    /// <summary>
    /// 触发打开项目事件
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void toolStripMenuItem_choose_Click(object sender, EventArgs e) {
      OpenFileDialog openFile = new OpenFileDialog {
        Multiselect = false,
        Title = "打开",
        Filter = "项目文件|*.laproj",
        FilterIndex = 1,
      };
      if (openFile.ShowDialog() == DialogResult.OK) {
        OpenProj(openFile.FileName);
      }
    }

    /// <summary>
    /// 触发保存项目
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void toolStripButton1_Click(object sender, EventArgs e) {
      if (Project == null) {
        return;
      }
      SaveProject();
      MessageBox.Show("保存成功", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
    }

    /// <summary>
    /// 保存项目
    /// </summary>
    private void SaveProject() {
      //没有导入文件不需要保存
      /* if (!IsImport) {
           return;
       }*/
      Project.UnknownPoints = ClAdj.UnknownPoints;
      Project.Options = ClAdj.Options;
      Project.RawDatas = ClAdj.RawDatas;
      Project.ObservedDatas = ClAdj.ObservedDatas;
      Project.KnownPoints = ClAdj.KnownPoints;
      JsonHelper.WriteJson(Project);
    }

    /// <summary>
    /// 清空最近打开的文件
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void toolStripMenuItem_clear_Click(object sender, EventArgs e) {
      ConfigHelper.DelAllSettings();
      while (((ToolStripDropDownItem)((ToolStripDropDownItem)menuStrip1.Items["FileToolStripMenuItem"]).DropDownItems["toolStripMenuItem_open"]).DropDownItems.Count > 2) {
        ((ToolStripDropDownItem)((ToolStripDropDownItem)menuStrip1.Items["FileToolStripMenuItem"]).DropDownItems["toolStripMenuItem_open"]).DropDownItems.RemoveAt(((ToolStripDropDownItem)((ToolStripDropDownItem)menuStrip1.Items["FileToolStripMenuItem"]).DropDownItems["toolStripMenuItem_open"]).DropDownItems.Count - 1);
      }
    }

    #endregion

    #region 数据处理
    /// <summary>
    /// 约束网平差按钮
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void LevelnetDropItem_Click(object sender, EventArgs e) {
      if (ClAdj.ObservedDatasNoRep == null) {
        throw new Exception("请选择平差文件");
      }
      if (File.Exists(Project.Options.OutputFiles.OutpathAdj)) {
        if (MessageBox.Show("平差结果文件已存在，是否重新计算？", "提示", MessageBoxButtons.YesNo, MessageBoxIcon.Information) == DialogResult.No) {

          AddTabPage(Project.Options.OutputFiles.OutpathAdj);  // 新建窗体同时新建一个标签
          return;
        }
      }

      ChooseKnownPoint chooseStablePoint = new ChooseKnownPoint(ClAdj.KnownPoints);
      chooseStablePoint.TransfChangeKnownPoint += CalcLS;
      chooseStablePoint.ShowDialog();
    }

    void CalcLS(List<PointData> Points) {
      ClAdj.KnownPoints = Points;
      ClAdj.Calc_Params();
      SimpleLoading loadingfrm = new SimpleLoading(this, "约束网平差中，请稍等...");
      //将Loaing窗口，注入到 SplashScreenManager 来管理
      GF2Koder.SplashScreenManager loading = new GF2Koder.SplashScreenManager(loadingfrm);
      loading.ShowLoading();
      /*      try {*/
      int i = ClAdj.LS_Adjustment();
      ClAdj.ExportAdjustResult(Project.Options.OutputFiles.OutpathAdj, split, space, "约束网");
      loading.CloseWaitForm();
      AddTabPage(Project.Options.OutputFiles.OutpathAdj);  // 新建窗体同时新建一个标签
      MessageBox.Show($"水准网平差完毕，迭代次数：{i}", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
      /*     }
           catch (Exception ex) {
               loading.CloseWaitForm();
               throw ex;
           }*/
    }


    /// <summary>
    /// 秩亏网平差
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void RankDefectNetworkDropItem_Click(object sender, EventArgs e) {
      if (ClAdj.ObservedDatasNoRep == null) {
        throw new Exception("请选择平差文件");
      }
      if (File.Exists(Project.Options.OutputFiles.OutpathAdjFree)) {
        if (MessageBox.Show("平差结果文件已存在，是否重新计算？", "提示", MessageBoxButtons.YesNo, MessageBoxIcon.Information) == DialogResult.No) {
          AddTabPage(Project.Options.OutputFiles.OutpathAdjFree);  // 新建窗体同时新建一个标签
          return;
        }
      }
      ClAdj.CalcApproximateHeight();
      ChooseStablePoint chooseStablePoint = new ChooseStablePoint(ClAdj.AllPoints);
      chooseStablePoint.TransfChangeStable += CalcStable;
      chooseStablePoint.ShowDialog();
    }

    private void CalcStable(List<PointData> Points) {
      ClAdj.AllPoints = Points;
      //ClAdj.AllPoints = Commom.Merge(ClAdj.KnownPointEable, Points);
      SimpleLoading loadingfrm = new SimpleLoading(this, "计算中，请稍等...");
      //将Loaing窗口，注入到 SplashScreenManager 来管理
      GF2Koder.SplashScreenManager loading = new GF2Koder.SplashScreenManager(loadingfrm);
      loading.ShowLoading();
      try {
        var i = 0;
        // 有拟稳点
        int count = ClAdj.AllPoints.FindAll(p => p.IsStable == true).Count;
        if (count <= 0) {
          i = ClAdj.FreeNetAdjust();
          ClAdj.ExportAdjustResult(Project.Options.OutputFiles.OutpathAdjFree, split, space, "自由网");
        }
        else {
          i = ClAdj.QuasiStable();
          ClAdj.ExportAdjustResult(Project.Options.OutputFiles.OutpathAdjFree, split, space, "拟稳");
        }

        loading.CloseWaitForm();
        AddTabPage(Project.Options.OutputFiles.OutpathAdjFree);  // 新建窗体同时新建一个标签
        MessageBox.Show($"水准网平差完毕，迭代次数：{i}", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
      }
      catch (Exception ex) {
        loading.CloseWaitForm();
        throw ex;
      }
    }



    /// <summary>
    /// 计算闭合差
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void ClosureErrorDropItem_Click(object sender, EventArgs e) {
      if (File.Exists(Project.Options.OutputFiles.OutpathClosure)) {
        if (MessageBox.Show("闭合差结果文件已存在，是否重新计算？", "提示", MessageBoxButtons.YesNo, MessageBoxIcon.Information) == DialogResult.No) {
          AddTabPage(Project.Options.OutputFiles.OutpathClosure);  // 新建窗体同时新建一个标签
          return;
        }
      }

      ChooseKnownPoint chooseStablePoint = new ChooseKnownPoint(ClAdj.KnownPoints);
      chooseStablePoint.TransfChangeKnownPoint += CalcClosureError;
      chooseStablePoint.ShowDialog();
    }

    void CalcClosureError(List<PointData> Points) {
      ClAdj.KnownPoints = Points;
      ClAdj.Calc_Params(false);
      SimpleLoading loadingfrm = new SimpleLoading(this, "计算中，请稍等...");
      //将Loaing窗口，注入到 SplashScreenManager 来管理
      GF2Koder.SplashScreenManager loading = new GF2Koder.SplashScreenManager(loadingfrm);
      loading.ShowLoading();
      try {
        ClAdj.CalcClosureError(Project.Options.OutputFiles.OutpathClosure, split, space);
        loading.CloseWaitForm();

        AddTabPage(Project.Options.OutputFiles.OutpathClosure);  // 新建窗体同时新建一个标签
        MessageBox.Show("闭合差计算完毕", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
      }
      catch (Exception ex) {
        loading.CloseWaitForm();
        throw ex;
      }
    }


    /// <summary>
    /// 粗差探测按钮
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void GrossErrorDropItem_Click(object sender, EventArgs e) {
      if (File.Exists(Project.Options.OutputFiles.OutpathGrossError)) {
        if (MessageBox.Show("粗差结果文件已存在，是否重新计算？", "提示", MessageBoxButtons.YesNo, MessageBoxIcon.Information) == DialogResult.No) {
          AddTabPage(Project.Options.OutputFiles.OutpathGrossError);  // 新建窗体同时新建一个标签
          return;
        }
      }
      flag = true;
      SimpleLoading loadingfrm = new SimpleLoading(this, "计算中，请稍等...");
      //将Loaing窗口，注入到 SplashScreenManager 来管理
      GF2Koder.SplashScreenManager loading = new GF2Koder.SplashScreenManager(loadingfrm);
      loading.ShowLoading();

      try {
        ClAdj.FindGrossError(split, space, Project.Options.OutputFiles.OutpathGrossError);
        loading.CloseWaitForm();

        AddTabPage(Project.Options.OutputFiles.OutpathGrossError);  // 新建窗体同时新建一个标签
        MessageBox.Show("粗差探测完毕", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
      }
      catch (Exception ex) {
        loading.CloseWaitForm();
        throw ex;
      }
    }

    #endregion

    #region 导出报表
    /// <summary>
    /// 导出观测手簿
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void HandbookDropItem_Click(object sender, EventArgs e) {

    }


    #endregion

    #region 设置多标签页面
    /// <summary>
    /// 添加一个标签
    /// </summary>
    /// <param name="frm"></param>
    private void AddTabPage(string filename) {
      var idx = FindIndexFromTabControl(filename);
      if (idx != -1) {
        CloseTabPage(idx);
      }
      TabPage tp;
      var ext = Path.GetExtension(filename);
      if (ext.Contains("xls")) {
        ExcelView excel = new ExcelView(filename) {
          MdiParent = this,//WindowState = FormWindowState.Maximized,
          ShowIcon = false,
          ShowInTaskbar = false,
          Dock = DockStyle.Fill,
          FormBorderStyle = FormBorderStyle.None,
        };
        excel.Show();
        tp = new TabPage {
          Tag = excel,  // 当前标签控制的窗体对象记录在Tag属性中
          Text = excel.Text,
          ToolTipText = excel.Text
        };
      }
      else {
        FileView fv = new FileView(filename) {
          MdiParent = this,//WindowState = FormWindowState.Maximized,
          ShowIcon = false,
          ShowInTaskbar = false,
          Dock = DockStyle.Fill,
          FormBorderStyle = FormBorderStyle.None,
        };
        fv.Show();
        tp = new TabPage {
          Tag = fv,  // 当前标签控制的窗体对象记录在Tag属性中
          Text = fv.Text,
          ToolTipText = fv.Text
        };
      }
      tabControl1.TabPages.Add(tp);
      tabControl1.SelectedIndex = tabControl1.TabCount - 1;  // 默认选中最后一个新建的标签
      if (!tabControl1.Visible) tabControl1.Visible = true;  // 如果自己是隐藏的则显示自己
    }

    private void menuStrip1_ItemAdded(object sender, ToolStripItemEventArgs e) {
      /*if (e.Item.Text.Length == 0 || e.Item.Text == "还原(&R)" || e.Item.Text == "最小化(&N)" || e.Item.Text == "关闭(&C)") {
          e.Item.Visible = false;
      }*/
    }

    /// <summary>
    /// 点击标签切换页面
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void tabControl1_SelectedIndexChanged(object sender, EventArgs e) {
      if (tabControl1.SelectedIndex > -1)
        (tabControl1.TabPages[tabControl1.SelectedIndex].Tag as Form).Focus();
      else {
        this.BackgroundImage = Properties.Resources.backgroundimage;
        this.BackgroundImageLayout = ImageLayout.Stretch;
      }
    }

    /// <summary>
    /// 删除一个标签
    /// </summary>
    /// <param name="selectedIndex"></param>
    private void CloseTabPage(int selectedIndex) {
      (tabControl1.TabPages[selectedIndex].Tag as Form).Close();
      tabControl1.TabPages.RemoveAt(selectedIndex);
      if (tabControl1.TabPages.Count == 0) tabControl1.Visible = false;
    }

    private void tabControl1_MouseDoubleClick_1(object sender, MouseEventArgs e) {
      if (e.Button == System.Windows.Forms.MouseButtons.Left) // 只有左键双击才响应关闭
        CloseTabPage(tabControl1.SelectedIndex);
    }

    /// <summary>
    /// 关闭当前页面
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void toolStripMenuItem_close_Click(object sender, EventArgs e) {
      int index = GetPageIndexWidthPoint(contextMenuStrip1.Left - this.Left);  // 这里也需要通过弹出菜单的位置来得到当前是哪个项弹出的，注意菜单位置是针对屏幕左边的距离
      CloseTabPage(index);
    }
    /// <summary>
    /// 关闭其他页面
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void ToolStripMenuItem_closeothers_Click(object sender, EventArgs e) {
      int index = GetPageIndexWidthPoint(contextMenuStrip1.Left - this.Left);
      for (int i = tabControl1.TabPages.Count - 1; i >= 1; i--) {
        CloseTabPage(i);
      }
      for (int i = tabControl1.TabPages.Count - 2; i >= 0; i--) {
        CloseTabPage(i);
      }

    }
    /// <summary>
    /// 关闭右侧页面
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void ToolStripMenuItem_closeright_Click(object sender, EventArgs e) {
      int index = GetPageIndexWidthPoint(contextMenuStrip1.Left - this.Left);
      for (int i = tabControl1.TabPages.Count - 1; i > index; i--) {
        CloseTabPage(i);
      }
    }
    /// <summary>
    /// 关闭所有页面
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void ToolStripMenuItem_closeall_Click(object sender, EventArgs e) {
      for (int i = tabControl1.TabPages.Count - 1; i >= 0; i--) {
        CloseTabPage(i);
      }
    }

    /// <summary>
    /// 在文件夹中打开
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void 在文件夹中打开ToolStripMenuItem_Click(object sender, EventArgs e) {
      int index = GetPageIndexWidthPoint(contextMenuStrip1.Left - this.Left);
      string path = (tabControl1.TabPages[index].Tag as FileView).FilePath;
      Process.Start("Explorer", "/select," + path);
    }

    /// <summary>
    /// 从菜单弹出位置得到当前所在的标签索引
    /// </summary>
    /// <returns></returns>
    private int GetPageIndexWidthPoint(int pointX) {
      int x = 0;
      for (int i = 0; i < tabControl1.TabPages.Count; ++i) {
        if (pointX >= x && pointX <= x + tabControl1.ItemSize.Width)
          return i;
        x += tabControl1.ItemSize.Width;
      }
      return tabControl1.TabPages.Count - 1;
    }
    #endregion


    /// <summary>
    /// 退出程序
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void ExitDropItem_Click(object sender, EventArgs e) {
      Application.Exit();
    }

    /// <summary>
    /// 确认关闭事件
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void MainForm_FormClosing(object sender, FormClosingEventArgs e) {
      // 直接退出
      if (DirectExit) {
        return;
      }
      if (Project != null) {
        SaveProject();
      }
      DialogResult result = MessageBox.Show("您确定要关闭软件吗？", "退出提示",
      MessageBoxButtons.YesNo, MessageBoxIcon.Question);
      if (result == DialogResult.Yes)
        Application.ExitThread();
      else {
        e.Cancel = true;
      }
    }


    /// <summary>
    /// 打开关于窗口
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void AboutDropItem_Click(object sender, EventArgs e) {
      About about = new About();
      about.ShowDialog();
    }

    private void 新建NToolStripButton_Click(object sender, EventArgs e) {
      NewDropItem_Click(sender, e);
    }

    private void 打开OToolStripButton_Click(object sender, EventArgs e) {
      toolStripMenuItem_choose_Click(sender, e);
    }

    private void 帮助LToolStripButton_Click(object sender, EventArgs e) {
      使用说明ToolStripMenuItem_Click(sender, e);
    }

    private void 使用说明ToolStripMenuItem_Click(object sender, EventArgs e) {
      var helpFile = Path.Combine(Application.StartupPath, "Readme.doc");
      if (!File.Exists(helpFile)) {
        ByteHelper.WriteByteToFile(Properties.Resources.Readme, helpFile);
      }
      Process.Start(helpFile);
    }
    private void timer1_Tick(object sender, EventArgs e) {
      this.toolStripStatusLabel3.Text = "系统当前时间：" + DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss");
    }

    private void MainForm_MdiChildActivate(object sender, EventArgs e) {

      this.BackgroundImage = null;

    }

    private void menuStrip1_ItemClicked(object sender, ToolStripItemClickedEventArgs e) {
      //没有新建项目
      if (ClAdj == null) {
        水准仪数据预处理ToolStripMenuItem.Enabled = false;
        AdjToolStripMenuItem.Enabled = false;
        return;
      }
      else {
        水准仪数据预处理ToolStripMenuItem.Enabled = true;
        AdjToolStripMenuItem.Enabled = true;
      }
      // 没有导入观测文件
      if (ClAdj.RawDatas?.Count == 0) {
        观测数据检核ToolStripMenuItem.Enabled = false;
        生成平差文件ToolStripMenuItem.Enabled = false;
        生成观测手簿ToolStripMenuItem.Enabled = false;
        清空数据ToolStripMenuItem.Enabled = false;
      }
      else {
        观测数据检核ToolStripMenuItem.Enabled = true;
        生成平差文件ToolStripMenuItem.Enabled = true;
        生成观测手簿ToolStripMenuItem.Enabled = true;
        清空数据ToolStripMenuItem.Enabled = true;
      }

      //没有导入平差文件
      if (ClAdj.ObservedDatas?.Count == 0) {
        往返测高差较差ToolStripMenuItem.Enabled = false;
        ClosureErrorDropItem.Enabled = false;
        GrossErrorDropItem.Enabled = false;
        ConstraintNetworkDropItem.Enabled = false;
        RankDefectNetworkDropItem.Enabled = false;
      }
      else {
        往返测高差较差ToolStripMenuItem.Enabled = true;
        ClosureErrorDropItem.Enabled = true;
        GrossErrorDropItem.Enabled = true;
        ConstraintNetworkDropItem.Enabled = true;
        RankDefectNetworkDropItem.Enabled = true;
      }
      高程平差报表ToolStripMenuItem.Enabled = ClAdj.X?.RowCount > 0;
      高差平差报表ToolStripMenuItem.Enabled = ClAdj.L?.Length > 0;
    }

    private void MainForm_DragEnter(object sender, DragEventArgs e) {
      if (e.Data.GetDataPresent(DataFormats.FileDrop)) {
        e.Effect = DragDropEffects.Link;
      }
      else {
        e.Effect = DragDropEffects.None;
      }
    }

    private void MainForm_DragDrop(object sender, DragEventArgs e) {
      string path = ((Array)e.Data.GetData(DataFormats.FileDrop)).GetValue(0).ToString();
      if (path.EndsWith(".laproj")) {
        OpenProj(path);
      }
    }

    new void Update() {
      var Version = Application.ProductVersion.ToString();
      string checkURL = "http://43.142.49.203:7001/check?Version=" + Version;//检测版本更新地址

      try {

        string getJson = HttpHelper.Get(checkURL);
        Response res = JsonConvert.DeserializeObject<Response>(getJson);
        if (res.Update) {
          DialogResult dr = MessageBox.Show("检测到新版本：" + res.LatestVersion + "\r\n当前版本：" + res.CurrentVersion + "\r\n更新内容：\r\n" + res.Remark + "\r\n\r\n是否更新?", "更新提示", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
          if (dr == DialogResult.Yes) {
            //string downloadURL = "http://43.142.49.203:7001/public/" + res.AppName;//下载EXE的地址
            string downloadURL = "http://43.142.49.203:7001/download";//下载EXE的地址
            Process.Start(downloadURL);
            DirectExit = true;
            this.Close();
          }

        }
      }
      catch (Exception ex) { throw ex; }
    }


    public partial class Response {
      public string CurrentVersion { get; set; } = "";
      public string LatestVersion { get; set; } = "";
      public string Remark { get; set; } = "";
      public bool Update { get; set; }
      public string AppName { get; set; } = "";

    }

    private void 检查更新ToolStripMenuItem_Click(object sender, EventArgs e) {
      Update();
    }

    private void 生成观测手簿ToolStripMenuItem_Click(object sender, EventArgs e) {
      if (File.Exists(Project.Options.OutputFiles.Handbook)) {
        if (MessageBox.Show("观测手簿已存在，是否查看？", "提示", MessageBoxButtons.YesNo, MessageBoxIcon.Information) == DialogResult.Yes) {
          Process.Start(Project.Options.OutputFiles.Handbook);
          return;
        }
      }
      SimpleLoading loadingfrm = new SimpleLoading(this, "导出中，请稍等...");
      //将Loaing窗口，注入到 SplashScreenManager 来管理
      GF2Koder.SplashScreenManager loading = new GF2Koder.SplashScreenManager(loadingfrm);
      loading.ShowLoading();
      try {
        var path = ExceHelperl.ExportHandbook(ClAdj.RawDatas, Project.Options.OutputFiles.Handbook, ClAdj.Options.ImportFiles);
        loading.CloseWaitForm();
        if (MessageBox.Show("导出成功，是否查看？", "提示", MessageBoxButtons.YesNo, MessageBoxIcon.Information) == DialogResult.Yes)
          Process.Start(path);
      }
      catch (Exception ex) {
        loading.CloseWaitForm();
        throw ex;
      }
    }

    private void 生成平差文件ToolStripMenuItem_Click(object sender, EventArgs e) {
      /*FrmZDSelect frmZDSelect = new FrmZDSelect();
      frmZDSelect.TransfEvevn += ChangeZD;
      frmZDSelect.ShowDialog();*/

      int stationNum = 0;
      if (ClAdj.Options.ManualModification.ChangedStationLine?.Count == 0) {
        ArrayList array = new ArrayList();
        foreach (var item in ClAdj.RawDatas) {
          if (item.IsStart) {
            stationNum++;
          }
          array.Add(item.BackPoint);
          if (item.IsEnd) {
            array.Add(item.FrontPoint);
            ClAdj.Options.ManualModification.ChangedStationLine.Add(Commom.Clone(array));
            array.Clear();
          }
        }
      }


      FrmModifyPointName frm = new FrmModifyPointName(ClAdj.RawDatas, ClAdj.KnownPoints, Project.Options.OutputFiles.COSADis, ClAdj.Options.ManualModification);
      frm.TransEvent += ExportFile;
      MessageBox.Show("请检查每个测段点名是否重复\r\n如重复请修改，否则将按照同一个点处理！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
      frm.ShowDialog();
    }

    void ExportFile(ManualModify manualModification) {
      ClAdj.Options.ManualModification = manualModification;
      Project.Options.ManualModification = manualModification;
      AddTabPage(Project.Options.OutputFiles.COSADis);  // 新建窗体同时新建一个标签
    }
    void ChangeZD(string zd) {
      FileHelper.ExportIN1(ClAdj.RawDatas, ClAdj.KnownPoints, zd, Project.Options.OutputFiles.COSADis);
      MessageBox.Show("导出成功", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
      AddTabPage(Project.Options.OutputFiles.COSADis);  // 新建窗体同时新建一个标签
    }
    private void 导入观测文件ToolStripMenuItem_Click(object sender, EventArgs e) {
      OpenFileDialog openFile = new OpenFileDialog {
        Multiselect = true,
        Title = "打开",
        Filter = "Trimble DINI|*.dat;*.DAT|Leica DNA|*.gsi;*.GSI|All files|*.*",
        FilterIndex = 1,
        RestoreDirectory = true,
      };
      if (openFile.ShowDialog() == DialogResult.OK) {
        SimpleLoading loadingfrm = new SimpleLoading(this, "读取中，请稍等...");
        //将Loaing窗口，注入到 SplashScreenManager 来管理
        GF2Koder.SplashScreenManager loading = new GF2Koder.SplashScreenManager(loadingfrm);
        loading.ShowLoading();
        //for (int i = ClAdj.Options.ImportFiles.Count - 1; i >= 0; i--) {
        //  string ext = Path.GetExtension(ClAdj.Options.ImportFiles[i].FileName).ToLower();
        //  if (ext.Contains("dat") || ext.Contains("gsi")) {
        //    ClAdj.Options.ImportFiles.RemoveAt(i);
        //  }
        //}
        try {
          var RawDatas = new List<RawData>();
          foreach (var item in openFile.FileNames) {
            if (ClAdj.Options.ImportFiles.Exists(t => t.FileName == Path.GetFileName(item))) {
              continue;
            }
            //把文件复制到项目文件夹中
            FileInfo fileInfo1 = new FileInfo(item);
            string targetPath = Path.Combine(Project.Path, Project.Name, "ImportFiles", Path.GetFileName(item));
            if (File.Exists(targetPath)) File.Delete(targetPath);
            fileInfo1.CopyTo(targetPath);
            ClAdj.Options.ImportFiles.Add(new InputFile {
              FilePath = item,
              FileName = Path.GetFileName(item)
            });
            switch (Path.GetExtension(item).ToLower()) {
              case ".dat":
                FileHelper.ReadDAT(RawDatas, item);
                break;
              case ".gsi":
                FileHelper.ReadGSI(item, RawDatas);
                break;
              default:
                break;
            }
          }
          ClAdj.RawDatas = ClAdj?.RawDatas.Count != 0 ? Commom.Merge(ClAdj.RawDatas, RawDatas) : RawDatas;
          //ClAdj.RawDatas = RawDatas;
          loading.CloseWaitForm();
        }
        catch (Exception ex) {
          loading.CloseWaitForm();
          throw ex;
        }
      }


    }

    private void 选择平差文件ToolStripMenuItem_Click(object sender, EventArgs e) {
      if (ClAdj.ObservedDatasNoRep.Count != 0) {
        if (MessageBox.Show("是否重新导入？", "提示", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.No) {
          return;
        }
      }
      OpenFileDialog openFile = new OpenFileDialog {
        Multiselect = false,
        Title = "打开",
        Filter = "in1 file|*.in1;*.IN1",
        FilterIndex = 1,
        RestoreDirectory = true,
        InitialDirectory = Path.Combine(Project.Path, Project.Name, "ExportFiles")
      };
      if (openFile.ShowDialog() == DialogResult.OK) {
        var knownPoints = new List<PointData>();
        var observedDatas = new List<ObservedData>();
        FileHelper.ReadOriginalFile(knownPoints, observedDatas, openFile.FileName);
        if (knownPoints.Count == 0) {
          throw new Exception("缺少已知点，请检查");
        }
        ClAdj.KnownPoints = knownPoints;
        ClAdj.ObservedDatas = observedDatas;
        ClAdj.Calc_Params();
        string msg = "";
        int j = 0;
        for (int i = 0; i < ClAdj.KnownPoints.Count; i++) {
          if (ClAdj.ObservedDatasNoRep.Exists(l => l.Start == ClAdj.KnownPoints[i].Number || l.End == ClAdj.KnownPoints[i].Number)) {
            continue;
          }
          else {
            j++;
            msg += $"{j}、点号：{ClAdj.KnownPoints[i].Number}，高程：{ClAdj.KnownPoints[i].Height}\r\n";
            ClAdj.KnownPoints[i].Enable = false;
          }
        }
        if (msg != "") {
          MessageBox.Show($"以下已知点在观测文件中未找到!\r\n{msg}", "警告", MessageBoxButtons.OK, MessageBoxIcon.Warning);
          ClAdj.Calc_Params();
        }
        else {
          MessageBox.Show("读取成功", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
      }
    }

    private void 设置处理参数ToolStripMenuItem_Click(object sender, EventArgs e) {
      FrmADJSetting rd = new FrmADJSetting(ClAdj.Options);
      rd.TransfEvent += ChangeLevelParams;
      rd.ShowDialog();
    }
    void ChangeLevelParams(Option option) {
      this.ClAdj.Options = option;
      this.Project.Options = option;
    }

    private void 导入已知点ToolStripMenuItem_Click(object sender, EventArgs e) {
      OpenFileDialog openFile = new OpenFileDialog {
        Multiselect = false,
        Title = "打开",
        Filter = "已知点文件|*.txt",
        FilterIndex = 1,
        RestoreDirectory = true,
      };
      if (openFile.ShowDialog() == DialogResult.OK) {
        ClAdj.KnownPoints = new List<PointData>();
        FileHelper.ReadKnPoints(openFile.FileName, ClAdj.KnownPoints);
      }
    }

    private void 往返测高差较差ToolStripMenuItem_Click(object sender, EventArgs e) {
      SimpleLoading loadingfrm = new SimpleLoading(this, "计算中，请稍等...");
      //将Loaing窗口，注入到 SplashScreenManager 来管理
      GF2Koder.SplashScreenManager loading = new GF2Koder.SplashScreenManager(loadingfrm);
      loading.ShowLoading();
      try {
        StringBuilder sb = new StringBuilder();
        sb.AppendLine(split);
        sb.AppendLine(space + "往返测高差较差统计结果");
        sb.AppendLine(split);
        sb.AppendLine($"{"序号",-5}{"起点",-8}{"终点",-7}{"往测距离/km",-9}{"返测距离/km",-9}{"往测高差/m",-9}{"返测高差/m",-8}{"较差/mm",-8}{"限差/mm",-10}");

        int i = 0;
        double m = 0.0;
        ClAdj.ObservedDataWFs.ForEach(l => {
          i++;
          double limit = ClAdj.Options.LevelParams.IsCP3 ? ClAdj.Options.LevelParams.CP3WangFan : ClAdj.Options.LevelParams.WangFan * Math.Sqrt((l.Distance_F + l.Distance_W) / 2);
          m += Math.Pow(l.HeightDiff_Diff, 2) / ((l.Distance_W + l.Distance_F) / 2);
          sb.AppendLine($"{i,-7}{l.Start,-10}{l.End,-11}{l.Distance_W,-12:#0.000}{l.Distance_F,-13:#0.000}{l.HeightDiff_W,-12:#0.00000}{l.HeightDiff_F,-13:#0.00000}{l.HeightDiff_Diff,-9:#0.00}{limit,-11:#0.00}");
        });
        m = Math.Sqrt(m / (4 * i));
        sb.AppendLine(split);
        sb.AppendLine($"{space}每公里水准测量的高差偶然中误差：{m,-6:#0.000}(mm)");
        sb.AppendLine(split);
        FileHelper.WriteStrToTxt(sb.ToString(), Project.Options.OutputFiles.WFDiff);
        AddTabPage(Project.Options.OutputFiles.WFDiff);
        loading.CloseWaitForm();
      }
      catch (Exception ex) {
        loading.CloseWaitForm();
        throw ex;
      }


    }
    private void 观测数据检核ToolStripMenuItem_Click(object sender, EventArgs e) {
      FrmCheckObsData frm = new FrmCheckObsData(ClAdj.Options.ObsDataLimits);
      frm.TransfEvent += SetObserverDataLimit;
      frm.ShowDialog();
    }
    void SetObserverDataLimit(ObsDataLimit limit) {
      ClAdj.Options.ObsDataLimits = limit;
      Project.Options.ObsDataLimits = limit;
      StringBuilder sb = new StringBuilder();
      int stationNum = 0;
      double total = 0;
      ClAdj.RawDatas.ForEach(r => {
        stationNum++;
        if (r.IsStart) {
          sb.AppendLine($"测站   后视    前视     后尺读数1   后视距1   前尺读数1   前视距1   后尺读数2   后视距2   前尺读数2   前视距2  前后视距差 累计前后视距差");
          sb.AppendLine("Start-Line");
        }
        double dis = r.DisDiffAve * 1000;
        total += dis;
        string backdiff1 = r.BackDiff1 < ClAdj.Options.ObsDataLimits.StafLow ? r.BackDiff1.ToString("#0.00000") + "!!!" : r.BackDiff1.ToString("#0.00000");
        string backdiff2 = r.BackDiff2 < ClAdj.Options.ObsDataLimits.StafLow ? r.BackDiff2.ToString("#0.00000") + "!!!" : r.BackDiff2.ToString("#0.00000");
        string frontdiff1 = r.FrontDiff1 < ClAdj.Options.ObsDataLimits.StafLow ? r.FrontDiff1.ToString("#0.00000") + "!!!" : r.FrontDiff1.ToString("#0.00000");
        string frontdiff2 = r.FrontDiff2 < ClAdj.Options.ObsDataLimits.StafLow ? r.FrontDiff2.ToString("#0.00000") + "!!!" : r.FrontDiff2.ToString("#0.00000");
        string disdiff = Math.Abs(dis) > ClAdj.Options.ObsDataLimits.FBDis ? dis.ToString("#0.000") + "!!!" : dis.ToString("#0.000");
        string totaldis = Math.Abs(total) > ClAdj.Options.ObsDataLimits.FBDisSum ? total.ToString("#0.000") + "!!!" : total.ToString("#0.000");
        sb.AppendLine($"{stationNum,-5}{r.BackPoint,-10}{r.FrontPoint,-10}{backdiff1,-12}{r.BackDis1 * 1000,-10:#0.000}{frontdiff1,-12}{r.FrontDis1 * 1000,-10:#0.000}{backdiff2,-12}{r.BackDis2 * 1000,-10:#0.000}{frontdiff2,-12}{r.FrontDis2 * 1000,-10:#0.000}{disdiff,-10}{totaldis,-10}");
        if (r.IsEnd) {
          sb.AppendLine("End-Line");
          sb.AppendLine("");
          total = 0;
        }
      });
      FileHelper.WriteStrToTxt(sb.ToString(), Project.Options.OutputFiles.CheakRawData);
      AddTabPage(Project.Options.OutputFiles.CheakRawData);
    }

    private void 清空数据ToolStripMenuItem_Click(object sender, EventArgs e) {
      if (MessageBox.Show("是否清空观测数据？", "提示", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.No) {
        return;
      }
      ClAdj.RawDatas.Clear();
      Project.RawDatas.Clear();
      ClAdj.Options.ImportFiles.Clear();
      ClAdj.Options.ManualModification = new ManualModify();
      Project.Options.ManualModification = new ManualModify();
    }

    private void 高程平差报表ToolStripMenuItem_Click(object sender, EventArgs e) {
      List<ExportHeight> exports = new List<ExportHeight>();
      int i = 0;
      ClAdj.AllPoints.ForEach(p => {
        ExportHeight height = new ExportHeight() {
          PointName = p.Number,
          Height = ClAdj.X[i, 0],
        };
        if (p.Number.StartsWith("z", true, null)) {
          height.IsExport = false;
        }
        exports.Add(height);
        i++;
      });

      exports.Sort((a, b) => b.PointName.CompareTo(a.PointName));

      ChooseExport export = new ChooseExport(exports);
      export.TransfEvent += Export_TransfEvent;
      export.ShowDialog();
    }

    private void Export_TransfEvent(List<ExportHeight> lists) {
      StringBuilder sb = new StringBuilder();
      sb.AppendLine($"{"序号",-8}{"点名",-10}{"高程/m",-11}");
      int i = 0;
      lists.ForEach(p => {
        if (p.IsExport) {
          i++;
          sb.AppendLine($"{i,-9}{p.PointName,-12}{p.Height,-11:#0.00000}");
        }
      });

      FileHelper.WriteStrToTxt(sb.ToString(), Project.Options.OutputFiles.ExportHeightPath);
      AddTabPage(Project.Options.OutputFiles.ExportHeightPath);
    }

    private void 高差平差报表ToolStripMenuItem_Click(object sender, EventArgs e) {
      List<ExportObserve> exports = new List<ExportObserve>();
      int i = 0;
      ClAdj.ObservedDatasNoRep.ForEach(p => {
        ExportObserve ob = new ExportObserve() {
          StartPoint = p.Start,
          EndPoint = p.End,
          Dis = p.Distance,
          HeightDiff = ClAdj.L[i]
        };
        if (p.Start.StartsWith("z", true, null) || p.End.StartsWith("z", true, null)) {
          ob.IsExport = false;
        }
        exports.Add(ob);
        i++;
      });

      exports.Sort((a, b) => b.StartPoint.CompareTo(a.StartPoint));

      ChooseExportOb export = new ChooseExportOb(exports);
      export.TransfEvent += Export_TransfEvent1;
      export.ShowDialog();
    }

    private void Export_TransfEvent1(List<ExportObserve> lists) {
      StringBuilder sb = new StringBuilder();
      sb.AppendLine($"{"序号",-5}{"起点",-8}{"终点",-8}{"距离/km",-9}{"高差/m",-9}");
      int i = 0;
      lists.ForEach(p => {
        if (p.IsExport) {
          i++;
          sb.AppendLine($"{i,-7}{p.StartPoint,-10}{p.EndPoint,-10}{p.Dis,-11:#0.0000}{p.HeightDiff,-15:#0.00000}");
        }
      });

      FileHelper.WriteStrToTxt(sb.ToString(), Project.Options.OutputFiles.ExportObPath);
      AddTabPage(Project.Options.OutputFiles.ExportObPath);
    }

    private void 高程多期对比ToolStripMenuItem_Click(object sender, EventArgs e) {
      OpenFileDialog openFile = new OpenFileDialog {
        Multiselect = true,
        Title = "选择高程平差成果文件",
        Filter = "txt file|*.txt",
        FilterIndex = 1,
      };
      if (openFile.ShowDialog() == DialogResult.OK) {
        List<List<ExportHeight>> lists = new List<List<ExportHeight>>();
        List<string> filenames = new List<string>();
        //读取文本文件为list
        foreach (var item in openFile.FileNames) {
          filenames.Add(Path.GetFileNameWithoutExtension(item));
          lists.Add(FileHelper.readHeight(item));
        }
        //求所有list的交集
        List<ExportHeight> result = lists[0];
        for (int i = 1; i < lists.Count; i++) {
          result = result.Where(a => lists[i].Any(b => b.PointName.ToLower() == a.PointName.ToLower())).ToList();
        }

        //读取交集的各期数据
        List<Comparison> comparisons = new List<Comparison>();
        int period = lists.Count;
        result.ForEach(t => {
          Dictionary<string, double> dic = new Dictionary<string, double>();
          for (int i = 0; i < period; i++) {
            dic.Add($"Height_{i + 1}", lists[i].Find(p => p.PointName.ToLower() == t.PointName.ToLower()).Height);
          }
          dynamic dynamicComparison = new Comparison() { PointName = t.PointName, PeriodData = dic };
          comparisons.Add(dynamicComparison);
        });
        MultiPeriodComparison multi = new MultiPeriodComparison() { Size = new Size((period + 2) * 120, 460) };
        multi.dataGridView1.Columns.Add(DataGridHelper.TextBoxAdd("点名", "PointName"));
        for (int i = 0; i < lists.Count; i++) {
          multi.dataGridView1.Columns.Add(DataGridHelper.TextBoxAdd(filenames[i], $"Height_{i + 1}"));
        }
        multi.dataGridView1.Columns.Add(DataGridHelper.LinkAdd("趋势图", "Check", "查看"));
        for (int i = 0; i < comparisons.Count; i++) {
          int idx = multi.dataGridView1.Rows.Add();
          multi.dataGridView1.Rows[idx].Cells[0].Value = comparisons[i].PointName;
          var data = comparisons[i].PeriodData;
          for (int j = 0; j < data.Count; j++) {
            data.TryGetValue($"Height_{j + 1}", out double value);
            multi.dataGridView1.Rows[idx].Cells[j + 1].Value = value;
          }
        }
        multi.Show();
      }
    }

    private void 打开文件ToolStripMenuItem_Click(object sender, EventArgs e) {
      string basePath = string.IsNullOrEmpty(Project?.Path) ? Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory) : Path.Combine(Project.Path, Project.Name, "ExportFiles");
      using (OpenFileDialog openFileDialog = new OpenFileDialog() { Filter = "All Files|*.*", InitialDirectory = basePath }) {
        if (openFileDialog.ShowDialog() == DialogResult.OK) {
          AddTabPage(openFileDialog.FileName);
        }
      }
    }
  }
}

