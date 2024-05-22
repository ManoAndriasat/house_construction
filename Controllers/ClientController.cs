using Microsoft.AspNetCore.Mvc;
using Models;
using Utils.helper;
using Npgsql;
using System;
using iTextSharp.text.pdf;
using iTextSharp.text;
using Maison.Models;

public class ClientController : Controller
{
    public IActionResult FormCreateDevise()
    {
        try
        {
            using (NpgsqlConnection conn = new Connection().GetConnection())
            {
                List<TypeMaison> typeMaisons = TypeMaison.GetAll(conn);
                List<Finition> finitions =  Finition.GetAll(conn);
                ViewBag.Finitions = finitions;
                ViewBag.TypeMaisons = typeMaisons;
                return View();
            }
        }
        catch (Exception ex)
        {
            return RedirectToAction("Error", "Error", new { errorMessage = ex.Message });
        }
    }

    public IActionResult Export(string idDeviseClient)
    {
        try
        {
            using (NpgsqlConnection conn = new Connection().GetConnection())
            {
                List<Payement> payements = Payement.GetByIdDevise(conn,idDeviseClient);
                PayementDetails payer =  PayementDetails.GetById(conn,idDeviseClient);
                List<DetailsDeviseClient> detailsDevises = DetailsDeviseClient.SelectByIdDeviseClient(conn, idDeviseClient);
                Document document = new Document();
                string filePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "DetailsDevise.pdf");
                PdfWriter.GetInstance(document, new FileStream(filePath, FileMode.Create));
                document.Open();

                PdfPTable table = new PdfPTable(7); // 7 colonnes pour chaque propriété de DetailsDeviseClient
                table.WidthPercentage = 100; // Largeur du tableau en pourcentage
                float[] columnWidths = { 15, 15, 15, 10, 10, 20, 15 }; // Largeurs des colonnes en pourcentage
                table.SetWidths(columnWidths);


                // Ajout des en-têtes de colonnes dans le tableau
                table.AddCell("Catégorie");
                table.AddCell("Désignation");
                table.AddCell("Unité");
                table.AddCell("PU");
                table.AddCell("Quantité");
                table.AddCell("Type de Maison");
                table.AddCell("Total");

                // Ajout des détails de la devise dans le tableau
                foreach (var detail in detailsDevises)
                {
                    table.AddCell(detail.Categorie);
                    table.AddCell(detail.Designation);
                    table.AddCell(detail.Unite);
                    table.AddCell(detail.PU.ToString());
                    table.AddCell(detail.Quantite.ToString());
                    table.AddCell(detail.TypeMaison);
                    table.AddCell(detail.Total.ToString());
                }

                table.AddCell("");
                table.AddCell("");
                table.AddCell("");
                table.AddCell("");
                table.AddCell("");
                table.AddCell("deja payer");
                table.AddCell(payer.Payer.ToString("F2"));

                table.AddCell("");
                table.AddCell("");
                table.AddCell("");
                table.AddCell("");
                table.AddCell("");
                table.AddCell("reste a payer");
                table.AddCell(payer.Reste.ToString("F2"));

                table.AddCell("");
                table.AddCell("");
                table.AddCell("");
                table.AddCell("");
                table.AddCell("");
                table.AddCell("total a payer");
                table.AddCell(payer.Total.ToString("F2"));


                table.AddCell("");
                table.AddCell("");
                table.AddCell("");
                table.AddCell("");
                table.AddCell("");
                table.AddCell("");
                table.AddCell(payer.Total.ToString("F2"));
                
                table.AddCell("liste payement");
                table.AddCell("");
                table.AddCell("");
                table.AddCell("");
                table.AddCell("");
                table.AddCell("reference");
                table.AddCell("montant");

                foreach (var payement in payements)
                {
                    table.AddCell("");
                    table.AddCell("");
                    table.AddCell("");
                    table.AddCell("");
                    table.AddCell("");
                    table.AddCell(payement.Reference);
                    table.AddCell(payement.Montant.ToString("F2"));
                }

                // Ajout du tableau au document
                document.Add(table);
                document.Close();

                byte[] fileBytes = System.IO.File.ReadAllBytes(filePath);
                System.IO.File.Delete(filePath);
                return File(fileBytes, "application/pdf", "DetailsDevise.pdf");
            }
        }
        catch (Exception ex)
        {
            return Json(new { montantRestant = 0, message = ex.Message });
        }
    }

    public IActionResult PayementForm()
    {
        try
        {
            using (NpgsqlConnection conn = new Connection().GetConnection())
            {
                string clientId = HttpContext.Session.GetString("id");
                if (string.IsNullOrEmpty(clientId))
                {
                throw new Exception("Client ID is missing.");
                }
                List<DeviseClient> DeviseClients = DeviseClient.GetByUsers(conn, clientId);
                ViewBag.DeviseClients = DeviseClients;

                return View("Payement");
            }
        }
                catch (Exception ex)
        {
            return RedirectToAction("Error", "Error", new { errorMessage = ex.Message });
        }
    }

    public IActionResult DetailsDevise(string idDeviseClient)
    {
        try
        {
            using (NpgsqlConnection conn = new Connection().GetConnection())
            {
                List<DetailsDeviseClient> DetailsDeviseClients = DetailsDeviseClient.SelectByIdDeviseClient(conn,idDeviseClient);
                ViewBag.DetailsDeviseClients = DetailsDeviseClients;
                return View("DetailsDevise");
            }
        }
        catch (Exception ex)
        {
            return RedirectToAction("Error", "Error", new { errorMessage = ex.Message });
        }
    }

    public IActionResult CreateDevise(string typeMaison, string finition, string designation, DateTime date)
    {
        try
        {
            using (NpgsqlConnection conn = new Connection().GetConnection())
            {
                string deviseID = DatabaseHelper.GetNextId(conn,"devise_client_id","DVS");
                string clientId = HttpContext.Session.GetString("id");


                if (string.IsNullOrEmpty(clientId))
                {
                    throw new Exception("Client ID is missing.");
                }

                DeviseClient deviseClient = new DeviseClient(deviseID,clientId, typeMaison, finition, date, designation);
                deviseClient.Insert(conn);
                
                DetailsDeviseClient.InsertFromVue(conn,deviseID,typeMaison);
                FinitionDeviseClient.InsertFinitionClient(conn,deviseID,finition);

                string idPayement = DatabaseHelper.GetNextId(conn, "payement_id", "PAY");
                Payement payement = new Payement(idPayement, deviseID, DateTime.Now,0,idPayement);
                payement.Insert(conn);
                return RedirectToAction("ListeDevise");
            }
        }
        catch (Exception ex)
        {
            return RedirectToAction("Error", "Error", new { errorMessage = ex.Message });
        }
    }

    public IActionResult Payer(string devis, decimal montant,DateTime date,string reference)
    {
        try
        {
            using (NpgsqlConnection conn = new Connection().GetConnection())
            {
                string idPayement = DatabaseHelper.GetNextId(conn, "payement_id", "PAY");
                Payement payement = new Payement(idPayement, devis, date, montant,reference);
                payement.Insert(conn);
                return RedirectToAction("ListeDevise", "Client");
            }
        }
        catch (Exception ex)
        {
            return RedirectToAction("Error", "Error", new { errorMessage = ex.Message });
        }
    }

    [HttpPost]
    public IActionResult GetMontantRestant([FromBody] string idDeviseClient)
    {
        try
        {
            using (NpgsqlConnection conn = new Connection().GetConnection())
            {
                PayementDetails payement = PayementDetails.GetById(conn, idDeviseClient);
                return Json(new { montantRestant = payement.Reste });
            }
        }
        catch (Exception ex)
        {
            return Json(new { montantRestant = 0, message = ex.Message });
        }
    }



    public IActionResult ListeDevise()
    {
        try
        {
            using (NpgsqlConnection conn = new Connection().GetConnection())
            {
                string clientId = HttpContext.Session.GetString("id");
                if (string.IsNullOrEmpty(clientId))
                {
                    throw new Exception("Client ID is missing.");
                }
                List<PayementDetails> payementDetails = PayementDetails.GetByUsers(conn, clientId);
                ViewBag.payementDetails = payementDetails;
                return View("ListeDevise");
            }
        }
        catch (Exception ex)
        {
            return RedirectToAction("Error", "Error", new { errorMessage = ex.Message });
        }
    }
}
