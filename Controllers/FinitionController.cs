using Microsoft.AspNetCore.Mvc;
using Models; 
using Npgsql;
using System;
using System.Collections.Generic;
using Utils.helper;

public class FinitionController : Controller
{
    public IActionResult Index()
    {
        try
        {
            using (NpgsqlConnection conn = new Connection().GetConnection())
            {
                List<Finition> finitions = Finition.GetAll(conn);
                ViewBag.finitions = finitions;
                return View();
            }
        }
        catch (Exception ex)
        {
            return RedirectToAction("Error", "Error", new { errorMessage = ex.Message });
        }
    }

    public IActionResult Formulaire(string idFinition)
    {
        try
        {
            using (NpgsqlConnection conn = new Connection().GetConnection())
            {
                Finition finition = Finition.GetById(conn, idFinition);
                ViewBag.finition = finition;
                return View("Form");
            }
        }
        catch (Exception ex)
        {
            return RedirectToAction("Error", "Error", new { errorMessage = ex.Message });
        }
    }

    public IActionResult Update(string idFinition, string nom, double taux)
    {
        try
        {
            using (NpgsqlConnection conn = new Connection().GetConnection())
            {
                Finition.Update(conn, idFinition, nom, taux);
                return RedirectToAction("Index", "Finition");
            }
        }
        catch (Exception ex)
        {
            return RedirectToAction("Error", "Error", new { errorMessage = ex.Message });
        }
    }
}
