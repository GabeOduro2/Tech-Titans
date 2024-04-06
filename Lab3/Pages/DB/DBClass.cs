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
        //Methods for getting admin eff review chat
        public static SqlDataReader GetMessages()
        {
            string sqlQuery = "SELECT * FROM Chat";

            SqlCommand cmdGetMessages = new SqlCommand(sqlQuery, Lab3DBConnection);

            Lab3DBConnection.Open();

            SqlDataReader tempReader = cmdGetMessages.ExecuteReader();

            return tempReader;
        }

        public static SqlDataReader GetBudgetMessages()
        {
            string sqlQuery = "SELECT * FROM BudgetChat";

            SqlCommand cmdGetMessages = new SqlCommand(sqlQuery, Lab3DBConnection);

            Lab3DBConnection.Open();

            SqlDataReader tempReader = cmdGetMessages.ExecuteReader();

            return tempReader;
        }

        //Method for getting citizen communication chat messages
        public static SqlDataReader GetCitizenMessages()
        {
            string sqlQuery = "SELECT * FROM CitizenChat";

            SqlCommand cmdGetMessages = new SqlCommand(sqlQuery, Lab3DBConnection);

            Lab3DBConnection.Open();

            SqlDataReader tempReader = cmdGetMessages.ExecuteReader();

            return tempReader;
        }

        public static SqlDataReader GetEcoDevMessages()
        {
            string sqlQuery = "SELECT * FROM EcoDevChat";

            SqlCommand cmdGetMessages = new SqlCommand(sqlQuery, Lab3DBConnection);

            Lab3DBConnection.Open();

            SqlDataReader tempReader = cmdGetMessages.ExecuteReader();

            return tempReader;
        }

        public static SqlDataReader GetPolicyMessages()
        {
            string sqlQuery = "SELECT * FROM PolicyChat";

            SqlCommand cmdGetMessages = new SqlCommand(sqlQuery, Lab3DBConnection);

            Lab3DBConnection.Open();

            SqlDataReader tempReader = cmdGetMessages.ExecuteReader();

            return tempReader;
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

        //Methods for inserting chats into each individual chat
        public static void InsertChatMessage(Chat chat)
        {
            string sqlQuery = "INSERT INTO Chat (Message, Username, Timestamp) VALUES (@Message, @Username, @Timestamp)";

            using (SqlConnection connection = new SqlConnection(Lab3DBConnString))
            {
                using (SqlCommand cmd = new SqlCommand(sqlQuery, connection))
                {
                    cmd.Parameters.AddWithValue("@Message", chat.Message);
                    cmd.Parameters.AddWithValue("@Username", chat.Username);
                    cmd.Parameters.AddWithValue("@Timestamp", chat.Timestamp);

                    connection.Open();

                    cmd.ExecuteNonQuery();
                }
            }
        }

        public static void InsertBudgetChatMessage(Chat chat)
        {
            string sqlQuery = "INSERT INTO BudgetChat (Message, Username, Timestamp) VALUES (@Message, @Username, @Timestamp)";

            using (SqlConnection connection = new SqlConnection(Lab3DBConnString))
            {
                using (SqlCommand cmd = new SqlCommand(sqlQuery, connection))
                {
                    cmd.Parameters.AddWithValue("@Message", chat.Message);
                    cmd.Parameters.AddWithValue("@Username", chat.Username);
                    cmd.Parameters.AddWithValue("@Timestamp", chat.Timestamp);

                    connection.Open();

                    cmd.ExecuteNonQuery();
                }
            }
        }

        public static void InsertCitizenChatMessage(Chat chat)
        {
            string sqlQuery = "INSERT INTO CitizenChat (Message, Username, Timestamp) VALUES (@Message, @Username, @Timestamp)";

            using (SqlConnection connection = new SqlConnection(Lab3DBConnString))
            {
                using (SqlCommand cmd = new SqlCommand(sqlQuery, connection))
                {
                    cmd.Parameters.AddWithValue("@Message", chat.Message);
                    cmd.Parameters.AddWithValue("@Username", chat.Username);
                    cmd.Parameters.AddWithValue("@Timestamp", chat.Timestamp);

                    connection.Open();

                    cmd.ExecuteNonQuery();
                }
            }
        }

        public static void InsertEcoDevChatMessage(Chat chat)
        {
            string sqlQuery = "INSERT INTO EcoDevChat (Message, Username, Timestamp) VALUES (@Message, @Username, @Timestamp)";

            using (SqlConnection connection = new SqlConnection(Lab3DBConnString))
            {
                using (SqlCommand cmd = new SqlCommand(sqlQuery, connection))
                {
                    cmd.Parameters.AddWithValue("@Message", chat.Message);
                    cmd.Parameters.AddWithValue("@Username", chat.Username);
                    cmd.Parameters.AddWithValue("@Timestamp", chat.Timestamp);

                    connection.Open();

                    cmd.ExecuteNonQuery();
                }
            }
        }

        public static void InsertPolicyChatMessage(Chat chat)
        {
            string sqlQuery = "INSERT INTO PolicyChat (Message, Username, Timestamp) VALUES (@Message, @Username, @Timestamp)";

            using (SqlConnection connection = new SqlConnection(Lab3DBConnString))
            {
                using (SqlCommand cmd = new SqlCommand(sqlQuery, connection))
                {
                    cmd.Parameters.AddWithValue("@Message", chat.Message);
                    cmd.Parameters.AddWithValue("@Username", chat.Username);
                    cmd.Parameters.AddWithValue("@Timestamp", chat.Timestamp);

                    connection.Open();

                    cmd.ExecuteNonQuery();
                }
            }
        }

        //Methods for displaying chat messages in each functional area
        public static List<Chat> GetChatMessages()
        {
            List<Chat> chatMessages = new List<Chat>();

            string sqlQuery = "SELECT Message, Username, Timestamp FROM Chat";

            using (SqlConnection connection = new SqlConnection(Lab3DBConnString))
            {
                using (SqlCommand cmd = new SqlCommand(sqlQuery, connection))
                {
                    connection.Open();

                    SqlDataReader reader = cmd.ExecuteReader();

                    while (reader.Read())
                    {
                        string message = reader["Message"].ToString();
                        string username = reader["Username"].ToString();
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

        //Budget chat retrieval
        public static List<Chat> GetBudgetChatMessages()
        {
            List<Chat> chatMessages = new List<Chat>();

            string sqlQuery = "SELECT Message, Username, Timestamp FROM BudgetChat";

            using (SqlConnection connection = new SqlConnection(Lab3DBConnString))
            {
                using (SqlCommand cmd = new SqlCommand(sqlQuery, connection))
                {
                    connection.Open();

                    SqlDataReader reader = cmd.ExecuteReader();

                    while (reader.Read())
                    {
                        string message = reader["Message"].ToString();
                        string username = reader["Username"].ToString();
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

        //Citizen chat retrieval
        public static List<Chat> GetCitizenChatMessages()
        {
            List<Chat> chatMessages = new List<Chat>();

            string sqlQuery = "SELECT Message, Username, Timestamp FROM CitizenChat";

            using (SqlConnection connection = new SqlConnection(Lab3DBConnString))
            {
                using (SqlCommand cmd = new SqlCommand(sqlQuery, connection))
                {
                    connection.Open();

                    SqlDataReader reader = cmd.ExecuteReader();

                    while (reader.Read())
                    {
                        string message = reader["Message"].ToString();
                        string username = reader["Username"].ToString();
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

        //Eco dev chat retrieval
        public static List<Chat> GetEcoDevChatMessages()
        {
            List<Chat> chatMessages = new List<Chat>();

            string sqlQuery = "SELECT Message, Username, Timestamp FROM EcoDevChat";

            using (SqlConnection connection = new SqlConnection(Lab3DBConnString))
            {
                using (SqlCommand cmd = new SqlCommand(sqlQuery, connection))
                {
                    connection.Open();

                    SqlDataReader reader = cmd.ExecuteReader();

                    while (reader.Read())
                    {
                        string message = reader["Message"].ToString();
                        string username = reader["Username"].ToString();
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

        public static List<Chat> GetPolicyChatMessages()
        {
            List<Chat> chatMessages = new List<Chat>();

            string sqlQuery = "SELECT Message, Username, Timestamp FROM PolicyChat";

            using (SqlConnection connection = new SqlConnection(Lab3DBConnString))
            {
                using (SqlCommand cmd = new SqlCommand(sqlQuery, connection))
                {
                    connection.Open();

                    SqlDataReader reader = cmd.ExecuteReader();

                    while (reader.Read())
                    {
                        string message = reader["Message"].ToString();
                        string username = reader["Username"].ToString();
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
