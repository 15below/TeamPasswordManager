using System;

namespace TeamPasswordManagerClient
{
    public class TpmConfig
    {
        public string BaseUrl { get; }
        public string PublicKey { get; }
        public string PrivateKey { get; }

        /// <summary>
        /// The required config for connecting to the TPM api, which can be found under your User Settings -> API Keys
        /// </summary>
        /// <param name="baseUrl"></param>
        /// <param name="publicKey"></param>
        /// <param name="privateKey"></param>
        public TpmConfig(string baseUrl, string publicKey, string privateKey)
        {
            BaseUrl = baseUrl ?? throw new ArgumentNullException(nameof(baseUrl));
            PublicKey = publicKey ?? throw new ArgumentNullException(nameof(publicKey));
            PrivateKey = privateKey ?? throw new ArgumentNullException(nameof(privateKey));
        }
    }
}
