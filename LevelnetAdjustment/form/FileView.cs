using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;

namespace LevelnetAdjustment.form {

    public partial class FileView : Form {
        private bool kd = false;
        public FileView(string _filePath) {
            InitializeComponent();
            this.FilePath = _filePath;
            this.ShowIcon = false;
        }
        public string FilePath { get; set; }
        public string SaveFilePath { get; set; }
        public bool isModified { get; set; } = false;

        private void FileView_Load(object sender, EventArgs e) {
            rtb.Clear();

            if (string.IsNullOrEmpty(FilePath)) {
                this.Text = "Undefined.ou1*";
                return;
            }
            else {
                rtb.LoadFile(FilePath, RichTextBoxStreamType.PlainText);
                rtb.Show();
                this.Text = Path.GetFileName(FilePath);
            }

        }


        /// <summary>
        /// 保存快捷键事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void rtb_KeyDown(object sender, KeyEventArgs e) {
            kd = true;
            if (e.Control && e.KeyCode == Keys.S) {
                SaveFile();
                this.Text = Path.GetFileName(SaveFilePath);
            }
        }

        /// <summary>
        /// 保存文本文件
        /// </summary>
        private void SaveFile() {
            SaveFileDialog saveFileDialog = new SaveFileDialog {
                Title = "另存为",
                Filter = "观测文件(*.in1)|*.in1|所有文件(*.*)|*.*",
                FilterIndex = 1,
                RestoreDirectory = true,
                FileName = Path.GetFileName(SaveFilePath),
            };
            if (saveFileDialog.ShowDialog() == DialogResult.OK) {
                SaveFilePath = saveFileDialog.FileName;
                rtb.SaveFile(saveFileDialog.FileName, RichTextBoxStreamType.PlainText);
                isModified = false;
                this.Close();
            }
        }

        private void rtb_TextChanged(object sender, EventArgs e) {
            if (kd) {
                isModified = true;
            }
        }

        /// <summary>
        /// 未保存事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void FileView_FormClosing(object sender, FormClosingEventArgs e) {
            if (isModified) {
                var res = MessageBox.Show("文件尚未保存，是否保存？", "提示", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Information);
                if (res == DialogResult.Yes) {
                    SaveFileDialog saveFileDialog = new SaveFileDialog {
                        Title = "另存为",
                        Filter = "观测文件(*.in1)|*.in1|所有文件(*.*)|*.*",
                        FilterIndex = 1,
                        RestoreDirectory = true,
                        FileName = Path.GetFileName(SaveFilePath),
                    };
                    if (saveFileDialog.ShowDialog() == DialogResult.OK) {
                        SaveFilePath = saveFileDialog.FileName;
                        rtb.SaveFile(saveFileDialog.FileName, RichTextBoxStreamType.PlainText);
                        isModified = false;
                    }
                }
                else if (res == DialogResult.Cancel) {
                    e.Cancel = true;
                }
            }
        }
    }
}
