namespace Brandix.DCAP.WebUI.Services
{
    public interface IAPIConfiguration
    {
        /*
            Note that each property here needs to exactly match the 
            name of each property in my appsettings.json config object
        */
        string ConnectionString { get; set; }       
           
    } 

      public interface IFOSSConn
    {
        /*
            Note that each property here needs to exactly match the 
            name of each property in my appsettings.json config object
        */
        string FOSSConn { get; set; }       
           
    } 
}