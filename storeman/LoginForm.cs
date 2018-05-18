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
    public partial class LoginForm : Form
    {
        public string firstname;
        public string role;
        public int id;

        //decrypt Connection string password
        Crypto mycrypto = new Crypto();
        static string conn = ConfigurationManager.ConnectionStrings["myConnectionString"].ConnectionString;
        dbAccess mydbAccess = new dbAccess(conn);

        public LoginForm()
        {
            InitializeComponent();
        }

        private void button3_Click(object sender, EventArgs e)
        {
           
            string username = textBox1.Text;
            string password = textBox2.Text;

            if (username == "ADMIN" && password == "new_store")
            {
                SignUpForm mysignupform = new SignUpForm(username,"super_admin",id);
                this.Hide();
                mysignupform.Show();
            }

            else if (username == "ADMIN" && password == "reset_admin")
            {
                ResetForm myresetform = new ResetForm(username, "super_admin", id);
                this.Hide();
                myresetform.Show();
            }

            else
            {
                //encrypt password
                mycrypto.Input = password;

                mycrypto.Encode();

                string epassword = mycrypto.Output;

                mydbAccess.Query = "select id,user_firstname,user_username,user_password,user_role,user_status from [user] where user_username='" + username + "'and user_password='" + epassword + "'";
                mydbAccess.Select();
                if (mydbAccess.Status == 1)
                {
                    var details = mydbAccess.Result;
                    string status = details.Rows[0]["user_status"].ToString();

                    if (status == "Inactive")
                    {

                        ChangePasswordForm mychangepasswordform = new ChangePasswordForm(username, password, epassword);
                        this.Hide();
                        mychangepasswordform.Show();
                    }
                    else
                    {
                        firstname = details.Rows[0]["user_firstname"].ToString();
                        role = details.Rows[0]["user_role"].ToString();
                        string id1 = details.Rows[0]["id"].ToString();
                        id = Int32.Parse(id1);

                        DashBoardForm mydashboardform = new DashBoardForm(firstname, role, id);
                        this.Hide();
                        mydashboardform.Show();
                    }

                }

                else
                {
                    if (mydbAccess.Message == null)
                    {
                        textBox1.Clear();
                        textBox2.Clear();
                        MessageBox.Show("Login Unsuccessful! Incorrect Username or Password");

                    }

                    if (mydbAccess.Message != null)
                    {
                        MessageBox.Show(mydbAccess.Message);
                    }

                }
            }
               
        }
           
    }

}
