using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Configuration;
using System.Configuration.Install;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml;

namespace storeman
{
    [RunInstaller(true)]
    public partial class Installer1 : System.Configuration.Install.Installer
    {
        public void InstallerSetup()
        {
            InitializeComponent();
        }

        public override void Install(System.Collections.IDictionary stateSaver)
        {
            base.Install(stateSaver);
        }

        public override void Commit(IDictionary savedState)
        {
            base.Commit(savedState);

            try
            {
                AddConfigurationFileDetails();
            }
            catch (Exception e)
            {
                MessageBox.Show("Falha ao atualizar o arquivo de configuração da aplicação: " + e.Message);
                base.Rollback(savedState);
            }
        }

        public override void Rollback(IDictionary savedState)
        {
            base.Rollback(savedState);
        }

        public override void Uninstall(IDictionary savedState)
        {
            base.Uninstall(savedState);
        }

        private void showParameters()
        {
            StringBuilder sb = new StringBuilder();
            StringDictionary myStringDictionary = this.Context.Parameters;
            if (this.Context.Parameters.Count > 0)
            {
                foreach (string myString in this.Context.Parameters.Keys)
                {
                    sb.AppendFormat("String={0} Value= {1}\n", myString,
                    this.Context.Parameters[myString]);
                }
            }
            MessageBox.Show(sb.ToString());
        }

        private void AddConfigurationFileDetails()
        {
            try
            {
                string serverName = Environment.MachineName;
                string databaseName = Context.Parameters["DATABASENAME"];
                string userId = Context.Parameters["USERID"];
                string password = Context.Parameters["PASSWORD"];

                string name = Context.Parameters["NAME"];
                string address = Context.Parameters["ADDRESS"];
                string contact = Context.Parameters["CONTACT"];

                string dataSource = "Data Source =" + serverName;
                string initialcatalog = "Initial Catalog=" + databaseName;
                dataSource = dataSource + ";" + initialcatalog;
                dataSource = dataSource + ";User ID=" +userId+ ";Password=" +password+ ";";

                Crypto myCrypto = new Crypto();
                myCrypto.Input = dataSource;
                myCrypto.Encode();

                string edataSource = myCrypto.Output;

                //MessageBox.Show("instance=" + dataSource);
                ExeConfigurationFileMap map = new ExeConfigurationFileMap();
               // MessageBox.Show(Assembly.GetExecutingAssembly().Location + ".config");

                //Getting the path location 
                string configFile = string.Concat(Assembly.GetExecutingAssembly().Location, ".config");
                map.ExeConfigFilename = configFile;
                Configuration config = ConfigurationManager.
                OpenMappedExeConfiguration(map, ConfigurationUserLevel.None);
                string connectionsection = config.ConnectionStrings.ConnectionStrings
                ["myConnectionString"].ConnectionString;

                ConnectionStringSettings connectionstring = null;
                if (connectionsection != null)
                {
                    config.ConnectionStrings.ConnectionStrings.Remove("myConnectionString");
                  //  MessageBox.Show("removing existing Connection String");
                }

                connectionstring = new ConnectionStringSettings("myConnectionString", edataSource );
                config.ConnectionStrings.ConnectionStrings.Add(connectionstring);

                config.Save(ConfigurationSaveMode.Modified, true);
                ConfigurationManager.RefreshSection("connectionStrings");

                // Get the path to the executable file that is being installed on the target computer  
                //string assemblypath = Context.Parameters["assemblypath"];
                string appConfigPath = string.Concat(Assembly.GetExecutingAssembly().Location, ".config");

                // Write the path to the app.config file  
                XmlDocument doc = new XmlDocument();
                doc.Load(appConfigPath);

                XmlNode configuration = null;
                foreach (XmlNode node in doc.ChildNodes)
                    if (node.Name == "configuration")
                        configuration = node;

                if (configuration != null)
                {
                    //MessageBox.Show("configuration != null");  
                    // Get the ‘appSettings’ node  
                    XmlNode settingNode = null;
                    foreach (XmlNode node in configuration.ChildNodes)
                    {
                        if (node.Name == "appSettings")
                            settingNode = node;
                    }

                    if (settingNode != null)
                    {
                        //MessageBox.Show("settingNode != null");  
                        //Reassign values in the config file  
                        foreach (XmlNode node in settingNode.ChildNodes)
                        {
                            //MessageBox.Show("node.Value = " + node.Value);  
                            if (node.Attributes == null)
                                continue;
                            XmlAttribute attribute = node.Attributes["value"];
                            //MessageBox.Show("attribute != null ");  
                            //MessageBox.Show("node.Attributes['value'] = " + node.Attributes["value"].Value);  
                            if (node.Attributes["key"] != null)
                            {
                                //MessageBox.Show("node.Attributes['key'] != null ");  
                                //MessageBox.Show("node.Attributes['key'] = " + node.Attributes["key"].Value);  
                                switch (node.Attributes["key"].Value)
                                {
                                    case "name":
                                        attribute.Value = name;
                                        break;

                                    case "address":
                                        attribute.Value = address;
                                        break;

                                    case "contact":
                                        attribute.Value = contact;
                                        break;
                                }
                            }
                        }
                    }
                    doc.Save(appConfigPath);
                }
            }
            catch
            {
                throw;
            }
        }
    }
}
