﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;
using Newtonsoft;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace SQL_Terminal {
    public class Methods {
        
        public void PrintCursor(ConsoleColor color = ConsoleColor.White) {
            Console.ForegroundColor = color;
            Console.Write("> ");
            Console.ResetColor();
        }
        public void CommandOutput(string text, bool print = false, ConsoleColor color = ConsoleColor.White, bool extra_lines = true) {
            if (print) {
                Console.ForegroundColor = color;
                if (extra_lines) Console.WriteLine();                    
                this.Print(text);
                Console.ResetColor();
                if (extra_lines) Console.WriteLine("\n");
            } else {
                if (extra_lines) Console.WriteLine();                
                Console.Write(text);
                if (extra_lines) Console.WriteLine("\n");                
            }
        }
        public void HelpOutput(string description, string[] flags, string[]? alias = null, string[]? parameters = null) {
            Console.WriteLine();
            
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine(description);
            Console.ForegroundColor = ConsoleColor.Cyan;

            Console.WriteLine("\nFlags:");
            Console.ResetColor();
            for (int i = 0; i < flags.Length; ++i) {
                Console.WriteLine($" {flags[i]}");
                Thread.Sleep(10);
            }

            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("\nAlias:");
            Console.ResetColor();
            if (alias != null) {
                for (int i = 0; i < alias.Length; ++i) {
                    Console.WriteLine($" -- {alias[i]}");
                    Thread.Sleep(10);
                }
            } else {
                Console.ForegroundColor = ConsoleColor.DarkGray;
                Console.WriteLine(" --none");
                Console.ResetColor();
            }

            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine($"\nParameters:");
            Console.ResetColor();
            if (parameters != null) {
                for (int i = 0; i < parameters.Length; ++i) {
                    Console.WriteLine($"-- {parameters[i]}");
                    Thread.Sleep(10);
                }
            } else {
                Console.ForegroundColor = ConsoleColor.DarkGray;
                Console.WriteLine(" --none");
                Console.ResetColor();
            }

            Console.WriteLine();
        }
        public void SuccessOutput(string input, bool print = false) {
            Console.WriteLine();
            Console.ForegroundColor = ConsoleColor.Green;

            Console.WriteLine();
        }
        public void ErrorOutput(string input, bool print = false) {
            Console.WriteLine();
            Console.ForegroundColor = ConsoleColor.Red;
            if (print) {
                this.Print(input);
            } else {
                Console.WriteLine(input);
            }
            Console.ResetColor();
            Console.WriteLine("\n");
        }
        public bool Hault(string text, ConsoleColor color = ConsoleColor.White) {
            bool read = true;
            while (true) {
                Console.ForegroundColor = color;                
                if (read) this.Print("\n" + text + "\n");
                read = false;
                Console.Write("   [y/n]\n\n");
                Console.ResetColor();
                Console.Write("> ");
                string input = Console.ReadLine();
                
                if (input == "y" || input == "Y") {
                    return true;
                } else if (input == "n" || input == "N") {
                    return false;
                } else {
                    Console.WriteLine("\nTry Again\n");
                }                
            }
        }
        public List<string> ValueOutput(string[] parameters) {
            List<string> values = new List<string>();            
            for (int i = 0; i < parameters.Length; ++i) {
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.Write($"  {parameters[i]}: ");

                Console.ResetColor();
                string input = Console.ReadLine();
                values.Add(input);
            }
            return values;
        }
        public void Print(string text, int speed = 10) {
            foreach (char c in text) {
                Console.Write(c);
                Thread.Sleep(speed);
            }
        }
        public string RandCharacters(int length) {
            return new string(Enumerable.Repeat("abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789", length).Select(s => s[new Random().Next(s.Length)]).ToArray());            
        }
        public string GetFilePath(string filePath) {
            string projectDirectory = Directory.GetCurrentDirectory();
            for (int i = 0; i < 3; ++i) projectDirectory = Directory.GetParent(projectDirectory).FullName;
            return Path.Combine(projectDirectory, filePath);
        }
    }
#pragma warning disable CS8602 // Dereference of a possibly null reference.
    public class SQL {
        public MySqlConnection Connection;
        public string IP_Address;
        public int Port;
        public string Database;
        public string Username;
        public string Password;
        public string[] AccountInfo = { "username", "email", "password", "mobile", "full_name" };
        public string[] AccountSettings = { "allow_mentions", "activity_status", "suggest_account" };
        public string[] AccountStats = { "login_count", "logout_count", "last_login_timestamp", "last_logout_timestamp", "password_attempts", "last_password_attempt_timestamp", "password_change_count", "last_password_change_timestamp", "member_since", "member_since_time", "last_update", "last_update_time" };
        public string[] PersonalInfo = { "first_name", "last_name", "birthday", "sex", "address", "home_town", "highschool", "education_status", "website", "looking_for", "interested_in", "relationship_status", "political_views", "interests", "favorite_music", "favorite_movies", "about_me" };
        public string[] SocialStats = { "friend_count", "friend_email_list", "blocked_count", "blocked_username_list", "reported_count", "message_all_count", "unread_message_count", "message_sent_count", "message_received_count", "verification_request_count", "verification_request_last_timestamp" };
        private Methods methods;
        public SQL() {
            this.methods = new Methods();
            var jo = JObject.Parse(File.ReadAllText(methods.GetFilePath("config.json")));
            this.IP_Address = jo["Database-Credentials"]["IP-Address"].ToString();
            this.Port = Convert.ToInt32(jo["Database-Credentials"]["Port"]);
            this.Database = jo["Database-Credentials"]["Database"].ToString();
            this.Username = jo["Database-Credentials"]["Username"].ToString();
            this.Password = jo["Database-Credentials"]["Password"].ToString();
        }
        public void Connect() {
            string connectionString = $"Server={this.IP_Address};Database={this.Database};Uid={this.Username};Pwd={this.Password}";
            this.Connection = new MySqlConnection(connectionString);
            this.Connection.Open();
        }
        public void CloseConnection() {
            this.Connection.Close();
        }
        public bool CheckConnection() {
            if (this.Connection.State == System.Data.ConnectionState.Open) {
                return true;
            } else return false;                
        }
        public void Query(string query_string) {
            using (MySqlCommand command = new MySqlCommand(query_string, this.Connection)) {
                command.ExecuteNonQuery();
            }
        }
        public void ClearDatabase() {
            List<string> tableNames = new List<string>();
            using (DataTable dt = this.Connection.GetSchema("Tables")) {
                foreach (DataRow row in dt.Rows) {
                    string tableName = row["TABLE_NAME"].ToString();
                    tableNames.Add(tableName);
                }
            }
            foreach (string tableName in tableNames) {
                string dropTable = $"DROP TABLE {tableName}";
                this.Query(dropTable);
            }
        }        

        public void CreateDefaultStructure() {
            this.Query(@"
                CREATE TABLE account_settings (
  	                settings_id INT PRIMARY KEY AUTO_INCREMENT,
  	                allow_mentions VARCHAR(255),
  	                activity_status BOOLEAN,
  	                suggest_account BOOLEAN
                );
                CREATE TABLE account_stats (
                    account_stats_id INT PRIMARY KEY AUTO_INCREMENT,
                    login_count INT UNSIGNED,
                    logout_count INT UNSIGNED,
                    last_login_timestamp VARCHAR(32),
                    last_logout_timestamp VARCHAR(32),
                    password_attempts INT UNSIGNED,
                    last_password_attempt_timestamp VARCHAR(32),
                    password_change_count INT UNSIGNED,
                    last_password_changed_timestamp VARCHAR(32),
                    member_since VARCHAR(32),
                    member_since_time VARCHAR(32),
                    last_update VARCHAR(32),
                    last_update_time VARCHAR(32)                    
                );
                CREATE TABLE social_stats (
                    social_stats_id INT PRIMARY KEY AUTO_INCREMENT,
                    friend_count INT UNSIGNED,
                    friend_email_list VARCHAR(512),
                    blocked_count INT UNSIGNED,
                    blocked_username_list VARCHAR(512),
                    reported_count INT UNSIGNED,
                    message_all_count INT UNSIGNED,
                    unread_message_count INT UNSIGNED, 
                    message_sent_count INT UNSIGNED, 
                    message_received_count INT UNSIGNED, 
                    verification_request_count INT UNSIGNED, 
                    verification_request_last_timestamp VARCHAR(32)
                );
                CREATE TABLE personal_info (
                    personal_info_id INT PRIMARY KEY AUTO_INCREMENT,
                    first_name VARCHAR(128),
                    last_name VARCHAR(128),
                    birthday DATE,
                    sex VARCHAR(32),
                    address VARCHAR(128),
                    home_town VARCHAR(128),
                    highschool VARCHAR(128),
                    education_status VARCHAR(32),
                    website VARCHAR(128),
                    looking_for VARCHAR(32),
                    interested_in VARCHAR(32),
                    relationship_status VARCHAR(32),
                    political_views VARCHAR(128),
                    interests VARCHAR(128),
                    favorite_music TEXT,
                    favorite_movies TEXT,
                    about_me TEXT
                );
                CREATE TABLE account_info (
                    account_id INT PRIMARY KEY AUTO_INCREMENT,
                    settings_id INT, 
                    account_stats_id INT, 
                    social_stats_id INT,
                    personal_info_id INT,
                    username VARCHAR(128),
                    email VARCHAR(128),
                    password VARCHAR(128),    
                    mobile VARCHAR(128),
                    full_name VARCHAR(128),    
                    CONSTRAINT account_settings_fk FOREIGN KEY (settings_id) 
                   		REFERENCES account_settings (settings_id),
                    CONSTRAINT account_stats_fk FOREIGN KEY (account_stats_id)
                    	REFERENCES account_stats (account_stats_id),
                    CONSTRAINT social_stats_fk FOREIGN KEY (social_stats_id)
                        REFERENCES social_stats (social_stats_id),
                    CONSTRAINT personal_info_fk FOREIGN KEY (personal_info_id)
                    	REFERENCES personal_info (personal_info_id)
                );
            ");
        }
        public void TruncateAllRelationalTables() {
            this.Query(@"
                ALTER TABLE account_info DROP FOREIGN KEY account_settings_fk;
                ALTER TABLE account_info DROP FOREIGN KEY account_stats_fk;
                ALTER TABLE account_info DROP FOREIGN KEY social_stats_fk;
                ALTER TABLE account_info DROP FOREIGN KEY personal_info_fk;
                TRUNCATE TABLE account_settings;
                TRUNCATE TABLE account_stats;
                TRUNCATE TABLE social_stats;
                TRUNCATE TABLE personal_info;
                TRUNCATE TABLE account_info;
                ALTER TABLE account_info ADD CONSTRAINT account_settings_fk FOREIGN KEY (settings_id) REFERENCES account_settings(settings_id);
                ALTER TABLE account_info ADD CONSTRAINT account_stats_fk FOREIGN KEY (account_stats_id) REFERENCES account_stats(account_stats_id);
                ALTER TABLE account_info ADD CONSTRAINT social_stats_fk FOREIGN KEY (social_stats_id) REFERENCES social_stats(social_stats_id);
                ALTER TABLE account_info ADD CONSTRAINT personal_info_fk FOREIGN KEY (personal_info_id) REFERENCES personal_info(personal_info_id);
            ");
        }
        public void CreateUserAccount(string[] values) { 
            
        }       
        public void CreateNullUserAccount() {
            string query = $@"
                INSERT INTO account_settings (allow_mentions, activity_status, suggest_account) VALUES (NULL, NULL, NULL);
                SET @last_settings_id = LAST_INSERT_ID();
                INSERT INTO account_stats (login_count, logout_count, last_login_timestamp, last_logout_timestamp, password_attempts, last_password_attempt_timestamp, password_change_count, last_password_changed_timestamp, member_since, member_since_time, last_update, last_update_time) VALUES (NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL);
                SET @last_account_stats_id = LAST_INSERT_ID();
                INSERT INTO social_stats (friend_count, friend_email_list, blocked_count, blocked_username_list, reported_count, message_all_count, unread_message_count, message_sent_count, message_received_count, verification_request_count, verification_request_last_timestamp) VALUES (NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL);
                SET @last_social_stats_id = LAST_INSERT_ID();
                INSERT INTO personal_info (first_name, last_name, birthday, sex, address, home_town, highschool, education_status, website, looking_for, interested_in, relationship_status, political_views, interests, favorite_music, favorite_movies, about_me) VALUES (NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL);
                SET @last_personal_info_id = LAST_INSERT_ID();
                INSERT INTO account_info (settings_id, account_stats_id, social_stats_id, personal_info_id, username, email, password, mobile, full_name) VALUES (@last_settings_id, @last_account_stats_id, @last_social_stats_id, @last_personal_info_id, '{methods.RandCharacters(6)}', '{methods.RandCharacters(6) + "@icloud.com"}', '123', NULL, NULL);
            ";
            MySqlCommand command = new MySqlCommand(query, this.Connection);
            command.Parameters.Add("@last_settings_id", MySqlDbType.Int32).Direction = ParameterDirection.Output;
            command.Parameters.Add("@last_account_stats_id", MySqlDbType.Int32).Direction = ParameterDirection.Output;
            command.Parameters.Add("@last_social_stats_id", MySqlDbType.Int32).Direction = ParameterDirection.Output;
            command.Parameters.Add("@last_personal_info_id", MySqlDbType.Int32).Direction = ParameterDirection.Output;
            command.ExecuteNonQuery();
        }
    }
#pragma warning restore CS8602 // Dereference of a possibly null reference.
}
