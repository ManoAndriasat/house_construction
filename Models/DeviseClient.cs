using Npgsql;
using System;
using System.Collections.Generic;

public class DeviseClient
{
    public string Id { get; set; }
    public string Client { get; set; }
    public string TypeMaison { get; set; }
    public string Finition { get; set; }
    public DateTime Date { get; set; }
    public string Designation { get; set; }

    public DeviseClient()
    {
    }

    public DeviseClient(string id, string client, string typeMaison, string finition, DateTime date, string designation)
    {
        Id = id;
        Client = client;
        TypeMaison = typeMaison;
        Finition = finition;
        Date = date;
        Designation = designation;
    }

    public void Insert(NpgsqlConnection conn)
    {
        string sql = "INSERT INTO devise_client (id, users, type_maison, finition, date, designation) VALUES (@Id, @Client, @TypeMaison, @Finition, @Date, @Designation)";
        using (NpgsqlCommand cmd = new NpgsqlCommand(sql, conn))
        {
            cmd.Parameters.AddWithValue("@Id", Id);
            cmd.Parameters.AddWithValue("@Client", Client);
            cmd.Parameters.AddWithValue("@TypeMaison", TypeMaison);
            cmd.Parameters.AddWithValue("@Finition", Finition);
            cmd.Parameters.AddWithValue("@Date", Date);
            cmd.Parameters.AddWithValue("@Designation", Designation);
            cmd.ExecuteNonQuery();
        }
    }

    public static List<DeviseClient> GetByUsers(NpgsqlConnection conn, string users)
    {
        List<DeviseClient> devises = new List<DeviseClient>();
        try
        {
            string sql = "SELECT id, users, type_maison, finition, date, designation FROM devise_client WHERE users = @users";
            using (NpgsqlCommand cmd = new NpgsqlCommand(sql, conn))
            {
                cmd.Parameters.AddWithValue("@users", users);

                using (NpgsqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        DeviseClient devise = new DeviseClient(
                            reader["id"].ToString(),
                            reader["users"].ToString(),
                            reader["type_maison"].ToString(),
                            reader["finition"].ToString(),
                            Convert.ToDateTime(reader["date"]),
                            reader["designation"].ToString()
                        );
                        devises.Add(devise);
                    }
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
        }
        return devises;
    }
    
}
