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

            radioButton1.Checked = true;

            dateTimePicker1.MaxDate = DateTime.Today;
            dateTimePicker2.MaxDate = DateTime.Today;

        }



        private void button5_Click(object sender, EventArgs e)
        {
            //obtain start and end date
            DateTime startDate = dateTimePicker1.Value;
            DateTime endDate = dateTimePicker2.Value;

            int id = (int)comboBox3.SelectedValue;
            int id1 = (int)comboBox1.SelectedValue;

            dbAccess mydbAccess2 = new dbAccess(conn);

            if (radioButton2.Checked == true && (id1 == -1 || id == -1))
            {
                MessageBox.Show("Product and Product Sub Category must be selected!");
            }

            else
            {
                if (startDate > endDate)
                {
                    MessageBox.Show("FROM date must be earlier than TO Date!");
                }

                else
                {
                    if (radioButton2.Checked == true)
                    {

                        if (startDate.Equals(endDate))
                        {
                            mydbAccess.Query = @"SELECT SUM(sales_quantity * sales_price_unit) as total_sales from sales WHERE CAST(sales_date AS DATE) = '"
                                          + startDate + "' AND product_sub_id = '" + id + "' AND product_id = '" + id1 + "'";

                            mydbAccess2.Query = @"SELECT SUM(sales_quantity * unit_cost_price) as total_cost from sales WHERE CAST(sales_date AS DATE) = '"
                                          + startDate + "' AND product_sub_id = '" + id + "' AND product_id = '" + id1 + "'";
                        }

                        else
                        {
                            mydbAccess.Query = @"SELECT SUM(sales_quantity * sales_price_unit) as total_sales from sales WHERE CAST(sales_date AS DATE) >= '"
                                     + startDate + "' AND CAST(sales_date AS DATE) <= '" + endDate + "' AND product_sub_id = '" + id + "' AND product_id = '" + id1 + "'";

                            mydbAccess2.Query = @"SELECT SUM(sales_quantity * unit_cost_price) as total_cost from sales WHERE CAST(sales_date AS DATE) >= '"
                                     + startDate + "' AND CAST(sales_date AS DATE) <= '" + endDate + "' AND product_sub_id = '" + id + "' AND product_id = '" + id1 + "'";
                        }

                    }

                    if (radioButton1.Checked == true)
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
                            if (radioButton1.Checked == true)
                            {
                                MessageBox.Show("Showing Sales Report of ALL Products for selected Period!");
                            }

                            else
                            {
                                MessageBox.Show("Showing Sales Report of " + comboBox1.Text + ": " + comboBox3.Text + " for selected Period!");
                            }

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

                            if (label10.Visible == true && label11.Visible == true && label11.Visible == true)
                            {
                                label10.Hide();
                                label11.Hide();
                                label12.Hide();
                            }

                        }
                    }

                    else
                    {
                        MessageBox.Show(mydbAccess.Message + mydbAccess2.Message);
                    }

                }
            }
        }


        private void button2_Click(object sender, EventArgs e)
        {
            string message = null;
            string message2 = null;

            string user = Environment.UserName;

            string path = @"C:\Users\" + user + @"\Documents\STOREMAN_REPORTS";

            //check if path exist
            bool exist = Directory.Exists(path);

            if (!exist)
            {
                try
                {
                    // Try to create the directory.
                    DirectoryInfo di = Directory.CreateDirectory(path);
                }
                catch (Exception ex)
                {
                    message = ex.Message;
                }
            }

            if (message == null)
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

                string dateTime = DateTime.Now.ToString("dd-MMM-yyyy_hh-mm-ss");
                string docName = "/Report_" + dateTime + ".pdf";

                try
                {
                    PdfWriter.GetInstance(doc1, new FileStream(path + docName, FileMode.Create));

                    //manipulate document
                    //header section

                    doc1.Open();

                    iTextSharp.text.Font georgia = FontFactory.GetFont("georgia", 16f, iTextSharp.text.Font.BOLD);
                    iTextSharp.text.Font georgia1 = FontFactory.GetFont("georgia", 12f, iTextSharp.text.Font.NORMAL);
                    iTextSharp.text.Font georgia2 = FontFactory.GetFont("georgia", 12f, iTextSharp.text.Font.BOLD);
                    iTextSharp.text.Font georgia3 = FontFactory.GetFont("georgia", 12f, iTextSharp.text.Font.ITALIC);

                    Chunk c1 = new Chunk(storeName.ToUpper(), georgia);
                    Phrase p1 = new Phrase(c1);

                    Chunk c6 = new Chunk(address + ", " + contact, georgia3);
                    Phrase p2 = new Phrase(c6);

                    Chunk c2 = new Chunk("SALES REPORT FROM ", georgia1);
                    Chunk c3 = new Chunk(startDate.ToUpper(), georgia2);
                    Chunk c4 = new Chunk(" to ", georgia1);
                    Chunk c5 = new Chunk(endDate.ToUpper(), georgia2);
                    Phrase p3 = new Phrase();

                    p3.Add(c2);
                    p3.Add(c3);
                    p3.Add(c4);
                    p3.Add(c5);

                    Paragraph pr1 = new Paragraph();
                    Paragraph pr2 = new Paragraph();
                    Paragraph pr3 = new Paragraph();

                    pr1.Add(p1);
                    pr2.Add(p2);
                    pr3.Add(p3);

                    pr2.SpacingAfter = 7f;
                    pr3.SpacingAfter = 3f;

                    pr1.Alignment = Element.ALIGN_CENTER;
                    pr2.Alignment = Element.ALIGN_CENTER;
                    pr3.Alignment = Element.ALIGN_CENTER;

                    doc1.Add(pr1);
                    doc1.Add(pr2);
                    doc1.Add(pr3);

                    //header section

                    //Table of record 

                    doc1.Close();

                    MessageBox.Show("Report Pdf saved to: " + path);
                }

                catch (Exception ex)
                {
                    message2 = ex.Message;
                }

            }

            else
            {
                MessageBox.Show(message);
            }

            if (message2 != null)
            {
                MessageBox.Show(message2);
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

                label10.Hide();
                label11.Hide();
                label12.Hide();
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

                label10.Hide();
                label11.Hide();
                label12.Hide();
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

                label10.Hide();
                label11.Hide();
                label12.Hide();
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

        private void radioButton2_CheckedChanged(object sender, EventArgs e)
        {
            comboBox1.Enabled = true;
            comboBox2.Enabled = true;
            comboBox3.Enabled = true;

            label10.Hide();
            label11.Hide();
            label12.Hide();
        }

        private void radioButton1_CheckedChanged(object sender, EventArgs e)
        {
            ComboBox2Update();
            ComboBox1Update(-1);
            ComboBox3Update(-1);

            comboBox1.Enabled = false;
            comboBox2.Enabled = false;
            comboBox3.Enabled = false;
        }

        private void comboBox3_SelectedIndexChanged(object sender, EventArgs e)
        {
            label10.Hide();
            label11.Hide();
            label12.Hide();
        }

        private void dateTimePicker1_ValueChanged(object sender, EventArgs e)
        {
            label10.Hide();
            label11.Hide();
            label12.Hide();
        }

        private void dateTimePicker2_ValueChanged(object sender, EventArgs e)
        {
            label10.Hide();
            label11.Hide();
            label12.Hide();
        }
    }
}
