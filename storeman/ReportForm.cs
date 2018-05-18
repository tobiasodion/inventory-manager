using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace storeman
{
    public partial class ReportForm : Form
    {
        public string userRole;
        public string firstname;
        public int userId;

        static string conn = ConfigurationManager.ConnectionStrings["myConnectionString"].ConnectionString;
        dbAccess mydbAccess = new dbAccess(conn);

        public ReportForm(string name, string role, int id)
        {
            firstname = name;
            userRole = role;
            userId = id;
            InitializeComponent();
            ComboBoxUpdate();
        }

        private void comboBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
            //get all product in that product_category
            int id = (int)comboBox2.SelectedValue;

            mydbAccess.Query = "SELECT id,product_name FROM product where product_category_id = '" + id + "' AND product_product_category_price IS NOT NULL";
            mydbAccess.Select();

            if (mydbAccess.Status == 1)
            {
                var result = mydbAccess.Result;
                comboBox1.ValueMember = "id";
                comboBox1.DisplayMember = "product_name";
                comboBox1.DataSource = result;
            }

            else
            {
                MessageBox.Show("No product Available in that product_category");
                ComboBoxUpdate();
            }

        }

        private void ComboBoxUpdate()
        {
            mydbAccess.Query = "SELECT * FROM product_category";
            mydbAccess.Select();

            if (mydbAccess.Status == 1)
            {
                comboBox1.ValueMember = "id";
                comboBox1.DisplayMember = "product_category";
                comboBox1.DataSource = mydbAccess.Result;
            }
        }

    }
}
