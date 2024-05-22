

using Npgsql;
using System;

public class DetailsDeviseClient
{
    public string IdDeviseClient { get; set; }
    public string Categorie { get; set; }
    public string Designation { get; set; }
    public string Unite { get; set; }
    public decimal PU { get; set; }
    public decimal Quantite { get; set; }
    public string TypeMaison { get; set; }
    public decimal Total { get; set; }

    public DetailsDeviseClient()
    {
    }

    public DetailsDeviseClient(string idDeviseClient, string categorie, string designation, string unite, decimal PU, decimal quantite, string typeMaison, decimal total)
    {
        IdDeviseClient = idDeviseClient;
        Categorie = categorie;
        Designation = designation;
        Unite = unite;
        PU = PU;
        Quantite = quantite;
        TypeMaison = typeMaison;
        Total = total;
    }


    public static void InsertFromVue(NpgsqlConnection conn, string idDeviseClient, string typeMaison)
    {
        try
        {
        string sql = @"INSERT INTO details_devise_client 
                        (id_devise_client, categorie_travaux, designation_travaux, unite_travaux, pu_travaux, quantite_devis, type_maison, total) 
                        SELECT @idDeviseClient, categorie_travaux, designation_travaux, unite_travaux, pu_travaux, quantite_devis, type_maison, total_devis 
                        FROM vue_devis
                        WHERE type_maison = @typeMaison";


            using (NpgsqlCommand cmd = new NpgsqlCommand(sql, conn))
            {
                cmd.Parameters.AddWithValue("@idDeviseClient", idDeviseClient);
                cmd.Parameters.AddWithValue("@typeMaison", typeMaison);

                cmd.ExecuteNonQuery();
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
        }
    }
    public static List<DetailsDeviseClient> SelectByIdDeviseClient(NpgsqlConnection conn, string idDeviseClient)
    {
        List<DetailsDeviseClient> detailsDevises = new List<DetailsDeviseClient>();
        try
        {
            string sql = "SELECT id_devise_client, categorie_travaux, designation_travaux, unite_travaux, pu_travaux, quantite_devis, type_maison, total FROM details_devise_client WHERE id_devise_client = @idDeviseClient";
            using (NpgsqlCommand cmd = new NpgsqlCommand(sql, conn))
            {
                cmd.Parameters.AddWithValue("@idDeviseClient", idDeviseClient);

                using (NpgsqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        DetailsDeviseClient detailsDevise = new DetailsDeviseClient(
                            reader["id_devise_client"].ToString(),
                            reader["categorie_travaux"].ToString(),
                            reader["designation_travaux"].ToString(),
                            reader["unite_travaux"].ToString(),
                            Convert.ToDecimal(reader["pu_travaux"]),
                            Convert.ToDecimal(reader["quantite_devis"]),
                            reader["type_maison"].ToString(),
                            Convert.ToDecimal(reader["total"])
                        );
                        detailsDevise.PU = Convert.ToDecimal(reader["pu_travaux"]);
                        detailsDevises.Add(detailsDevise);
                    }
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
        }
        return detailsDevises;
    }
}
