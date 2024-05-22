using Microsoft.AspNetCore.Mvc;
using Models;
using Utils.helper;
using Npgsql;
using System;

public class UsersController : Controller
{
    public IActionResult Index()
    {
        return View();
    }

    public IActionResult LoginAdminForm()
    {
        return View();
    }

    public IActionResult Login(String numero)
    {
        try
        {
            using (NpgsqlConnection conn = new Connection().GetConnection())
            {
                Users users = Users.GetUsersByNumero(numero,conn);
                if(users != null)
                {
                    HttpContext.Session.SetString("id", numero);
                    return RedirectToAction("FormCreateDevise", "Client");
                }else{
                    throw new Exception("Utilisateur non trouvé");
                }
            }
        }
        catch (Exception ex){
            return RedirectToAction("Error", "Error", new { errorMessage = ex.Message });
        }
    }

    public IActionResult LoginAdmin(String nom, String mdp)
    {
        try
        {
            using (NpgsqlConnection conn = new Connection().GetConnection())
            {
                Admin admin = Admin.GetAdminByNom(nom,mdp,conn);
                if(admin != null)
                {
                    HttpContext.Session.SetString("id",admin.Id);
                    return RedirectToAction("ListeDevise", "Admin");
                   
                }else{
                    throw new Exception("Utilisateur non trouvé");
                }
            }
        }
        catch (Exception ex){
            ModelState.AddModelError(string.Empty, "Une erreur s'est produite : " + ex.Message);
            return View();
        }
    }
}
