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
    public partial class SignUpForm : Form
    {
        public string userRole;
        public string firstname;
        public int userId;

        static string conn = ConfigurationManager.ConnectionStrings["myConnectionString"].ConnectionString;
        dbAccess mydbAccess = new dbAccess(conn);

        public SignUpForm(string name, string role, int id)
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
        }

        private void button3_Click(object sender, EventArgs e)
        {
            string check = textBox5.Text;
            //key to add store managers.
            string secretKey = "mysecretkey";

            if (check == secretKey || userRole == "Admin")
            {
                Crypto mycrypto = new Crypto();
               
                string firstname = textBox1.Text;
                string lastname = textBox2.Text;
                string username = textBox3.Text;
                string password = textBox4.Text;
                string role;

                mycrypto.Input = password;
                //encrypt password
                mycrypto.Encode();
               

                string epassword = mycrypto.Output;
                if (userRole == "Admin")
                {
                    role = "user";
                }
                else
                {
                    role = "Admin";
                }
               
                string status = "Inactive";
   
                mydbAccess.Query = "insert into [user] (user_firstname, user_lastname,user_username,user_password,user_role,user_status) values('"+firstname+"','"+lastname+"','"+username+"','"+epassword+ "','" + role + "','" + status + "')";
                mydbAccess.Insert();
                
                if (mydbAccess.Status == 1)
                {
                    MessageBox.Show("Account Created Successfully");
                }

                else
                {
                    if (mydbAccess.Message != null)
                    {
                        MessageBox.Show(mydbAccess.Message);
                    }
                    else
                    {
                        MessageBox.Show("Account Creation Failed!");
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

       
    }

}
