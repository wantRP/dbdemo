using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.SqlClient;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Input;

namespace dbdemo {
	public partial class cash : Window {
		private void Window_Loaded(object sender, RoutedEventArgs e) {
			
		}
		public void limitnumber(object sender, TextCompositionEventArgs e) {
			Regex re = new Regex("[^0-9]+");
			e.Handled = re.IsMatch(e.Text);
		}
		SqlConnection connection = new SqlConnection();
		SqlCommand cmd;
		SqlDataReader dataReader;
		public string sworkerid;
		public class wker {
			public string workerid { get; set; }
		}
		public wker wid = new wker();
		
		public class Singleorder {
			public string goodsid { get; set; }
			public string goodsname { get; set; }
			public string goodsunit { get; set; }
			public string goodsprice { get; set; }
			public string goodsquantity { get; set; }
			public string goodstotalprice { get; set; }
		}
		List<Singleorder> order = new List<Singleorder>();
		public cash() {
			InitializeComponent();
			txtworkerid.DataContext = wid;
			//txtworkerid.Text = workerid;
		}
		
		private void submit_Click(object sender, RoutedEventArgs e) {
		if(int.Parse(txttotal.Text)==0){
				MessageBox.Show("没有任何项结算");
				return;
		}
			connection.Close();
			connection.ConnectionString = "Data Source=.\\SQLEXPRESS01;;Initial Catalog=test1;Integrated Security=SSPI;";
			connection.Open();
			cmd = new SqlCommand("select orderscount from currentstats", connection);
			dataReader = cmd.ExecuteReader();
			dataReader.Read();
			string currentoid = dataReader["orderscount"].ToString();
			connection.Close();
			int id = int.Parse(currentoid);
			++id;
			string newid = id.ToString();
			connection.ConnectionString = "Data Source=.\\SQLEXPRESS01;;Initial Catalog=test1;Integrated Security=SSPI;";
			connection.Open();
			//订单号
			cmd = new SqlCommand("insert into orders(oid,total) values(" + newid + "," + txttotal.Text + ")", connection);
			cmd.ExecuteNonQuery();
			connection.Close();
			for (int i = 0; i < order.Count(); ++i) {
				connection.Open();
				cmd = new SqlCommand("update stocks set quantity=quantity-" + order[i].goodsquantity + " where sno=" + order[i].goodsid, connection);
				cmd.ExecuteNonQuery();
				connection.Close();
				connection.Open();
				cmd = new SqlCommand("insert into orderdetail values(" + newid + "," + order[i].goodsid + ",\'" + order[i].goodsname + "\'," + order[i].goodsquantity + "," + order[i].goodsprice + "," + order[i].goodstotalprice + ")", connection);
				cmd.ExecuteNonQuery();
				connection.Close();
			}
			connection.Close();
			connection.Open();
			cmd = new SqlCommand("update currentstats set orderscount=" + newid, connection);
			cmd.ExecuteNonQuery();
			connection.Close();
			MessageBox.Show("总价" + txttotal.Text + "订单已结算");
			order.Clear();
			txtgoodno.Text = "";
			txtnum.Text = "";
			refreshlist();
		}

		private void additems_Click(object sender, RoutedEventArgs e) {
			if (txtgoodno.Text != "" && txtnum.Text != "") {
				if (int.Parse(txtnum.Text) < 1) {
					MessageBox.Show("数量不能为零！");
					return;
				}
				string no = txtgoodno.Text;
				string num = txtnum.Text;
				bool hasthisid = false;
				connection.Close();
				connection.ConnectionString = "Data Source=.\\SQLEXPRESS01;;Initial Catalog=test1;Integrated Security=SSPI;";
				connection.Open();
				cmd = new SqlCommand("select * from stocks where sno=" + no, connection);
				dataReader = cmd.ExecuteReader();
				hasthisid = dataReader.HasRows;
				if (!hasthisid) {
					MessageBox.Show("id not exists");
					connection.Close();
					return;
				}
				dataReader.Read();
				int storege, now;
				now = int.Parse(num);
				storege = int.Parse(dataReader["quantity"].ToString());
				connection.Close();
				if (now > storege) {
					MessageBox.Show("太多了");
					return;
				}
				connection.Close();
				connection.Open();
				cmd = new SqlCommand("select * from goods where gno=" + no, connection);
				dataReader = cmd.ExecuteReader();
				dataReader.Read();
				string price = dataReader["price"].ToString();
				string name = dataReader["gname"].ToString();
				string unit = dataReader["unit"].ToString();
				string quantity = num;
				
				float fprice = float.Parse(price);
				int fqt = int.Parse(quantity);
				float ftotal;
				ftotal = fprice * fqt;
				string total = ftotal.ToString();
				for(int i=0;i<order.Count;++i){
					if(order[i].goodsid==no){
						int oldq = int.Parse(order[i].goodsquantity);
						int dq = int.Parse(quantity);
						int q = oldq + dq;
						float price1 = float.Parse(order[i].goodsprice);
						float newtotal = price1 * q;
						order[i].goodsquantity = q.ToString();
						order[i].goodstotalprice = newtotal.ToString();
						refreshlist();
						return;
					}
				}
				order.Add(
					new Singleorder() {
						goodsid = no,
						goodsname = name,
						goodsprice = price,
						goodsunit = unit,
						goodsquantity = quantity,
						goodstotalprice=total
					}
				);
				refreshlist();
				//list1.Items.Add(no + " " +name+ price + "  " + quantity + " " + unit + " " + total);
			}
		}

		private void refreshlist() {
			float alltotal=0.0f;
			list1.Items.Clear();
			for(int i=0;i<order.Count;++i){
				alltotal = alltotal + float.Parse(order[i].goodstotalprice);
				list1.Items.Add(order[i].goodsid+" "+order[i].goodsname.Substring(0,15)+order[i].goodsquantity+" "+order[i].goodsunit+"  "+order[i].goodsprice+" "+order[i].goodstotalprice );
			}
			txttotal.Text = alltotal.ToString();
		}

		private void Button_Click(object sender, RoutedEventArgs e) {
			order.RemoveAt(list1.SelectedIndex);
			refreshlist();
		}

		private void list1_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e) {
			 editcount.IsEnabled = (list1.SelectedIndex != -1);
			 delit.IsEnabled= (list1.SelectedIndex != -1);
		}

		private void editcount_Click(object sender, RoutedEventArgs e) {
			string input = Microsoft.VisualBasic.Interaction.InputBox("数量", "修改库存", "Default", 0, 0);
			try{
				int l = int.Parse(input);
				if (l <= 0) MessageBox.Show("无效");
				else {
					int nn = list1.SelectedIndex;
					order[nn].goodsquantity = input;
					order[nn].goodstotalprice = (l * float.Parse(order[nn].goodsprice)).ToString();
					refreshlist();
				}
			} catch{
				MessageBox.Show("无效");
			}
		}

		
	}
}
