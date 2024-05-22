using Npgsql;
using System;
using System.Collections.Generic;

public class FinitionDeviseClient
{
    public string IdDeviseClient { get; set; }
    public string Id { get; set; }
    public string Nom { get; set; }
    public decimal Taux { get; set; }

    public FinitionDeviseClient()
    {
    }

    public FinitionDeviseClient(string idDeviseClient, string id, string nom, decimal taux)
    {
        IdDeviseClient = idDeviseClient;
        Id = id;
        Nom = nom;
        Taux = taux;
    }

    public static void InsertFinitionClient(NpgsqlConnection conn, string idDeviseClient, string idFinition)
    {
        try
        {
            string sql = @"INSERT INTO finition_devise_client 
                            (id_devise_client, id, nom, taux) 
                            SELECT @idDeviseClient, id, nom, taux 
                            FROM finition 
                            WHERE id = @idFinition;";

            using (NpgsqlCommand cmd = new NpgsqlCommand(sql, conn))
            {
                cmd.Parameters.AddWithValue("@idDeviseClient", idDeviseClient);
                cmd.Parameters.AddWithValue("@idFinition", idFinition);

                cmd.ExecuteNonQuery();
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
        }
    }
}
