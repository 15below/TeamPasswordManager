namespace TeamPasswordManagerClient
{
    public interface ITpmClient
    {
        ITpmPasswordClient Passwords { get; }
        ITpmPersonalPasswordClient PersonalPasswords { get; }
        ITpmProjectClient Projects { get; }
        ITpmUserClient Users { get; }
        ITpmGroupClient Groups { get; }
    }

    public class TpmClient : ITpmClient
    {
        public TpmClient(ITpmPasswordClient PasswordClient,
                         ITpmPersonalPasswordClient PersonalPasswordClient,
                         ITpmProjectClient ProjectClient,
                         ITpmUserClient UserClient,
                         ITpmGroupClient GroupClient)
        {
            this.Passwords = PasswordClient;
            this.PersonalPasswords = PersonalPasswordClient;
            this.Projects = ProjectClient;
            this.Users = UserClient;
            this.Groups = GroupClient;
        }

        public TpmClient(TpmConfig config)
        {
            this.Passwords = new TpmPasswordClient(config);
            this.PersonalPasswords = new TpmPersonalPasswordClient(config);
            this.Projects = new TpmProjectClient(config);
            this.Users = new TpmUserClient(config);
            this.Groups = new TpmGroupClient(config);
        }

        public ITpmPasswordClient Passwords { get; private set; }

        public ITpmPersonalPasswordClient PersonalPasswords { get; private set; }

        public ITpmProjectClient Projects { get; private set; }

        public ITpmUserClient Users { get; private set; }

        public ITpmGroupClient Groups { get; private set; }
    }
}
