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

            //if product does not exist
            else if (mydbAccess.Status == 0)
            {
                
                mydbAccess.Query = @"INSERT INTO [product] (product_name, product_sub_name, product_full_name, product_sales_unit, 
                                product_category_id) VALUES ('" + productName + "','" + productSubName + "','"
                                + productFullName + "','" + productSalesUnit + "','" + product_categoryId + "')";

                mydbAccess.Insert();

                if (mydbAccess.Status == 1)
                {
                    MessageBox.Show("Product Added Successfully. Go to Add Subcategory and Add a subcategory to Activate product");
                    ProductForm myproductform = new ProductForm(firstname, userRole, userId);
                    this.Hide();
                    myproductform.Show();
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
            int id = (int)comboBox2.SelectedValue;

            if ( id != -1)
            {
                listBox2.Hide();
                listBox1.Show();
                button8.Show();

                string query1 = @"select * from product_sub where product_id = '" + id + "'";
                mydbAccess.Query = query1;
                mydbAccess.Select();

                var result = mydbAccess.Result;

                if (mydbAccess.Status == 1)
                {
                    listBox1.DisplayMember = "product_sub_name";
                    listBox1.ValueMember = "id";
                    listBox1.DataSource = result;

                    if (checkBox1.Visible == true)
                    {
                        checkBox1.Hide();
                    }

                    if (button10.Visible == true)
                    {
                        button10.Hide();
                    }
                }

                else
                {
                    MessageBox.Show("No subcategory for this product! Add a sub category or tick the box if sub categories do not exist.");
                    if (checkBox1.Visible == false)
                    {
                        checkBox1.Show();
                    }

                    if(button10.Visible == false)
                    {
                        button10.Show();
                    }

                    label22.Hide();
                    label23.Hide();
                    label15.Hide();
                    label19.Hide();
                    label20.Hide();
                    label21.Hide();
                    label27.Hide();
                    radioButton1.Hide();
                    radioButton2.Hide();
                    checkBox2.Hide();
                    checkBox3.Hide();
                    checkBox4.Hide();

                    numericUpDown1.Hide();
                    numericUpDown2.Hide();
                    textBox6.Hide();

                    listBox1.Hide();
                    listBox2.Show();

                    button8.Hide();
                }
            }

            else
            {
                label22.Hide();
                label23.Hide();
                label15.Hide();
                label19.Hide();
                label20.Hide();
                label21.Hide();
                label27.Hide();
                radioButton1.Hide();
                radioButton2.Hide();
                checkBox2.Hide();
                checkBox3.Hide();
                checkBox4.Hide();

                numericUpDown1.Hide();
                numericUpDown2.Hide();
                textBox6.Hide();

                listBox1.Hide();
                listBox2.Show();
                button8.Hide();

            }
        }


        private void button6_Click(object sender, EventArgs e)
        {
            int productId = (int)comboBox2.SelectedValue;
            string productSub;

           
                if (productId == -1)
                {
                    MessageBox.Show("Select a Product!");
                }

                else
                {
                    if (checkBox1.Checked == false)
                    {
                        if (textBox8.Text != "")
                        {
                            productSub = textBox8.Text;

                            mydbAccess.Query = @"insert into product_sub(product_id, product_sub_name, product_sub_status)
                                   values('" + productId + "','" + productSub + "','4')";
                            mydbAccess.Insert();

                            if (mydbAccess.Status == 1)
                            {
                                MessageBox.Show("Product SubCategory Add Successful! Prduct Can be stocked Now");
                            }

                            else if (mydbAccess.Message == null)
                            {
                                MessageBox.Show("Product SubCategory Add NOT Successful!");
                            }

                            else
                            {
                                MessageBox.Show(mydbAccess.Message);
                            }
                        }

                        else
                        {
                            MessageBox.Show("Fill in Sub-Cat");
                            textBox8.Clear();
                        }
                    }


                    else if (checkBox1.Checked == true)
                    {
                        mydbAccess.Query = @"select product_full_name from product where id = '" + productId + "'";
                        mydbAccess.Select();

                        if (mydbAccess.Status == 1)
                        {
                            var result = mydbAccess.Result;
                            productSub = result.Rows[0]["product_full_name"].ToString();

                            mydbAccess.Query = @"insert into product_sub(product_id, product_sub_name, product_sub_status)
                                   values('" + productId + "','" + productSub + "','4')";
                            mydbAccess.Insert();

                            if (mydbAccess.Status == 1)
                            {
                                MessageBox.Show("Product Activation Successful! Product Can be stocked Now");
                            }

                            else if (mydbAccess.Message == null)
                            {
                                MessageBox.Show("Product Activation NOT Successful!");
                            }

                            else
                            {
                                MessageBox.Show(mydbAccess.Message);
                            }
                        }

                    }

                    //RefreshPage
                    ProductForm myproductform = new ProductForm(firstname, userRole, userId);
                    this.Hide();
                    myproductform.Show();
                    myproductform.button9.PerformClick();

                }      

            }

        private void button9_Click(object sender, EventArgs e)
        {
            tabControl1.SelectedTab = tabPage3;
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox1.Checked == true)
            {
                textBox8.Clear();
                button6.Text = "Activate";
            }

            else
            {
                button6.Text = "Add";
            }
           
        }

        private void button8_Click(object sender, EventArgs e)
        {
           
        }

        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            string name;
            string productStatus;
            string currentPrice;
            string unit;
            string stockLeft;
            string restockLevel;
            string id1;

            if (listBox1.SelectedItems.Count > 0)
            {
                int id = (int)listBox1.SelectedValue;

                checkBox2.Checked = false;
                checkBox3.Checked = false;
                checkBox4.Checked = false;

                mydbAccess.Query = @"select r.product_sub_name,r.product_sub_restock_level,r.product_sales_unit, r.product_sub_price,r.product_id,r.status from
                                    (select ps.*,plk.status, p.product_sales_unit from [product_sub] ps 
                                    inner join [product_sub_status_LK] plk on ps.product_sub_status = plk.id
                                    inner join [product] p on p.id = ps.product_id) as r where r.id = '" + id + "'";

                mydbAccess.Select();


                if (mydbAccess.Status == 1)
                {
                    var result = mydbAccess.Result;

                    name = result.Rows[0]["product_sub_name"].ToString();
                    productStatus = result.Rows[0]["status"].ToString();
                    currentPrice = result.Rows[0]["product_sub_price"].ToString();
                    unit = result.Rows[0]["product_sales_unit"].ToString();
                    restockLevel = result.Rows[0]["product_sub_restock_level"].ToString();
                    id1 = result.Rows[0]["product_id"].ToString();

                    //if product sub category is not new
                    if (productStatus != "NEW PRODUCT")
                    {
                        //calculate stockLeft

                        mydbAccess.Query = @"select stock_left from stock_tracker where product_sub_id = '" + id + "'";
                        mydbAccess.Select();

                        if (mydbAccess.Status == 1)
                        {
                            var result1 = mydbAccess.Result;
                            int sum = 0;

                            for (int i = 0; i < result1.Rows.Count; i++)
                            {
                                int stockLeftInt = (int)result1.Rows[i]["stock_left"];
                                sum += stockLeftInt;
                            }

                            stockLeft = sum.ToString();
                        }

                        else
                        {
                            stockLeft = "0";
                        }

                        //Display Properties
                        label14.Text = id.ToString();
                        label28.Text = id1;

                        label22.Text = productStatus;
                        label23.Text = stockLeft;                   

                        label22.Show();
                        label23.Show();
                        label15.Show();
                        label19.Show();
                        label20.Show();
                        label21.Show();
            
                        label27.Show();

                        checkBox2.Show();
                        checkBox3.Show();
                        checkBox4.Show();

                        radioButton1.Hide();
                        radioButton2.Hide();

                        numericUpDown1.Value = Decimal.Parse(restockLevel);
                        numericUpDown1.Show();
                        numericUpDown1.Enabled = false;

                        numericUpDown2.Value = Decimal.Parse(currentPrice);
                        numericUpDown2.Show();
                        numericUpDown2.Enabled = false;

                        textBox6.Text = name;
                        textBox6.Show();
                        textBox6.Enabled = false;

                    }


                    else
                    {
                        //Display Properties
                        label14.Text = id.ToString();
                        label28.Text = id1;

                        label22.Text = productStatus;
                        label22.Show();
                        label15.Show();
                        label27.Show();

                        radioButton1.Show();
                        radioButton2.Show();

                        label23.Hide();
                        label19.Hide();
                        label20.Hide();
                        label21.Hide();

                        checkBox2.Hide();
                        checkBox3.Hide();
                        checkBox4.Hide();

                        numericUpDown1.Hide();
                        numericUpDown2.Hide();

                        checkBox2.Show();
                        textBox6.Text = name;
                        textBox6.Show();
                        textBox6.Enabled = false;
                    }
                }


            }

            else
            {
                //Set product sub properties here
                MessageBox.Show("Select A Product to Update");
            }
        }

        private void checkBox5_CheckedChanged(object sender, EventArgs e)
        {
            button6.Text = "Delete";
        }

        private void button9_Click_1(object sender, EventArgs e)
        {
            tabControl1.SelectedTab = tabPage3;
        }

        private void button10_Click(object sender, EventArgs e)
        {
            int productId = (int)comboBox2.SelectedValue;

            mydbAccess.Query = "Delete from product where id = '" + productId + "'";
            mydbAccess.Delete();

            if (mydbAccess.Status == 1)
            {
                MessageBox.Show("Product Delete Successful!");
            }

            else if (mydbAccess.Message != null)
            {
                MessageBox.Show("Product Delete NOT Successful " + mydbAccess.Message);
            }

            else if (mydbAccess.Message != null)
            {
                MessageBox.Show("Product Delete NOT Successful!");
            }

        }
    }
 }
