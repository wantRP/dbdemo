using System.Data.SqlClient;
using System.Windows;
using System.Text.RegularExpressions;
using System.Windows.Input;
using System.Collections.Generic;

namespace dbdemo {
	public partial class Window3 : Window {
		public Window3() {
			InitializeComponent();
		}
		SqlConnection connection = new SqlConnection();
		SqlCommand cmd;
		SqlDataReader dataReader;
		public void limitnumber(object sender, TextCompositionEventArgs e) {
			Regex re = new Regex("[^0-9]+");
			e.Handled = re.IsMatch(e.Text);
		}
		private void ok_Click(object sender, RoutedEventArgs e) {
			int flag = 0;
			if (txt_userid.Text == "" || txt_Pwd.Password == "" || txt_name.Text == "")
				MessageBox.Show("存在空值");
			else {
				connection.Close();
				connection.ConnectionString = "Data Source=.\\SQLEXPRESS01;;Initial Catalog=test1;Integrated Security=SSPI;";
				connection.Open();
				cmd = new SqlCommand("select * from worker where id=" + txt_userid.Text, connection);
				dataReader = cmd.ExecuteReader();
				if (dataReader.HasRows) MessageBox.Show("工号重复");
				else if (txt_Pwd.Password != txt_confirm.Password) {
					notequalhint.Visibility = Visibility.Visible;
					notequalhint.Text = "密码不一致";
				} else if (txt_Pwd.Password.Length < 4) {
					notequalhint.Visibility = Visibility.Visible;
					notequalhint.Text = "请检查密码长度";
				} else {
					connection.Close();
					connection.Open();
					int ps = txt_Pwd.Password.GetHashCode();
					string n = txt_userid.Text;
					string name = txt_name.Text;
					int tpn = workertypebox.SelectedIndex;
					Dictionary<int, string> dict = new Dictionary<int, string>();
					dict.Add(0, "worker");
					dict.Add(1, "storekeeper");
					dict.Add(2, "superuser");
					try {
						cmd = new SqlCommand("insert into worker values(" + n + ",\'" + name + "\'," + ps +",\'"+dict[tpn]+"\')", connection);
						flag = cmd.ExecuteNonQuery();
					} catch {
						connection.Close();
						MessageBox.Show("err");
					}
					notequalhint.Visibility = Visibility.Hidden;
					if (flag > 0) {
					
						MessageBox.Show("success");
						this.DialogResult = true;
						Close();
					} else MessageBox.Show("未添加");
				}
			}
		}
		private void cancel_Click(object sender, RoutedEventArgs e) {
			this.DialogResult = false;
			Close();
		}
	}
}
