using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Routing;
using System.Diagnostics;

using Microsoft.Win32;
using System.Security.Cryptography;
using System.Text;
using MySql.Data;



namespace SimpleRESTServer
{
    public class WebApiApplication : System.Web.HttpApplication
    {
        public static MySql.Data.MySqlClient.MySqlConnection conn;
        /*  MariaDB [(none)]> CREATE DATABASE IF NOT EXISTS test;
            MariaDB [(none)]> USE test;
            MariaDB [test]> CREATE USER 'sirese'@'localhost' IDENTIFIED BY '';
            MariaDB [test]> GRANT ALL PRIVILEGES ON test.* TO 'sirese'@'localhost';
            MariaDB [test]> FLUSH PRIVILEGES;
        */
        // https://www.youtube.com/playlist?list=PLDQIAo9A3-DaBMQ0ZZFlcej2we3uC5VT8

        protected void Application_Start()
        {
            Debug.WriteLine("init...");
            GlobalConfiguration.Configure(WebApiConfig.Register);

            // https://www.c-sharpcorner.com/UploadFile/f9f215/windows-registry/
            //accessing the CurrentUser root element  
            //and adding "OurSettings" subkey to the "SOFTWARE" subkey  
            RegistryKey key = Registry.CurrentUser.CreateSubKey(@"SOFTWARE\wittrup\SimpleRESTServer");

            //if it does exist, retrieve the stored values  
            if (key != null)
            {
                // https://security.stackexchange.com/questions/20358/what-is-the-purpose-of-the-entropy-parameter-for-dpapi-protect
                string dbPwdEntropy;
                object regValAddEnt = key.GetValue("pwdAddEnt", null);
                if (regValAddEnt == null)
                {
                    //code if key Not Exist
                    dbPwdEntropy = RandomString(16);
                    key.SetValue("pwdAddEnt", dbPwdEntropy);
                } else
                {
                    dbPwdEntropy = regValAddEnt.ToString();
                }

                object regValPwdDb = key.GetValue("pwd", null);
                // https://stackoverflow.com/questions/4276138/how-to-check-if-a-registry-value-exists-using-c
                string plaintext;
                if (regValPwdDb == null)
                {
                    //code if key Not Exist
                    Debug.WriteLine("CIPHER not found, prompting for password");
                    // https://stackoverflow.com/questions/5427020/prompt-dialog-in-windows-forms
                    plaintext = Prompt.ShowDialog("Enter password", "Please enter password for the DB connection");
                    // https://stackoverflow.com/questions/34194223/dpapi-password-encryption-in-c-sharp-and-saving-into-database-then-decrypting-it
                    string protectedValue = Protect(plaintext, dbPwdEntropy, DataProtectionScope.CurrentUser);
                    //plaintext = null;
                    key.SetValue("pwd", protectedValue);
                    Debug.WriteLine("CIPHER FETCH: " + TruncString(protectedValue, 30));
                }
                else
                {
                    //code if key Exist
                    Debug.WriteLine("CIPHER FOUND: " + TruncString(regValPwdDb.ToString(), 30));
                    plaintext = Unprotect(regValPwdDb.ToString(), dbPwdEntropy, DataProtectionScope.CurrentUser);
                }

                try
                {
                    conn = new MySql.Data.MySqlClient.MySqlConnection();
                    // https://dev.mysql.com/doc/dev/connector-net/6.10/html/P_MySql_Data_MySqlClient_MySqlConnection_ConnectionString.htm
                    conn.ConnectionString = "server=localhost;uid=sirese;database=test;pwd=" + plaintext;
                    conn.Open();
                }
                catch (MySql.Data.MySqlClient.MySqlException ex)
                {
                    Debug.WriteLine(ex);
                }
                finally
                {
                    // TODO Make sure this is safest way to delete variable/string from memory
                    plaintext = null;
                }
                key.Close();
            }
        }

        public static string Protect(string stringToEncrypt, string optionalEntropy, DataProtectionScope scope)
        {
            return Convert.ToBase64String(
                ProtectedData.Protect(
                    Encoding.UTF8.GetBytes(stringToEncrypt)
                    , optionalEntropy != null ? Encoding.UTF8.GetBytes(optionalEntropy) : null
                    , scope));
        }

        public static string Unprotect(string encryptedString, string optionalEntropy, DataProtectionScope scope)
        {
            return Encoding.UTF8.GetString(
                ProtectedData.Unprotect(
                    Convert.FromBase64String(encryptedString)
                    , optionalEntropy != null ? Encoding.UTF8.GetBytes(optionalEntropy) : null
                    , scope));
        }

        public static string TruncString(string str, int maxLength)
        {
            return str.Substring(0, Math.Min(str.Length, maxLength));
        }

        public static string RandomString(int length)
        {
            const string valid = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890";
            StringBuilder res = new StringBuilder();
            using (RNGCryptoServiceProvider rng = new RNGCryptoServiceProvider())
            {
                byte[] uintBuffer = new byte[sizeof(uint)];

                while (length-- > 0)
                {
                    rng.GetBytes(uintBuffer);
                    uint num = BitConverter.ToUInt32(uintBuffer, 0);
                    res.Append(valid[(int)(num % (uint)valid.Length)]);
                }
            }

            return res.ToString();
        }
    }
}
