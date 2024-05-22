using Npgsql;
using System;
using System.Collections.Generic;

public class Finition
{
    public string Id { get; set; }
    public string Nom { get; set; }
    public double Taux { get; set; }

    public Finition()
    {
    }

    public Finition(string id, string nom, double taux)
    {
        Id = id;
        Nom = nom;
        Taux = taux;
    }

    public static List<Finition> GetAll(NpgsqlConnection conn)
    {
        List<Finition> finitions = new List<Finition>();
        try
        {
            string sql = "SELECT * FROM finition";
            using (NpgsqlCommand cmd = new NpgsqlCommand(sql, conn))
            {
                using (NpgsqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        Finition finition = new Finition(
                            reader["id"].ToString(),
                            reader["nom"].ToString(),
                            Convert.ToDouble(reader["taux"])
                        );
                        finitions.Add(finition);
                    }
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
        }
        return finitions;
    }
    public static void Update(NpgsqlConnection conn, string id, string nom, double taux)
    {
        try
        {
            string sql = "UPDATE finition SET nom = @Nom, taux = @Taux WHERE id = @Id";
            using (NpgsqlCommand cmd = new NpgsqlCommand(sql, conn))
            {
                cmd.Parameters.AddWithValue("@Id", id);
                cmd.Parameters.AddWithValue("@Nom", nom);
                cmd.Parameters.AddWithValue("@Taux", taux);
                
                cmd.ExecuteNonQuery();
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
        }
    }


    public static Finition GetById(NpgsqlConnection conn, string id)
    {
        Finition finition = null;
        try
        {
            string sql = "SELECT * FROM finition WHERE id = @Id";
            using (NpgsqlCommand cmd = new NpgsqlCommand(sql, conn))
            {
                cmd.Parameters.AddWithValue("@Id", id);
                using (NpgsqlDataReader reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        finition = new Finition(
                            reader["id"].ToString(),
                            reader["nom"].ToString(),
                            Convert.ToDouble(reader["taux"])
                        );
                    }
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
        }
        return finition;
    }

}
