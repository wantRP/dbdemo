using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
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
	/// editpassword.xaml 的交互逻辑
	/// </summary>
	public partial class editpassword : Window {
		public editpassword() {
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
			connection.Close();
			connection.ConnectionString = "Data Source=.\\SQLEXPRESS01;;Initial Catalog=test1;Integrated Security=SSPI;";
			connection.Open();
			string uid = txt_userid.Text;
			string newhashedpassword = txt_Pwd.GetHashCode().ToString();
			//MessageBox.Show("select * from worker where id=" + uid + " and hashedpassword=" + txt_oldPwd.GetHashCode().ToString());
			cmd = new SqlCommand("select * from worker where id=" + uid + " and hashedpassword=" + txt_oldPwd.Password.GetHashCode().ToString(),connection);
			dataReader = cmd.ExecuteReader();
			if (!dataReader.HasRows) MessageBox.Show("用户名或密码错误");else{
				connection.Close();
				connection.Open();
				cmd = new SqlCommand("update worker set hashedpassword=" + newhashedpassword + " where id=" + uid, connection);
				cmd.ExecuteNonQuery();
				connection.Close();
				MessageBox.Show("修改成功！");
				this.Close();
			}
		}
		void setequalsstat(){
			if (txt_Pwd.Password != txt_confirm.Password) 
				notequalhint.Visibility = Visibility.Visible;
			 else 
				notequalhint.Visibility = Visibility.Hidden;
			setokbuttonability();
		}
		void setokbuttonability(){
			ok.IsEnabled = false;
			if (txt_userid.Text == "") return;
			//if (txt_oldPwd.Password == "") return;
			if (txt_Pwd.Password.Length < 4) return;
			if (txt_Pwd.Password != txt_confirm.Password) return;
			ok.IsEnabled = true;
		}
		private void txt_userid_TextChanged(object sender, TextChangedEventArgs e) {
			setokbuttonability();
		}
		private void txt_Pwd_PasswordChanged(object sender, RoutedEventArgs e) {
			setequalsstat();
			setokbuttonability();
			if (txt_Pwd.Password.Length < 4) 
				hintlength.Foreground = new SolidColorBrush(Colors.Red);
			 else 
				hintlength.Foreground = new SolidColorBrush(Colors.Black);
			
		}
		private void txt_confirm_PasswordChanged(object sender, RoutedEventArgs e) {
			setequalsstat();
			setokbuttonability();

		}
		private void cancel_Click(object sender, RoutedEventArgs e) {
			this.Close();
		}

		
	}
}
