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

        int category; //determines report category sales or stock

        static string conn = ConfigurationManager.ConnectionStrings["myConnectionString"].ConnectionString;
        dbAccess mydbAccess = new dbAccess(conn);

        public ReportForm(string name, string role, int id)
        {
            firstname = name;
            userRole = role;
            userId = id;
            InitializeComponent();

            radioButton1.Checked = true;
            radioButton3.Checked = true;

            dateTimePicker1.MaxDate = DateTime.Today;
            dateTimePicker2.MaxDate = DateTime.Today;

        }

        private void button1_Click(object sender, EventArgs e)
        {
            Report(category);
        }

        private void Report(int category)
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
                    //report for particular product
                    if (radioButton2.Checked == true)
                    {
                        {
                            if (startDate.Equals(endDate) && category == 1)
                            {
                                //report for particular product for one day
                                mydbAccess.Query = @"SELECT SUM(sales_quantity * sales_price_unit) as total_sales,SUM(sales_quantity * unit_cost_price) 
                                                   as total_cost from sales WHERE CAST(sales_date AS DATE) = '"
                                                    + startDate + "' AND product_sub_id = '" + id + "' AND product_id = '" + id1 + "'";
                            }

                            else if ((startDate < endDate) && category == 1)
                            {
                                //report for a particular product for a period of time
                                mydbAccess.Query = @"SELECT SUM(sales_quantity * sales_price_unit) as total_sales,SUM(sales_quantity * unit_cost_price)            
                                                  as total_cost from sales WHERE CAST(sales_date AS DATE) >= '"
                                               + startDate + "' AND CAST(sales_date AS DATE) <= '" + endDate + "' AND product_sub_id = '" + id + "' AND product_id = '" + id1 + "'";
                            }

                            else
                            {
                                //stock report for particular product
                                mydbAccess.Query = @"select  sum(r.stock_left*r.product_sub_price) as total_sales,
                                                   sum(r.stock_left*r.stock_unit_cost_price) as total_cost 
                                                   from (select st.* , ps.product_sub_price from stock_tracker st
                                                   inner join product_sub ps on st.product_sub_id = ps.id) as r 
                                                   where product_sub_id = '" + id + "' AND product_id = '" + id1 + "'";
                            }

                        }
                    }

                    //report for all product
                    if (radioButton1.Checked == true)
                    {
                        if (startDate.Equals(endDate) && category == 1)
                        {
                            //report for all product for one day
                            mydbAccess.Query = @"SELECT SUM(sales_quantity * sales_price_unit) as total_sales,
                                               SUM(sales_quantity * unit_cost_price) as total_cost 
                                                from sales WHERE CAST(sales_date AS DATE) = '"
                                               + startDate + "'";
                        }

                        else if ((startDate < endDate) && category == 1)
                        {
                            //report for all product for a period of time
                            mydbAccess.Query = @"SELECT SUM(sales_quantity * sales_price_unit) as total_sales,
                                               SUM(sales_quantity * unit_cost_price) as total_cost
                                              from sales WHERE CAST(sales_date AS DATE) >= '"
                                              + startDate + "' AND CAST(sales_date AS DATE) <= '" + endDate + "'";
                        }

                        else
                        {
                            //stock report for particular product
                            mydbAccess.Query = @"select sum(r.stock_left*r.product_sub_price) as total_sales,
                                              sum(r.stock_left*r.stock_unit_cost_price) as total_cost 
                                               from (select st.* , ps.product_sub_price from stock_tracker st
                                               inner join product_sub ps on st.product_sub_id = ps.id) as r";
                        }
                    }

                    mydbAccess.Select();

                    if (mydbAccess.Status == 1)
                    {
                        var result = mydbAccess.Result;

                        string totalSales = result.Rows[0]["total_sales"].ToString();
                        string totalCost = result.Rows[0]["total_cost"].ToString();

                        if (totalCost != "" && totalSales != "")
                        {
                            if (radioButton1.Checked == true)
                            {
                                if (category == 1)
                                {
                                    MessageBox.Show("Showing Sales Report of ALL Products for selected Period!");
                                }

                                if (category == 2)
                                {
                                    MessageBox.Show("Showing Stock Report of ALL Products");
                                }
                            }

                            else
                            {
                                if (category == 1)
                                {
                                    MessageBox.Show("Showing Sales Report of " + comboBox1.Text + ": " + comboBox3.Text + " for selected Period!");
                                }

                                else
                                {
                                    MessageBox.Show("Showing Stock Report of " + comboBox1.Text + ": " + comboBox3.Text);
                                }
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

                            if(radioButton2.Checked == true && radioButton4.Checked == true)
                            {
                                mydbAccess.Query = @"select sum(stock_left) as total_stock_left from stock_tracker 
                                                   where product_sub_id = '"+id+"'";
                                mydbAccess.Select();

                                if (mydbAccess.Status == 1)
                                {
                                    var result2 = mydbAccess.Result;
                                    label14.Text = result2.Rows[0]["total_stock_left"].ToString();
                                    label13.Show();
                                    label14.Show();
                                }                    
                            }
                        }

                        else
                        {
                            if (category == 1)
                            {
                                MessageBox.Show("No sales record for specified Period");
                            }

                            if (category == 2)
                            {
                                MessageBox.Show("Product out of stock");
                            }

                            if (label10.Visible == true && label11.Visible == true && label11.Visible == true)
                            {
                                label10.Hide();
                                label11.Hide();
                                label12.Hide();
                                label13.Hide();
                                label14.Hide();
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
            if (label10.Visible == true)
            {
                int id = (int)comboBox3.SelectedValue;

                DateTime startDate = dateTimePicker1.Value;
                DateTime endDate = dateTimePicker2.Value;

                string message = null;
                string message2 = null;

                string user = Environment.UserName;
                string path;
                string docName;

                if (category == 1)
                {
                    path = @"C:\Users\" + user + @"\Documents\STOREMAN_REPORTS\SALES";
                }

                else
                {
                    path = @"C:\Users\" + user + @"\Documents\STOREMAN_REPORTS\STOCK";
                }
               

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

                    string totalSales = label10.Text;
                    string totalCost = label11.Text;
                    string Profit = label12.Text;

                    var doc1 = new Document();

                    string dateTime = DateTime.Now.ToString("dd-MMM-yyyy_hh-mm-ss");
                    if(category == 1)
                    {
                        docName = "/Sales_Report_" + dateTime + ".pdf";
                    }

                    else
                    {
                        docName = "/Stock_Report_" + dateTime + ".pdf";
                    }

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

                        Phrase p3 = new Phrase();

                        if (category == 1)
                        {
                            Chunk c2 = new Chunk("SALES REPORT FROM ", georgia1);
                            Chunk c3 = new Chunk(startDate.ToString("dd-MMM-yyyy").ToUpper(), georgia2);
                            Chunk c4 = new Chunk(" to ", georgia1);
                            Chunk c5 = new Chunk(endDate.ToString("dd-MMM-yyyy").ToUpper(), georgia2);

                            p3.Add(c2);
                            p3.Add(c3);
                            p3.Add(c4);
                            p3.Add(c5);
                        }

                        else
                        {
                            Chunk c2 = new Chunk("CURRENT STOCK REPORT", georgia2);
                            p3.Add(c2);
                        }

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
                        int columns;
                        float[] widths;

                        if (category == 1)
                        {
                            columns = 9;
                            
                            //relative col widths in proportions - 1/3 and 2/3
                            widths = new float[] { 3f, 7.5f, 13f, 13f, 4f, 6f, 6f, 6.5f, 6.5f };
                        }

                        else
                        {
                            columns = 8;
                            //relative col widths in proportions - 1/3 and 2/3
                            widths = new float[] { 3f, 13f, 13f, 4f, 6f, 6f, 6.5f, 6.5f };
                        }

                        //Table of record 
                        PdfPTable table = new PdfPTable(columns);

                        //actual width of table in points
                        //units in pts 1inch = 72pts printing terms
                        table.TotalWidth = 530f;

                        //fix the absolute width of the table
                        table.LockedWidth = true;

                      
                        table.SetWidths(widths);
                        table.HorizontalAlignment = 0;

                        //leave a gap before and after the table
                        table.SpacingBefore = 20f;
                        table.SpacingAfter = 30f;


                        PdfPCell cell = new PdfPCell();
                        cell.Colspan = 9;
                        cell.Border = 0;
                        cell.HorizontalAlignment = 1;

                        if (category == 1)
                        {
                            table.AddCell("S/N");
                            table.AddCell("DATE");
                            table.AddCell("PRODUCT");
                            table.AddCell("SUBCATEGORY");
                            table.AddCell("QTY");
                            table.AddCell("CP");
                            table.AddCell("SP ");
                            table.AddCell("TOTAL COST");
                            table.AddCell("TOTAL SALES");
                        }

                        else
                        {
                            table.AddCell("S/N");
                            table.AddCell("PRODUCT");
                            table.AddCell("SUBCATEGORY");
                            table.AddCell("QTY");
                            table.AddCell("SC");
                            table.AddCell("SP ");
                            table.AddCell("TOTAL COST");
                            table.AddCell("TOTAL WORTH");
                        }
                        

                        if (radioButton1.Checked == true)
                        {
                            if (category == 1)
                            {
                                mydbAccess.Query = @"select r.*,r.sales_quantity*r.unit_cost_price as total_cost,
                                               r.sales_quantity* product_sub_price as total_sales
                                               from(select ps.id, CAST(s.sales_date AS DATE) as sales_date, 
                                               p.product_full_name,ps.product_sub_name, s.sales_quantity,
                                               s.unit_cost_price, ps.product_sub_price from sales s
                                               inner join product_sub ps on ps.id = s.product_sub_id
                                               inner join product p on p.id = ps.product_id) as r where 
                                               r.sales_date >= '"
                                             + startDate + "' AND r.sales_date <= '" + endDate
                                             + "' order by r.product_sub_name asc";
                            }

                            else
                            {
                                mydbAccess.Query = @"select r.id, r.product_full_name, r.product_sub_name,
                                                   r.stock_left,r.stock_unit_cost_price, r.product_sub_price, 
                                                   r.stock_left*r.stock_unit_cost_price as total_cost,
                                                   r.stock_left * r.product_sub_price as total_sales
                                                   from (select ps.id, st.product_sub_id, st.product_id, 
                                                   p.product_full_name, ps.product_sub_name, st.stock_left, 
                                                   st.stock_unit_cost_price, ps.product_sub_price
                                                   from stock_tracker st
                                                   inner join product_sub ps on st.product_sub_id = ps.id
                                                   inner join product p on st.product_id = p.id ) as r 
                                                   order by r.product_sub_name asc";
                            }
                          
                            mydbAccess.Select();
                        }

                        else if (radioButton2.Checked == true)
                        {
                            if(category == 1)
                            {
                                mydbAccess.Query = @"select r.*,r.sales_quantity*r.unit_cost_price as total_cost,
                                               r.sales_quantity* product_sub_price as total_sales
                                               from(select ps.id, CAST(s.sales_date AS DATE) as sales_date, 
                                               p.product_full_name,ps.product_sub_name, s.sales_quantity,
                                               s.unit_cost_price, ps.product_sub_price from sales s
                                               inner join product_sub ps on ps.id = s.product_sub_id
                                               inner join product p on p.id = ps.product_id) as r where 
                                               r.sales_date >= '"
                                            + startDate + "' AND r.sales_date <= '" + endDate
                                            + "' AND r.id = '" + id + "'order by r.sales_date asc";
                            }
                            else
                            {
                                mydbAccess.Query = @"select r.id, r.product_full_name, r.product_sub_name,
                                                   r.stock_left,r.stock_unit_cost_price, r.product_sub_price, 
                                                   r.stock_left*r.stock_unit_cost_price as total_cost,
                                                   r.stock_left * r.product_sub_price as total_sales
                                                   from (select ps.id, st.product_sub_id, st.product_id, 
                                                   p.product_full_name, ps.product_sub_name, st.stock_left, 
                                                   st.stock_unit_cost_price, ps.product_sub_price
                                                   from stock_tracker st
                                                   inner join product_sub ps on st.product_sub_id = ps.id
                                                   inner join product p on st.product_id = p.id ) as r 
                                                   where r.id = '"+id+"'";
                            }
                          
                            mydbAccess.Select();
                        }

                       
                        if (mydbAccess.Status == 1)
                        {
                            var result = mydbAccess.Result;

                            if (category == 1)
                            {
                                for (int i = 0; i < result.Rows.Count; i++)
                                {
                                    string sn = (i + 1).ToString();

                                    table.AddCell(sn);
                                    table.AddCell(result.Rows[i]["sales_date"].ToString().Replace("12:00:00 AM", ""));
                                    table.AddCell(result.Rows[i]["product_full_name"].ToString());
                                    table.AddCell(result.Rows[i]["product_sub_name"].ToString());
                                    table.AddCell(result.Rows[i]["sales_quantity"].ToString());
                                    table.AddCell(result.Rows[i]["unit_cost_price"].ToString());
                                    table.AddCell(result.Rows[i]["product_sub_price"].ToString());
                                    table.AddCell(result.Rows[i]["total_cost"].ToString());
                                    table.AddCell(result.Rows[i]["total_sales"].ToString());
                                }

                                table.AddCell("");
                                table.AddCell("");
                                table.AddCell(new Phrase("PROFIT", georgia2));
                                table.AddCell(new Phrase("#" + label12.Text.Replace(".00", ""), georgia2));
                                table.AddCell("");
                                table.AddCell(new Phrase("GRAND", georgia2));
                                table.AddCell(new Phrase("TOTAL", georgia2));
                                table.AddCell(new Phrase("#" + label11.Text.Replace(".00", ""), georgia2));
                                table.AddCell(new Phrase("#" + label10.Text.Replace(".00", ""), georgia2));
                            }

                            else
                            {
                                for (int i = 0; i < result.Rows.Count; i++)
                                {
                                    string sn = (i + 1).ToString();

                                    table.AddCell(sn);
                                    table.AddCell(result.Rows[i]["product_full_name"].ToString());
                                    table.AddCell(result.Rows[i]["product_sub_name"].ToString());
                                    table.AddCell(result.Rows[i]["stock_left"].ToString());
                                    table.AddCell(result.Rows[i]["stock_unit_cost_price"].ToString());
                                    table.AddCell(result.Rows[i]["product_sub_price"].ToString());
                                    table.AddCell(result.Rows[i]["total_cost"].ToString());
                                    table.AddCell(result.Rows[i]["total_sales"].ToString());
                                }

                                table.AddCell("");
                                table.AddCell(new Phrase("PROFIT", georgia2));
                                table.AddCell(new Phrase("#" + label12.Text.Replace(".00", ""), georgia2));

                                if(radioButton2.Checked == true && radioButton4.Checked == true)
                                {
                                    table.AddCell(new Phrase(label14.Text, georgia2));
                                }
                                else
                                {
                                    table.AddCell("");
                                }
                               
                                table.AddCell(new Phrase("GRAND", georgia2));
                                table.AddCell(new Phrase("TOTAL", georgia2));
                                table.AddCell(new Phrase("#" + label11.Text.Replace(".00", ""), georgia2));
                                table.AddCell(new Phrase("#" + label10.Text.Replace(".00", ""), georgia2));
                            }
                           
                        }

                        

                        doc1.Add(table);
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
            else
            {
                MessageBox.Show("View Report before Printing Details!");
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
                label13.Hide();
                label14.Hide();
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
                label13.Hide();
                label14.Hide();
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
                label13.Hide();
                label14.Hide();
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

        private void comboBox3_SelectedIndexChanged(object sender, EventArgs e)
        {
            label10.Hide();
            label11.Hide();
            label12.Hide();
            label13.Hide();
            label14.Hide();
        }

        private void dateTimePicker1_ValueChanged(object sender, EventArgs e)
        {
            label10.Hide();
            label11.Hide();
            label12.Hide();
            label13.Hide();
            label14.Hide();
        }

        private void dateTimePicker2_ValueChanged(object sender, EventArgs e)
        {
            label10.Hide();
            label11.Hide();
            label12.Hide();
            label13.Hide();
            label14.Hide();
        }

        private void radioButton3_CheckedChanged(object sender, EventArgs e)
        {
            category = 1;

            label1.Text = "Sales Report";

            label2.Text = "Total Sales";
            label4.Text = "Total Cost";
            label5.Text = "Profit";

            dateTimePicker1.Enabled = true;
            dateTimePicker2.Enabled = true;

            label10.Hide();
            label11.Hide();
            label12.Hide();
            label13.Hide();
            label14.Hide();
        }

        private void radioButton4_CheckedChanged(object sender, EventArgs e)
        {
            category = 2;

            label1.Text = "Stock Report";

            label2.Text = "Stock Worth";
            label4.Text = "Stock Cost";
            label5.Text = "Profit";

            dateTimePicker1.Value = DateTime.Today;
            dateTimePicker2.Value = DateTime.Today;

            dateTimePicker1.Enabled = false;
            dateTimePicker2.Enabled = false;

            label10.Hide();
            label11.Hide();
            label12.Hide();
            label13.Hide();
            label14.Hide();
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

        private void radioButton2_CheckedChanged(object sender, EventArgs e)
        {
            comboBox1.Enabled = true;
            comboBox2.Enabled = true;
            comboBox3.Enabled = true;

            label10.Hide();
            label11.Hide();
            label12.Hide();
            label13.Hide();
            label14.Hide();
        }
    }
}
