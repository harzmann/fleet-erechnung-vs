using System.Diagnostics;
using System.Security.Cryptography;
using System.Security.Cryptography.Pkcs;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Text.Json;

try
{
    var assembly = typeof(SignedCms).Assembly;
    var version = FileVersionInfo.GetVersionInfo(assembly.Location).FileVersion;
    if (version != "8.0.3226.50405") throw new Exception("Unexpected Pkcs security patch: " + version);
    using var key = RSA.Create(2048);
    var request = new CertificateRequest("CN=EHFleet Pkcs Smoke Test", key, HashAlgorithmName.SHA256, RSASignaturePadding.Pkcs1);
    request.CertificateExtensions.Add(new X509KeyUsageExtension(X509KeyUsageFlags.DigitalSignature | X509KeyUsageFlags.KeyEncipherment, true));
    using var certificate = request.CreateSelfSigned(DateTimeOffset.UtcNow.AddMinutes(-1), DateTimeOffset.UtcNow.AddHours(1));
    var payload = Encoding.UTF8.GetBytes("EHFleet Pkcs security patch round-trip");
    var signed = new SignedCms(new ContentInfo(payload), detached: false);
    signed.ComputeSignature(new CmsSigner(certificate), silent: true);
    var encoded = signed.Encode();
    var decoded = new SignedCms();
    decoded.Decode(encoded);
    decoded.CheckSignature(verifySignatureOnly: true);
    if (!decoded.ContentInfo.Content.SequenceEqual(payload)) throw new Exception("Signed CMS content mismatch");
    encoded[^1] ^= 1;
    var rejected = false;
    try { var tampered = new SignedCms(); tampered.Decode(encoded); tampered.CheckSignature(true); }
    catch (CryptographicException) { rejected = true; }
    if (!rejected) throw new Exception("Tampered signature accepted");
    var envelope = new EnvelopedCms(new ContentInfo(payload));
    envelope.Encrypt(new CmsRecipient(certificate));
    var decrypted = new EnvelopedCms();
    decrypted.Decode(envelope.Encode());
    decrypted.Decrypt(new X509Certificate2Collection(certificate));
    if (!decrypted.ContentInfo.Content.SequenceEqual(payload)) throw new Exception("Encrypted CMS content mismatch");
    Console.WriteLine(JsonSerializer.Serialize(new { Passed = true, Host = Environment.MachineName, PkcsFileVersion = version, SignedCms = "passed", TamperedSignature = "rejected", EnvelopedCms = "passed" }));
    return 0;
}
catch (Exception error)
{
    Console.WriteLine(JsonSerializer.Serialize(new { Passed = false, Host = Environment.MachineName, ErrorType = error.GetType().Name, Error = error.Message }));
    return 1;
}
