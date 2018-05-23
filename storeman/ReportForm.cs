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
using iTextSharp.text;
using iTextSharp.text.pdf;
using System.IO;

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

            ComboBox2Update();
            ComboBox1Update(-1);
            ComboBox3Update(-1);

            dateTimePicker1.MaxDate = DateTime.Today;
            dateTimePicker2.MaxDate = DateTime.Today;

            label10.Hide();
            label11.Hide();
            label12.Hide();
        }

     

        private void button5_Click(object sender, EventArgs e)
        {
            DialogResult dialogResult = MessageBox.Show("YES to View A Particular product sales report for specified Period. NO to view all product sales report for specified period.", "Report Scope", MessageBoxButtons.YesNo);

            if (dialogResult == DialogResult.No)
            {
                dbAccess mydbAccess2 = new dbAccess(conn);
                //obtain start and end date
                DateTime startDate = dateTimePicker1.Value;
                DateTime endDate = dateTimePicker2.Value;

                if (startDate > endDate)
                {
                    MessageBox.Show("FROM date must be earlier than TO Date!");

                }

                else
                {
                    
                    if (startDate.Equals(endDate))
                    {
                        mydbAccess.Query = @"SELECT SUM(sales_quantity * sales_price_unit) as total_sales from sales WHERE CAST(sales_date AS DATE) = '"
                                      + startDate + "'";

                        mydbAccess2.Query = @"SELECT SUM(sales_quantity * unit_cost_price) as total_cost from sales WHERE CAST(sales_date AS DATE) = '"
                                      + startDate + "'";
                    }

                    else
                    {
                        mydbAccess.Query = @"SELECT SUM(sales_quantity * sales_price_unit) as total_sales from sales WHERE CAST(sales_date AS DATE) >= '"
                                 + startDate + "' AND CAST(sales_date AS DATE) <= '" + endDate + "'";

                        mydbAccess2.Query = @"SELECT SUM(sales_quantity * unit_cost_price) as total_cost from sales WHERE CAST(sales_date AS DATE) >= '"
                                 + startDate + "' AND CAST(sales_date AS DATE) <= '" + endDate + "'";
                    }

                    mydbAccess.Select();
                    mydbAccess2.Select();

                    if (mydbAccess.Status == 1 && mydbAccess2.Status == 1)
                    {
                        var result = mydbAccess.Result;
                        var result2 = mydbAccess2.Result;

                        string totalSales = result.Rows[0]["total_sales"].ToString();
                        string totalCost = result2.Rows[0]["total_cost"].ToString();

                        if (totalCost != "" && totalSales != "")
                        {
                            MessageBox.Show("Showing ALL Transactions for selected Period!");

                            float totalSales1 = float.Parse(totalSales);
                            float totalCost1 = float.Parse(totalCost);

                            float profit = totalSales1 - totalCost1;

                            label10.Text = totalSales1.ToString("0.00");
                            label11.Text = totalCost1.ToString("0.00");
                            label12.Text = profit.ToString("0.00");

                            label10.Show();
                            label11.Show();
                            label12.Show();

                            ComboBox2Update();
                            ComboBox1Update(-1);
                            ComboBox3Update(-1);

                        }

                        else
                        {
                            MessageBox.Show("No sales record for specified Period");

                            if (label10.Visible == true && label11.Visible == true && label11.Visible == true)
                            {
                                label10.Hide();
                                label11.Hide();
                                label12.Hide();
                            }

                            ComboBox2Update();
                            ComboBox1Update(-1);
                            ComboBox3Update(-1);
                        }
                    }
                    else
                    {
                        MessageBox.Show("No sales record for specified Period");

                        if (label10.Visible == true && label11.Visible == true && label11.Visible == true)
                        {
                            label10.Hide();
                            label11.Hide();
                            label12.Hide();
                        }

                        ComboBox2Update();
                        ComboBox1Update(-1);
                        ComboBox3Update(-1);
                    }

                }
            }

            else if (dialogResult == DialogResult.Yes)
            {
                int id = (int)comboBox1.SelectedValue;
                int id1 = (int)comboBox3.SelectedValue;

                if (id == -1 || id1 == -1)
                {
                    MessageBox.Show("Product and sub Category must be Selected!");
                }
                else
                {

                    dbAccess mydbAccess2 = new dbAccess(conn);
                    //obtain start and end date
                    DateTime startDate = dateTimePicker1.Value;
                    DateTime endDate = dateTimePicker2.Value;

                    if (startDate > endDate)
                    {
                        MessageBox.Show("FROM date must be earlier than TO Date!");

                    }

                    else
                    {
                        
                        if (startDate.Equals(endDate))
                        {
                            mydbAccess.Query = @"SELECT SUM(sales_quantity * sales_price_unit) as total_sales from sales WHERE CAST(sales_date AS DATE) = '"
                                          + startDate + "' AND product_sub_id = '"+id1+"' AND product_id = '"+id+"'";

                            mydbAccess2.Query = @"SELECT SUM(sales_quantity * unit_cost_price) as total_cost from sales WHERE CAST(sales_date AS DATE) = '"
                                          + startDate + "' AND product_sub_id = '" + id1 + "' AND product_id = '" + id + "'";
                        }

                        else
                        {
                            mydbAccess.Query = @"SELECT SUM(sales_quantity * sales_price_unit) as total_sales from sales WHERE CAST(sales_date AS DATE) >= '"
                                     + startDate + "' AND CAST(sales_date AS DATE) <= '" + endDate + "' AND product_sub_id = '" + id1 + "' AND product_id = '" + id + "'";

                            mydbAccess2.Query = @"SELECT SUM(sales_quantity * unit_cost_price) as total_cost from sales WHERE CAST(sales_date AS DATE) >= '"
                                     + startDate + "' AND CAST(sales_date AS DATE) <= '" + endDate + "' AND product_sub_id = '" + id1 + "' AND product_id = '" + id + "'";
                        }

                        mydbAccess.Select();
                        mydbAccess2.Select();

                        if (mydbAccess.Status == 1 && mydbAccess2.Status == 1)
                        {
                            var result = mydbAccess.Result;
                            var result2 = mydbAccess2.Result;

                            string totalSales = result.Rows[0]["total_sales"].ToString();
                            string totalCost = result2.Rows[0]["total_cost"].ToString();

                            if (totalCost != "" && totalSales != "")
                            {
                                MessageBox.Show("Showing Sales Report for " + comboBox1.Text + ": " + comboBox3.Text + " for selected Period!");

                                float totalSales1 = float.Parse(totalSales);
                                float totalCost1 = float.Parse(totalCost);

                                float profit = totalSales1 - totalCost1;

                                label10.Text = totalSales1.ToString("0.00");
                                label11.Text = totalCost1.ToString("0.00");
                                label12.Text = profit.ToString("0.00");

                                label10.Show();
                                label11.Show();
                                label12.Show();
                            }

                            else
                            {
                                MessageBox.Show("No sales record for specified Period");

                                if(label10.Visible == true && label11.Visible == true && label11.Visible == true)
                                {
                                    label10.Hide();
                                    label11.Hide();
                                    label12.Hide();
                                }
                               

                                ComboBox2Update();
                                ComboBox1Update(-1);
                                ComboBox3Update(-1);
                            }
                        }

                        else
                        {
                            MessageBox.Show("No sales record for specified Period");

                            if (label10.Visible == true && label11.Visible == true && label11.Visible == true)
                            {
                                label10.Hide();
                                label11.Hide();
                                label12.Hide();
                            }

                            ComboBox2Update();
                            ComboBox1Update(-1);
                            ComboBox3Update(-1);
                        }

                    }

                }
            }
         
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
                                    + myId + "'";
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

        private void button2_Click(object sender, EventArgs e)
        {
            string storeName = ConfigurationManager.AppSettings["name"];
            string address = ConfigurationManager.AppSettings["address"];
            string contact = ConfigurationManager.AppSettings["contact"];

            string startDate = dateTimePicker1.Value.ToString("dd-MMM-yyyy");
            string endDate = dateTimePicker2.Value.ToString("dd-MMM-yyyy");

            string totalSales = label10.Text;
            string totalCost = label11.Text;
            string Profit = label12.Text;

            var doc1 = new Document();

            //has to be made variable could be placed in the configuration file
            string path = "C:/Users/TOBI/Documents";
            PdfWriter.GetInstance(doc1, new FileStream(path + "/Report.pdf", FileMode.Create));

            //manipulate document
            doc1.Open();

            doc1.Add(new Paragraph(storeName.ToUpper()));
            doc1.Add(new Paragraph(address.ToUpper()));
            doc1.Add(new Paragraph(contact.ToUpper()));

            doc1.Add(new Paragraph(""));

            doc1.Add(new Paragraph("SALES REPORT FROM " +startDate.ToUpper()+ " - " + endDate.ToUpper() ));

            doc1.Add(new Paragraph(""));

            doc1.Add(new Paragraph("TOTAL SALES: " + totalSales));
            doc1.Add(new Paragraph(" TOTAL COST: " + totalCost));
            doc1.Add(new Paragraph("     PROFIT: " + Profit));

            doc1.Add(new Paragraph(""));

            doc1.Close();

            MessageBox.Show("Report Pdf saved to: " + path );
        }
    }
}
