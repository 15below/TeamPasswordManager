using System;
using System.Threading.Tasks;
using TeamPasswordManagerClient;

namespace TeamPasswordManagerClientTestHarness
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var client = new TpmClient(new TpmConfig(args[0], args[1], args[2], args[3]));
            var passwords = await client.Passwords.ListPasswords(611);
            foreach (var password in passwords)
            {
                var value = await client.Passwords.GetPassword(password.Id);
                Console.WriteLine($"{value.Name} - User: {value.Username} / Value: {value.Password}");
            }

            Console.WriteLine("Done...");
            Console.ReadLine();
        }
    }
}
