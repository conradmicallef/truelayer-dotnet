using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
#if NETSTANDARD2_0
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.OpenSsl;
using System.Linq;
#endif

namespace TrueLayer
{
    /// <summary>
    /// ES512 signing key used to sign API requests
    /// </summary>
    public class SigningKey
    {
        private readonly Lazy<ECDsa> _key;

        public SigningKey()
        {
            _key = new Lazy<ECDsa>(() => CreateECDsaKey(PrivateKey));
        }
        
        /// <summary>
        /// Sets the private key. Should not be shared with anyone outside of your organisation.
        /// </summary>
        public string PrivateKey { internal get; set; } = null!;

        /// <summary>
        /// Gets the TrueLayer Key identifier available from the Console
        /// </summary>
        public string KeyId { get; set; } = null!;

        internal ECDsa Value => _key.Value;
#if NETSTANDARD2_0
        private static byte[] FixSize(byte[] input, int expectedSize)
        {
            if (input.Length == expectedSize)
            {
                return input;
            }

            byte[] tmp;

            if (input.Length < expectedSize)
            {
                tmp = new byte[expectedSize];
                Buffer.BlockCopy(input, 0, tmp, expectedSize - input.Length, input.Length);
                return tmp;
            }

            if (input.Length > expectedSize + 1 || input[0] != 0)
            {
                throw new InvalidOperationException();
            }

            tmp = new byte[expectedSize];
            Buffer.BlockCopy(input, 1, tmp, 0, expectedSize);
            return tmp;
        }

        private static ECDsa CreateECDsaKey(string privateKey)
        {
            privateKey.NotNullOrWhiteSpace(nameof(privateKey));

            var ask = new PemReader(new StringReader(privateKey)).ReadObject() as AsymmetricCipherKeyPair;
            if (ask == null)
                throw new Exception("unable to decode privatekey");
            var pk = (ECPrivateKeyParameters)ask.Private;
            var puk = (ECPublicKeyParameters)ask.Public;
            ECParameters par = new ECParameters
            {
                Curve = ECCurve.CreateFromValue(pk.PublicKeyParamSet.Id),
                D = pk.D.ToByteArrayUnsigned(),
                Q = new ECPoint
                {
                    X = puk.Q.XCoord.GetEncoded(),
                    Y = puk.Q.YCoord.GetEncoded()
                }
            };
            par.D = FixSize(par.D, par.Q.X.Length);
            par.Validate();
            var key = ECDsaCng.Create(par);
            return key;
        }
#else

        private static ECDsa CreateECDsaKey(string privateKey)
        {           
            privateKey.NotNullOrWhiteSpace(nameof(privateKey));
            
            var key = ECDsa.Create();

#if (NET5_0 || NET5_0_OR_GREATER)
            // Ref https://www.scottbrady91.com/C-Sharp/PEM-Loading-in-dotnet-core-and-dotnet
            key.ImportFromPem(privateKey);
#else
            byte[] decodedPem = ReadPemContents(privateKey);
            key.ImportECPrivateKey(decodedPem, out _);
#endif
            return key;
        }
#endif

        /// <summary>
        /// Reads and decodes the contents of the PEM private key, removing the header/trailer
        /// Required before .NET 5.0
        /// </summary>
        /// <param name="privateKey"></param>
        /// <returns></returns>
        private static byte[] ReadPemContents(string privateKey)
        {
            var sb = new StringBuilder();
            using (var reader = new StringReader(privateKey))
            {
                string? line = null;
                while ((line = reader.ReadLine()) != null)
                {
                    if (!line.StartsWith("--"))
                        sb.Append(line);
                }
            }

            return Convert.FromBase64String(sb.ToString());
        }
    }
}
