using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Configuration;

namespace storeman
{
    public partial class ResetForm : Form
    {
        public string userRole;
        public string firstname;
        public int userId;

        static string conn = ConfigurationManager.ConnectionStrings["myConnectionString"].ConnectionString;
        dbAccess mydbAccess = new dbAccess(conn);

        public ResetForm(string name, string role, int id)
        {
            userRole = role;
            firstname = name;
            userId = id;

            InitializeComponent();

            if (userRole == "Admin")
            {
                textBox5.Visible = false;
                button2.Text = "Home";
            }

            comboBoxUpdate();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            int id = (int)comboBox1.SelectedValue;
            string check = textBox5.Text;
            //key to add store managers.
            string secretKey = "mysecretkey";

            if (check == secretKey || userRole == "Admin")
            {
                Crypto mycrypto = new Crypto();
                
                string password = textBox4.Text;

                mycrypto.Input = password;
                //encrypt password
                mycrypto.Encode();
                string epassword = mycrypto.Output;
               
                string status = "Inactive";
   
                mydbAccess.Query = "update [user] set user_password = '"+epassword+"', user_status = '"+status+"' where id ='"+id+"'";
                mydbAccess.Update();
                
                if (mydbAccess.Status == 1)
                {
                    MessageBox.Show("Account Reset Successful");
                }

                else
                {
                    if (mydbAccess.Message != null)
                    {
                        MessageBox.Show(mydbAccess.Message);
                    }
                    else
                    {
                        MessageBox.Show("Account Reset Failed!");
                    }        

                }
            }

            else
            {
                if (userRole == "Admin")
                {
                    MessageBox.Show("Something went wrong");
                    DashBoardForm mydashboardform = new DashBoardForm(firstname, userRole, userId);
                    this.Hide();
                    mydashboardform.Show();
                }

                else
                {
                    MessageBox.Show("Unauthorized");
                    button2.PerformClick();
                }            
            }

          
                button2.PerformClick();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (userRole == "Admin")
            {
                DashBoardForm mydashboardform = new DashBoardForm(firstname, userRole, userId);
                this.Hide();
                mydashboardform.Show();
            }

            else
            {
                LoginForm myloginform = new LoginForm();
                this.Hide();
                myloginform.Show();
            }
           
        }

     private void comboBoxUpdate()
        {
            if (userRole == "Admin")
            {
                string user = "user";
                mydbAccess.Query = "select id, user_username from [user] where user_role = '" + user + "'";
                mydbAccess.Select();

                if (mydbAccess.Status == 1)
                {
                    comboBox1.ValueMember = "id";
                    comboBox1.DisplayMember = "user_username";
                    comboBox1.DataSource = mydbAccess.Result;
                }
            }

            else
            {
                string Admin = "Admin";
                mydbAccess.Query = "select id, user_username from [user] where user_role = '" + Admin + "'";
                mydbAccess.Select();

                if (mydbAccess.Status == 1)
                {
                    comboBox1.ValueMember = "id";
                    comboBox1.DisplayMember = "user_username";
                    comboBox1.DataSource = mydbAccess.Result;
                }

            }

            
        }  
    }

}
