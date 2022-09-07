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
        Adapter adapter = new Adapter();
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


        //Quand l'utilisateur à renseigné ses informations
        public ActionResult Login(string User)
        {
            try
            {
                int IdUser = 0;

                //Récupère le nom du poste utilisé
                string host = Dns.GetHostName();

                //Récupère l'adresse IP du poste utilisé
                string ipLocal = Dns.GetHostByName(host).AddressList[0].ToString();

                //Vérification si l'utilisateur existe
                IdUser = adapter.GetIdUser(User);

                if (IdUser != null)
                {
                    //Vérification sa dernière adresse IP de connexion
                    string ipInBDD = adapter.GetIpInBDD(IdUser);

                    if (ipLocal != ipInBDD)
                    {
                        //Generate random number serie
                        string serieNombre = "";
                        Random aleatoire = new Random();
                        for (int i = 0; i <= 5; i++)
                        {
                            int entierUnChiffre = aleatoire.Next(10);
                            serieNombre += Convert.ToString(entierUnChiffre);
                        }

                        //Envoie d'un mail avec la serie de nombres générée aléatoirement
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

                        //Stockage des informaions dans des ViewBags
                        ViewBag.IpAdresse = ipLocal;
                        ViewBag.UserId = IdUser;
                        ViewBag.User = $"{User}";

                        //Modificaion dans la BDD du token à renseigné
                        adapter.UpdateUserToken(serieNombre, User);

                        return View("Verification");
                    }

                    else
                    {
                        return View("Index");
                    }
                }

                else
                {
                    return View("Connection");
                }
            }

            catch (Exception ex)
            {
                return View("Home");
            }
        }

        //Une foie que l'utilisateur à saisit le token
        public ActionResult LoadPage(string token, string UserName, string UserId, string IpLocal)
        {
            string TokenInBDD = "";
            try
            {
                //Récupération du token/liste de nombres en BDD
                TokenInBDD = adapter.GettokenInBDD(UserName, token);

                //Vérification si le token saisit est correcte
                if (token == TokenInBDD)
                {
                    //Modification de l'addresse IP de connexion
                    adapter.UpdateIpInBDD(UserId, IpLocal);
                    return View("Index");
                }

                else
                {
                    ViewBag.Message = "Code incorrecte !";
                    return View("Verification");
                }
            }

            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
                return View("Verification");
            }
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
    }
}