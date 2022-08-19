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

        private void FileView_Load(object sender, EventArgs e) {
            rtb.Clear();
            rtb.LoadFile(FilePath, RichTextBoxStreamType.PlainText);
            rtb.Show();
            this.Text = Path.GetFileName(FilePath);
        }

        /// <summary>
        /// 保存文本文件
        /// </summary>
        private void SaveToFile() {
            //saveFileDialog.InitialDirectory = pname;//设置保存的默认目录
            saveFileDialog.FileName = Path.GetFileNameWithoutExtension(FilePath);
            saveFileDialog.Filter = "观测文件(*.INP)|*.INP|结果文件(*.OUP)|*.OUP|所有文件(*.*)|*.*";
            saveFileDialog.FilterIndex = 1;//默认显示保存类型为TXT
            saveFileDialog.RestoreDirectory = true;
            rtb.SaveFile(saveFileDialog.FileName, RichTextBoxStreamType.PlainText);
        }

        /// <summary>
        /// 保存快捷键事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void rtb_KeyDown(object sender, KeyEventArgs e) {
            if (e.Control && e.KeyCode == Keys.S) {
                SaveToFile();
            }
        }
    }
}
