namespace test_peformance;

public class AppDbConfig
{
    public string Server { get; private set; }
    public string UserName { get; private set; }
    private static AppDbConfig _singletonPattern = null;
    private static readonly object padlock = new object();

    private AppDbConfig(string server, string userName)
    {
        Server = server;
        UserName = userName;
    }

    public static AppDbConfig getInstance()
    {
        if (_singletonPattern == null)
        {
            lock (padlock)
            {
                if (_singletonPattern == null)
                {
                    Random rnd = new Random();
                    //read file appsetting 
                    var server = "";
                    var userName = "";
                    _singletonPattern = new AppDbConfig(server,userName);
                }
            }
        }

        return _singletonPattern;
    }

    public void Log()
    {
        //Console.WriteLine($"Age : {Age}");
    }
}