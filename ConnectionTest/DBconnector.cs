// Class used to connect to the database and execute stored procedures or commands that either
// Create, Read, Update, or Delete data.  The class depends on the application's
// configuration file for the connection string.  Results are returned as either
// a DataTable or DataSet types by using GetDataTable() or GetDataSet() methods.
// ExecuteStoreProcedure() method depends on a list of parameter values; the calling
// process is responsible for creating and adding values to the parameter list

using System;
using System.Data;
using System.Diagnostics;
using System.Configuration;
using System.Data.SqlClient;
using System.Collections.Generic;

public class DBconnector : IDisposable
{
    private DataTable Table { get; set; }
    private DataSet Results { get; set; }
    private SqlConnection Connection { get; set; }
    private SqlCommand Command { get; set; }
    private string ConnectionString { get; set; }
    private string Information { get; set; }
    public ConnectionState State { get; private set; }
    public bool HasError { get; private set; }
    public string ErrorMessage { get; private set; }
    public int ReturnedInt { get; private set; }
    public string ReturnedString { get; private set; }
    public decimal ReturnedDecimal { get; private set; }

    public DBconnector()
    {
        // First connection string is considered to be the default
        string dbSource = ConfigurationManager.ConnectionStrings[1].Name;
        Initialize(dbSource);
    }

    public DBconnector(string dbSource)
    {
        Initialize(dbSource);
    }

    public void CloseConnection()
    {
        Connection.Close();
        State = Connection.State;
        Information += "Connection state: " + State.ToString() + "\r\n";
        Dispose();
    }

    public void ExecuteCommand(string command)
    {
        try
        {
            Command = new SqlCommand(command, Connection);
            Command.CommandType = CommandType.Text;

            Execute(CommandType.Text);
        }
        catch (Exception exception)
        {
            SetErrorMessage(exception.ToString());
        }
    }

    public void ExecuteStoredProcedure(string procedureName, List<SPparameter> parameters, CommandType commandType)
    {
        try
        {
            Information += "Stored procedure name: " + procedureName + "\r\n";

            Command = new SqlCommand(procedureName, Connection);
            Command.CommandType = CommandType.StoredProcedure;

            if (parameters != null)
                AddParameters(parameters);

            Execute(commandType);
        }
        catch (Exception exception)
        {
            SetErrorMessage(exception.ToString());
        }

    }

    public DataTable GetDataTable()
    {
        return Table;
    }

    public DataSet GetDataSet()
    {
        return Results;
    }

    private void Execute(CommandType commandType)
    {
        if (commandType == CommandType.Text)
            FillDataTable();
        else
            ReturnedInt = Command.ExecuteNonQuery();
    }

    private void FillDataTable()
    {
        try
        {
            SqlDataAdapter da = new SqlDataAdapter(Command);
            da.Fill(Results);

            if (Results.Tables.Count > 0)
            {
                Table = Results.Tables[0];

                // Check to see if a single row is returned.  It may be that the stored procedure returns a single value
                if (Table.Rows.Count == 1)
                    SetReturnValue();
            }
        }
        catch (Exception exception)
        {
            SetErrorMessage(exception.ToString());
        }
    }

    private void SetReturnValue()
    {
        // This method will test for single integer and boolean values and set the ReturnValue property accordingly
        DataRow row = Table.Rows[0];
        // If only one column in the row then use that column value to update public returned values
        if (row.ItemArray.GetLength(0) == 1)
        {
            ReturnedString = row[0].ToString();
            // If the value is a boolean then prepare string value so that the return value will be equal to 1
            if (ReturnedString.ToUpper() == "TRUE")
                ReturnedString = "1";

            int intValue;
            bool goodParse = int.TryParse(ReturnedString, out intValue);
            // Set the integer value
            if (goodParse)
                ReturnedInt = intValue;

            decimal decimalValue;
            goodParse = decimal.TryParse(ReturnedString, out decimalValue);
            // Set the decimal value
            if (goodParse)
                ReturnedDecimal = decimalValue;
        }
    }

    private void AddParameters(List<SPparameter> parameters)
    {
        foreach (var parameter in parameters)
        {
            Command.Parameters.AddWithValue(parameter.Name, parameter.Value);

            Information += "Parameter name: " + parameter.Name + "\r\n";
            Information += "Parameter value: " + parameter.Value + "\r\n";
        }
    }

    private void SetConnectionString(string dbSource)
    {
        ConnectionString = ConfigurationManager.ConnectionStrings[dbSource].ToString();

        Debug.Assert(ConnectionString != null);
        if (ConnectionString == null)
        {
            SetErrorMessage("There was an error trying to set the connection string inside DBconnector...");
        }
    }

    private void ConnectToDatabase()
    {
        try
        {
            if (!HasError)
            {
                Information += "Connection String: " + ConnectionString + "\r\n";
                Connection = new SqlConnection(ConnectionString);
                Connection.Open();
                Debug.Assert(Connection.State == ConnectionState.Open);
                State = Connection.State;
                Debug.Assert(Connection != null);
            }
        }
        catch (Exception exception)
        {
            SetErrorMessage(exception.ToString());
        }
    }

    private void SetErrorMessage(string exceptionMessage)
    {
        // Let the calling process know there was an error
        HasError = true;

        exceptionMessage += "\r\n\r\n Additional information:\r\n\r\n" + Information;

        ErrorMessage = exceptionMessage;
    }

    private void Initialize(string dbSource)
    {
        HasError = false;
        try
        {
            SetConnectionString(dbSource);
            ConnectToDatabase();
            Table = new DataTable();
            Results = new DataSet();
        }
        catch (Exception exception)
        {
            SetErrorMessage(exception.ToString());
        }
    }

    private bool disposed;

    protected virtual void Dispose(Boolean disposing)
    {
        if (disposed)
            return;

        if (disposing)
        {
            try
            {
                if (Connection != null)
                    Connection.Dispose();

                if (Table != null)
                    Table.Dispose();

                if (Results != null)
                    Results.Dispose();
            }
            catch (Exception exception)
            {
                SetErrorMessage(exception.ToString());
            }
        }

        disposed = true;
    }

    public void Dispose()
    {
        this.Dispose(true);

        GC.SuppressFinalize(this);
    }

    ~DBconnector()
    {
        this.Dispose(false);
    }

} // End DBconnector class