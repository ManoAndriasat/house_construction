using Npgsql;
using System;
using Utils.helper;

public class ImportMaison
{
    public string type_maison { get; set; }
    public string description { get; set; }
    public string surface { get; set; }
    public string code_travaux { get; set; }
    public string type_travaux { get; set; }
    public string unite { get; set; }
    public string prix_unitaire { get; set; }
    public string quantite { get; set; }
    public string duree_travaux { get; set; }

    public ImportMaison()
    {
    }

    public ImportMaison(string type_maison, string description, string surface, string code_travaux, string type_travaux, string unite, string prix_unitaire, string quantite, string duree_travaux)
    {
        this.type_maison = type_maison;
        this.description = description;
        this.surface = surface;
        this.code_travaux = code_travaux;
        this.type_travaux = type_travaux;
        this.unite = unite;
        this.prix_unitaire = prix_unitaire;
        this.quantite = quantite;
        this.duree_travaux = duree_travaux;
    }

    public void Insert(NpgsqlConnection conn)
    {
        try
        {
            string sql = "INSERT INTO ImportMaison (type_maison, description, surface, code_travaux, type_travaux, unite, prix_unitaire, quantite, duree_travaux) VALUES (@TypeMaison, @Description, @Surface, @CodeTravaux, @TypeTravaux, @Unite, @PrixUnitaire, @Quantite, @DureeTravaux)";
            using (NpgsqlCommand cmd = new NpgsqlCommand(sql, conn))
            {
                cmd.Parameters.AddWithValue("@TypeMaison", type_maison);
                cmd.Parameters.AddWithValue("@Description", description);
                cmd.Parameters.AddWithValue("@Surface", surface);
                cmd.Parameters.AddWithValue("@CodeTravaux", code_travaux);
                cmd.Parameters.AddWithValue("@TypeTravaux", type_travaux);
                cmd.Parameters.AddWithValue("@Unite", unite);
                cmd.Parameters.AddWithValue("@PrixUnitaire", prix_unitaire);
                cmd.Parameters.AddWithValue("@Quantite", quantite);
                cmd.Parameters.AddWithValue("@DureeTravaux", duree_travaux);

                cmd.ExecuteNonQuery();
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
        }
    }
    public static void InsertTypeMaison(NpgsqlConnection conn)
    {
        try
        {
            string sql = @"
                INSERT INTO type_maison (id, nom, details, duree_construction, surface)
                SELECT DISTINCT
                    TRIM(type_maison) AS id,
                    TRIM(type_maison) AS nom,
                    description AS details,
                    CAST(duree_travaux AS decimal) AS duree_construction,
                    CAST(surface AS decimal) AS surface
                FROM ImportMaison
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

    public static void InsertCategorieTravaux(NpgsqlConnection conn)
    {
        try
        {
            string sql = @"
                INSERT INTO categorie_travaux (id_categorie, designation)
                SELECT DISTINCT
                    TRIM(code_travaux) AS id_categorie,
                    TRIM(code_travaux) AS designation
                FROM ImportMaison
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

    public static void InsertTravaux(NpgsqlConnection conn)
    {
        try
        {
            string sql = @"
                INSERT INTO travaux (id_travaux, id_categorie, designation, unite, pu)
                SELECT DISTINCT
                    TRIM(code_travaux) AS id_travaux,
                    TRIM(code_travaux) AS id_categorie,
                    TRIM(type_travaux) AS designation,
                    TRIM(unite) AS unite,
                    CAST(REPLACE(prix_unitaire, ',', '.') AS DECIMAL(10, 2)) AS pu
                FROM ImportMaison
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

    public static void InsertDevisTypeMaison(NpgsqlConnection conn)
    {
        try
        {
            string sql = @"
                INSERT INTO devis_type_maison (id_type_maison, id_travaux, quantite)
                SELECT
                    TRIM(type_maison) AS id_type_maison,
                    TRIM(code_travaux) AS id_travaux,
                    CAST(REPLACE(quantite, ',', '.') AS DECIMAL) AS quantite
                FROM ImportMaison
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
