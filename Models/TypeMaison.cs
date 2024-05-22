using Npgsql;
using System;
using System.Collections.Generic;

public class TypeMaison
{
    public string Id { get; set; }
    public string Nom { get; set; }
    public string Details { get; set; }
    public decimal DureeConstruction { get; set; }
    public decimal Surface { get; set; }
    public decimal Total { get; set; }

    public TypeMaison()
    {
    }

    public TypeMaison(string id, string nom, string details, decimal dureeConstruction, decimal surface, decimal total) // Correction ici
    {
        Id = id;
        Nom = nom;
        Details = details;
        DureeConstruction = dureeConstruction;
        Surface = surface;
        Total = total; // Assigner le paramètre total à la propriété Total de la classe
    }

    public static List<TypeMaison> GetAll(NpgsqlConnection conn)
    {
        List<TypeMaison> typeMaisons = new List<TypeMaison>();

        string sql = "SELECT * FROM vue_type_maison";
        NpgsqlCommand cmd = new NpgsqlCommand(sql, conn);
        using (NpgsqlDataReader reader = cmd.ExecuteReader())
        {
            while (reader.Read())
            {
                TypeMaison typeMaison = new TypeMaison(
                    reader["id"].ToString(),
                    reader["nom"].ToString(),
                    reader["details"].ToString(),
                    Convert.ToDecimal(reader["duree_construction"]),
                    Convert.ToDecimal(reader["surface"]),
                    Convert.ToDecimal(reader["total"])
                );
                typeMaisons.Add(typeMaison);
            }
        }
        return typeMaisons;
    }
}
