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
    public partial class ChangePasswordForm : Form
    {
        public string firstname;
        static string conn = ConfigurationManager.ConnectionStrings["myConnectionString"].ConnectionString;
        dbAccess mydbAccess = new dbAccess(conn);
        string user;
        string pass;
        string epass;

        public ChangePasswordForm(string username, string old_password, string old_epassword)
        {
            InitializeComponent();
            textBox1.Text = old_password;
            user = username;
            pass = old_password;
            epass = old_epassword;
        }

        private void button3_Click(object sender, EventArgs e)
        {
            //get new passwords
            string password1 = textBox2.Text;
            string password2 = textBox3.Text;

            // check if password1 characters is greater than or equal to 6
            if (password1.Length >= 6)
            {
                //check if password1 equals password2
                if (password1 == password2)
                {
                    if (pass != password1)
                    {
                        //encode password1 
                        Crypto mycrypto = new Crypto();
                        mycrypto.Input = password1;
                        mycrypto.Encode();
                        string epassword = mycrypto.Output;

                        string role = "Active";

                        //update user record
                        mydbAccess.Query = "update [user] set user_password = '" + epassword + "', user_status = '" + role + "' where user_username='" + user + "'and user_password='" + epass + "'";
                        mydbAccess.Insert();
                        //return to login form
                        if (mydbAccess.Status == 1)
                        {
                            MessageBox.Show("Account Activated!");
                            button2.PerformClick();
                        }

                        else
                        {
                            if (mydbAccess.Message != null)
                            {
                                MessageBox.Show(mydbAccess.Message);
                            }

                            else
                            {
                                MessageBox.Show("Something went Wrong");
                                textBox2.Clear();
                                textBox3.Clear();
                            }
                        }
                    }
                    else
                    {
                        MessageBox.Show("Password must be different from former password!");
                        textBox2.Clear();
                        textBox3.Clear();
                    }         
                   
                }

                else
                {
                    MessageBox.Show("Passwords do not match");
                    textBox2.Clear();
                    textBox3.Clear();
                }
            }

            else
            {
                MessageBox.Show("Password must be at least six(6) characters long");
                textBox2.Clear();
                textBox3.Clear();
            }       
        }

        private void button2_Click(object sender, EventArgs e)
        {
            LoginForm myloginform = new LoginForm();
            this.Hide();
            myloginform.Show();
        }     
    }

}
