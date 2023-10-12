# DimaDevi
Device components builder
Inspired in [DeviceId](https://github.com/MatthewKing/DeviceId)

# Features
<details><summary>Remote WMI</summary>
<p>
If you dont defined this the WMI is Local Computer by default

```c#
   GeneralConfigs.GetInstance().RemoteWmi = new RemoteWMICredential("192.168.0.2", "Administrator", "123456");
```
</p>
</details>

#### In Global General Configuration exists these features:
`RemoteWmi` for Remote WMI property see "Remote WMI"<br>
`Encoding` Global encoding. If set UTF8 all device process as UTF8<br>
`ProcessComponentsWhileAdd` this process the components that you add in runtime after call ToString() only return result of component runtime added not re-process<br>
`WarningBigKeySizeRSA` is for RSAForm `Formatter` if `KeySize` is greather or equal than 4096 throw warning message box or Console  (if use Console)

| Hardwares (all enum admit flags) | Components (support disposed) | Formatters |
| :---      | :---       | :---       |
| CPU       | Assembly   | AES        |
| RAM       | File       | RSA        |
| Disk | Devi | ChaCha20 |
| Motherboard | Network | JSON |
| MacAddress | WMI | Hash |
| BIOS | Registry | Support ECDH (Eliptic Curve Diffie-Helman) |

### Create Hardware 'Outside'

```csharp
HardwareComponents.GetInstance()
    .AddComponent(typeof(Enumerations.CPU), "L3CacheSize")
    .AddComponent(typeof(Enumerations.CPU), "L2CacheSize");
```
# How to use ECDH with AES
```cs
DeviBuild devi = new DeviBuild()
    .AddCPU(Enumerations.CPU.All)
    .AddMachineName()
    .AddMacAddress()
    .AddMotherboard()
    .AddUUID()
    .AddRam(Enumerations.RAM.All)
    .AddRegistry(@"SOFTWARE\\DefaultUserEnvironment", "Path");

var bob = new ElipticCurveDiffieHellman(); //Instance ECDH Bob
var alice = new ElipticCurveDiffieHellman(bob.GetPublicKey()); //Instance ECDH alice with public key of Bob. Alice have public key too.

Func<string> deviString = () => devi.ToString("<MyCoolCustomSeparator>");

var aesAlice = new AESForm(alice); //What happen with salt? Salt is a Derivate of ECDH
Console.WriteLine($"Original Data: {deviString()}");
Console.WriteLine("-----------");
devi.Formatter = aesAlice; //Set AES Formatter

var IVAlice = aesAlice.GetIV(); //Get IV Vector Alice
string content = deviString(); //
var aesBob = new AESForm(bob.AddPublic(alice)); //in ECDH of BOB add Public key of Alice
aesBob.SetIV(IVAlice); //Set IV vector of Alice in AES Bob
Console.WriteLine($"Cipher Data: {content}");
Console.WriteLine("-----------");
var decrypt = devi.Decryption(content, aesBob);
Console.WriteLine($"Decrypt data: {decrypt}");
```

# Hardware "Outside"
```cs
HardwareComponents.GetInstance()
    .AddComponent(typeof(Enumerations.CPU), "L3CacheSize")
    .AddComponent(typeof(Enumerations.CPU), "L2CacheSize");
DeviBuild devi = new DeviBuild()
    .AddCPU(Enumerations.CPU.Description | Enumerations.CPU.Name)
    .AddMotherboard(Enumerations.Motherboard.Name | Enumerations.Motherboard.Manufacturer)
    .AddMachineName()
    .AddMacAddress()
    .AddRam(Enumerations.RAM.All)
    .AddRegistry(@"SOFTWARE\\DefaultUserEnvironment", "Path");
string content = devi.ToString("<separ>"); //Print Description, Name CPU and L3CacheSize, L2CacheSize of CPU and other components
Console.WriteLine(content);
```

# Common Formatters
```cs
DeviBuild devi = new DeviBuild();
devi.AddGPU(Enumerations.GPU.All).AddMacAddress().AddMotherboard(Enumerations.Motherboard.All);
devi.Formatter = new HashForm();
Console.WriteLine($"Hash: {devi.ToString("#")}");
devi.Formatter = new JsonForm();
Console.WriteLine($"Json: {devi.ToString("#")}");
devi.Formatter = new XmlForm();
Console.WriteLine($"Xml: {devi.ToString("#")}");
devi.Dispose();
```

# TODO
- [ ] Fix `Import` DeviBuild
- [x] Add `BaseXFormatter`
- [x] Add more examples
- [ ] Allow add more hardware (like "Win32_CacheMemory" for example) in `HardwaresComponents`
- [ ] Fix, Add & Improvement Validation DeviBuild
