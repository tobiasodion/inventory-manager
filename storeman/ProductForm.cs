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
    public partial class ProductForm : Form
    {
        public string userRole;
        public string firstname;
        public int userId;

        static string conn = ConfigurationManager.ConnectionStrings["myConnectionString"].ConnectionString;
        dbAccess mydbAccess = new dbAccess(conn);

        public ProductForm(string name, string role, int id)
        {
            firstname = name;
            userRole = role;
            userId = id;
            InitializeComponent();

            label15.Hide();
            textBox6.Hide();

            label14.Hide();
            textBox7.Hide();

            label16.Hide();
            textBox8.Hide();

            button6.Hide();
            button8.Hide();
            ComboBoxUpdate();
        }


        private void button1_Click_1(object sender, EventArgs e)
        {

            string productName = (textBox1.Text).ToLower();
            string productSubName = (textBox5.Text).ToLower();
            string productFullName = productName + "(" + productSubName + ")";
            string productSalesUnit = (textBox4.Text).ToLower();
            int product_categoryId = (int)comboBox1.SelectedValue;


            //  int stockStatus = 4;
            //  int stockLeft = 0;

            //check if product already exist
            string check1 = productName.Substring(0, 3) + "%";
            string check2 = productSubName.Substring(0, 3) + "%";

            string query1 = @"select product_full_name from product where product_name like '"
                             + check1 + "' AND product_sub_name like '" + check2 + "'";

            mydbAccess.Query = query1;

            mydbAccess.Select();

            if (mydbAccess.Status == 0 && mydbAccess.Message != null)
            {
                MessageBox.Show(mydbAccess.Message);
            }

            else if (mydbAccess.Status == 0)
            {
                List<string> queryList = new List<string>();

                string query2 = @"INSERT INTO [product] (product_name, product_sub_name, product_full_name, product_sales_unit, 
                                product_category_id) VALUES ('" + productName + "','" + productSubName + "','"
                                + productFullName + "','" + productSalesUnit + "','" + product_categoryId + "')";

                string query3 = @"INSERT INTO [product_sub] (product_id, product_sub_name) VALUES 
                                (IDENT_CURRENT('product'),'" + productFullName + "')";

                string query4 = @"INSERT INTO [stock_tracker] (product_sub_id, stock_left, stock_status) VALUES
                                  (IDENT_CURRENT('product_sub'), '0', '4')";

                queryList.Add(query2);
                queryList.Add(query3);
                queryList.Add(query4);

                mydbAccess.QueryList = queryList;
                mydbAccess.TransactionOperation();

                if (mydbAccess.Status == 1)
                {
                    MessageBox.Show("Product Added Successfully");

                    //RefreshPage
                    ProductForm myproductform = new ProductForm(firstname, userRole, userId);
                    this.Hide();
                    myproductform.Show();
                    //  myproductform.button9.PerformClick();
                }

                else
                {
                    if (mydbAccess.Message == null)
                    {
                        MessageBox.Show("Product Add not Successful");
                    }

                    else
                    {
                        MessageBox.Show(mydbAccess.Message);
                    }
                }

                textBox1.Clear();
                textBox4.Clear();
                textBox5.Clear();
            }

            else
            {
                MessageBox.Show("product already exist!");
                textBox1.Clear();
                textBox4.Clear();
                textBox5.Clear();
            }


        }

        private void button4_Click(object sender, EventArgs e)
        {
            DashBoardForm form = new DashBoardForm(firstname, userRole, userId);
            this.Hide();
            form.Show();
        }

        private void button7_Click(object sender, EventArgs e)
        {
            this.Close();
            LoginForm form3 = new LoginForm();
            form3.Show();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            string categoryName = textBox3.Text;
            string categoryDescription = textBox2.Text;

            mydbAccess.Query = "INSERT INTO [product_category] (product_category_name, product_category_description) VALUES ('" + categoryName + "','" + categoryDescription + "')";
            mydbAccess.Insert();

            if (mydbAccess.Status == 1)
            {
                MessageBox.Show("Product Category Added Successfully");
            }

            else
            {
                if (mydbAccess.Message == null)
                {
                    MessageBox.Show("Product Category Add not Successful");
                }

                else
                {
                    MessageBox.Show(mydbAccess.Message);
                }
            }

            textBox3.Clear();
            textBox2.Clear();
            ComboBoxUpdate();
            tabControl1.SelectedTab = tabPage1;
        }

        private void button5_Click(object sender, EventArgs e)
        {
            tabControl1.SelectedTab = tabPage2;
        }

        private void ComboBoxUpdate()
        {
            mydbAccess.Query = "SELECT * FROM product_category";
            mydbAccess.Select();

            if (mydbAccess.Status == 1)
            {
                var result = mydbAccess.Result;

                DataRow row = result.NewRow();
                row["id"] = -1;
                row["product_category_name"] = "--Select--";
                result.Rows.InsertAt(row, 0);

                comboBox1.ValueMember = "id";
                comboBox1.DisplayMember = "product_category_name";
                comboBox1.DataSource = result;

            }

            mydbAccess.Query = "SELECT * FROM product";
            mydbAccess.Select();

            if (mydbAccess.Status == 1)
            {
                var result = mydbAccess.Result;

                DataRow row = result.NewRow();
                row["id"] = -1;
                row["product_full_name"] = "--Select--";
                result.Rows.InsertAt(row, 0);

                comboBox2.ValueMember = "id";
                comboBox2.DisplayMember = "product_full_name";
                comboBox2.DataSource = result;


            }
        }

        private void comboBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
            if ((int)comboBox2.SelectedValue != -1)
            {
                int productSubId = (int)(comboBox2.SelectedValue);

                string query1 = @"select * from product_sub where product_id = '" + productSubId + "'";
                mydbAccess.Query = query1;
                mydbAccess.Select();

                var result = mydbAccess.Result;

                if (mydbAccess.Status == 1)
                {
                    if (result.Rows.Count == 1)
                    {
                        MessageBox.Show("You must add two product sub categories for first time");
                        listBox1.Hide();

                        label15.Show();
                        textBox6.Show();

                        label16.Hide();
                        textBox8.Hide();

                        label14.Show();
                        textBox7.Show();

                        if (button8.Visible == false) {

                            button8.Show();
                        }
                    }

                    else
                    {
                        if (label15.Visible == true)
                        {
                            label15.Hide();
                        }

                        if (textBox6.Visible == true)
                        {
                            textBox6.Hide();
                        }

                        if (label14.Visible == true)
                        {
                            label14.Hide();
                        }

                        if (textBox7.Visible == true)
                        {
                            textBox7.Hide();
                        }

                        if (button6.Visible == false)
                        {
                            button6.Show();
                        }

                        textBox8.Show();
                        label16.Show();

                        listBox1.DisplayMember = "product_sub_name";
                        listBox1.ValueMember = "id";
                        listBox1.DataSource = result;

                        if (listBox1.Visible == false)
                        {
                            listBox1.Show();
                        }
                    }
                }

                else
                {
                    MessageBox.Show(mydbAccess.Message);
                }
            }

            else
            {
                textBox6.Hide();
                label15.Hide();

                textBox7.Hide();
                label14.Hide();

                listBox1.Hide();
            }
        }

        private void button8_Click(object sender, EventArgs e)
        {

            //check to ensure both box are not empty
            if (textBox6.Text != "" || textBox7.Text != "")
            {
                int productId = (int)comboBox2.SelectedValue;
                string productSub1 = textBox7.Text;
                string productSub2 = textBox6.Text;
                //TRANSACTION
                //update the product subcategory with product_id comboBox
                //insert new product sub category for product with product_id comboBox
                //TRANSACTION

                List<string> queryList = new List<string>();

                string query1 = @"update product_sub set product_sub_name = '"
                                + productSub1 + "' where product_id = '" + productId + "'";

                string query2 = @"insert into product_sub(product_id, product_sub_name)
                                values('" + productId + "','" + productSub2 + "')";

                string query3 = @"INSERT INTO [stock_tracker] (product_sub_id, stock_left, stock_status) VALUES
                                  (IDENT_CURRENT('product_sub'), '0', '4')";

                queryList.Add(query1);
                queryList.Add(query2);
                queryList.Add(query3);

                mydbAccess.QueryList = queryList;
                mydbAccess.TransactionOperation();

                if (mydbAccess.Status == 1)
                {
                    MessageBox.Show("Product SubCategories Add Successful!");
                }

                else if (mydbAccess.Message == null)
                {
                    MessageBox.Show("Product SubCategories Add NOT Successful!");
                }

                else
                {
                    MessageBox.Show(mydbAccess.Message);
                }

                //RefreshPage
                ProductForm myproductform = new ProductForm(firstname, userRole, userId);
                this.Hide();
                myproductform.Show();
                myproductform.button9.PerformClick();
            }
            else
            {
                MessageBox.Show("Fill in Sub-Cat 1 and Sub-Cat 2");
                textBox6.Clear();
                textBox7.Clear();
            }

        }

        private void button6_Click(object sender, EventArgs e)
        {
            if (textBox8.Text != "")
            {
                int productId = (int)comboBox2.SelectedValue;
                string productSub = textBox8.Text;
                List<string> queryList = new List<string>();

                string query1 = @"insert into product_sub(product_id, product_sub_name)
                                values('" + productId + "','" + productSub + "')";

                string query2 = @"INSERT INTO [stock_tracker] (product_sub_id, stock_left, stock_status) VALUES
                                  (IDENT_CURRENT('product_sub'), '0', '4')";

                queryList.Add(query1);
                queryList.Add(query2);


                mydbAccess.QueryList = queryList;
                mydbAccess.TransactionOperation();


                if (mydbAccess.Status == 1)
                {
                    MessageBox.Show("Product SubCategory Add Successful!");
                }

                else if (mydbAccess.Message == null)
                {
                    MessageBox.Show("Product SubCategory Add NOT Successful!");
                }

                else
                {
                    MessageBox.Show(mydbAccess.Message);
                }

                //RefreshPage
                ProductForm myproductform = new ProductForm(firstname, userRole, userId);
                this.Hide();
                myproductform.Show();
                myproductform.button9.PerformClick();

            }

            else
            {
                MessageBox.Show("Fill in Sub-Cat");
                textBox8.Clear();
            }

        }

        private void button9_Click(object sender, EventArgs e)
        {
            tabControl1.SelectedTab = tabPage3;
        }

    
        }
    }
