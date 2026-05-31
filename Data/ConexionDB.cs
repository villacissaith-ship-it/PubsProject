using System;
using System.Data;
using System.Data.SqlClient;

namespace PubsProject.Data
{
    public class ConexionDB
    {
        // *** CAMBIA ESTOS DATOS SEGÚN TU SERVIDOR ***
        private static string connectionString =
            "Server=THEVICTOROG\\SQLEXPRESS;Database=pubs;Integrated Security=True;TrustServerCertificate=True;";

        public static SqlConnection ObtenerConexion()
        {
            return new SqlConnection(connectionString);
        }

        public static DataTable EjecutarConsulta(string query, SqlParameter[] parametros = null)
        {
            DataTable dt = new DataTable();
            using (SqlConnection conn = ObtenerConexion())
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand(query, conn);
                if (parametros != null)
                    cmd.Parameters.AddRange(parametros);
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                da.Fill(dt);
            }
            return dt;
        }

        public static int EjecutarComando(string query, SqlParameter[] parametros = null)
        {
            using (SqlConnection conn = ObtenerConexion())
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand(query, conn);
                if (parametros != null)
                    cmd.Parameters.AddRange(parametros);
                return cmd.ExecuteNonQuery();
            }
        }

        public static bool ValidarUsuario(string usuario, string clave)
        {
            // Usuario de prueba hardcodeado (puedes conectarlo a una tabla de usuarios)
            return usuario == "admin" && clave == "admin123";
        }
    }
}
