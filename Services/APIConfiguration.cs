namespace Brandix.DCAP.WebUI.Services
{
    public class APIConfiguration : IAPIConfiguration
    {
        /*
            Note that each property here needs to exactly match the 
            name of each property in my appsettings.json config object
        */
        public string ConnectionString { get; set; } 
    }

     public class FOSSConnection : IFOSSConn
    {
        /*
            Note that each property here needs to exactly match the 
            name of each property in my appsettings.json config object
        */
        public string FOSSConn { get; set; } 
    }
}