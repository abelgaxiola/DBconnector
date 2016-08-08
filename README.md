# DBconnector
C# Class to make quick connections to SQL databases.

The year was 2008 and I was writing software that connected to SQL server databases on the back end. The process was repetitive and went something like this:

* Create a SqlConnection passing a connection string as a parameter value
* Create SqlCommand passing the newly created SqlConnection as a parameter value
* Create a SqlDataAdapter passing the SqlCommand as a parameter value
* Create a DataSet
* Fill the DataSet by passing it as a parameter value to the SqlDataAdapter.Fill method
* Etc, etc.

So I decided to wrap it all in one class. I called it DBconnector.

The DBconnector class has served me throughout the years and I no longer use it in its original form. But it has been transformed many times depending on the need. If you have a need to quickly connect to a database and either execute a T-SQL command or stored procedure this class can help you. All you need to do is point it to the right SQL database by modifying the connection string inside the configuration file (app or web).

