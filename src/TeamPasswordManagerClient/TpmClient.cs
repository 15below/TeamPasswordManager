namespace TeamPasswordManagerClient
{
    public interface ITpmClient
    {
        ITpmPasswordClient Passwords { get; }
        ITpmMyPasswordClient MyPasswords { get; }
        ITpmProjectClient Projects { get; }
        ITpmUserClient Users { get; }
        ITpmGroupClient Groups { get; }
    }

    public class TpmClient : ITpmClient
    {
        public TpmClient(ITpmPasswordClient PasswordClient,
                         ITpmMyPasswordClient PersonalPasswordClient,
                         ITpmProjectClient ProjectClient,
                         ITpmUserClient UserClient,
                         ITpmGroupClient GroupClient)
        {
            this.Passwords = PasswordClient;
            this.MyPasswords = PersonalPasswordClient;
            this.Projects = ProjectClient;
            this.Users = UserClient;
            this.Groups = GroupClient;
        }

        public TpmClient(TpmConfig config)
        {
            var http = new TpmHttp(config);
            this.Passwords = new TpmPasswordClient(http);
            this.MyPasswords = new TpmMyPasswordClient(http);
            this.Projects = new TpmProjectClient(http);
            this.Users = new TpmUserClient(http);
            this.Groups = new TpmGroupClient(http);
        }

        public ITpmPasswordClient Passwords { get; private set; }

        public ITpmMyPasswordClient MyPasswords { get; private set; }

        public ITpmProjectClient Projects { get; private set; }

        public ITpmUserClient Users { get; private set; }

        public ITpmGroupClient Groups { get; private set; }
    }
}
