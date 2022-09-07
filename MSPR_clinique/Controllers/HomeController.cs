using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Net;
using System.Net.Mail;
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
        public ActionResult Login(string User)
        {
            try
            {
                string serieNombre = "";
                Random aleatoire = new Random();
                for (int i = 0; i <= 5; i++)
                {
                    int entierUnChiffre = aleatoire.Next(10);
                    serieNombre += Convert.ToString(entierUnChiffre);
                }

                SqlConnection con = new SqlConnection("Server=LAPTOP-114PRVUO; Database=MSPR_09_09_2022; Integrated Security=True;");
                con.Open();
                SqlCommand commande = con.CreateCommand();
                commande.CommandText = $@"SELECT login 
                                         FROM Users
                                         where login = '{User}'";
                SqlDataReader reader = commande.ExecuteReader();

                if (reader.HasRows == true)
                {
                    con.Close();
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
                        con.Open();
                        SqlCommand commande2 = con.CreateCommand();
                        commande2.CommandText = $@"UPDATE [MSPR_09_09_2022].[dbo].[Users]
                                              SET [token] = '{serieNombre}'
                                              where login = '{User}'";
                        SqlDataReader reader2 = commande2.ExecuteReader();
                        con.Close();
                    }
                    catch(Exception ex)
                    {

                    }


                    return View("Verification");
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

        public ActionResult LoadPage(string token)
        {
            try
            {
                SqlConnection con = new SqlConnection("Server=LAPTOP-114PRVUO; Database=MSPR_09_09_2022; Integrated Security=True;");
                con.Open();
                SqlCommand commande = con.CreateCommand();
                commande.CommandText = $@"SELECT login 
                                         FROM Users
                                         where login = '{token}'";
                SqlDataReader reader = commande.ExecuteReader();
                while(reader.Read())
                {

                }
            }

            catch (Exception ex)
            {

            }

            return View("Index");
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
    }
}