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
    public partial class StockForm : Form
    {
        public string userRole;
        public string firstname;
        public int userId;

        string stockLeft;

        //NEW PRODUCT, IN STOCK, LOW STOCK or OUT OF STOCK
        string productStatus;
        string currentPrice;
        string unit;


        static string conn = ConfigurationManager.ConnectionStrings["myConnectionString"].ConnectionString;
        dbAccess mydbAccess = new dbAccess(conn);

        public StockForm(string name, string role, int id)
        {
            firstname = name;
            userRole = role;
            userId = id;

            InitializeComponent();
            label1.Hide();
            label2.Hide();
            label4.Hide();
            label15.Hide();
           
            ComboBox2Update();
            ComboBox1Update(-1);
            ComboBox3Update(-1);
        }

        private void button1_Click(object sender, EventArgs e)
        {
         

            int id = (int)comboBox3.SelectedValue;
            int id1 = (int)comboBox1.SelectedValue;

            if (id == -1 || id1 == -1)
            {
                MessageBox.Show("product AND Sub-Category must be selected!");
                if (label4.Visible == true)
                {
                    label4.Hide();
                    label2.Hide();
                    label15.Hide();
                }          
            }
            else
            {
     
                int newStockQuantity = (int)numericUpDown1.Value;
                int restockLevel = (int)numericUpDown4.Value;

                float totalCost = (int)numericUpDown2.Value;
                float costPerUnit = totalCost / newStockQuantity;

                float sellingPrice = (int)numericUpDown3.Value;

                int oldStockQuantity = Int32.Parse(stockLeft);
                int totalStock = newStockQuantity + oldStockQuantity;
       
                    //perform checks
                    //restock level > new stock(quantity) + old stock(label) : totalStock
                    if (restockLevel < totalStock)
                    {
                        //selling price < cost/Unit
                        if (sellingPrice > costPerUnit)
                        {
                            //insert new record into stock table(id, date, productid, newStockQuantity, stock cost, unit cost
                            //update or insert into stock_tracker table(stockleft, unit cost, open/close status) depending stock cost change
                            //update product_category_price in productcategory_table (sellingPrice, product status and product restock level)

                            DateTime date = new DateTime();
                            date = DateTime.Today;
                            

                            List<string> queryList = new List<string>();

                            string query1 = @"INSERT INTO [stock] (stock_date,product_sub_id,product_id,stock_quantity,
                                           stock_cost,stock_unit_cost) VALUES ('" + date + "','"+ id + "','" + id1 + "','"
                                           + newStockQuantity + "','" + totalCost + "','" + costPerUnit + "')";

                            string query2 = @"UPDATE [product_sub] SET product_sub_price = '"+ sellingPrice + "',product_sub_restock_level = '"
                                           +restockLevel+"', product_sub_status = '1' WHERE id = '" + id + "'";

                        
                            string query3 = @"Insert into stock_tracker (product_sub_id, product_id, stock_left, stock_unit_cost_price
                                            ) values('"+id+ "','" + id1 + "','" + newStockQuantity + "', '" + costPerUnit + "')";

                             string query4 = @"update product_sub set product_sub_status ='1' where id = '"+id+"' ";
                       

                        queryList.Add(query1);
                        queryList.Add(query2);
                        queryList.Add(query3);
                        queryList.Add(query4);


                        mydbAccess.QueryList = queryList;
                        mydbAccess.TransactionOperation();

                            if (mydbAccess.Status == 1)
                            {
                                MessageBox.Show("Product stocked successfully");
                            }

                            if (mydbAccess.Status == 0)
                            {
                                MessageBox.Show("Product stock NOT successful " + mydbAccess.Message);
                            }


                            numericUpDown1.Value = 0;
                            numericUpDown2.Value = 0;
                            numericUpDown3.Value = 0;
                            numericUpDown4.Value = 0;

                        ComboBox2Update();
                        ComboBox1Update(-1);
                        ComboBox3Update(-1);
                    }
                        else
                        {
                            MessageBox.Show("Selling Price cannot be less than Cost per Unit(" + costPerUnit + ")" );
                           numericUpDown3.Value = 0;
                    }
                    }
                    else
                    {
                        MessageBox.Show("Restock level cannot be greater than total stock available");
                    }
              
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
            LoginForm form3 = new LoginForm();
            this.Close();
            form3.Show();
        }

        //category change to populate product dropdown
        private void comboBox2_SelectedIndexChanged(object sender, EventArgs e)
       {
           int id = (int)comboBox2.SelectedValue;
            ComboBox1Update(id);

       }

        //product change to populate product category dropdown
        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if(label4.Visible == true)
            {
                label4.Hide();
                label2.Hide();
                label15.Hide();
            }
            int id = (int)comboBox1.SelectedValue;
            ComboBox3Update(id);
        }

        private void comboBox3_SelectedIndexChanged(object sender, EventArgs e)
        {
           
            int id = (int)comboBox3.SelectedValue;

            //when a subcategory must be selected
            if (id != -1)
            {
               
                //Get Product Details(name, sales_unit, current_price, status 
                mydbAccess.Query = @"select r.product_sub_name, r.product_sales_unit, r.product_sub_price, r.status from
                                    (select ps.*,plk.status, p.product_sales_unit from [product_sub] ps 
                                    inner join [product_sub_status_LK] plk on ps.product_sub_status = plk.id
                                    inner join [product] p on p.id = ps.product_id) as r where r.id = '" + id + "'";

                mydbAccess.Select();

                
                if (mydbAccess.Status == 1)
                {
                    var result = mydbAccess.Result;
                    productStatus = result.Rows[0]["status"].ToString();
                    currentPrice = result.Rows[0]["product_sub_price"].ToString();
                    unit = result.Rows[0]["product_sales_unit"].ToString();

                    label1.Text = "(" + unit + ")";
                    label1.Show();

                    //if product sub category is not new
                    if (productStatus != "NEW PRODUCT")
                    {

                        //calculate stockLeft

                        mydbAccess.Query = @"select stock_left from stock_tracker where product_sub_id = '"+ id +"'";
                        mydbAccess.Select();

                        if (mydbAccess.Status == 1)
                        {
                            var result1 = mydbAccess.Result;
                            int sum = 0;

                            for(int i = 0; i < result1.Rows.Count; i++)
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


                        label2.Text = stockLeft + " " + unit + " in stock";
                        label2.Show();

                        label4.Text = "STATUS: " + productStatus;
                        label4.Show();

                        label15.Text = "CURRENT PRICE: #" + currentPrice;
                        label15.Show();

                    }

                    //if product sub category is new
                    else
                    {
                        if(label2.Visible == true)
                        {
                            label2.Hide();
                            label15.Hide();
                        }

                        stockLeft = "0";
                        label4.Text = "STATUS: " + productStatus;
                        label4.Show();
                    }
                }
              
            }

        }


        private void ComboBox2Update()
        {
            mydbAccess.Query = "SELECT id, product_category_name FROM product_category";
            mydbAccess.Select();

            if (mydbAccess.Status == 1 || (mydbAccess.Status == 0 && mydbAccess.Message == null))
            {
                var result = mydbAccess.Result;

                DataRow row = result.NewRow();
                row["id"] = -1;
                row["product_category_name"] = "--Select--";
                result.Rows.InsertAt(row, 0);

                comboBox2.ValueMember = "id";
                comboBox2.DisplayMember = "product_category_name";
                comboBox2.DataSource = result;
            } 
        }

        private void ComboBox1Update(int id)
        {
            int myId = id;

            if (id == -1)
            {
                mydbAccess.Query = "SELECT id, product_full_name FROM product";
               
            }

            else
            {
                mydbAccess.Query = "SELECT id, product_full_name FROM product where product_category_id = '"+myId+"'";
            }

            mydbAccess.Select();

            if (mydbAccess.Status == 1 ||(mydbAccess.Status == 0 && mydbAccess.Message == null))
            {
                var result = mydbAccess.Result;

                DataRow row = result.NewRow();
                row["id"] = -1;
                row["product_full_name"] = "--Select--";
                result.Rows.InsertAt(row, 0);

                comboBox1.ValueMember = "id";
                comboBox1.DisplayMember = "product_full_name";
                comboBox1.DataSource = result;
            }
        }

        private void ComboBox3Update(int id)
        {
            int myId = id;

            if (id == -1)
            {
                mydbAccess.Query = "SELECT id, product_sub_name FROM product_sub";
            }
            else
            {
                mydbAccess.Query = "SELECT id, product_sub_name FROM product_sub where product_id = '"
                                    +myId+"'";
            }

            mydbAccess.Select();

            if (mydbAccess.Status == 1 || (mydbAccess.Status == 0 && mydbAccess.Message == null))
            {
                var result = mydbAccess.Result;

                DataRow row = result.NewRow();
                row["id"] = -1;
                row["product_sub_name"] = "--Select--";
                result.Rows.InsertAt(row, 0);

                comboBox3.ValueMember = "id";
                comboBox3.DisplayMember = "product_sub_name";
                comboBox3.DataSource = result;
            }
        }
      
    }
}
