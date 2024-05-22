using Microsoft.AspNetCore.Mvc;
using Models;
using Utils.helper;
using Npgsql;
using Newtonsoft.Json;
using System;

public class AdminController : Controller
{
    public IActionResult Dashboard(){
        try
        {
            using (NpgsqlConnection conn = new Connection().GetConnection())
            {
                double TotalDevis = PayementDetails.GetSumTotal(conn);
                double TotalPayement = PayementDetails.GetSumPayementTotal(conn);

                List<(DateTime Mois, double Somme)> SommeMois = PayementDetails.GetSommesTotalMensuelles(conn);
                List<(DateTime Annee, double Somme)> SommeAnnee = PayementDetails.GetSommesTotalAnnuel(conn);
                string sommeAnneeJson = JsonConvert.SerializeObject(SommeAnnee);
                string sommeMoisJson = JsonConvert.SerializeObject(SommeMois);
                ViewBag.SommeMois = sommeMoisJson;
                ViewBag.SommeAnnee = sommeAnneeJson;
                ViewBag.TotalDevis = TotalDevis;
                ViewBag.TotalPayement = TotalPayement;
                return View();
            }
        }
        catch (Exception ex)
        {
            return RedirectToAction("Error", "Error", new { errorMessage = ex.Message });
        }
    }
    


    public IActionResult ListeDevise(){
        try
        {
            using (NpgsqlConnection conn = new Connection().GetConnection())
            {
                List<PayementDetails> payementDetails =  PayementDetails.GetAll(conn);
                foreach(var dd in payementDetails){
                    dd.Pourcentage = (dd.Payer * 100)/dd.Total;
                }
                ViewBag.payementDetails = payementDetails;
                return View("Liste");
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
}
