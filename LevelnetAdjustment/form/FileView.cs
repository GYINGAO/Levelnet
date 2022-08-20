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
        public FileView(string _filePath) {
            InitializeComponent();
            this.FilePath = _filePath;
        }
        public string FilePath { get; set; }
        public string SaveFilePath { get; set; }

        private void FileView_Load(object sender, EventArgs e) {
            rtb.Clear();
            if (string.IsNullOrEmpty(FilePath)) {
                this.Text = "Undefined.OUP*";
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
            if (e.Control && e.KeyCode == Keys.S) {
                SaveFile();
                this.Text = Path.GetFileName(SaveFilePath);
                this.Close();
            }
        }

        /// <summary>
        /// 保存文本文件
        /// </summary>
        private void SaveFile() {
            SaveFileDialog saveFileDialog = new SaveFileDialog {
                Title = "另存为",
                Filter = "观测文件(*.INP)|*.INP|所有文件(*.*)|*.*",
                FilterIndex = 1,
                RestoreDirectory = true
            };
            if (saveFileDialog.ShowDialog() == DialogResult.OK) {
                SaveFilePath = saveFileDialog.FileName;
                rtb.SaveFile(saveFileDialog.FileName, RichTextBoxStreamType.PlainText);
            }
        }
    }
}
