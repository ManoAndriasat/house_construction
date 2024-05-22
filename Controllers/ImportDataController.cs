using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Linq;
using CsvHelper;
using CsvHelper.Configuration;
using Npgsql;
using Models; 
using Utils.helper;


public class ImportDataController : Controller
{
    public IActionResult ImportFormPayement()
    {
        return View();
    }
        public IActionResult ImportMaisonDevis()
    {
        return View();
    }

    public IActionResult ImportPDFPayement(IFormFile csvFile)
    {
        List<ImportPayement> dataList = new List<ImportPayement>();

        using (var streamReader = new StreamReader(csvFile.OpenReadStream(), Encoding.UTF8))
        using (var csvReader = new CsvReader(streamReader, new CsvConfiguration(System.Globalization.CultureInfo.InvariantCulture) { HasHeaderRecord = true,Delimiter = "," }))
        {
            dataList = csvReader.GetRecords<ImportPayement>().ToList();
        }
        Dictionary<string, bool> refPayementExists = new Dictionary<string, bool>();

        // Filtrer dataList en gardant uniquement la première occurrence de chaque ref_paiement
        dataList = dataList.Where(item =>
        {
            if (!refPayementExists.ContainsKey(item.ref_paiement))
            {
                refPayementExists[item.ref_paiement] = true;
                return true;
            }
            return false;
        }).ToList();
        ViewBag.Message = $"Imported {dataList.Count} records successfully.";

        using (NpgsqlConnection conn = new Connection().GetConnection())
        {
            try
            {
                foreach (var data in dataList)
                {
                    data.Insert(conn);
                }
            }
            catch (Exception)
            {
                throw;
            }
                ImportPayement.InsertPayementFromImport(conn);
        }

        return View("ImportFormPayement");
    }

    public IActionResult ImportPDFMaison(IFormFile csvFile)
    {
        List<ImportMaison> dataList = new List<ImportMaison>();

        using (var streamReader = new StreamReader(csvFile.OpenReadStream(), Encoding.UTF8))
        using (var csvReader = new CsvReader(streamReader, new CsvConfiguration(System.Globalization.CultureInfo.InvariantCulture) { HasHeaderRecord = true,Delimiter = "," }))
        {
            dataList = csvReader.GetRecords<ImportMaison>().ToList();
        }

        ViewBag.Message = $"Imported {dataList.Count} records successfully.";

        using (NpgsqlConnection conn = new Connection().GetConnection())
        {
            try
            {
                foreach (var data in dataList)
                {
                    data.Insert(conn);
                }
                    ImportMaison.InsertTypeMaison(conn);
                    ImportMaison.InsertCategorieTravaux(conn);
                    ImportMaison.InsertTravaux(conn);
                    ImportMaison.InsertDevisTypeMaison(conn);
            }
            catch (Exception)
            {
            }
        }

        return View("ImportMaisonDevis");
    }

    public IActionResult ImportPDFDevis(IFormFile csvFile)
    {
        List<ImportDevis> dataList = new List<ImportDevis>();

        using (var streamReader = new StreamReader(csvFile.OpenReadStream(), Encoding.UTF8))
        using (var csvReader = new CsvReader(streamReader, new CsvConfiguration(System.Globalization.CultureInfo.InvariantCulture) { HasHeaderRecord = true,Delimiter = "," }))
        {
            dataList = csvReader.GetRecords<ImportDevis>().ToList();
        }

        ViewBag.Message = $"Imported {dataList.Count} records successfully.";

        using (NpgsqlConnection conn = new Connection().GetConnection())
        {
            try
            {
                foreach (var data in dataList)
                {
                    data.Insert(conn);
                }
                    ImportDevis.InsertUsers(conn);
                    ImportDevis.InsertFinition(conn);
                    ImportDevis.InsertDevisClient(conn);
    }
            catch (Exception)
            {
                throw;
            }
            try{
                foreach (var data in dataList)
                {
                    DetailsDeviseClient.InsertFromVue(conn,data.ref_devis,data.type_maison);
                    FinitionDeviseClient.InsertFinitionClient(conn,data.ref_devis,data.finition);
                    string idPayement = DatabaseHelper.GetNextId(conn, "payement_id", "PAY");
                    Payement payement = new Payement(idPayement, data.ref_devis, DateTime.Now,0,idPayement);
                    payement.Insert(conn);
                }
            }catch(Exception){
                throw;
            }
        }

        return View("ImportMaisonDevis");
    }

    public IActionResult Delete()
    {
        using (NpgsqlConnection conn = new Connection().GetConnection())
        {
                                                                                       
                string[] deleteCommands = new string[]
                {
                    "DELETE FROM ImportDevis;",
                    "DELETE FROM ImportMaison;",
                    "DELETE FROM ImportPayement;",
                    "DELETE FROM payement;",
                    "DELETE FROM details_devise_client;",
                    "DELETE FROM finition_devise_client;",
                    "DELETE FROM devis_type_maison;",
                    "DELETE FROM devise_client;",
                    "DELETE FROM travaux;",
                    "DELETE FROM categorie_travaux;",
                    "DELETE FROM finition;",
                    "DELETE FROM type_maison;",
                    "DELETE FROM users;"
                };
                foreach (string commandText in deleteCommands)
                {
                    using (NpgsqlCommand command = new NpgsqlCommand(commandText, conn))
                    {
                        command.ExecuteNonQuery();
                        Console.WriteLine("Commande exécutée avec succès : " + commandText);
                    }
                }
            
                return View("ImportMaisonDevis");

            }

        }

}
