using System.Data.SqlClient;
using System.Text;
using Xpto.Core.Shared.Sql;

namespace Xpto.Core.Emails
{
    public class EmailRepository
    {
        private readonly string _connectionString = "Data Source=NT20\\ROBSONDB;Initial Catalog=DB_XPTO;Persist Security Info=True;User ID=sa;Password=R55108105";

        public Email Insert(Email email)
        {
            //passar o address já com o customerCode
            var insertQuery = new StringBuilder()
            .AppendLine("INSERT INTO [tb_customer_email]")
                .AppendLine("(")
                .AppendLine("[id],")
                .AppendLine("[customer_code],")
                .AppendLine("[type],")
                .AppendLine("[address],")
                .AppendLine("[note]")
                .AppendLine(")")
                .AppendLine("VALUES")
                .AppendLine("(")
                .AppendLine("@id,")
                .AppendLine("@customer_code,")
                .AppendLine("@type,")
                .AppendLine("@address,")
                .AppendLine("@note")
                .AppendLine(")");

            using (var sqlCommand = Connection(insertQuery))
            {
                SetParameters(email, sqlCommand);
                sqlCommand.ExecuteNonQuery();
            }

            return email;
        }

        public List<Email> Get(int customerCode)
        {
            var emails = new List<Email>();

            StringBuilder getQuery = GetBaseQuery();
            getQuery.AppendLine("WHERE customer_code = @code");

            using (var sqlCommand = Connection(getQuery))
            {
                sqlCommand.Parameters.Add(new SqlParameter("@code", customerCode.GetDbValue()));

                var dataReader = sqlCommand.ExecuteReader();
                while (dataReader.Read())
                {
                    var email = LoadDataReader(dataReader);
                    emails.Add(email);
                }
            }

            return emails;

        }

        private void SetParameters(Email email, SqlCommand sqlCommand)
        {
            sqlCommand.Parameters.Add(new SqlParameter("@id", email.Id.GetDbValue()));
            sqlCommand.Parameters.Add(new SqlParameter("@customer_code", email.CustomerCode.GetDbValue()));
            sqlCommand.Parameters.Add(new SqlParameter("@type", email.Type.GetDbValue()));
            sqlCommand.Parameters.Add(new SqlParameter("@address", email.Address.GetDbValue()));
            sqlCommand.Parameters.Add(new SqlParameter("@note", email.Note.GetDbValue()));
        }

        private SqlCommand Connection(StringBuilder query)
        {
            var connection = new SqlConnection(_connectionString);

            connection.Open();

            var sqlCommand = connection.CreateCommand();
            sqlCommand.CommandText = query.ToString();

            return sqlCommand;
        }

        private StringBuilder GetBaseQuery()
        {
            StringBuilder getQuery = new StringBuilder()
                .AppendLine("SELECT ")
                .AppendLine("[id]")
                .AppendLine(",[customer_code]")
                .AppendLine(",[type]")
                .AppendLine(",[address]")
                .AppendLine(",[note]")
                .AppendLine("FROM [DB_XPTO].[dbo].[tb_customer_email]");

            return getQuery;
        }

        private Email LoadDataReader(SqlDataReader dataReader)
        {
            return new Email
            {
                Id = dataReader.GetGuid("id"),
                CustomerCode = dataReader.GetInt32("customer_code"),
                Type = dataReader.GetString("type"),
                Address = dataReader.GetString("address"),
                Note = dataReader.GetString("note")

            };
        }

    }
}
