using System.Collections.ObjectModel;
using System.Data.SqlClient;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
namespace dbdemo {
	public partial class storemanage : Window {
		SqlConnection connection = new SqlConnection();
		SqlCommand cmd;
		SqlDataReader dataReader;
		public storemanage() {
			InitializeComponent();
			getgoodsinfolist();
		}
		public void limitnumber(object sender, TextCompositionEventArgs e) {
			Regex re = new Regex("[^0-9]+");
			e.Handled = re.IsMatch(e.Text);
		}
		public class goodsinfo {
			public string stdid { get; set; }
			public string gno { get; set; }
			public string gunit { get; set; }
			public string gprice { get; set; }
			public string gname { get; set; }
		}
		public class stocksinfo {
			public string sno { get; set; }
			public string quantity { get; set; }
			public string sstdid { get; set; }
			public string sname { get; set; }
		}
		ObservableCollection<goodsinfo> goods = new ObservableCollection<goodsinfo>();
		ObservableCollection<stocksinfo> stocks = new ObservableCollection<stocksinfo>();
		public void getgoodsinfolist() {
			goods.Clear();
			connection.Close();
			connection.ConnectionString = "Data Source=.\\SQLEXPRESS01;Initial Catalog=test1;Integrated Security=SSPI;";
			connection.Open();
			cmd = new SqlCommand("select * from goods", connection);
			dataReader = cmd.ExecuteReader();
			while (dataReader.Read())
				goods.Add(
				new goodsinfo() {
					stdid = dataReader["standardid"].ToString(),
					gno = dataReader["gno"].ToString(),
					gunit = dataReader["unit"].ToString(),
					gname = dataReader["gname"].ToString(),
					gprice = dataReader["price"].ToString()
				});
			connection.Close();
			goodsgrid.DataContext = goods;
		}
		private void goodsgotfocus(object sender, RoutedEventArgs e) {
			getgoodsinfolist();
		}
		private void addinfo_Click(object sender, RoutedEventArgs e) {
			getgoodsinfolist();
		}
		private void addstock_Click(object sender, RoutedEventArgs e) {
			if (stocksgrid.SelectedIndex != -1) {
				int n = int.Parse(numchanged.Text);
				connection.Close();
				connection.ConnectionString = "Data Source=.\\SQLEXPRESS01;Initial Catalog=test1;Integrated Security=SSPI;";
				connection.Open();
				cmd = new SqlCommand("update stocks set quantity=quantity+" + n + " where sno=" + stocks[stocksgrid.SelectedIndex].sno, connection);
				dataReader = cmd.ExecuteReader();
				//stocks[stocksgrid.SelectedIndex].quantity = (int.Parse(stocks[stocksgrid.SelectedIndex].sno) + n).ToString();
				txtsearch.Text = txtsearch.Text + " ";
				txtsearch.Text = txtsearch.Text.Substring(0, txtsearch.Text.Length - 1);
			}
		}
		private void decstock_Click(object sender, RoutedEventArgs e) {
			if (stocksgrid.SelectedIndex != -1) {
				int n = int.Parse(numchanged.Text);
				connection.Close();
				connection.ConnectionString = "Data Source=.\\SQLEXPRESS01;Initial Catalog=test1;Integrated Security=SSPI;";
				connection.Open();
				cmd = new SqlCommand("update stocks set quantity=quantity-" + n + " where sno=" + stocks[stocksgrid.SelectedIndex].sno, connection);
				Microsoft.VisualBasic.Interaction.InputBox("原因", "修改理由", "过期", 0, 0);
				try { dataReader = cmd.ExecuteReader(); } catch { MessageBox.Show("未保存，你减少的量超过了库存量。","信息",MessageBoxButton.OK,MessageBoxImage.Exclamation); }
				txtsearch.Text = txtsearch.Text + " "; txtsearch.Text = txtsearch.Text.Substring(0, txtsearch.Text.Length - 1);
				
				//stocks[stocksgrid.SelectedIndex].quantity = (int.Parse(stocks[stocksgrid.SelectedIndex].sno) - n).ToString();
			}
		}
		private void save_Click(object sender, RoutedEventArgs e) {
			//MessageBox.Show(goods[goodsgrid.SelectedIndex].gname);
			connection.ConnectionString = "Data Source=.\\SQLEXPRESS01;;Initial Catalog=test1;Integrated Security=SSPI;";

			//MessageBox.Show("rows:" + goods.Count());
			for (int i = 0; i < goods.Count(); ++i) {
				try {
					connection.Open();
					cmd = new SqlCommand("select * from goods where gno=" + goods[i].gno, connection);
					dataReader = cmd.ExecuteReader();
					if (dataReader.Read()) {
						connection.Close();
						connection.Open();
					//MessageBox.Show("!" + i.ToString() + "!" + goods[i].gname);
					//MessageBox.Show("update goods set standardid=" + goods[i].stdid + ",gno=" + goods[i].gno + ",unit=\'" + goods[i].gunit + "\',price=" + goods[i].gprice + ",gname=\'" + goods[i].gname + "\' where gno=" + goods[i].gno);
						cmd = new SqlCommand("update goods set standardid=" + goods[i].stdid + ",gno=" + goods[i].gno + ",unit=\'" + goods[i].gunit + "\',price=" + goods[i].gprice + ",gname=\'" + goods[i].gname + "\' where gno=" + goods[i].gno, connection);
						cmd.ExecuteNonQuery();
					} else {
						connection.Close();
						connection.Open();
						//MessageBox.Show("insert into goods values(" + goods[i].stdid + "," + goods[i].gno + ",\'" + goods[i].gunit + "\'," + goods[i].gprice + ",\'" + goods[i].gname + "\') ");
						cmd = new SqlCommand("insert into goods values(" + goods[i].stdid + "," + goods[i].gno + ",\'" + goods[i].gunit + "\'," + goods[i].gprice + ",\'" + goods[i].gname + "\') ", connection);
						cmd.ExecuteNonQuery();
					}
				} catch {
					MessageBox.Show("未完全保存，请检查格式或是否修改了已有库存记录的商品的货号。");

				}
				
				connection.Close();
			}
			getgoodsinfolist();
		}
		private void refresh_Click(object sender, RoutedEventArgs e) {
			getgoodsinfolist();
		}
		private void Button_Click(object sender, RoutedEventArgs e) {
			if (goodsgrid.SelectedIndex != -1)
				if (System.Windows.MessageBox.Show("您确定要删除吗？", "提示：", MessageBoxButton.OKCancel) == MessageBoxResult.OK) {
					connection.ConnectionString = "Data Source=.\\SQLEXPRESS01;;Initial Catalog=test1;Integrated Security=SSPI;";
					try {
						connection.Close();
						connection.Open();
						cmd = new SqlCommand("delete from goods where gno=" + goods[goodsgrid.SelectedIndex].gno, connection);
						cmd.ExecuteNonQuery();
						connection.Close();
					} catch {
						MessageBox.Show("删除失败，存在库存记录");
					}
					getgoodsinfolist();
				}
		}
		private void txtsearch_TextChanged(object sender, TextChangedEventArgs e) {
			stocks.Clear();
			connection.Close();
			connection.ConnectionString = "Data Source=.\\SQLEXPRESS01;;Initial Catalog=test1;Integrated Security=SSPI;";
			try {
				connection.Open();
				string comm = "select * from stocks,goods where sno=gno and ";
				if (bygoodid.IsChecked == true) comm += "gno ";
				if (bystdid.IsChecked == true) comm += "standardid ";
				if (byname.IsChecked == true) comm += "gname ";
				string commtail = comm + "like \'%" + txtsearch.Text + "%\'";
				//testtxt.Text = commtail;
				cmd = new SqlCommand(commtail, connection);
				dataReader = cmd.ExecuteReader();
				while (dataReader.Read())
					stocks.Add(
					new stocksinfo() {
						sstdid = dataReader["standardid"].ToString(),
						sno = dataReader["gno"].ToString(),
						sname = dataReader["gname"].ToString(),
						quantity = dataReader["quantity"].ToString()
					});
				connection.Close();
				stocksgrid.DataContext = stocks;
			} catch {
			}
		}
		private void bygoodid_Checked(object sender, RoutedEventArgs e) {
			txtsearch.Text = "";
		}
		private void bystdid_Checked(object sender, RoutedEventArgs e) {
			txtsearch.Text = "";
		}
		private void byname_Checked(object sender, RoutedEventArgs e) {
			txtsearch.Text = "";
		}
		private void addnewstore_Click(object sender, RoutedEventArgs e) {
			newstorege n = new newstorege();
			n.ShowDialog();
			txtsearch.Text = txtsearch.Text + " ";
			txtsearch.Text = txtsearch.Text.Substring(0, txtsearch.Text.Length - 1);
		}

		private void Button_Click_1(object sender, RoutedEventArgs e) {
			MessageBox.Show(goods[2].gname);
		}

		private void onload(object sender, RoutedEventArgs e) {
			stocks.Clear();
			connection.Close();
			connection.ConnectionString = "Data Source=.\\SQLEXPRESS01;;Initial Catalog=test1;Integrated Security=SSPI;";
			try {
				connection.Open();
				string comm = "select * from stocks,goods where sno=gno";
				cmd = new SqlCommand(comm, connection);
				dataReader = cmd.ExecuteReader();
				while (dataReader.Read())
					stocks.Add(
					new stocksinfo() {
						sstdid = dataReader["standardid"].ToString(),
						sno = dataReader["gno"].ToString(),
						sname = dataReader["gname"].ToString(),
						quantity = dataReader["quantity"].ToString()
					});
				connection.Close();
				stocksgrid.DataContext = stocks;
			} catch {
			}
		}
	}

}
