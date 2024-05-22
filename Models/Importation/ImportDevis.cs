using Npgsql;
using System;
using Utils.helper;

public class ImportDevis
{
    public string client { get; set; }
    public string ref_devis { get; set; }
    public string type_maison { get; set; }
    public string finition { get; set; }
    public string taux_finition { get; set; }
    public string date_devis { get; set; }
    public string date_debut { get; set; }
    public string lieu { get; set; }

    public ImportDevis()
    {
    }

    public ImportDevis(string client, string ref_devis, string type_maison, string finition, string taux_finition, string date_devis, string date_debut, string lieu)
    {
        this.client = client;
        this.ref_devis = ref_devis;
        this.type_maison = type_maison;
        this.finition = finition;
        this.taux_finition = taux_finition;
        this.date_devis = date_devis;
        this.date_debut = date_debut;
        this.lieu = lieu;
    }

    public void Insert(NpgsqlConnection conn)
    {
        try
        {
            string sql = "INSERT INTO ImportDevis (client, ref_devis, type_maison, finition, taux_finition, date_devis, date_debut, lieu) VALUES (@Client, @RefDevis, @TypeMaison, @Finition, @TauxFinition, @DateDevis, @DateDebut, @Lieu)";
            using (NpgsqlCommand cmd = new NpgsqlCommand(sql, conn))
            {
                cmd.Parameters.AddWithValue("@Client", client);
                cmd.Parameters.AddWithValue("@RefDevis", ref_devis);
                cmd.Parameters.AddWithValue("@TypeMaison", type_maison);
                cmd.Parameters.AddWithValue("@Finition", finition);
                cmd.Parameters.AddWithValue("@TauxFinition", taux_finition);
                cmd.Parameters.AddWithValue("@DateDevis", date_devis);
                cmd.Parameters.AddWithValue("@DateDebut", date_debut);
                cmd.Parameters.AddWithValue("@Lieu", lieu);

                cmd.ExecuteNonQuery();
            }

        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
        }
    }

    public static void InsertUsers(NpgsqlConnection conn)
    {
        try
        {
            string sql = @"
                INSERT INTO users (numero)
                SELECT DISTINCT
                    client AS numero
                FROM ImportDevis
            ";
            using (NpgsqlCommand cmd = new NpgsqlCommand(sql, conn))
            {
                cmd.ExecuteNonQuery();
            }

        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
        }
    }

    public static void InsertDevisClient(NpgsqlConnection conn)
    {
        try
        {
            string sql = @"
                INSERT INTO devise_client (id, users, type_maison, finition, date, designation,lieu)
                SELECT DISTINCT
                    ref_devis AS id,
                    client AS users,
                    type_maison,
                    finition,
                    TO_DATE(date_devis, 'DD/MM/YYYY') AS date,
                    ref_devis AS designation,
                    lieu as lieu
                FROM ImportDevis
            ";
            using (NpgsqlCommand cmd = new NpgsqlCommand(sql, conn))
            {
                cmd.ExecuteNonQuery();
            }

        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
        }
    }

        public static void InsertFinition(NpgsqlConnection conn)
    {
        try
        {
            string sql = "INSERT INTO finition (id, nom, taux) SELECT DISTINCT TRIM(finition) AS id, TRIM(finition) AS nom, REPLACE(TRIM(REPLACE(taux_finition, '%', '')), ',', '.')::numeric AS taux FROM ImportDevis";
            using (NpgsqlCommand cmd = new NpgsqlCommand(sql, conn))
            {
                cmd.ExecuteNonQuery();
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
        }
    }
}
