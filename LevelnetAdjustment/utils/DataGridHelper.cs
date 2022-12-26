using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace LevelnetAdjustment.utils {
  public class DataGridHelper {
    /// <summary>
    /// 添加列
    /// </summary>
    /// <param name="name"></param>
    /// <returns></returns>
    public static DataGridViewTextBoxColumn TextBoxAdd(string headerText, string name) {
      return new DataGridViewTextBoxColumn() {
        DataPropertyName = name,
        Name = name,
        HeaderText = headerText,
      };
    }

    public static DataGridViewCheckBoxColumn CheckBoxAdd(string headerText, string name) {
      return new DataGridViewCheckBoxColumn() {
        DataPropertyName = name,
        Name = name,
        HeaderText = headerText,
      };
    }
    public static DataGridViewLinkColumn LinkAdd(string headerText, string name,string linkText) {
      return new DataGridViewLinkColumn() {
        Name = name,
        HeaderText = headerText,
        DefaultCellStyle = new DataGridViewCellStyle() {
          NullValue = linkText
        }
      };
    }
  }
}
