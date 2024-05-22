using Npgsql;
using Utils.helper;
using System;

public class ImportPayement
{
    public string ref_devis { get; set; }
    public string ref_paiement { get; set; }
    public string date_paiement { get; set; }
    public string montant { get; set; }

    public ImportPayement()
    {
    }

    public ImportPayement(string ref_devis, string ref_paiement, string date_paiement, string montant)
    {
        this.ref_devis = ref_devis;
        this.ref_paiement = ref_paiement;
        this.date_paiement = date_paiement;
        this.montant = montant;
    }

    public void Insert(NpgsqlConnection conn)
    {
        try
        {
            string sql = "INSERT INTO ImportPayement (ref_devis, ref_paiement, date_paiement, montant) VALUES (@RefDevis, @RefPaiement, @DatePaiement, @Montant)";
            using (NpgsqlCommand cmd = new NpgsqlCommand(sql, conn))
            {
                cmd.Parameters.AddWithValue("@RefDevis", ref_devis);
                cmd.Parameters.AddWithValue("@RefPaiement", ref_paiement);
                cmd.Parameters.AddWithValue("@DatePaiement", date_paiement);
                cmd.Parameters.AddWithValue("@Montant", montant);

                cmd.ExecuteNonQuery();
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
        }
    }

    public static void InsertPayementFromImport(NpgsqlConnection conn)
    {
        try
        {
            string sql = @"
                INSERT INTO Payement (id_payement, id_devise_client, date, montant, reference)
                SELECT
                    ref_paiement AS id_payement,
                    ref_devis as id_devise_client,
                    TO_DATE(date_paiement, 'DD-MM-YYYY') AS date,
                    CAST(REPLACE(montant, ',', '.') AS DECIMAL) AS montant,
                    ref_paiement AS reference
                FROM ImportPayement
                WHERE ref_paiement not in (select reference from payement);
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

}
