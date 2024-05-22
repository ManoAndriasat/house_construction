using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Maison.Models;

public class ErrorController : Controller
{
    public IActionResult Error(string errorMessage)
    {
        ViewData["Title"] = "hide";
        ViewBag.ErrorMessage = errorMessage;
        return View();
    }
}
