using LevelnetAdjustment.model;
using LevelnetAdjustment.utils;
using SplashScreenDemo;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace LevelnetAdjustment.form {

    public partial class FrmModifyPointName : Form {
        public delegate void ModifyPointaName();
        public List<RawData> Rds { get; set; }
        public List<PointData> Knownp { get; set; }
        public ArrayList StationNums { get; set; }
        public string FilePath { get; set; }

        private List<ArrayList> Lists { get; set; }
        public event ModifyPointaName TransEvent;
        public FrmModifyPointName(List<RawData> rds, List<PointData> knownp, string file, ArrayList list) {
            InitializeComponent();
            Rds = rds;
            StationNums = list;
            Knownp = knownp;
            FilePath = file;
        }

        private void FrmModifyPointName_Load(object sender, EventArgs e) {
            SimpleLoading loadingfrm = new SimpleLoading(this, "计算中，请稍等...");
            //将Loaing窗口，注入到 SplashScreenManager 来管理
            GF2Koder.SplashScreenManager loading = new GF2Koder.SplashScreenManager(loadingfrm);
            loading.ShowLoading();
            int stationNum = 0;
            ArrayList array = new ArrayList();
            Lists = new List<ArrayList>();
            foreach (var item in Rds) {
                if (item.IsStart) {
                    stationNum++;
                }
                array.Add(item.BackPoint);
                if (item.IsEnd) {
                    array.Add(item.FrontPoint);
                    Lists.Add(Commom.Clone(array));
                    array.Clear();
                }
            }
            //Console.WriteLine("123");
            panel1.AutoScroll = true;
            for (int i = 0; i < Lists.Count; i++) {
                int txtY = 10 + 40 * i;
                int lblY = 15 + 40 * i;
                int idx = -1;
                for (int j = 0; j < Lists[i].Count; j++) {
                    idx++;
                    int txtX = 10 + 100 * j;
                    int lblX = 90 + 100 * j;
                    TextBox text = new TextBox {
                        Name = $"txt-{i}-{j}",
                        Text = Lists[i][j].ToString(),
                        Location = new Point(txtX, txtY),
                        Size = new Size(75, 21),
                    };
                    panel1.Controls.Add(text);

                    if (idx != Lists[i].Count - 1) {
                        Label lbl = new Label {
                            Name = $"lbl-{i}-{j}",
                            Text = "→",
                            Location = new Point(lblX, lblY),
                            Size = new Size(17, 12),
                        };
                        panel1.Controls.Add(lbl);
                    }
                }
            }
            loading.CloseWaitForm();

        }

        private void button1_Click(object sender, EventArgs e) {
            foreach (var item in panel1.Controls) {
                if (item is TextBox) {
                    TextBox t = (TextBox)item;
                    string[] str = t.Name.Split('-');
                    int i = int.Parse(str[1]);
                    int j = int.Parse(str[2]);
                    Lists[i][j] = t.Text;
                }
            }

            List<ObservedData> ods = new List<ObservedData>();
            List<ObservedData> ods_mid = new List<ObservedData>();

            int stationNum = -1;
            int num = 0;
            foreach (var item in Rds) {
                if (item.MidDis != 0) {
                    ods_mid.Add(new ObservedData { Start = item.BackPoint, End = item.MidPoint, Distance = item.MidDis, HeightDiff = item.MidDiff, StationNum = 1 });
                }
                if (item.IsStart) {
                    stationNum++;
                    num = 0;
                }
                num++;
                ods.Add(new ObservedData {
                    Start = Lists[stationNum][num - 1].ToString(),
                    End = Lists[stationNum][num].ToString(),
                    Distance = item.DisAve,
                    HeightDiff = item.DiffAve,
                    StationNum = 1
                });
            }

            FileStream fileStream = new FileStream(FilePath, FileMode.Create);
            StreamWriter streamWriter = new StreamWriter(fileStream, Encoding.Default);
            if (Knownp != null && Knownp.Count > 0) {
                Knownp.ForEach(l => {
                    streamWriter.WriteLine($"{l.Number},{l.Height}");
                });
            }
            Commom.Merge(ods, ods_mid).ForEach(l => {
                streamWriter.WriteLine($"{l.Start},{l.End},{Math.Round(l.HeightDiff, 5)},{Math.Round(l.Distance, 5)}");
            });
            streamWriter.Flush();
            streamWriter.Close();
            fileStream.Close();

            Close();
            TransEvent();
        }

        private void button2_Click(object sender, EventArgs e) {
            Close();
        }
    }
}
