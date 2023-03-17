using System.Data.SqlClient;
using System.Text;
using Xpto.Core.Shared.Sql;

namespace Xpto.Core.Adresses
{
    internal class AddressRepository
    {
        private readonly string _connectionString = "Data Source=NT20\\ROBSONDB;Initial Catalog=DB_XPTO;Persist Security Info=True;User ID=sa;Password=R55108105";

        public Address Insert(Address address)
        {
            //passar o address já com o customerCode
            var insertQuery = new StringBuilder()
            .AppendLine("INSERT INTO [tb_customer_address]")
                .AppendLine("(")
                .AppendLine("[id],")
                .AppendLine("[customer_code],")
                .AppendLine("[type],")
                .AppendLine("[street],")
                .AppendLine("[complement],")
                .AppendLine("[district],")
                .AppendLine("[city],")
                .AppendLine("[state],")
                .AppendLine("[zip_code],")
                .AppendLine("[note]")
                .AppendLine(")")
                .AppendLine("VALUES")
                .AppendLine("(")
                .AppendLine("@id,")
                .AppendLine("@customer_code,")
                .AppendLine("@type,")
                .AppendLine("@street,")
                .AppendLine("@complement,")
                .AppendLine("@district,")
                .AppendLine("@city,")
                .AppendLine("@state,")
                .AppendLine("@zip_code,")
                .AppendLine("@note")
                .AppendLine(")");

            using (var sqlCommand = Connection(insertQuery))
            {
                SetParameters(address, sqlCommand);
                sqlCommand.ExecuteNonQuery();
            }

            return address;
        }

        public List<Address> Get(int customerCode)
        {
            var addresses = new List<Address>();

            StringBuilder getQuery = GetBaseQuery();
            getQuery.AppendLine("WHERE customer_code = @code");

            using (var sqlCommand = Connection(getQuery))
            {
                sqlCommand.Parameters.Add(new SqlParameter("@code", customerCode.GetDbValue()));

                var dataReader = sqlCommand.ExecuteReader();
                while (dataReader.Read())
                {
                    var address = LoadDataReader(dataReader);
                    addresses.Add(address);
                }
            }

            return addresses;

        }

        public void UpdateNumberOfAddress(Address address, int number)
        {
            var updateNumberQuery = new StringBuilder()
                .AppendLine("UPDATE tb_customer_address")
                .AppendLine("SET number = @number")
                .AppendLine("WHERE id = @id");

            using (var sqlCommand = Connection(updateNumberQuery))
            {
                sqlCommand.Parameters.Add(new SqlParameter("@number", number.GetDbValue()));
                sqlCommand.Parameters.Add(new SqlParameter("@id", address.Id.GetDbValue()));

                sqlCommand.ExecuteNonQuery();
            }
                
        }

        public void UpdateAddress(Address address, Guid id)
        {

            var updateNumberQuery = new StringBuilder()
                .AppendLine("UPDATE tb_customer_address")
                .AppendLine("SET street = @street,")
                .AppendLine("SET complement = @complement,")
                .AppendLine("SET district = @district,")
                .AppendLine("SET city = @city,")
                .AppendLine("SET state = @state,")
                .AppendLine("SET zip_code = @zip_code,")
                .AppendLine("SET note = @note")
                .AppendLine("WHERE id = @id");

            using (var sqlCommand = Connection(updateNumberQuery))
            {
                SetParameters(address, sqlCommand);

                sqlCommand.ExecuteNonQuery();
            }

        }

        private void SetParameters(Address address, SqlCommand sqlCommand)
        {
            sqlCommand.Parameters.Add(new SqlParameter("@id", address.Id.GetDbValue()));
            sqlCommand.Parameters.Add(new SqlParameter("@customer_code", address.CustomerCode.GetDbValue()));
            sqlCommand.Parameters.Add(new SqlParameter("@type", address.Type.GetDbValue()));
            sqlCommand.Parameters.Add(new SqlParameter("@street", address.Street.GetDbValue()));
            sqlCommand.Parameters.Add(new SqlParameter("@complement", address.Complement.GetDbValue()));
            sqlCommand.Parameters.Add(new SqlParameter("@district", address.District.GetDbValue()));
            sqlCommand.Parameters.Add(new SqlParameter("@city", address.City.GetDbValue()));
            sqlCommand.Parameters.Add(new SqlParameter("@state", address.State.GetDbValue()));
            sqlCommand.Parameters.Add(new SqlParameter("@zip_code", address.ZipCode.GetDbValue()));
            sqlCommand.Parameters.Add(new SqlParameter("@note", address.Note.GetDbValue()));
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
                .AppendLine(",[street]")
                .AppendLine(",[number]")
                .AppendLine(",[complement]")
                .AppendLine(",[district]")
                .AppendLine(",[city]")
                .AppendLine(",[state]")
                .AppendLine(",[zip_code]")
                .AppendLine(",[note]")
                .AppendLine("FROM [DB_XPTO].[dbo].[tb_customer_address]");

            return getQuery;
        }

        private Address LoadDataReader(SqlDataReader dataReader)
        {
            return new Address
            {
                Id = dataReader.GetGuid("id"),
                CustomerCode = dataReader.GetInt32("customer_code"),
                Type = dataReader.GetString("type"),
                Street = dataReader.GetString("street"),
                Number = dataReader.GetString("number"),
                Complement = dataReader.GetString("complement"),
                District = dataReader.GetString("district"),
                City = dataReader.GetString("city"),
                State = dataReader.GetString("state"),
                ZipCode = dataReader.GetString("zip_code"),
                Note = dataReader.GetString("note")

            }; 
        }
    }
}
