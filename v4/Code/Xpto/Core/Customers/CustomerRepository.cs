// COMPANY: Ajinsoft
// AUTHOR: Uilan Coqueiro
// DATE: 2023-02-12

using System.Data;
using System.Data.SqlClient;
using System.Text;
using Xpto.Core.Adresses;
using Xpto.Core.Emails;
using Xpto.Core.Shared.Sql;

namespace Xpto.Core.Customers
{
    public class CustomerRepository
    {
        private readonly string connectionString = "Data Source=NT20\\ROBSONDB;Initial Catalog=DB_XPTO;Persist Security Info=True;User ID=sa;Password=R55108105";
        private AddressRepository addressRepository;
        private EmailRepository emailRepository;

        public CustomerRepository()
        {
            this.addressRepository = new AddressRepository();
            this.emailRepository = new EmailRepository();
        }

        public Customer Insert(Customer customer)
        {
            var commandText = new StringBuilder()
            .AppendLine(" INSERT INTO [tb_customer]")
            .AppendLine(" (")
            .AppendLine(" [id],")
            .AppendLine(" [name],")
            .AppendLine(" [nickname],")
            .AppendLine(" [birth_date],")
            .AppendLine(" [person_type],")
            .AppendLine(" [identity],")
            .AppendLine(" [note],")
            .AppendLine(" [creation_date],")
            .AppendLine(" [creation_user_id],")
            .AppendLine(" [creation_user_name],")
            .AppendLine(" [change_date],")
            .AppendLine(" [change_user_id],")
            .AppendLine(" [change_user_name]")
            .AppendLine(" )")
            .AppendLine(" VALUES")
            .AppendLine(" (")
            .AppendLine(" @id,")
            .AppendLine(" @name,")
            .AppendLine(" @nickname,")
            .AppendLine(" @birth_date,")
            .AppendLine(" @person_type,")
            .AppendLine(" @identity,")
            .AppendLine(" @note,")
            .AppendLine(" @creation_date,")
            .AppendLine(" @creation_user_id,")
            .AppendLine(" @creation_user_name,")
            .AppendLine(" @change_date,")
            .AppendLine(" @change_user_id,")
            .AppendLine(" @change_user_name")
            .AppendLine(" )")
            .AppendLine(" SET @code = SCOPE_IDENTITY(); ");

            using var connection = new SqlConnection(this.connectionString);

            connection.Open();

            var cm = connection.CreateCommand();

            cm.CommandText = commandText.ToString();

            var code = cm.Parameters.Add(new SqlParameter("@code", customer.Code) { Direction = ParameterDirection.Output });

            this.SetParameters(customer, cm);

            cm.ExecuteNonQuery();

            customer.Code = (int)code.Value;

            return customer;
        }

        public void Update(Customer customer)
        {
            var commandText = new StringBuilder()
            .AppendLine(" UPDATE [tb_customer]")
            .AppendLine(" SET")
            .AppendLine(" [id] = @id,")
            .AppendLine(" [code] = @code,")
            .AppendLine(" [name] = @name,")
            .AppendLine(" [nickname] = @nickname,")
            .AppendLine(" [birth_date] = @birth_date,")
            .AppendLine(" [person_type] = @person_type,")
            .AppendLine(" [note] = @note,")
            .AppendLine(" [creation_date] = @creation_date,")
            .AppendLine(" [creation_user_id] = @creation_user_id,")
            .AppendLine(" [creation_user_name] = @creation_user_name,")
            .AppendLine(" [change_date] = @change_date,")
            .AppendLine(" [change_user_id] = @change_user_id,")
            .AppendLine(" [change_user_name] = @change_user_name")
            .AppendLine(" WHERE [id] = @id");

            var connection = new SqlConnection(this.connectionString);
            connection.Open();

            var cm = connection.CreateCommand();

            cm.CommandText = commandText.ToString();

            this.SetParameters(customer, cm);

            cm.ExecuteNonQuery();

            connection.Close();
        }

        public int Delete(Guid id)
        {
            var commandText = new StringBuilder()
            .AppendLine(" DELETE FROM [tb_customer]")
            .AppendLine(" WHERE [id] = @id");

            var connection = new SqlConnection(this.connectionString);
            connection.Open();
            var cm = connection.CreateCommand();

            cm.CommandText = commandText.ToString();

            cm.Parameters.Add(new SqlParameter("@id", id));

            var result = cm.ExecuteNonQuery();

            connection.Close();

            return result;
        }

        public Customer Get(Guid id)
        {
            var commandText = this.GetSelectQuery()
                    .AppendLine(" WHERE [id] = @id");

            Customer customer = null;

            using (var connection = new SqlConnection(this.connectionString))
            {
                connection.Open();
                var cm = connection.CreateCommand();

                cm.CommandText = commandText.ToString();

                cm.Parameters.Add(new SqlParameter("@id", id));

                var dataReader = cm.ExecuteReader();

                while (dataReader.Read())
                {
                    customer = LoadDataReader(dataReader);
                }
            }

            return customer;

        }

        public Customer Get(int code)
        {
            var commandText = this.GetSelectQuery()
               .AppendLine(" WHERE [code] = @code");

            var connection = new SqlConnection(this.connectionString);
            connection.Open();
            var cm = connection.CreateCommand();

            cm.CommandText = commandText.ToString();

            cm.Parameters.Add(new SqlParameter("@code", code));

            var dataReader = cm.ExecuteReader();

            Customer customer = null;

            while (dataReader.Read())
            {
                customer = LoadDataReader(dataReader);
            }

            customer.Addresses = addressRepository.Get(code);
            customer.Emails = emailRepository.Get(code);

            connection.Close();

            return customer;

        }

        public IList<Customer> Find()
        {
            var listCustomers = new List<Customer>();

            var commandText = this.GetSelectQuery();

            var connection = new SqlConnection(this.connectionString);
            connection.Open();

            var cm = connection.CreateCommand();

            cm.CommandText = commandText.ToString();

            var dataReader = cm.ExecuteReader();

            while (dataReader.Read())
            {
                var customer = LoadDataReader(dataReader);

                listCustomers.Add(customer);
            }

            return listCustomers;
        }

        private static Customer LoadDataReader(SqlDataReader dataReader)
        {
            var customer = new Customer
            {
                Id = dataReader.GetGuid("id"),
                Code = dataReader.GetInt32("code"),
                Name = dataReader.GetString("name"),
                Nickname = dataReader.GetString("nickname"),
                BirthDate = dataReader.GetDateTime("birth_date"),
                PersonType = dataReader.GetString("person_type"),
                Note = dataReader.GetString("note"),
                CreationDate = dataReader.GetDateTime("creation_date"),
                CreationUserId = dataReader.GetGuid("creation_user_id"),
                CreationUserName = dataReader.GetString("creation_user_name"),
                ChangeDate = dataReader.GetDateTime("change_date"),
                ChangeUserId = dataReader.GetGuid("change_user_id"),
                ChangeUserName = dataReader.GetString("change_user_name")
            };

            //customer.PersonType = dataReader["person_type"].ToString();

            //customer.Note = dataReader["note"].ToString();

            //DateTime.TryParse(dataReader["creation_date"].ToString(), out var creationDate);
            //customer.CreationDate = creationDate;

            //Guid.TryParse(dataReader["creation_user_id"]?.ToString(), out var creationUserId);
            //customer.CreationUserId = creationUserId;

            //customer.CreationUserName = dataReader["creation_user_name"].ToString();

            //DateTime.TryParse(dataReader["change_date"].ToString(), out var changeDate);
            //customer.ChangeDate = changeDate;

            //Guid.TryParse(dataReader["change_user_id"]?.ToString(), out var changeUserId);
            //customer.ChangeUserId = changeUserId;

            //customer.ChangeUserName = dataReader["change_user_name"].ToString();

            return customer;
        }

        public DataTable LoadDataTable()
        {
            var commandText = GetSelectQuery();

            var connection = new SqlConnection(this.connectionString);
            connection.Open();

            var cm = connection.CreateCommand();
            cm.CommandText = commandText.ToString();

            var da = new SqlDataAdapter(cm);
            var dataTable = new DataTable();
            da.Fill(dataTable);
            connection.Close();
            da.Dispose();

            return dataTable;
        }

        public long Count()
        {
            var commandText = new StringBuilder()
                .AppendLine(" SELECT")
                .AppendLine(" COUNT(*) AS [COUNT]")
                .AppendLine(" FROM [tb_customer] AS A");

            var connection = new SqlConnection(this.connectionString);
            connection.Open();

            var cm = connection.CreateCommand();

            cm.CommandText = commandText.ToString();

            var count = cm.ExecuteScalar(); //Executar uma linha

            var result = count == null ? 0 : Convert.ToInt64(count);

            return result;
        }

        private StringBuilder GetSelectQuery()
        {
            var sb = new StringBuilder()
                .AppendLine(" SELECT")
                .AppendLine(" A.[id],")
                .AppendLine(" A.[code],")
                .AppendLine(" A.[name],")
                .AppendLine(" A.[nickname],")
                .AppendLine(" A.[birth_date],")
                .AppendLine(" A.[person_type],")
                .AppendLine(" A.[note],")
                .AppendLine(" A.[creation_date],")
                .AppendLine(" A.[creation_user_id],")
                .AppendLine(" A.[creation_user_name],")
                .AppendLine(" A.[change_date],")
                .AppendLine(" A.[change_user_id],")
                .AppendLine(" A.[change_user_name]")
                .AppendLine(" FROM [tb_customer] AS A");

            return sb;
        }

        private void SetParameters(Customer customer, SqlCommand cm)
        {
            cm.Parameters.Add(new SqlParameter("@id", customer.Id.GetDbValue()));
            cm.Parameters.Add(new SqlParameter("@name", customer.Name.GetDbValue()));
            cm.Parameters.Add(new SqlParameter("@nickname", customer.Nickname.GetDbValue()));
            cm.Parameters.Add(new SqlParameter("@birth_date", customer.BirthDate.GetDbValue()));
            cm.Parameters.Add(new SqlParameter("@person_type", customer.PersonType.GetDbValue()));
            cm.Parameters.Add(new SqlParameter("@identity", customer.Identity.GetDbValue()));
            cm.Parameters.Add(new SqlParameter("@note", customer.Note.GetDbValue()));
            cm.Parameters.Add(new SqlParameter("@creation_date", customer.CreationDate.GetDbValue()));
            cm.Parameters.Add(new SqlParameter("@creation_user_id", customer.CreationUserId.GetDbValue()));
            cm.Parameters.Add(new SqlParameter("@creation_user_name", customer.CreationUserName.GetDbValue()));
            cm.Parameters.Add(new SqlParameter("@change_date", customer.ChangeDate.GetDbValue()));
            cm.Parameters.Add(new SqlParameter("@change_user_id", customer.ChangeUserId.GetDbValue()));
            cm.Parameters.Add(new SqlParameter("@change_user_name", customer.ChangeUserName.GetDbValue()));
        }

    }
}
