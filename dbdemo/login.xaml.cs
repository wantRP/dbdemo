using System;
using System.Data.SqlClient;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Input;
using WpfApp1;

namespace dbdemo{
	public partial class Window1 : Window{
		SqlConnection connection = new SqlConnection();
		SqlCommand cmd;
		SqlDataReader dataReader;
		bool opened = false;
		public Window1(){
			InitializeComponent();
		}
		private void btn_exit_Click(object sender, RoutedEventArgs e){
			this.Close();
			MainWindow mainWindow = new MainWindow();
			mainWindow.Show();
			Environment.Exit(0);
		}
		public void limitnumber(object sender, TextCompositionEventArgs e){
			Regex re = new Regex("[^0-9]+");
			e.Handled = re.IsMatch(e.Text);
		}
		private void btn_login_Click(object sender, RoutedEventArgs e){
			string userclass;
			string n;
			int hp;
			n = txt_userName.Text;
			if (n == "")
				n = "NULL";
			hp = txt_Pwd.Password.GetHashCode(); //ok
			if (!opened){
				connection.ConnectionString = "Data Source=.\\SQLEXPRESS01;;Initial Catalog=test1;Integrated Security=SSPI;";
				connection.Open();
				opened = true;
			}
			cmd = new SqlCommand("select * from worker where id=" + n + " and hashedpassword=" + hp.ToString(), connection);
			dataReader = cmd.ExecuteReader();
			if (dataReader.HasRows){
				dataReader.Read();
				userclass = dataReader["class"].ToString().Replace(" ", "");
				dataReader.Close();
				App.workerid = n;		
				if (userclass == "superuser"){
					MainWindow mainWindow = new MainWindow();
					mainWindow.wid.workerid = txt_userName.Text;
					this.Close();
					mainWindow.Show();
				}else if (userclass == "storekeeper"){
					storemanage mng = new storemanage();
					this.Close();
					mng.Show();
				}else{
					cash csh = new cash();
					csh.wid.workerid = txt_userName.Text;
					this.Close();
					csh.Show();
				}
			}
			dataReader.Close();
			wrongmsg.Visibility = Visibility.Visible;
		}

		private void linkDmsite_Click(object sender, RoutedEventArgs e) {
			editpassword edt = new editpassword();
			edt.Show();
		}
	}
}
