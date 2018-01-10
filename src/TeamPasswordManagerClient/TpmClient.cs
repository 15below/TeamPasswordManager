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
            this.Http = new TpmHttp(config);
            this.Passwords = new TpmPasswordClient(Http);
            this.MyPasswords = new TpmMyPasswordClient(Http);
            this.Projects = new TpmProjectClient(Http);
            this.Users = new TpmUserClient(Http);
            this.Groups = new TpmGroupClient(Http);
        }

        internal TpmHttp Http { get; }

        public ITpmPasswordClient Passwords { get; private set; }

        public ITpmMyPasswordClient MyPasswords { get; private set; }

        public ITpmProjectClient Projects { get; private set; }

        public ITpmUserClient Users { get; private set; }

        public ITpmGroupClient Groups { get; private set; }
    }
}
