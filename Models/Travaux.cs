using Npgsql;
using System;
using System.Collections.Generic;

public class Travaux
{
    public string IdTravaux { get; set; }
    public string IdCategorie { get; set; }
    public string Designation { get; set; }
    public string Unite { get; set; }
    public decimal PU { get; set; }

    public Travaux(){}

    public Travaux(string idTravaux, string idCategorie, string designation, string unite, decimal pu)
    {
        IdTravaux = idTravaux;
        IdCategorie = idCategorie;
        Designation = designation;
        Unite = unite;
        PU = pu;
    }

    public static void Update(NpgsqlConnection conn, string idTravaux, string idCategorie, string designation, string unite, decimal pu)
    {
        string sql = "UPDATE travaux SET id_categorie = @IdCategorie, designation = @Designation, unite = @Unite, pu = @PU WHERE id_travaux = @IdTravaux";
        using (NpgsqlCommand cmd = new NpgsqlCommand(sql, conn))
        {
            cmd.Parameters.AddWithValue("@IdCategorie", idCategorie);
            cmd.Parameters.AddWithValue("@Designation", designation);
            cmd.Parameters.AddWithValue("@Unite", unite);
            cmd.Parameters.AddWithValue("@PU", pu);
            cmd.Parameters.AddWithValue("@IdTravaux", idTravaux);

            cmd.ExecuteNonQuery();
        }
    }

    public static List<Travaux> GetAll(NpgsqlConnection conn)
    {
        List<Travaux> travauxList = new List<Travaux>();
        string sql = "SELECT id_travaux, id_categorie, designation, unite, pu FROM travaux";
        using (NpgsqlCommand cmd = new NpgsqlCommand(sql, conn))
        {
            using (NpgsqlDataReader reader = cmd.ExecuteReader())
            {
                while (reader.Read())
                {
                    Travaux travaux = new Travaux(
                        reader["id_travaux"].ToString(),
                        reader["id_categorie"].ToString(),
                        reader["designation"].ToString(),
                        reader["unite"].ToString(),
                        Convert.ToDecimal(reader["pu"])
                    );
                    travauxList.Add(travaux);
                }
            }
        }
        return travauxList;
    }

    public static Travaux GetById(NpgsqlConnection conn, string idTravaux)
    {
        string sql = "SELECT id_travaux, id_categorie, designation, unite, pu FROM travaux WHERE id_travaux = @IdTravaux";
        using (NpgsqlCommand cmd = new NpgsqlCommand(sql, conn))
        {
            cmd.Parameters.AddWithValue("@IdTravaux", idTravaux);
            using (NpgsqlDataReader reader = cmd.ExecuteReader())
            {
                if (reader.Read())
                {
                    Travaux travail = new Travaux(
                        reader["id_travaux"].ToString(),
                        reader["id_categorie"].ToString(),
                        reader["designation"].ToString(),
                        reader["unite"].ToString(),
                        Convert.ToDecimal(reader["pu"])
                    );
                    return travail;
                }
                else
                {
                    return null;
                }
            }
        }
    }

}
