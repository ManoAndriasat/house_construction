using Microsoft.AspNetCore.Mvc;
using Models;
using Utils.helper;
using Npgsql;
using System;

public class TravauxController : Controller
{
    public IActionResult Index()
    {
        try
        {
            using (NpgsqlConnection conn = new Connection().GetConnection())
            {
                List<Travaux> travaux = Travaux.GetAll(conn);
                ViewBag.travaux = travaux;
                return View();
            }
        }
        catch (Exception ex){
            return RedirectToAction("Error", "Error", new { errorMessage = ex.Message });
        }
    }

    public IActionResult Formulaire(string idTravaux){
        try
        {
            using (NpgsqlConnection conn = new Connection().GetConnection())
            {
                Travaux travau = Travaux.GetById(conn,idTravaux);
                ViewBag.travau = travau;
                return View("Form");
            }
        }
        catch (Exception ex){
            return RedirectToAction("Error", "Error", new { errorMessage = ex.Message });
        }   
    }

    public IActionResult Update(string idTravaux, string idCategorie, string designation, string unite, decimal pu)
    {
        try
        {
            using (NpgsqlConnection conn = new Connection().GetConnection())
            {
                Travaux.Update(conn, idTravaux,idCategorie,designation, unite, pu);
                return RedirectToAction("Index","Travaux");
            }
        }
        catch (Exception ex){
            return RedirectToAction("Error", "Error", new { errorMessage = ex.Message });
        }
    }
}
