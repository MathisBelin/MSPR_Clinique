using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.DirectoryServices;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Resources;
using System.Web;
using System.Web.Mvc;

namespace MSPR_clinique.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Connection()
        {
            string user = User.Identity.Name;
            if (user != null) // user connecté
            {
                // generer otp
                //envoie sms/mail
                //redirige ver page ou il peut saisir l'otp
                // verif si otp is true

                // if otp is true, return view
                //declare variable session userconnecte=1

                //sinon page erreur

            }

            return View();
        }

        public static string GetIpInBDD(int IdUser)
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

        public ActionResult Login(string User)
        {
            try
            {
                int IdUser = 0;
                string host = Dns.GetHostName();
                string ipLocal = Dns.GetHostByName(host).AddressList[0].ToString();

                //GetDirectoryEntry();
                string serieNombre = "";
                Random aleatoire = new Random();
                for (int i = 0; i <= 5; i++)
                {
                    int entierUnChiffre = aleatoire.Next(10);
                    serieNombre += Convert.ToString(entierUnChiffre);
                }
                SqlConnection con = ConnectionDatabase();
                con.Open();
                SqlCommand commande = con.CreateCommand();
                commande.CommandText = $@"SELECT id,login 
                                         FROM Users
                                         where login = '{User}'";
                SqlDataReader reader = commande.ExecuteReader();
                while(reader.Read())
                {
                    IdUser = int.Parse(reader[0].ToString());
                }

                if (reader.HasRows == true)
                {
                    string ipInBDD = GetIpInBDD(IdUser);
                    con.Close();
                    if(ipLocal != ipInBDD)
                    {
                        MailMessage mail = new MailMessage();
                        MailAddress fromAddress = new MailAddress("matbelin5@gmail.com");
                        mail.IsBodyHtml = true;
                        mail.From = fromAddress;
                        mail.To.Add("matbelin6@gmail.com");
                        mail.Subject = $"Articles éronnés";
                        mail.Body = $@"Code de validation : {serieNombre}";

                        SmtpClient smtpServer = new SmtpClient();
                        smtpServer.Host = "localhost";
                        smtpServer.Send(mail);

                        try
                        {
                            ViewBag.IpAdresse = ipLocal;
                            ViewBag.UserId = IdUser;
                            ViewBag.User = $"{User}";
                            con.Open();
                            SqlCommand commande2 = con.CreateCommand();
                            commande2.CommandText = $@"UPDATE Users
                                              SET [token] = '{serieNombre}'
                                              where login = '{User}'";
                            SqlDataReader reader2 = commande2.ExecuteReader();
                            con.Close();
                        }
                        catch (Exception ex)
                        {

                        }
                        return View("Verification");
                    }
                    else
                    {
                        return View("Index");
                    }
                }
                else
                {
                    con.Close();
                    return View("Connection");
                }
            }

            catch (Exception ex)
            {
                return View("Home");
            }
        }

        public ActionResult LoadPage(string token, string UserName, string UserId, string IpLocal)
        {
            SqlConnection con = ConnectionDatabase();
            string TokenInBDD = "";

            try
            {
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

                if (token == TokenInBDD)
                {
                    UpdateIpInBDD(UserId, IpLocal);
                    return View("Index");
                }

                else
                {
                    con.Close();
                    ViewBag.Message = "Code incorrecte !";
                    return View("Verification");
                }
            }

            catch (Exception ex)
            {
                con.Close();
                ViewBag.Message = ex.Message;
                return View("Verification");
            }
        }

        public static void UpdateIpInBDD(string UserId, string IpLocal)
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

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";
            if (Session["userConnecte"].Equals(1))
                return View();

            //else
            //    return Error();

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
        public static DirectoryEntry GetDirectoryEntryByLogin(String login)
        {
            DirectoryEntry de;
            try
            {
                de = new DirectoryEntry();
                de.Path = "LDAP://AD-MSPR-VM";
                de.Username = "mathis";
                de.Password = "Xefimdp69140";
                DirectorySearcher searcher = new DirectorySearcher(de);
                searcher.Filter = "(objectClass=user)";

                foreach (SearchResult result in searcher.FindAll())
                {
                    DirectoryEntry DirEntry = result.GetDirectoryEntry();
                    Console.WriteLine("Nom", DirEntry.Properties["cn"].Value);
                }
            }
            catch (Exception e)
            {
                throw e;
            }

            return de;
        }

        public static SqlConnection ConnectionDatabase()
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
    }
}