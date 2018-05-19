using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace storeman
{
    public partial class POSForm : Form
    {
        public string userRole;
        public string firstname;
        public int userId;

        static string conn = ConfigurationManager.ConnectionStrings["myConnectionString"].ConnectionString;
        dbAccess mydbAccess = new dbAccess(conn);

        float grandTotal = 0;
        float Total = 0;

        string change1;
        decimal change;
        decimal newChange; //if the customer has outstanding change

        int stockLeft;
        string unit;

      //  int stockStatus;

        public POSForm(string name, string role, int id)
        {

            firstname = name;
            userRole = role;
            userId = id;

            InitializeComponent();

            label25.Hide();

            ComboBox2Update();
            ComboBox1Update(-1);
            ComboBox3Update(-1);
        }

        //Add Item to cart
        private void button5_Click(object sender, EventArgs e)
        {
            string Quantity = numericUpDown1.Value.ToString();
            int myquantity = (int)numericUpDown1.Value;

            if (myquantity <= stockLeft  && myquantity != 0)
            {
                int id = (int)comboBox3.SelectedValue;
                int id1 = (int)comboBox1.SelectedValue;

                if (id == -1 || id1 == -1)
                {
                    MessageBox.Show("product AND Sub-Category must be selected!");
                }
                else
                {
                    mydbAccess.Query = @"select product_sub_price from [product_sub] where id='" 
                                       + id + "' AND product_id = '"+id1+"'";
                    mydbAccess.Select();

                    if (mydbAccess.Status == 1)
                    {
                        var result = mydbAccess.Result;

                        string product = comboBox1.Text + ": " + comboBox3.Text;

                        int productId = id;
                        string productId1 = productId.ToString();

                        int productSubId = id1;
                        string productSubId1 = productSubId.ToString();

                        string product_sub_price = result.Rows[0]["product_sub_price"].ToString();
                        float UnitPrice = Int32.Parse(product_sub_price);

                        Total = UnitPrice * myquantity;
                        string stringtotal = Total.ToString();

                        grandTotal = Int32.Parse(label4.Text) + Total;

                        //grand total display at cart form
                        label4.Text = grandTotal.ToString();

                        //grand total at checkout form
                        label5.Text = label4.Text;

                       
                        string[] cartItem = { product, product_sub_price,Quantity,stringtotal,productId1,productSubId1 };
                        var listViewItem = new ListViewItem(cartItem);
                        listView1.Items.Add(listViewItem);
                        numericUpDown1.Value = 0;
                    }
                }

                ComboBox2Update();
                ComboBox1Update(-1);
                ComboBox3Update(-1);

                numericUpDown4.Value = 0;
                if (label13.Text != "")
                {
                    if (button9.Enabled == true)
                    {
                        button9.Enabled = false;
                    }

                    if (radioButton1.Checked == true)
                    {
                        radioButton1.Checked = false;
                    }

                    if (radioButton2.Checked == true)
                    {
                        radioButton2.Checked = false;
                    }

                    if (checkBox1.Checked == true)
                    {
                        checkBox1.Checked = false;
                    }
                }
            }
            else
            {
                if (myquantity <= 0)
                {
                    MessageBox.Show("Enter a valid quantity ");
                    numericUpDown1.Focus();
                }

                else
                {
                    MessageBox.Show("Quantity exceeds quantity in stock. Re-enter quantity ");
                }

                numericUpDown1.Value = 0;
                numericUpDown1.Focus();
            }

           
        }

        //proceeding to checkout
        private void button1_Click(object sender, EventArgs e)
        {
            if (listView1.Items.Count == 0)
            {
                MessageBox.Show("Add at least an Item to Cart!");
            }

            else
            {
                if (numericUpDown4.Value == 0)
                {
                    MessageBox.Show("Input Amount Tendered by customer!");
                    numericUpDown4.Focus();
                }

                else
                {
                    if (button9.Enabled == true)
                    {
                        button9.Enabled = false;
                    }

                    if (radioButton1.Checked == true)
                    {
                        radioButton1.Checked = false;
                    }

                    if (radioButton2.Checked == true)
                    {
                        radioButton2.Checked = false;
                    }

                    if (checkBox1.Checked == true)
                    {
                        checkBox1.Checked = false;
                    }

                    int amountPayable = Int32.Parse(label5.Text);
                    int amountTendered = (int)numericUpDown4.Value;

                    if (amountTendered >= amountPayable)
                    {
                        int change = amountTendered - amountPayable;
                        label13.Text = change.ToString();
                        tabControl1.SelectedTab = tabPage2;
                    }

                    else
                    {
                        MessageBox.Show("Amount Tendered is less than price of goods");
                        numericUpDown4.Value = 0; ;
                        label13.Text = "";
                        tabControl1.SelectedTab = tabPage1;
                    }

                    label15.Text = numericUpDown4.Text;
                }
            }
            
        }

       

        private void radioButton1_CheckedChanged(object sender, EventArgs e)
        {
            //Check if the proceed to checkout button was used
            if (label13.Text != "")
            {
                if (checkBox1.Checked == true)
                {
                    checkBox1.Checked = false;
                }

                label16.Hide();
                label11.Hide();
                label12.Hide();
                label17.Hide();
                numericUpDown2.Hide();
                textBox4.Hide();
                textBox5.Hide();
                textBox7.Hide();
            }
            else
            {
                radioButton1.Checked = false;
                tabControl1.SelectedTab = tabPage1;
                MessageBox.Show("Click on the Proceed to check out button!");
                button1.Focus();
            }
        }

        private void radioButton2_CheckedChanged(object sender, EventArgs e)
        {
            if (label13.Text != "")
            {
                if (checkBox1.Checked == true)
                {
                    checkBox1.Checked = false;
                }

                label16.Show();
                label11.Show();
                label12.Show();
                label17.Show();
                numericUpDown2.Show();
                textBox4.Show();
                textBox5.Show();
                textBox7.Show();
            }
            else
            {
                radioButton2.Checked = false;
                tabControl1.SelectedTab = tabPage1;
                MessageBox.Show("Click on the Proceed to check out button!");
                button1.Focus();
            }
        }

        //Remove item from cart
        private void button6_Click(object sender, EventArgs e)
        {
           
            foreach (ListViewItem cartItem in listView1.SelectedItems)
            {
                string price = listView1.SelectedItems[0].SubItems[3].Text;
                listView1.Items.Remove(cartItem);
                float myprice = Int32.Parse(price);
                grandTotal = grandTotal - myprice;
                label4.Text = grandTotal.ToString();
                label5.Text = label4.Text;
            }

            ComboBox2Update();
            ComboBox1Update(-1);
            ComboBox3Update(-1);

            numericUpDown4.Value = 0;

            if(label13.Text != "")
            {
                if (button9.Enabled == true)
                {
                    button9.Enabled = false;
                }

                if (radioButton1.Checked == true)
                {
                    radioButton1.Checked = false;
                }

                if (radioButton2.Checked == true)
                {
                    radioButton2.Checked = false;
                }

                if (checkBox1.Checked == true)
                {
                    checkBox1.Checked = false;
                }
            }        
        }

        private void button9_Click(object sender, EventArgs e)
        {
            //Get Time 
            DateTime datetime = DateTime.Now;

            int stockLeft = 0;
            int statusCount = 1;

            //create transaction query list for sales processing
            List<string> queryList = new List<string>();

            //create update product sub status list
            List<int> statusUpdate = new List<int>();

            if (radioButton2.Checked == true )
            {
                //add change insert query to global querylist
                //Get change owed customer
                int changeDue = Int32.Parse(label13.Text);
                int changeIssued = (int)numericUpDown2.Value;

                change = changeDue - changeIssued;

                string firstName = textBox7.Text;
                string LastName = textBox4.Text;
                string contact = textBox5.Text;

                // Check if customer Details already exist
                mydbAccess.Query = "select creditor_amount from creditor where creditor_contact = '"+contact+"'";
                mydbAccess.Select();

                //if record exist, update the record
                //else create new record

                if (mydbAccess.Status == 1)
                {
                    var result = mydbAccess.Result;
                    int oldChange = (int)result.Rows[0]["creditor_amount"];
                    newChange = change + oldChange;

                    queryList.Add("update creditor set creditor_amount = '" + newChange + "', creditor_status = 'not paid' where creditor_contact = '" + contact + "'");

                }

                else
                {
                    queryList.Add(@"insert into creditor(creditor_date,creditor_firstname,creditor_lastname,creditor_contact,
                                   creditor_amount, creditor_status)
                                values('" + datetime + "','" + firstName + "','" + LastName + "','" + contact + "','"
                               + change + "','not paid')");
                }
               
            }

            //Loop through all product on the cart
            //check if stock left is grater than quantity ordered for a paricular product
            //get new stock left (Quantity - stock_left)

            //RHEMA:As we cross from one stock tracker record to another for a particular product only
            //sales_quantity(from listview) &
            //unit_cost_price(from stock_tracker)
            //may vary

            //for each cross a new sales record is created

            //get details for insert in sales table 
            //sales_date(computed)
            //product_sub_id(from listview)
            //product_id(from listview)
            //sales_price_unit(from listview)
            //user_id(from class)
            //create query for sales table(query2)

            foreach (ListViewItem cartItem in listView1.Items)
            {

                //get the salesPrice/Unit, quantity, product Id(id) and product subcategory id (id1) from listView Control

                //sales_price_unit
                string salesPrice1 = cartItem.SubItems[1].Text;
                int salesPrice = Int32.Parse(salesPrice1);
                //quantity
                string quantity1 = cartItem.SubItems[2].Text;
                int quantity = Int32.Parse(quantity1);
                //sub id
                string sId = cartItem.SubItems[4].Text;
                int id = Int32.Parse(sId);

                //update product sub status update List
                statusUpdate.Add(id);

                //product id
                string sId1 = cartItem.SubItems[5].Text;
                int id1 = Int32.Parse(sId1);

                int newQuantity = quantity;
                int check = 0;

                //get all stock from stock tracker for particular product and sub;
                mydbAccess.Query = "select * from stock_tracker where product_sub_id = '"+id+ "' AND product_id = '" + id1 + "'  ";
                mydbAccess.Select();

                if (mydbAccess.Status == 1)
                {
                    var result = mydbAccess.Result;

                    //ALREADY AVAILABLE VALUES
                    //sales_date(computed)
                    //product_sub_id(from listview)
                    //product_id(from listview)
                    //sales_price_unit(from listview)
                    //user_id(from class)

                    int i = 0;

                    do
                    {
                        statusCount = statusCount + i;

                        int recordStockLeft = (int)result.Rows[i]["stock_left"];
                        int recordId = (int)result.Rows[i]["id"];

                        //get varying table values
                        int quantityStore = newQuantity;

                        newQuantity = newQuantity - recordStockLeft;

                        string unitCostPrice1 = result.Rows[i]["stock_unit_cost_price"].ToString();
                        decimal unitCostPrice = Decimal.Parse(unitCostPrice1);

                        //check if result is zero or negative i.e the current st record can take care of the order
                        if (newQuantity <= 0)
                        {
                            if (newQuantity == 0)
                            {
                                //ceate Delete current record from stock tracker
                                queryList.Add("delete from stock_tracker where id = '" + recordId + "'");
                            }

                            else
                            {
                                //update stocktacker record reduce stock_left(modulus of newQuantity)
                                queryList.Add("update stock_tracker set stock_left = '" + Math.Abs(newQuantity) + "'where id = '" + recordId + "'");
                            }

                            stockLeft = Math.Abs(newQuantity);

                            newQuantity = quantityStore;
                            //create insert into sales(quantity will be quantity)
                            queryList.Add("insert into sales(sales_date,product_sub_id,product_id,sales_quantity,sales_price_unit,unit_cost_price,user_id)values('" + datetime + "','" + id + "','" + id1 + "','" + newQuantity + "','" + salesPrice + "','" + unitCostPrice + "','" + userId + "')");
                        
                            //proceed to house cleaning i.e set check to 1
                            check = 1;
                        }

                        //if A cross occured
                        else
                        {
                            //create insert into sales table(quantity will be recordStockLeft)
                            queryList.Add("insert into sales(sales_date,product_sub_id,product_id,sales_quantity,sales_price_unit,unit_cost_price,user_id)values('" + datetime + "','" + id + "','" + id1 + "','"+ recordStockLeft + "','" + salesPrice + "','" + unitCostPrice + "','" + userId + "')");

                            //ceate Delete current record from stock tracker
                            queryList.Add("delete from stock_tracker where id = '" + recordId + "'");

                            //continue process i.e leave check = 0 & increment i
                            i++;
                        }

                    }
                    while (check == 0);
                }
            }

            foreach(var id in statusUpdate)
            {
               
                int restockLevel = 0;
                int status = 0;

                //select the product restock level
                mydbAccess.Query = @"select product_sub_restock_level from product_sub where id = '" + id + "'";
                mydbAccess.Select();

                if (mydbAccess.Status == 1)
                {
                    var result = mydbAccess.Result;
                    restockLevel = (int)result.Rows[0]["product_sub_restock_level"];

                    //calculate the stock left 
                    mydbAccess.Query = @"select stock_left from stock_tracker where product_sub_id = '" + id + "'";
                    mydbAccess.Select();

                    if (mydbAccess.Status == 1)
                    {
                        var result1 = mydbAccess.Result;
                        int sum = 0;

                        if (result.Rows.Count > 1)
                        {

                            for (int i = statusCount; i < result1.Rows.Count; i++)
                            {
                                int stockLeftInt = (int)result1.Rows[i]["stock_left"];
                                sum += stockLeftInt;
                            }

                            stockLeft += sum;
                        }

                        //compare with the product sub restock level and set the status accordingly

                        if (stockLeft == 0)
                        {
                            status = 3;
                        }

                        else if (stockLeft > restockLevel)
                        {
                            status = 1;
                        }

                        else
                        {
                            status = 2;
                        }            
                    }
                }
              
                //add query to query list

                queryList.Add("update product_sub set product_sub_status = '"+status+"' where id = '"+id+"'");
            }

            mydbAccess.QueryList = queryList;
            mydbAccess.TransactionOperation();

            if(mydbAccess.Status == 1)
            {
                if (change!= 0)
                {
                    MessageBox.Show("Transaction Successful! Customer is being owed #" + change + " for this transaction");
                }
                else
                {
                    MessageBox.Show("Transaction Successful!");
                }
                
                button4.PerformClick();
            }

            else
            {
                MessageBox.Show(mydbAccess.Message);
            }
           
        }

        //find change owed customer
        private void button2_Click(object sender, EventArgs e)
        {
            string firstname;
            string lastname;
            string amount;
            string phoneNumber = textBox6.Text;
            string status;

            mydbAccess.Query = "SELECT creditor_firstname, creditor_lastname, creditor_amount, creditor_status FROM [creditor] WHERE creditor_contact = '" + phoneNumber + "'";
            mydbAccess.Select();
            var dt = mydbAccess.Result;

            if (mydbAccess.Status == 1 )
            {
                status = dt.Rows[0]["creditor_status"].ToString();

                if (status == "not paid")
                {
                    firstname = dt.Rows[0]["creditor_firstname"].ToString();
                    lastname = dt.Rows[0]["creditor_lastname"].ToString();
                    amount = dt.Rows[0]["creditor_amount"].ToString();

                    label21.Text = firstname;
                    label22.Text = lastname;
                    label23.Text = amount;
                }

                else
                {
                    MessageBox.Show("All outstanding change have been Collected!");
                }
            }

           else
            {
                MessageBox.Show("No Record of customer Found! Ensure PhoneNumber is correct!");
            }

        }

        //Pay Change to customer
        private void button8_Click(object sender, EventArgs e)
        {
            string phoneNumber = textBox6.Text;
            int changeIssued = (int)numericUpDown3.Value;
            Decimal newChange = 0;
            string paidstatus = "paid";

            // Get change currently owed if any
            mydbAccess.Query = "select creditor_amount from creditor where creditor_contact = '" + phoneNumber + "'";
            mydbAccess.Select();

            if (mydbAccess.Status == 1 && radioButton3.Checked == true)
            {
                var result = mydbAccess.Result;
                Decimal oldChange = (Decimal)result.Rows[0]["creditor_amount"];
                newChange = oldChange - changeIssued;
            }

            if (newChange != 0)
            {
                paidstatus = "not paid";
            }
          
           
            mydbAccess.Query = "update [creditor] set creditor_status = '" + paidstatus + "', creditor_amount = '" + newChange + "' where creditor_contact = '" + phoneNumber + "'";
            mydbAccess.Update();

            if (mydbAccess.Status == 1)
            {
                MessageBox.Show("Customer has been paid.");
            }

            button4.PerformClick();
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

         private void comboBox3_SelectedIndexChanged(object sender, EventArgs e)
         {
            
            int id = (int)comboBox3.SelectedValue;
            int id1 = (int)comboBox1.SelectedValue;

            //when a subcategory must be selected
            if (id != -1 && id1 == -1)
            {
                numericUpDown1.Value = 0;
                MessageBox.Show("Select A Product!");
            }
            else
            {
                //Get Product Details(name, sales_unit, current_price, status 
                mydbAccess.Query = @"select r.product_sales_unit, r.product_sub_price from (select ps.id, ps.product_sub_price,
                                   p.product_sales_unit from product_sub ps inner join product p on p.id = ps.product_id) as r 
                                    where r.id = '" + id + "'";

                mydbAccess.Select();

              
                if (mydbAccess.Status == 1)
                {
                    var result = mydbAccess.Result;
                    unit = result.Rows[0]["product_sales_unit"].ToString();
                    string price = result.Rows[0]["product_sub_price"].ToString();

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

                        stockLeft = sum;
                    }

                    label25.Text = stockLeft + " " + unit + " Left";
                    label29.Text = "price:" + " #" + price;
                    label25.Visible = true;
                    label29.Visible = true;

                }
            }
        }
            
        private void comboBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (label25.Visible == true)
            {
                label25.Hide();
            }

            if (label29.Visible == true)
            {
                label29.Hide();
            }


            int id = (int)comboBox2.SelectedValue;
            ComboBox1Update(id);

        }

        //product change to populate product category dropdown
        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (label25.Visible == true)
            {
                label25.Hide();
            }
            if (label29.Visible == true)
            {
                label29.Hide();
            }

            numericUpDown1.Value = 0;

            int id = (int)comboBox1.SelectedValue;
            ComboBox3Update(id);
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
                mydbAccess.Query = "SELECT id, product_full_name FROM product where product_category_id = '" + myId + "'";
            }

            mydbAccess.Select();

            if (mydbAccess.Status == 1 || (mydbAccess.Status == 0 && mydbAccess.Message == null))
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
                                    + myId + "' AND product_sub_price IS NOT NULL";
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

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            if(numericUpDown4.Value != 0)
            {
                if (checkBox1.Checked)
                {
                    if (radioButton2.Checked)
                    {
                        decimal changeDue = Int32.Parse(label13.Text);
                        decimal changeIssued = numericUpDown2.Value;

                        if (changeDue <= changeIssued)
                        {
                            MessageBox.Show("Change issued cannot be greater than or equal change Due");
                            numericUpDown2.Value = 0;
                        }

                        else
                        {
                            change = changeDue - changeIssued;
                            change1 = change.ToString();
                        }
                        //check if customer details is complete
                        if (numericUpDown2.Value < 0 || textBox4.Text == "" || textBox5.Text == "" || textBox7.Text == "")
                        {
                            MessageBox.Show("Fill in Customer Complete Details");
                            checkBox1.Checked = false;
                        }

                        else
                        {
                            button9.Enabled = true;
                        }
                    }

                    else if (radioButton1.Checked)
                    {
                        button9.Enabled = true;
                    }

                    else
                    {
                        MessageBox.Show("Indicate if the complete change is available before processing payment");
                        checkBox1.Checked = false;
                    }

                }

                else
                {
                    button9.Enabled = false;
                }
            }

            else
            {
                checkBox1.Checked = false;
                tabControl1.SelectedTab = tabPage1;
                MessageBox.Show("Click on the Proceed to check out button!");
                button1.Focus();
            }
        }

        private void checkBox2_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox2.Checked)
            {
                if (radioButton3.Checked)
                {
                    if (label23.Text != "")
                    {
                        decimal changeDue = Decimal.Parse(label23.Text);
                        decimal changeIssued = numericUpDown3.Value;

                        if (changeDue <= changeIssued)
                        {
                            MessageBox.Show("Change issued cannot be greater than or equal change Due");
                            numericUpDown3.Value = 0;
                        }

                        else
                        {
                            change = changeDue - changeIssued;
                            change1 = change.ToString();
                        }

                        //check Amount Issued is filled
                        if (numericUpDown3.Value <= 0)
                        {
                            MessageBox.Show("Fill Amount Available");
                            checkBox2.Checked = false;
                        }

                        else
                        {
                            button8.Enabled = true;
                        }
                    }
                    else
                    {
                        MessageBox.Show("You have to search for a customer with mobile Number");
                    }
                }

                else if (radioButton4.Checked)
                {
                    button8.Enabled = true;
                }

                else
                {
                    MessageBox.Show("Indicate if the complete change is available before processing payment");
                    checkBox2.Checked = false;
                }

            }

            else
            {
                button8.Enabled = false;
            }
        }

        private void radioButton4_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox2.Checked == true)
            {
                checkBox2.Checked = false;
            }

            numericUpDown3.Hide();
            label27.Hide();
        }

        private void radioButton3_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox2.Checked == true)
            {
                checkBox2.Checked = false;
            }

            label27.Show();
            numericUpDown3.Show();
        }
    }
}
