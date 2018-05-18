using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using System.Configuration;
using System.Data.SqlClient;
using System.Transactions;

namespace storeman
{
    public class dbAccess
    {
        private SqlConnection myConnection;

        private string connection;
        public string Connection
        {
            get
            {
                return connection;
            }
        }
        

        private string query;
        public string Query
        {
            get
            {
                return query;
            }
            set
            {
                query = value;
            }
        }

        private List<string> queryList;
        public List<string> QueryList
        {
            set
            {
                queryList = value;
            }
        }

        private DataTable result;
        public DataTable Result
        {
            get
            {
                return result;
            }
            set
            {
                result = value;
            }
        }
        private int status;
        public int Status
        {
            get
            {
                return status;
            }
            set
            {
                status = value;
            }
        }
        private string message = null;
        public string Message
        {
            get
            {
                return message;
            }
        }

        public dbAccess(string connect)
        {
            Crypto mycrypto = new Crypto();
            mycrypto.Input = connect;
            mycrypto.Decode();
            connection = mycrypto.Output;
        }

        public void Insert()
        {
            try
            {
                //establish connection
                myConnection = new SqlConnection(connection);
                myConnection.Open();

                //execute Query
                SqlCommand cmd = new SqlCommand(query, myConnection);
                SqlDataReader myReader;
                myReader = cmd.ExecuteReader();

                //Check for affected rows
                //int check = cmd.ExecuteNonQuery();
                //if (check != -1)

                if (myReader != null)
                {
                    //query succeeds: rows affected/ status 1 message null
                    status = 1;
                    myConnection.Close();
                }

                else
                {
                    //query succeeds: no rows affected /status 0 message null
                    status = 0;
                    myConnection.Close();
                }

            }
            catch (Exception ex)
            {
                //query did not succeed: error occured /status 0 message !null
                myConnection.Close();
                message = ex.Message;
                status = 0;
            }
        }




        public void Select()
        {
            try
            {
                myConnection = new SqlConnection(connection);
                myConnection.Open();
                SqlCommand cmd = new SqlCommand(query, myConnection);

                SqlDataAdapter MyAdapter = new SqlDataAdapter(cmd);
                result = new DataTable();
                MyAdapter.Fill(result);

                if (result.Rows.Count != 0)
                {
                    //query succeeds: rows affected/ status 1 message null
                    status = 1;
                }

                else
                {
                    //query succeeds: no rows affected /status 0 message null
                    status = 0;
                }

                myConnection.Close();
            }

            catch (Exception ex)
            {
                //query did not succeed: error occured /status 0 message !null
                status = 0;
                message = ex.Message;
                myConnection.Close();
            }
        }

        public void Update()
        {
            try
            {
                myConnection = new SqlConnection(connection);
                myConnection.Open();

                SqlCommand cmd = new SqlCommand(query, myConnection);
                SqlDataReader myReader;
                myReader = cmd.ExecuteReader();

                if (myReader.RecordsAffected != 0)
                {
                    //query succeeds: rows affected/ status 1 message null
                    status = 1;
                    myConnection.Close();
                }

                else
                {
                    //query succeeds: no rows affected /status 0 message null
                    status = 0;
                }

            }
            catch (Exception ex)
            {
                //query did not succeed: error occured /status 0 message !null
                message = ex.Message;
                status = 0;
                myConnection.Close();
            }
        }

        public void Delete()
        {

        }

        public void TransactionOperation()
        {
            using (var myTransaction = new TransactionScope(TransactionScopeOption.RequiresNew))
            {
                try
                {
                    using (myConnection = new SqlConnection(connection))
                    {
                        myConnection.Open();

                        foreach (var query in queryList)
                        {
                            SqlCommand cmd = new SqlCommand(query, myConnection);
                            cmd.ExecuteNonQuery();
                        }

                        myTransaction.Complete();
                        //query succeeds: rows affected/ status 1 message null
                        status = 1;
                        myConnection.Close();
                    }
                }
                catch (Exception ex)
                {
                    //query did not succeed: error occured /status 0 message !null
                    status = 0;
                    message = ex.Message;
                    myConnection.Close();
                    myTransaction.Dispose();
                }
            }
        }

    }
}