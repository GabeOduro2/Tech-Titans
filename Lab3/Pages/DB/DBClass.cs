using Lab3.Pages.DataClasses;
using System.Data;
using System.Data.SqlClient;

namespace Lab3.Pages.DB
{
    public class DBClass
    {
        // Connection Object at Data Field Level
        public static SqlConnection Lab3DBConnection = new SqlConnection();
        public static SqlConnection Lab3AUTHConnection = new SqlConnection();

        // Connection String - How to find and connect to DB
        private static readonly String? Lab3DBConnString =
            "Server=Localhost;Database=Lab3;Trusted_Connection=True";

        private static readonly String? Lab3AUTHConnString =
            "Server=Localhost;Database=AUTH;Trusted_Connection=True";

        //Basic Knowledge Reader
        public static SqlDataReader KnowledgeReader()
        {
            SqlCommand cmdUserRead = new SqlCommand();
            cmdUserRead.Connection = Lab3DBConnection;
            cmdUserRead.Connection.ConnectionString = Lab3DBConnString;
            cmdUserRead.CommandText = "SELECT * FROM KNOWLEDGEITEM";
            cmdUserRead.Connection.Open(); // Open connection here, close in Model!

            SqlDataReader tempReader = cmdUserRead.ExecuteReader(); // Or Scalar/ NonQuery

            return tempReader;
        }
        public static int LoginQuery(string loginQuery)
        {
            // This method expects to receive an SQL SELECT
            // query that uses the COUNT command.

            SqlCommand cmdLogin = new SqlCommand();
            cmdLogin.Connection = Lab3DBConnection;
            cmdLogin.Connection.ConnectionString = Lab3DBConnString;
            cmdLogin.CommandText = loginQuery;
            cmdLogin.Connection.Open();

            // ExecuteScalar() returns back data type Object
            // Use a typecast to convert this to an int.
            // Method returns first column of first row.
            int rowCount = (int)cmdLogin.ExecuteScalar();

            return rowCount;
        }

        // Can run and return results for any query, if results exist.
        // Query is passed from the invoking code.
        public static SqlDataReader GeneralReaderQuery(string sqlQuery, SqlParameter[] parameters)
        {

            SqlCommand cmdUserRead = new SqlCommand();
            cmdUserRead.Connection = Lab3DBConnection;
            cmdUserRead.Connection.ConnectionString = Lab3DBConnString;
            cmdUserRead.CommandText = sqlQuery;

            // Adds parameters if present
            if (parameters != null)
            {
                cmdUserRead.Parameters.AddRange(parameters);
            }

            cmdUserRead.Connection.Open();
            SqlDataReader tempReader = cmdUserRead.ExecuteReader();

            return tempReader;
        }

        public static SqlDataReader GeneralReaderQuery(string sqlQuery)
        {
            SqlCommand cmdUserRead = new SqlCommand();
            cmdUserRead.Connection = Lab3DBConnection;
            cmdUserRead.Connection.ConnectionString = Lab3DBConnString;
            cmdUserRead.CommandText = sqlQuery;

            cmdUserRead.Connection.Open();
            SqlDataReader tempReader = cmdUserRead.ExecuteReader();

            return tempReader;
        }

        public static void InsertUser(User u)
        {
            String sqlQuery = "INSERT INTO [User] (UserName, FirstName, LastName, email, phone, address, UserType) " +
                              "VALUES (@Username, @FirstName, @LastName, @Email, @Phone, @Address, @UserType)";

            using (SqlConnection connection = new SqlConnection(Lab3DBConnString))
            {
                using (SqlCommand cmd = new SqlCommand(sqlQuery, connection))
                {
                    cmd.Parameters.AddWithValue("@Username", u.Username);
                    cmd.Parameters.AddWithValue("@FirstName", u.FirstName);
                    cmd.Parameters.AddWithValue("@LastName", u.LastName);
                    cmd.Parameters.AddWithValue("@Email", u.Email);
                    cmd.Parameters.AddWithValue("@Phone", u.Phone);
                    cmd.Parameters.AddWithValue("@Address", u.Address);
                    cmd.Parameters.AddWithValue("@UserType", u.UserType);

                    connection.Open(); // Open the connection

                    cmd.ExecuteNonQuery();
                }
            }

        }
        public static void InsertKnowledgeItem(KnowledgeItemModel k, string category)
        {
            String sqlQuery = "INSERT INTO KnowledgeItem (UserID, Title, Category, Information) VALUES (@UserID, @Title, @Category, @Information)";

            using (SqlConnection connection = new SqlConnection(Lab3DBConnString))
            {
                using (SqlCommand cmd = new SqlCommand(sqlQuery, Lab3DBConnection))
                {
                    cmd.Parameters.AddWithValue("@UserID", k.UserID);
                    cmd.Parameters.AddWithValue("@Title", k.Title);

                    if (string.IsNullOrEmpty(category))
                    {
                        cmd.Parameters.AddWithValue("@Category", DBNull.Value);
                    }
                    else
                    {
                        cmd.Parameters.AddWithValue("@Category", category);
                    }

                    cmd.Parameters.AddWithValue("@Information", k.Information);

                    Lab3DBConnection.Open();
                    cmd.ExecuteNonQuery();
                }
            }
        }
        public static void InsertQueryCSV(string sqlQuery)
        {
            Console.WriteLine($"Generated SQL Query: {sqlQuery}");

            SqlCommand cmdProductRead = new SqlCommand();

            using (SqlConnection connection = new SqlConnection(Lab3DBConnString))

            {

                using (SqlCommand cmd = new SqlCommand(sqlQuery, connection))
                {
                    if (connection.State != ConnectionState.Open)

                    {
                        connection.Open();
                    }
                    cmd.ExecuteNonQuery();
                }
            }
        }
        public static SqlDataReader GetMessages()
        {
            string sqlQuery = "SELECT * FROM Chat";

            SqlCommand cmdGetMessages = new SqlCommand(sqlQuery, Lab3DBConnection);

            Lab3DBConnection.Open();

            SqlDataReader tempReader = cmdGetMessages.ExecuteReader();

            return tempReader;
        }

        public static void InsertNewCollabArea(CollabClass c)
        {
            String sqlQuery = "INSERT INTO Collaboration " +
                "(Name, Chat) VALUES('";
            using (SqlConnection connection = new SqlConnection("Lab3DBConnString"))
            {
                using (SqlCommand cmd = new SqlCommand(sqlQuery, Lab3DBConnection))
                {
                    cmd.Parameters.AddWithValue("@Name", c.Name);
                    cmd.Parameters.AddWithValue("@Chats", c.Chats);

                    Lab3DBConnection.Open();
                    cmd.ExecuteNonQuery();
                }
            }
        }

        public static void InsertNewPlan(Plan p)
        {
            // Construct the SQL query to insert the plan's name and number of steps
            string sqlQuery = "INSERT INTO [Plan] (Name, NumSteps) VALUES (@Name, @NumSteps); SELECT SCOPE_IDENTITY();";

            // Create a new SqlCommand object and set its properties
            using (SqlCommand cmd = new SqlCommand(sqlQuery, Lab3DBConnection))
            {
                cmd.Parameters.AddWithValue("@Name", p.Name);
                cmd.Parameters.AddWithValue("@NumSteps", p.Steps.Count);

                Lab3DBConnection.Open();

                // Execute the SQL command to insert the plan and retrieve the generated PlanID
                int planID = Convert.ToInt32(cmd.ExecuteScalar());

                // Insert each step into the Step table
                foreach (var step in p.Steps)
                {
                    InsertStep(planID, step); // Call the method to insert the step
                }

                Lab3DBConnection.Close();
            }
        }

        // Method to insert a step into the Step table
        private static void InsertStep(int planID, Step step)
        {
            // Construct the SQL query to insert a step into the Step table
            string sqlQuery = "INSERT INTO Step (Name, Priority, Information, PlanID) VALUES (@Name, @Priority, @Information, @PlanID)";

            // Create a new SqlCommand object and set its properties
            using (SqlCommand cmd = new SqlCommand(sqlQuery, Lab3DBConnection))
            {
                // Add parameters to the SqlCommand object to prevent SQL injection and ensure data integrity
                cmd.Parameters.AddWithValue("@Name", step.Name);
                cmd.Parameters.AddWithValue("@Priority", step.Priority);
                cmd.Parameters.AddWithValue("@Information", step.Information);
                cmd.Parameters.AddWithValue("@PlanID", planID);

                // Execute the SQL command
                cmd.ExecuteNonQuery();
            }
        }

        public static SqlDataReader SingleKnowledgeReader(int knowledgeID)
        {
            SqlCommand cmdProductRead = new SqlCommand();
            cmdProductRead.Connection = Lab3DBConnection;
            cmdProductRead.Connection.ConnectionString =
            Lab3DBConnString;
            cmdProductRead.CommandText = "SELECT * FROM KnowledgeItem WHERE KnowledgeItem = " + knowledgeID;
            cmdProductRead.Connection.Open();
            SqlDataReader tempReader = cmdProductRead.ExecuteReader();
            return tempReader;
        }

        public static bool StoredProcedureLogin(string Username, string Password)
        {
            // Command type invokes stored precedure with parameters. Must match parameters exactly
            SqlCommand cmdLogin = new SqlCommand();
            cmdLogin.Connection = Lab3AUTHConnection;
            cmdLogin.Connection.ConnectionString = Lab3AUTHConnString;
            cmdLogin.CommandType = System.Data.CommandType.StoredProcedure;
            cmdLogin.Parameters.AddWithValue("@Username", Username);

            // CommandText = name of stored procedure in DB
            cmdLogin.CommandText = "sp_Lab3Login";
            cmdLogin.Connection.Open();

            using (SqlDataReader hashReader = cmdLogin.ExecuteReader())
            {
                if (hashReader.Read())
                {
                    string correctHash = hashReader["Password"].ToString();

                    if (PasswordHash.ValidatePassword(Password, correctHash))
                    {
                        return true;
                    }
                }
            }
            return false;
        }
        public static void CreateHashedUser(string Username, string Password)
        {
            string loginQuery =
                "INSERT INTO HashedCredentials (Username,Password) values (@Username, @Password)";

            SqlCommand cmdLogin = new SqlCommand();
            cmdLogin.Connection = Lab3AUTHConnection;
            cmdLogin.Connection.ConnectionString = Lab3AUTHConnString;

            cmdLogin.CommandText = loginQuery;
            cmdLogin.Parameters.AddWithValue("@Username", Username);
            cmdLogin.Parameters.AddWithValue("@Password", PasswordHash.HashPassword(Password));

            cmdLogin.Connection.Open();

            cmdLogin.ExecuteNonQuery();
        }

        public static void InsertChatMessage(Chat chat)
        {
            string sqlQuery = "INSERT INTO Chat (Username, Message, Timestamp) VALUES (@Username, @Message, @Timestamp)";

            using (SqlConnection connection = new SqlConnection(Lab3DBConnString))
            {
                using (SqlCommand cmd = new SqlCommand(sqlQuery, connection))
                {
                    cmd.Parameters.AddWithValue("@Username", chat.Username);
                    cmd.Parameters.AddWithValue("@Message", chat.Message);
                    cmd.Parameters.AddWithValue("@Timestamp", chat.Timestamp);

                    connection.Open(); // Open the connection

                    cmd.ExecuteNonQuery();
                }
            }
        }

        public static List<Chat> GetChatMessages()
        {
            List<Chat> chatMessages = new List<Chat>();

            string sqlQuery = "SELECT Username, Message, Timestamp FROM Chat";

            using (SqlConnection connection = new SqlConnection(Lab3DBConnString))
            {
                using (SqlCommand cmd = new SqlCommand(sqlQuery, connection))
                {
                    connection.Open();

                    SqlDataReader reader = cmd.ExecuteReader();

                    while (reader.Read())
                    {
                        string username = reader["Username"].ToString();
                        string message = reader["Message"].ToString();
                        DateTime timestamp = Convert.ToDateTime(reader["Timestamp"]);

                        Chat chat = new Chat
                        {
                            Username = username,
                            Message = message,
                            Timestamp = timestamp
                        };

                        chatMessages.Add(chat);
                    }
                }
            }

            return chatMessages;
        }
    }
}
