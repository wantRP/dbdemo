using dbdemo;
using System.Collections.ObjectModel;
using System.Data.SqlClient;
using System.Windows;
using System.Windows.Controls;

namespace WpfApp1{
	public partial class MainWindow : Window{
		SqlConnection connection = new SqlConnection();
		SqlCommand cmd;
		SqlDataReader dataReader;
		public MainWindow(){
			InitializeComponent();
			wid.workerid = sworkerid;
			textblock_workerid.DataContext = wid;
		}
		public class workerinfo{
			public string name { get; set; }
			public string id { get; set; }
			public string job{ get; set; }
		}
		public class stocksinfo{
			public string goodsid { get; set; }
			public string goodsname { get; set; }
			public string goodsunit { get; set; }
			public string goodsquantity { get; set; }
			public string goodsprice { get; set; }

		}
		public class wker {
			public string workerid { get; set; }
		}
		public wker wid = new wker();
		public string sworkerid;
		ObservableCollection<workerinfo> workers = new ObservableCollection<workerinfo>();
		ObservableCollection<stocksinfo> stocks = new ObservableCollection<stocksinfo>();
		private void Window_Loaded(object sender, RoutedEventArgs e){
			
		}
		string getjob(string s){
			s = s.Replace(" ", "");
			if (s == "casher") return ("收银员");
			if (s == "superuser") return ("HR");
			return ("库管");
		}
		public void getworkerlist(){
			workers.Clear();
			connection.ConnectionString = "Data Source=.\\SQLEXPRESS01;Initial Catalog=test1;Integrated Security=SSPI;";
			connection.Open();
			cmd = new SqlCommand("select * from worker ", connection);
			dataReader = cmd.ExecuteReader();
			if (dataReader.HasRows)
				while (dataReader.Read()){
					workers.Add(new workerinfo() {
						name = dataReader["name"].ToString(),
						id = dataReader["id"].ToString(),
						job = getjob(dataReader["class"].ToString())
					});
				}
			connection.Close();
			workergrid.DataContext = workers;
			deleteworker.IsEnabled = true;
		}
		private void getworker_Click(object sender, RoutedEventArgs e){
			getworkerlist();
		}
		private void addworker_Click(object sender, RoutedEventArgs e){
			dbdemo.Window3 aw = new dbdemo.Window3();
			aw.ShowDialog();
			getworkerlist();
		}
		private void Button_Click(object sender, RoutedEventArgs e){		
			MessageBox.Show(workers[workergrid.SelectedIndex].name);
		}
		private void deleteworker_Click(object sender, RoutedEventArgs e){
			if (workergrid.SelectedIndex != -1)
				if (System.Windows.MessageBox.Show("您确定要删除吗？", "提示：", MessageBoxButton.OKCancel) == MessageBoxResult.OK){
					connection.ConnectionString = "Data Source=.\\SQLEXPRESS01;;Initial Catalog=test1;Integrated Security=SSPI;";
					connection.Open();
					cmd = new SqlCommand("delete from worker where id=" + (workergrid.Columns[0].GetCellContent(workergrid.Items[workergrid.SelectedIndex]) as TextBlock).Text, connection);
					cmd.ExecuteNonQuery();
					connection.Close();
					getworkerlist();
				}
		}
		private void onshow(object sender, RoutedEventArgs e){
			stocks.Clear();
			connection.ConnectionString = "Data Source=.\\SQLEXPRESS01;Initial Catalog=test1;Integrated Security=SSPI;";
			connection.Open();
			cmd = new SqlCommand("select * from stocks,goods where sno=gno", connection);
			dataReader = cmd.ExecuteReader();
			while (dataReader.Read())
				stocks.Add(new stocksinfo(){
					goodsname = dataReader["gname"].ToString(),
					goodsid = dataReader["gno"].ToString(),
					goodsunit = dataReader["unit"].ToString(),
					goodsquantity = dataReader["quantity"].ToString(),
					goodsprice = dataReader["price"].ToString()
				});
			connection.Close();
			stocksgrid.DataContext = stocks;
		}

		private void workergrid_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e) {
			//MessageBox.Show(workergrid.SelectedIndex.ToString());
			editworker edwker = new editworker();
			edwker.worker.workerid = workers[workergrid.SelectedIndex].id;
			edwker.worker.workername = workers[workergrid.SelectedIndex].name;
			if (edwker.ShowDialog() == true) getworkerlist();
		}
	}
}