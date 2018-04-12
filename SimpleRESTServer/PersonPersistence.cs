using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MySql.Data;

using System.Diagnostics;
using SimpleRESTServer.Models;
using System.Collections;

namespace SimpleRESTServer
{
    public class PersonPersistence
    {
        public PersonPersistence()
        {
            Debug.WriteLine("PersonPersistence()");
        }

        public Person GetPerson(long ID)
        // https://youtu.be/3_KJc03Xb8E?list=PLDQIAo9A3-DaBMQ0ZZFlcej2we3uC5VT8&t=410
        {
            Person p = new Person();
            MySql.Data.MySqlClient.MySqlDataReader mySQLReader= null;  // Hvorfor?

            String sqlString = "SELECT * FROM persons WHERE idpersons = " + ID.ToString();
            MySql.Data.MySqlClient.MySqlCommand cmd = new MySql.Data.MySqlClient.MySqlCommand(sqlString, WebApiApplication.conn);

            try
            {
                mySQLReader = cmd.ExecuteReader();
            }
            catch (MySql.Data.MySqlClient.MySqlException ex)
            {
                Debug.WriteLine(ex);
            }
            if (mySQLReader != null && mySQLReader.Read())
            {
                p.ID = mySQLReader.GetInt32(0);
                p.LastName = mySQLReader.GetString(1);
                p.FirstName = mySQLReader.GetString(2);
                p.E_Mail = mySQLReader.GetString(3);
                p.Telephone = mySQLReader.GetString(4);
                mySQLReader.Close();  // MySql.Data.MySqlClient.MySqlException (0x80004005): There is already an open DataReader associated with this Connection which must be closed first.
                return p;
            } else
            {
                mySQLReader.Close();
                return null;
            }


        }

        public long savePerson(Person personToSave)
        // https://youtu.be/LpySuvYPMZQ?list=PLDQIAo9A3-DaBMQ0ZZFlcej2we3uC5VT8&t=591
        {
            String sqlString = "INSERT INTO persons (LastName, FirstName, E_Mail, Telephone) VALUES ('" + personToSave.LastName + "', '" + personToSave.FirstName + "', '" + personToSave.E_Mail + "', '" + personToSave.Telephone + "');";
            MySql.Data.MySqlClient.MySqlCommand cmd = new MySql.Data.MySqlClient.MySqlCommand(sqlString, WebApiApplication.conn);
            cmd.ExecuteNonQuery();
            long id = cmd.LastInsertedId;
            return id;
        }

        public ArrayList GetPersons()
        // https://youtu.be/3_KJc03Xb8E?list=PLDQIAo9A3-DaBMQ0ZZFlcej2we3uC5VT8&t=410
        {
            ArrayList personArray = new ArrayList();
            MySql.Data.MySqlClient.MySqlDataReader mySQLReader = null;  // Hvorfor?

            String sqlString = "SELECT * FROM persons";
            MySql.Data.MySqlClient.MySqlCommand cmd = new MySql.Data.MySqlClient.MySqlCommand(sqlString, WebApiApplication.conn);

            try
            {
                mySQLReader = cmd.ExecuteReader();
            }
            catch (MySql.Data.MySqlClient.MySqlException ex)
            {
                Debug.WriteLine(ex);
            }
            if (mySQLReader != null)
            {
                while (mySQLReader.Read())
                {
                    Person p = new Person();
                    p.ID = mySQLReader.GetInt32(0);
                    p.LastName = mySQLReader.GetString(1);
                    p.FirstName = mySQLReader.GetString(2);
                    p.E_Mail = mySQLReader.GetString(3);
                    p.Telephone = mySQLReader.GetString(4);
                    personArray.Add(p);
                    
                }
                mySQLReader.Close();  // MySql.Data.MySqlClient.MySqlException (0x80004005): There is already an open DataReader associated with this Connection which must be closed first.
                return personArray;
            }
            else
            {
                mySQLReader.Close();
                return null;
            }


        }


    }
}
