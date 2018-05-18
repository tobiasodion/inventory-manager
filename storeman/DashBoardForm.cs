using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing.Drawing2D;
using System.Configuration;

namespace storeman
{
    public partial class DashBoardForm : Form
    {
        static string conn = ConfigurationManager.ConnectionStrings["myConnectionString"].ConnectionString;
        dbAccess mydbAccess = new dbAccess(conn);

        public string firstname;
        public string userRole;
        public int userId;

        public DashBoardForm(string name, string role, int id)
        {
            firstname = name;
            userRole = role;
            userId = id;

            InitializeComponent();
            label3.Text = "Hi, " + firstname;
            ComboBox1Update();

            if (userRole == "user")
            {
                button2.Enabled = false;
                button3.Enabled = false;
                button5.Enabled = false;
                button6.Enabled = false;
                button8.Enabled = false;
            }
        }

        private void button7_Click(object sender, EventArgs e)
        {
            this.Close();
            LoginForm myloginform = new LoginForm();
            myloginform.Show();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            ProductForm myproductform = new ProductForm(firstname, userRole, userId);
            this.Hide();
            myproductform.Show();
        }

        private void button3_Click(object sender, EventArgs e)
        {
           StockForm mystockform = new StockForm(firstname, userRole, userId);
           this.Hide();
           mystockform.Show();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            //do housekeeping
            //get total stockleft, compare stock left with restock level set
            POSForm myposform = new POSForm(firstname, userRole, userId);
            this.Hide();
            myposform.Show();
        }
        
        private void button5_Click(object sender, EventArgs e)
        {
            SignUpForm mysignupform = new SignUpForm(firstname, userRole, userId);
            this.Hide();
            mysignupform.Show();
        }

        private void button8_Click(object sender, EventArgs e)
        {
            ResetForm myresetform = new ResetForm(firstname, userRole, userId);
            this.Hide();
            myresetform.Show();
        }

        private void button6_Click(object sender, EventArgs e)
        {
            ReportForm myreportform = new ReportForm(firstname, userRole, userId);
            this.Hide();
            myreportform.Show();
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            listView1.Items.Clear();

            int id = (int)comboBox1.SelectedValue;

            mydbAccess.Query = @"select r.* from 
                               (select p.product_full_name, ps.product_sub_name,
                               plk.id from product_sub ps
                               inner join product p on ps.product_id = p.id
                               inner join product_sub_status_LK plk on ps.product_sub_status = plk.id) 
                               as r where r.id = '" + id + "' order by r.product_full_name asc";

            mydbAccess.Select();
            var dt = mydbAccess.Result;

            if (mydbAccess.Status == 1)
            {
                foreach (DataRow row in dt.Rows)
                {
                    string productFullName = row["product_full_name"].ToString();
                    string productSubName = row["product_sub_name"].ToString();
                  

                    string[] data = { productFullName.ToUpper(), productSubName };
                    var listViewItem = new ListViewItem(data);
                    listView1.Items.Add(listViewItem);
                }

            }

            else
            {
                string[] data = { "NIL", "NIL" };
                var listViewItem = new ListViewItem(data);
                listView1.Items.Add(listViewItem);

            }

        }

        private void ComboBox1Update()
        {
            mydbAccess.Query = "SELECT * FROM product_sub_status_LK order by id desc";
            mydbAccess.Select();

            if (mydbAccess.Status == 1)
            {
                var result = mydbAccess.Result;

                comboBox1.ValueMember = "id";
                comboBox1.DisplayMember = "status";
                comboBox1.DataSource = result;
            }
        }

      
    }
}
