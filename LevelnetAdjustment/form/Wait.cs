﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace LevelnetAdjustment.form {
    public partial class Wait : Form {
        public int Num { get; set; } // 数量
        public string Title { get; set; } //标题

        public Wait() {
            InitializeComponent();
        }
        public void RefreshForm()// 父窗口 定义 委托具体逻辑
        {
            //this.label1.Text = "我是刷新后的label文本";
        }
    }
}