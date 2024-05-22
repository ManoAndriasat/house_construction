using Npgsql;
using System;
using System.Collections.Generic;

public class PayementDetails
{
    public string IdDeviseClient { get; set; }
    public string Users { get; set; }
    public decimal Total { get; set; }
    public decimal Reste { get; set; }
    public decimal Payer { get; set; }
    public DateTime Date { get; set; }
    public string Designation { get; set; }
    public string Finition { get; set; }
    public string TypeMaison { get; set; }
    public decimal TauxFinition { get; set; }
    public decimal Pourcentage { get; set; }
    public decimal DureeConstruction { get; set; }

    public PayementDetails() { }

    public PayementDetails(string idDeviseClient, string users, decimal total, decimal reste, decimal payer, DateTime date, string designation, string finition, string typeMaison, decimal tauxFinition, decimal dureeConstruction)
    {
        IdDeviseClient = idDeviseClient;
        Users = users;
        Total = total;
        Reste = reste;
        Payer = payer;
        Date = date;
        Designation = designation;
        Finition = finition;
        TypeMaison = typeMaison;
        TauxFinition = tauxFinition;
        DureeConstruction = dureeConstruction;
    }

    public static List<PayementDetails> GetAll(NpgsqlConnection conn)
    {
        List<PayementDetails> payements = new List<PayementDetails>();

        string sql = "SELECT id_devise_client, users, total, reste, payer, date, designation, finition, type_maison, taux_finition, duree_construction FROM vue_payement_final order by id_devise_client asc";
        using (NpgsqlCommand cmd = new NpgsqlCommand(sql, conn))
        {
            using (NpgsqlDataReader reader = cmd.ExecuteReader())
            {
                while (reader.Read())
                
                {
                    PayementDetails payement = new PayementDetails(
                        reader["id_devise_client"].ToString(),
                        reader["users"].ToString(),
                        Convert.ToDecimal(reader["total"]),
                        Convert.ToDecimal(reader["reste"]),
                        Convert.ToDecimal(reader["payer"]),
                        Convert.ToDateTime(reader["date"]),
                        reader["designation"].ToString(),
                        reader["finition"].ToString(),
                        reader["type_maison"].ToString(),
                        Convert.ToDecimal(reader["taux_finition"]),
                        Convert.ToDecimal(reader["duree_construction"])
                    );
                    payements.Add(payement);
                }
            }
        }
        return payements;
    }

    public static List<PayementDetails> GetByUsers(NpgsqlConnection conn, string user)
    {
        List<PayementDetails> payements = new List<PayementDetails>();

        string sql = "SELECT id_devise_client, users, total, reste, payer, date, designation, finition, type_maison, taux_finition, duree_construction FROM vue_payement_final WHERE users = @user";
        using (NpgsqlCommand cmd = new NpgsqlCommand(sql, conn))
        {
            cmd.Parameters.AddWithValue("@user", user);
            using (NpgsqlDataReader reader = cmd.ExecuteReader())
            {
                while (reader.Read())
                {
                    PayementDetails payement = new PayementDetails(
                        reader["id_devise_client"].ToString(),
                        reader["users"].ToString(),
                        Convert.ToDecimal(reader["total"]),
                        Convert.ToDecimal(reader["reste"]),
                        Convert.ToDecimal(reader["payer"]),
                        Convert.ToDateTime(reader["date"]),
                        reader["designation"].ToString(),
                        reader["finition"].ToString(),
                        reader["type_maison"].ToString(),
                        Convert.ToDecimal(reader["taux_finition"]),
                        Convert.ToDecimal(reader["duree_construction"])
                    );
                    payements.Add(payement);
                }
            }
        }
        return payements;
    }

    public static PayementDetails GetById(NpgsqlConnection conn, string id)
    {
        PayementDetails payements = new PayementDetails();

        string sql = "SELECT id_devise_client, users, total, reste, payer, date, designation, finition, type_maison, taux_finition, duree_construction FROM vue_payement_final WHERE id_devise_client = @id";
        using (NpgsqlCommand cmd = new NpgsqlCommand(sql, conn))
        {
            cmd.Parameters.AddWithValue("@id", id);
            using (NpgsqlDataReader reader = cmd.ExecuteReader())
            {
                while (reader.Read())
                {
                    PayementDetails payement = new PayementDetails(
                        reader["id_devise_client"].ToString(),
                        reader["users"].ToString(),
                        Convert.ToDecimal(reader["total"]),
                        Convert.ToDecimal(reader["reste"]),
                        Convert.ToDecimal(reader["payer"]),
                        Convert.ToDateTime(reader["date"]),
                        reader["designation"].ToString(),
                        reader["finition"].ToString(),
                        reader["type_maison"].ToString(),
                        Convert.ToDecimal(reader["taux_finition"]),
                        Convert.ToDecimal(reader["duree_construction"])
                    );
                    payements=payement;
                }
            }
        }
        return payements;
    }


    public static double GetSumTotal(NpgsqlConnection conn)
    {
        double sumTotal = 0.0;

        string sql = "SELECT SUM(total) AS sum_total FROM vue_payement_final";
        using (NpgsqlCommand cmd = new NpgsqlCommand(sql, conn))
        {
            using (NpgsqlDataReader reader = cmd.ExecuteReader())
            {
                if (reader.Read())
                {
                    sumTotal = Convert.ToDouble(reader["sum_total"]);
                }
            }
        }
        return sumTotal;
    }

    public static double GetSumPayementTotal(NpgsqlConnection conn)
    {
        double sumTotal = 0.0;

        string sql = "SELECT SUM(payer) AS sum_total FROM vue_payement_final";
        using (NpgsqlCommand cmd = new NpgsqlCommand(sql, conn))
        {
            using (NpgsqlDataReader reader = cmd.ExecuteReader())
            {
                if (reader.Read())
                {
                    sumTotal = Convert.ToDouble(reader["sum_total"]);
                }
            }
        }
        return sumTotal;
    }

    public static List<(DateTime Mois, double Somme)> GetSommesTotalMensuelles(NpgsqlConnection conn)
    {
        List<(DateTime Mois, double Somme)> sommesMensuelles = new List<(DateTime, double)>();

        string sql = "SELECT mois, somme FROM vue_payement_mensuel";
        using (NpgsqlCommand cmd = new NpgsqlCommand(sql, conn))
        {
            using (NpgsqlDataReader reader = cmd.ExecuteReader())
            {
                while (reader.Read())
                {
                    DateTime mois = Convert.ToDateTime(reader["mois"]);
                    double sommeTotal = Convert.ToDouble(reader["somme"]);
                    sommesMensuelles.Add((mois, sommeTotal));
                }
            }
        }
        return sommesMensuelles;
    }

    public static List<(DateTime Annee, double Somme)> GetSommesTotalAnnuel(NpgsqlConnection conn)
    {
        List<(DateTime Annee, double Somme)> sommesAnnuel = new List<(DateTime, double)>();

        string sql = "SELECT annee, somme FROM  vue_payement_annuel";
        using (NpgsqlCommand cmd = new NpgsqlCommand(sql, conn))
        {
            using (NpgsqlDataReader reader = cmd.ExecuteReader())
            {
                while (reader.Read())
                {
                    DateTime annee = Convert.ToDateTime(reader["annee"]);
                    double sommeTotal = Convert.ToDouble(reader["somme"]);
                    sommesAnnuel.Add((annee, sommeTotal));
                }
            }
        }
        return sommesAnnuel;
    }
}
