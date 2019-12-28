using System.Data.SqlClient;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Input;
namespace dbdemo {
	public partial class newstorege : Window {
		public void limitnumber(object sender, TextCompositionEventArgs e) {
			Regex re = new Regex("[^0-9]+");
			e.Handled = re.IsMatch(e.Text);
		}
		public newstorege() {
			InitializeComponent();
		}
		SqlConnection connection = new SqlConnection();
		SqlCommand cmd;
		SqlDataReader dataReader;
		private void cancel(object sender, RoutedEventArgs e) {
			this.DialogResult = false;
		}
		private void ok(object sender, RoutedEventArgs e) {
			if (txtno.Text == "") {
				MessageBox.Show("blank");
				return;
			}
			connection.Close();
			connection.ConnectionString = "Data Source=.\\SQLEXPRESS01;;Initial Catalog=test1;Integrated Security=SSPI;";
			connection.Open();
			cmd = new SqlCommand("select * from stocks where sno=" + txtno.Text, connection);
			dataReader = cmd.ExecuteReader();
			if (dataReader.HasRows)
				MessageBox.Show("货号重复");
			else {
				connection.Close();
				connection.Open();
				cmd = new SqlCommand("select * from goods where gno=" + txtno.Text, connection);
				dataReader = cmd.ExecuteReader();
				if (!dataReader.HasRows)
					MessageBox.Show("无信息");
				else {
					string sno = txtno.Text;
					string snum = txtnum.Text;
					int flag = 0;
					try {
						connection.Close();
						connection.Open();
						cmd = new SqlCommand("insert into stocks values(" + sno + "," + snum + ")", connection);
						flag = cmd.ExecuteNonQuery();
					} catch {
						connection.Close();
						MessageBox.Show("err");
					}
					if (flag > 0) {
						MessageBox.Show("success");
						this.DialogResult = true;
						Close();
					} else
						MessageBox.Show("未添加");
				}
			}
		}
		private void txtno_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e) {
			txtno.Text = txtno.Text.Replace(" ", "");
		}
	}
}
