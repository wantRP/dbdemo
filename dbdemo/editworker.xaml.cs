using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace dbdemo {
	/// <summary>
	/// editworker.xaml 的交互逻辑
	/// </summary>
	public partial class editworker : Window {
		public editworker() {
			InitializeComponent();
			this.KeyDown += ModifyPrice_KeyDown;
			txtid.DataContext = worker;
			txtname.DataContext = worker;
		}
		public string sworkerid;
		public class wker {
			public string workerid { get; set; }
			public string workername { get; set; }
		}
		public wker worker = new wker();
		SqlConnection connection = new SqlConnection();
		SqlCommand cmd;
		SqlDataReader dataReader;
		private void ModifyPrice_KeyDown(object sender, KeyEventArgs e) {
			if (e.Key == Key.Escape)//Esc键  
			{
				this.Close();
			}
		}
		string getclass(){
			if (workertypebox.SelectedIndex == 0) return "casher";
			if (workertypebox.SelectedIndex == 1) return "storekeeper";
			return "superuser";
		}
		private void Button_Click(object sender, RoutedEventArgs e) {
			connection.Close();
			connection.ConnectionString = "Data Source=.\\SQLEXPRESS01;;Initial Catalog=test1;Integrated Security=SSPI;";
			connection.Open();
			cmd = new SqlCommand("update worker set name='"+txtname.Text+"', class='"+getclass()+"' where id=" + txtid.Text, connection);
			cmd.ExecuteNonQuery();
			connection.Close();
			this.DialogResult = true;
		}

		private void buttonreset_Click(object sender, RoutedEventArgs e) {
			if (MessageBox.Show("您确定要重置这名员工的密码吗\n将清空密码，不可撤消", "重置密码", MessageBoxButton.YesNo, MessageBoxImage.Information) == MessageBoxResult.Yes) {
				connection.Close();
				connection.ConnectionString = "Data Source=.\\SQLEXPRESS01;;Initial Catalog=test1;Integrated Security=SSPI;";
				connection.Open();
				cmd = new SqlCommand("update worker set hashedpassword=371857150 where id=" + txtid.Text, connection);
				cmd.ExecuteNonQuery();
				connection.Close();
				MessageBox.Show("已重置");
			}
		}

		private void buttoncancel_Click(object sender, RoutedEventArgs e) {
			this.DialogResult = false;
			this.Close();
		}
	}
}
