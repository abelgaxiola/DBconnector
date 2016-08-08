// Class created for stored procedure parameter names and values that are
// in turn saved to a list and passed to the DBconnector.ExecuteStoredProcedure()
// method.

public class SPparameter
{
    public string Name { get; set; }
    public object Value { get; set; }
}
