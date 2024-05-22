using Npgsql;
using System;

public class Payement
{
    public string IdPayement { get; set; }
    public string IdDeviseClient { get; set; }
    public DateTime Date { get; set; }
    public decimal Montant { get; set; }
    public string Reference { get; set; }

    public Payement()
    {
    }

    public Payement(string idPayement, string idDeviseClient, DateTime date, decimal montant, string reference)
    {
        IdPayement = idPayement;
        IdDeviseClient = idDeviseClient;
        Date = date;
        Montant = montant;
        Reference = reference;
    }

    public void Insert(NpgsqlConnection conn)
    {
        // PayementDetails payement = PayementDetails.GetById(conn,IdDeviseClient);
        // if(Montant>payement.Reste ){
        //     throw new Exception("tsy mety Montant superieur :" + payement.Reste);
        // }
        // else if( Montant<0 ){
        //     throw new Exception("tsy mety Montant negatif");
        // }
        // else{
            string sql = "INSERT INTO Payement (id_payement, id_devise_client, date, montant,reference) VALUES (@IdPayement, @IdDeviseClient, @Date, @Montant,@Reference)";
            using (NpgsqlCommand cmd = new NpgsqlCommand(sql, conn))
            {
                cmd.Parameters.AddWithValue("@IdPayement", IdPayement);
                cmd.Parameters.AddWithValue("@IdDeviseClient", IdDeviseClient);
                cmd.Parameters.AddWithValue("@Date", Date);
                cmd.Parameters.AddWithValue("@Montant", Montant);
                cmd.Parameters.AddWithValue("@Reference", Reference);

                cmd.ExecuteNonQuery();
            }
        // }
    }

    public static List<Payement> GetByIdDevise(NpgsqlConnection conn, string idDeviseClient)
    {
        List<Payement> payments = new List<Payement>();
        try
        {
            string sql = "SELECT id_payement, id_devise_client, date, montant, reference FROM Payement WHERE id_devise_client = @IdDeviseClient";
            using (NpgsqlCommand cmd = new NpgsqlCommand(sql, conn))
            {
                cmd.Parameters.AddWithValue("@IdDeviseClient", idDeviseClient);

                using (NpgsqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        Payement payment = new Payement(
                            reader["id_payement"].ToString(),
                            reader["id_devise_client"].ToString(),
                            Convert.ToDateTime(reader["date"]),
                            Convert.ToDecimal(reader["montant"]),
                            reader["reference"].ToString()
                        );
                        payments.Add(payment);
                    }
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
        }
        return payments;
    }

    public static List<Payement> GetByMonth(NpgsqlConnection conn, int year, int month)
    {
        List<Payement> payments = new List<Payement>();
        try
        {
            string sql = "SELECT id_payement, id_devise_client, date, montant, reference FROM Payement WHERE EXTRACT(YEAR FROM date) = @Year AND EXTRACT(MONTH FROM date) = @Month";
            using (NpgsqlCommand cmd = new NpgsqlCommand(sql, conn))
            {
                cmd.Parameters.AddWithValue("@Year", year);
                cmd.Parameters.AddWithValue("@Month", month);

                using (NpgsqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        Payement payment = new Payement(
                            reader["id_payement"].ToString(),
                            reader["id_devise_client"].ToString(),
                            Convert.ToDateTime(reader["date"]),
                            Convert.ToDecimal(reader["montant"]),
                            reader["reference"].ToString()
                        );
                        payments.Add(payment);
                    }
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
        }
        return payments;
    }

    public static List<Payement> GetByYear(NpgsqlConnection conn, int year)
    {
        List<Payement> payments = new List<Payement>();
        try
        {
            string sql = "SELECT id_payement, id_devise_client, date, montant, reference FROM Payement WHERE EXTRACT(YEAR FROM date) = @Year";
            using (NpgsqlCommand cmd = new NpgsqlCommand(sql, conn))
            {
                cmd.Parameters.AddWithValue("@Year", year);

                using (NpgsqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        Payement payment = new Payement(
                            reader["id_payement"].ToString(),
                            reader["id_devise_client"].ToString(),
                            Convert.ToDateTime(reader["date"]),
                            Convert.ToDecimal(reader["montant"]),
                            reader["reference"].ToString()
                        );
                        payments.Add(payment);
                    }
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
        }
        return payments;
    }
}
