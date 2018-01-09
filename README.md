# TeamPasswordManager

C# Client Library for TeamPasswordManager API v4: http://teampasswordmanager.com/docs/api/

Basic usage is as follows:

```C#

    var config = new TpmConfig
    {
        BaseUrl = "https://tpm-dev.15below.local/index.php/",
        PublicKey = "5650a2df5b56e81926e06b33ad52c9c541c2aa30e474aa7574654ceb9123056e",
        PrivateKey = "f271d7c2197805dd73ffa1e11c0c25973177ae7b2244984f808dca6c610abe8e"
    };

	var client = new TpmClient(config);
	
    // Get all root projects
    var projects = client.Projects.ListAllProjects();

    // Select the test project
    var testProject = projects.First(where => where.Name == "Test Project");
    
    // Get passwords entries in the test project
    var passwordEntries = client.Passwords.ListAllPasswords(testProject.Id);
    
    // Retrieve the password for the test password entry
    var password = client.Passwords.GetPassword(passwordEntries.First(where => where.Name == "Test Password").Id);
    
    Console.WriteLine(password.Password);
	
```
