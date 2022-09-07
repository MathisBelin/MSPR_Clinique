using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MSPR_clinique.Controllers
{
    public class Adapter
    {

        //Connection---------------------------------------------------------------------------
        public SqlConnection ConnectionDatabase()
        {
            try
            {
                SqlConnection sql = new SqlConnection("Server=" + Properties.Resources.DB_SERVER + ";Database=" + Properties.Resources.DB_DATABASE +
                     ";User Id=" + Properties.Resources.DB_USERNAME + ";Password=" + Properties.Resources.DB_PASSWORD + ";");

                return sql;
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        //Select Data---------------------------------------------------------------------------
        public string GetIpInBDD(int IdUser)
        {
            string IpAdresse = "";
            SqlConnection cnn = ConnectionDatabase();
            try
            {
                cnn.Open();
                SqlCommand commande = cnn.CreateCommand();
                commande.CommandText = $@"SELECT ip_addresse 
                                         FROM log
                                         where id = {IdUser}";
                SqlDataReader reader = commande.ExecuteReader();
                while (reader.Read())
                {
                    IpAdresse = reader[0].ToString();
                }
                return IpAdresse;
            }
            catch (Exception ex)
            {
                cnn.Close();
                return IpAdresse;
            }
        }

        public int GetIdUser(string User)
        {
            int IdUser = 0;
            SqlConnection con = ConnectionDatabase();
            con.Open();
            SqlCommand commande = con.CreateCommand();
            commande.CommandText = $@"SELECT id,login 
                                         FROM Users
                                         where login = '{User}'";
            SqlDataReader reader = commande.ExecuteReader();
            while (reader.Read())
            {
                IdUser = int.Parse(reader[0].ToString());
            }
            return IdUser;
        }

        public string GettokenInBDD(string UserName, string token)
        {
            string TokenInBDD="";
            SqlConnection con = ConnectionDatabase();
            con.Open();
            SqlCommand commande = con.CreateCommand();
            commande.CommandText = $@"SELECT token 
                                         FROM Users
                                         where login = '{UserName}' and token = '{token}'";
            SqlDataReader reader = commande.ExecuteReader();
            while (reader.Read())
            {
                TokenInBDD = reader[0].ToString();
            }

            con.Close();
            return TokenInBDD;
        }


        //Update Data---------------------------------------------------------------------------

        public void UpdateUserToken(string serieNombre, string User)
        {
            SqlConnection con = ConnectionDatabase();
            con.Open();
            SqlCommand commande2 = con.CreateCommand();
            commande2.CommandText = $@"UPDATE Users
                                              SET [token] = '{serieNombre}'
                                              where login = '{User}'";
            SqlDataReader reader2 = commande2.ExecuteReader();
            con.Close();
        }

        public void UpdateIpInBDD(string UserId, string IpLocal)
        {
            SqlConnection con = ConnectionDatabase();
            con.Open();

            SqlCommand commande2 = con.CreateCommand();
            commande2.CommandText = $@"UPDATE log
                                              SET [ip_addresse] = '{IpLocal}'
                                              where id = '{UserId}'";
            SqlDataReader reader2 = commande2.ExecuteReader();
            con.Close();

        }
    }
}