using System;
using System.Collections.Generic;

namespace ConnectionTest
{
    class Program
    {
        static void Main(string[] args)
        {
            NumberOfRecords();
            TestExecuteCommand();
            TestExecuteStoredProcedure();
            NumberOfRecords();
            TestExecuteStoredProcedureAsCommand();
            DeleteUser();
            NumberOfRecords();
            Console.ReadLine();
        }

        private static void TestExecuteCommand()
        {
            using (var connector = new DBconnector())
            {
                connector.ExecuteCommand("SELECT TOP 1 * FROM Users");
                var result = connector.GetDataTable().Rows[0];
                if (connector.HasError)
                    Console.WriteLine(connector.ErrorMessage);
                else
                    Console.WriteLine($"ID: {result["Id"]}, Name: {result["FirstName"]} {result["LastName"]}, City: {result["City"]} ");
            }
        }

        private static void TestExecuteStoredProcedure()
        {
            using (var connector = new DBconnector())
            {
                List<SPparameter> parameters = new List<SPparameter>()
                {
                    new SPparameter() {Name = "UserId", Value = "10" },
                    new SPparameter() {Name = "FirstName", Value = "Tester" },
                    new SPparameter() {Name = "LastName", Value = "Ten" },
                    new SPparameter() {Name = "Age", Value = "30" },
                    new SPparameter() {Name = "City", Value = "Salem" },
                    new SPparameter() {Name = "State", Value = "OR" }
                };

                connector.ExecuteStoredProcedure("AddUser", parameters, System.Data.CommandType.StoredProcedure);
                if (connector.HasError)
                    Console.WriteLine(connector.ErrorMessage);
                else
                    Console.WriteLine("Insert of new user row has been successfull...");
            }
        }

        private static void TestExecuteStoredProcedureAsCommand()
        {
            using (var connector = new DBconnector())
            {
                connector.ExecuteCommand("EXEC GetUser @userId = 10");
                var result = connector.GetDataTable().Rows[0];
                if (connector.HasError)
                    Console.WriteLine(connector.ErrorMessage);
                else
                    Console.WriteLine($"ID: {result["Id"]}, Name: {result["FirstName"]} {result["LastName"]}, City: {result["City"]}, State: {result["State"]} ");
            }
        }

        private static void DeleteUser()
        {
            // Assuming that the TestExecuteStoredProcedure successfully added user with Id = 10
            using (var connector = new DBconnector())
            {
                connector.ExecuteCommand("DELETE Users WHERE Id = 10");
                if (connector.HasError)
                    Console.WriteLine(connector.ErrorMessage);
                else
                    Console.WriteLine("User Successfully deleted...");
            }
        }

        private static int NumberOfRecords()
        {
            int result;

            using (var connector = new DBconnector())
            {
                connector.ExecuteCommand("SELECT COUNT(*) FROM Users");
                result = connector.ReturnedInt;

                if (connector.HasError)
                    Console.WriteLine(connector.ErrorMessage);
                else
                    Console.WriteLine($"Total number of users in database: {result}");
            }

            return result;
        }

    }
}
