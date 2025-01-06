using System;
using System.Management;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;
using System.Security.Cryptography;
using System.Diagnostics;
using System.Windows.Forms;
using System.Net;
using System.Net.Sockets;
using System.Net.NetworkInformation;
using System.Net.Http;
using Newtonsoft.Json;
using System.Threading;
using System.Collections;


namespace Multitool
{
    internal class Program
    {

        static Dictionary<string, string> dnsCache = new Dictionary<string, string>();

        // Const table for better structure and oversight

        // Main menu

        private const String String_Equation_Calculator = "calculator";
        private const String File_Extensions_Arcive = "file extensions";
        private const String Text_En_Decryption = "text en/decryption";
        private const String Network_Tools = "network tools";
        private const String External_Programs = "external programs";
        private const String System_Information = "systeminfo";

        // Text en/decryption

        private const String Hashing = "hashing";
        private const String AES_En_Decryption = "aes";
        private const String BASE64 = "base64";
        private const String Caesar_Cipher = "caesar cipher";

        // Network tools

        private const String Ping = "ping";
        private const String Traceroute = "traceroute";
        private const String DNS_Lookup = "dns lookup";
        private const String HTTP_Requests = "http requests";
        private const String Port_Scanner = "port scanner";
        private const String Network_Interface_Information = "nii";

        // External programs

        private const String Digital_Storage_Measurement_System = "bit/byte calculator";
        private const String Dynamic_Password_Generator = "password generator";
        private const String Unit_Converter = "unit converter";

        // Systeminfo

        private const String System_Details = "system details";
        private const String Hardware_Monitoring = "hardware monitoring";
        private const String Energy_Status = "energy status";
        private const String Enviroment_Variables = "enviroment variables";


        public class FileExtensionInfo // Get the Extension info from JSON
        {
            public string Extension { get; set; }
            public string FullName { get; set; }
            public string Description { get; set; }
            public string Group { get; set; }
        }

        static double lastResult = 0.0; // Global variable for calculator

        static void Main(string[] args) // Main menu
        {
            Console.Title = "Multitool.exe";

            while (true)
            {
                Console.SetWindowSize(120, 30);
                Console.Clear();
                Console.WriteLine("");
                Console.ForegroundColor = ConsoleColor.DarkGray;
                Console.WriteLine("=============================");
                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.WriteLine("         Multitool");
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("         Main Menu");
                Console.ForegroundColor = ConsoleColor.DarkGray;
                Console.WriteLine("=============================");
                Console.ResetColor();
                Console.WriteLine("");
                Console.WriteLine("");
                Console.WriteLine("  Possible arguments: \n  | \n  |-> Calculator \n  | \n  |-> File Extensions \n  | \n  |-> Text En/Decryption \n  | \n  |-> Network Tools \n  | \n  |-> External Programs \n  | \n  |-> System Information  \n  | \n  |-> Info \n  | \n  |-> Exit"); // Menu structure
                Console.WriteLine("");
                Console.WriteLine("");

                Console.Write("  Option: ");
                string MainMenuOption = Console.ReadLine().ToLower();

                switch (MainMenuOption)
                {
                    case String_Equation_Calculator:
                        RunCalculator();
                        break;

                    case File_Extensions_Arcive:
                        RunFileExtensionsArchive();
                        break;

                    case Text_En_Decryption:
                        RunTextEn_Decryption();
                        break;

                    case Network_Tools:
                        RunNetworkTools();
                        break;

                    case External_Programs:
                        RunExternalPrograms();
                        break;

                    case System_Information:
                        RunSystemInformation();
                        break;

                    case "info":             
                        RunAnimation();
                        break;
                                                
                    case "exit":
                        Environment.Exit(0);
                        break;

                    default:
                        Console.WriteLine("");
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine(" Invalid or empty argument!");
                        Task.Delay(2000).Wait();
                        Console.ResetColor();
                        break;
                }
            }
        }

        static void RunCalculator()
        {
            while (true)
            {
                Console.Clear();
                Console.WriteLine("");
                Console.WriteLine(" String-equation calculator");
                Console.WriteLine("");
                Console.WriteLine("");
                Console.WriteLine(" Type 'help' to get more information about this function and see usage examples.");
                Console.WriteLine("");
                Console.WriteLine(" Type 'exit' to return to the main menu.");
                Console.WriteLine("");
                Console.WriteLine("");
                Console.Write(" Enter an equation: ");

                string Equation = Console.ReadLine().ToLower();

                if (Equation == "exit")
                {
                    break;
                }

                if (string.IsNullOrWhiteSpace(Equation)) // Catch for empty equation or null
                {
                    Console.WriteLine("");
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine(" The input is empty. Please enter a valid equation.");
                    Task.Delay(2000).Wait();
                    Console.ResetColor();
                    continue;
                }

                try
                {
                    double Result = EvaluateExpression(Equation); // Call Calculator engine

                    Console.WriteLine("");
                    Console.WriteLine("");
                    Console.WriteLine(" Result: " + Result);
                }

                // Catch errors

                catch (DivideByZeroException)
                {
                    Console.WriteLine("");
                    Console.WriteLine("");
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine(" Math.Error: Devision by zero is not allowed.");
                    Console.ResetColor();
                    Console.WriteLine("");
                }
                catch (ArgumentException ex)
                {
                    Console.WriteLine("");
                    Console.WriteLine("");
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine(" Calculation error: " + ex.Message);
                    Console.ResetColor();
                    Console.WriteLine("");
                }
                catch (Exception ex)
                {
                    Console.WriteLine("");
                    Console.WriteLine("");
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine(" Unexpected error: " + ex.Message);
                    Console.ResetColor();
                    Console.WriteLine("");
                }

                if (Equation == "help")
                {
                    DisplayHelp("calculator");
                }


                Console.ReadLine();
            }
        }

        static double EvaluateExpression(string Equation) // This method takes the input and splits it for calculation
        {
            // Handle square root
            Equation = Regex.Replace(Equation, @"sqrt\(([^)]*)\)", m =>
            {
                double value = EvaluateExpression(m.Groups[1].Value);
                if (value < 0)
                    throw new ArgumentException(" Square root of a negative number is not allowed.");
                return Math.Sqrt(value).ToString();
            });

            // Replace "ans" with the value of the last result
            Equation = Equation.Replace("ans", lastResult.ToString());

            // Handle sin function (in degrees)
            Equation = Regex.Replace(Equation, @"sin\(([^)]*)\)", m =>
            {
                double valueInDegrees = EvaluateExpression(m.Groups[1].Value);
                double valueInRadians = valueInDegrees * (Math.PI / 180);
                return Math.Sin(valueInRadians).ToString();
            });

            // Handle cos function (in degrees)
            Equation = Regex.Replace(Equation, @"cos\(([^)]*)\)", m =>
            {
                double valueInDegrees = EvaluateExpression(m.Groups[1].Value);
                double valueInRadians = valueInDegrees * (Math.PI / 180);
                return Math.Cos(valueInRadians).ToString();
            });

            // Handle tan function (in degrees)
            Equation = Regex.Replace(Equation, @"tan\(([^)]*)\)", m =>
            {
                double valueInDegrees = EvaluateExpression(m.Groups[1].Value);
                double valueInRadians = valueInDegrees * (Math.PI / 180);
                return Math.Tan(valueInRadians).ToString();
            });

            // Handle natural logarithm
            Equation = Regex.Replace(Equation, @"ln\(([^)]*)\)", m =>
            {
                double value = EvaluateExpression(m.Groups[1].Value);
                return Math.Log(value).ToString();
            });

            // Handle base-10 logarithm
            Equation = Regex.Replace(Equation, @"log\(([^)]*)\)", m =>
            {
                double value = EvaluateExpression(m.Groups[1].Value);
                return Math.Log10(value).ToString();
            });

            // Handle exponential function
            Equation = Regex.Replace(Equation, @"exp\(([^)]*)\)", m =>
            {
                double value = EvaluateExpression(m.Groups[1].Value);
                return Math.Exp(value).ToString();
            });

            // Replace pi with its value
            Equation = Equation.Replace("pi", Math.PI.ToString());
            Equation = Equation.Replace("Pi", Math.PI.ToString());

            // Replace e with its value
            Equation = Equation.Replace("e", Math.E.ToString());

            // Handle Percentage
            Equation = Regex.Replace(Equation, @"(\d+(\.\d+)?)%", m => (double.Parse(m.Groups[1].Value) / 100).ToString());

            // Split the equation into parts
            string[] parts = Regex.Split(Equation, @"([+\-*/%^()])")
                                  .Where(s => !string.IsNullOrEmpty(s))
                                  .ToArray();

            // Handle negative numbers
            for (int i = 0; i < parts.Length; i++)
            {
                if (parts[i] == "-" && (i == 0 || parts[i - 1] == "(" || IsOperator(parts[i - 1])))
                {
                    parts[i + 1] = "-" + parts[i + 1];
                    parts[i] = null;
                }
            }

            parts = parts.Where(p => p != null).ToArray();

            // Evaluate the expression
            double result = Evaluate(parts);

            // Store the result in the global variable
            lastResult = result;

            return result;
        }

        static bool IsOperator(string op) // Define Operators
        {
            return op == "+" || op == "-" || op == "*" || op == "/" || op == "^" || op == "%";
        }

        static double Evaluate(string[] Tokens) // Main evaluation function
        {
            Stack<double> Values = new Stack<double>(); // Generates a new Stack for storing values

            Stack<string> Operators = new Stack<string>(); // generates a new Stack for storing Operators

            Dictionary<string, int> Precedence = new Dictionary<string, int> // Dictionary to define the precedence of operators
            {
                { "+", 1 },
                { "-", 1 },
                { "*", 2 },
                { "/", 2 },
                { "%", 2 },
                { "^", 3 }
            };

            // Iterate through each token
            for (int i = 0; i < Tokens.Length; i++)
            {
                string Token = Tokens[i];

                if (double.TryParse(Token, out double Number)) // If the token is a number, push it to the values stack
                {
                    Values.Push(Number);
                }

                else if (Token == "(") // If the token is an opening parenthesis, push it to the operators stack
                {
                    Operators.Push(Token);
                }

                else if (Token == ")")
                {
                    while (Operators.Peek() != "(") // Apply operators until an opening parenthesis is encountered
                    {
                        Values.Push(ApplyOperator(Operators.Pop(), Values.Pop(), Values.Pop()));
                    }
                    Operators.Pop(); // Remove the opening parenthesis from the stack
                }

                else if (Precedence.ContainsKey(Token)) // If the token is an operator
                {
                    while (Operators.Count > 0 && Precedence.ContainsKey(Operators.Peek()) && Precedence[Operators.Peek()] >= Precedence[Token]) // Apply operators with higher or equal precedence
                    {
                        Values.Push(ApplyOperator(Operators.Pop(), Values.Pop(), Values.Pop()));
                    }
                    Operators.Push(Token); // Push it to the stack
                }
            }

            while (Operators.Count > 0)
            {
                Values.Push(ApplyOperator(Operators.Pop(), Values.Pop(), Values.Pop()));
            }

            return Values.Pop();
        }

        static double ApplyOperator(string op, double b, double a) // Function to apply operator
        {
            switch (op)
            {
                case "+": return a + b;
                case "-": return a - b;
                case "*": return a * b;
                case "/":
                    if (b == 0)
                        throw new DivideByZeroException();
                    return a / b;
                case "^": return Math.Pow(a, b);
                default: throw new ArgumentException(" Invalid operator");
            }
        }

        

        static void RunFileExtensionsArchive() // Read JSON and retrieve infos about entered extension
        {
            while (true)
            {
                Console.Clear();
                Console.WriteLine("");
                Console.WriteLine(" File Extensions archive");
                Console.WriteLine("");
                Console.WriteLine("");
                Console.WriteLine(" An Archive with all common and some less common file extensions.");
                Console.WriteLine("");
                Console.WriteLine(" It will display some info about the entered file extension.");
                Console.WriteLine("");
                Console.WriteLine("");
                Console.WriteLine(" Type 'help' to get more information about this function and see usage examples.");
                Console.WriteLine("");
                Console.WriteLine(" Type 'exit' to return to the main menu.");
                Console.WriteLine("");
                Console.WriteLine("");

                Console.Write(" Enter a file extension (For example .exe): ");
                string InputExtension = Console.ReadLine().ToLower();

                if (InputExtension == "exit")
                {
                    break;
                }

                if (InputExtension == "help")
                {
                    DisplayHelp("FileExtensionsArchive");
                    break;
                }

                if (string.IsNullOrWhiteSpace(InputExtension))
                {
                    Console.WriteLine("");
                    Console.WriteLine("");
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine(" The input is empty. Please enter a valid file extension.");
                    Console.ResetColor();
                    Console.ReadLine();
                    continue;
                }

                // Prepare the directory and file paths
                string BaseDirectory = AppDomain.CurrentDomain.BaseDirectory;
                string ProjectDirectory = Path.GetFullPath(Path.Combine(BaseDirectory, @"..\..\..\")); // Construct and format full path
                string JsonFilePath = Path.Combine(ProjectDirectory, "FileExtensions.json"); // Name of the .json file to resolve destination file

                if (File.Exists(JsonFilePath))
                {
                    try
                    {
                        // Read the JSON file content
                        string jsonContent = File.ReadAllText(JsonFilePath);

                        // Deserialize the JSON content into a dictionary
                        var fileExtensions = JsonConvert.DeserializeObject<Dictionary<string, FileExtensionInfo>>(jsonContent);

                        if (fileExtensions.ContainsKey(InputExtension))
                        {
                            var info = fileExtensions[InputExtension];
                            Console.WriteLine("");
                            Console.WriteLine("");
                            Console.WriteLine("");
                            Console.ForegroundColor = ConsoleColor.Cyan;
                            Console.WriteLine($" Extension: {info.Extension}");
                            Console.WriteLine("");
                            Console.ForegroundColor = ConsoleColor.DarkCyan;
                            Console.WriteLine($" Full Name: {info.FullName}");
                            Console.WriteLine("");
                            Console.WriteLine($" Description: {info.Description}");
                            Console.WriteLine("");
                            Console.ForegroundColor = ConsoleColor.Green;
                            Console.WriteLine($" Group: {info.Group}");

                            Task.Delay(20000).Wait();
                            Console.ResetColor();
                        }
                        else
                        {
                            Console.WriteLine("");
                            Console.WriteLine("");
                            Console.ForegroundColor = ConsoleColor.Red;
                            Console.WriteLine(" The file extension '" + InputExtension + "' does not exist in the JSON file.");
                            Console.ResetColor();
                            Task.Delay(5000).Wait();
                        }
                    }
                    catch (JsonException ex)
                    {
                        MessageBox.Show($"An error occurred while reading the JSON file: {ex.Message}", "Error reading file", MessageBoxButtons.OK, MessageBoxIcon.Error);

                        Task.Delay(5000).Wait();
                    }
                }
                else
                {
                    MessageBox.Show("File not found [FileExtentions.json]", "Error 404", MessageBoxButtons.OK, MessageBoxIcon.Error);

                    Task.Delay(10000).Wait();
                }
            }
        }

        static void RunDigitalStorageMeasurementSystem()
        {

            string Option;

            Console.Clear();
            Console.WriteLine("");
            Console.WriteLine(" System of measurements for digital storage units - Calculator");
            Console.WriteLine("");
            Console.WriteLine(" Opens a Windows Forms UI calculator for digital storage units.");
            Console.WriteLine("");
            Console.WriteLine("");
            Console.WriteLine(" Type 'help' to get more information about this function and see usage examples.");
            Console.WriteLine("");
            Console.WriteLine(" Type 'exit' to return to the main menu.");
            Console.WriteLine("");
            Console.WriteLine("");

            Console.Write(" Press any key or type 'exit' / 'help'... ");

            Option = Console.ReadLine().ToLower();

            string BaseDirectory = AppDomain.CurrentDomain.BaseDirectory;

            string ProjectDirectory = Path.GetFullPath(Path.Combine(BaseDirectory, @"..\..\..\ExternalPrograms"));

            string ProjectEXEFileName = "Digital-Storage-Units-Calculator.exe";

            string exePath = Path.Combine(ProjectDirectory, ProjectEXEFileName);

            if (Option == "exit")
            {
                return;
            }

            if (Option == "help")
            {
                DisplayHelp("DSMS");
                return;
            }

            if (File.Exists(exePath))
            {
                try
                {
                    // Start the process using the .lnk file
                    ProcessStartInfo startInfo = new ProcessStartInfo
                    {
                        FileName = exePath,
                        UseShellExecute = true // Use the operating system shell to start the process
                    };

                    using (Process process = Process.Start(startInfo))
                    {
                        Console.WriteLine("");
                        Console.ForegroundColor = ConsoleColor.Green;
                        Console.WriteLine(" Process started successfully: " + exePath);
                        Console.ResetColor();
                        // Wait for the process to exit
                        process.WaitForExit();
                        Console.WriteLine("");
                        Console.ForegroundColor = ConsoleColor.Yellow;
                        Console.WriteLine(" Process exited with exitcode: " + process.ExitCode);
                        Console.ResetColor();
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("An error occurred while starting the process: " + ex.Message, "Error starting process", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            else
            {
                MessageBox.Show("The file: " + exePath + " does not exist.", "Error 404", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            Task.Delay(5000).Wait();
        }

        static void RunDynamicPasswordGenerator() // Start external program (Relative Path) [Dynamic-Password-Generator.exe]
        {
            string Option;

            Console.Clear();
            Console.WriteLine("");
            Console.WriteLine(" Dynamic Password Generator");
            Console.WriteLine("");
            Console.WriteLine("");
            Console.WriteLine(" Opens a customizable Windows Forms UI password generator");
            Console.WriteLine("");
            Console.WriteLine(" Customizable elements: Password length and password buildig pools (Numbers, Letters etc.");
            Console.WriteLine("");
            Console.WriteLine("");
            Console.WriteLine(" Type 'help' to get more information about this function and see usage examples.");
            Console.WriteLine("");
            Console.WriteLine(" Type 'exit' to return to the main menu.");
            Console.WriteLine("");
            Console.WriteLine("");

            Console.Write(" Press any key or type 'exit' / 'help'... ");

            Option = Console.ReadLine().ToLower();

            string BaseDirectory = AppDomain.CurrentDomain.BaseDirectory;

            string ProjectDirectory = Path.GetFullPath(Path.Combine(BaseDirectory, @"..\..\..\ExternalPrograms"));

            string ProjectEXEFileName = "Dynamic-Password-Generator.exe";

            string exePath = Path.Combine(ProjectDirectory, ProjectEXEFileName);

            if (Option == "exit")
            {
                return;
            }

            if (Option == "help")
            {
                DisplayHelp("PasswordGenerator");
                return;
            }

            if (File.Exists(exePath))
            {
                try
                {
                    // Start the process using the .lnk file
                    ProcessStartInfo startInfo = new ProcessStartInfo
                    {
                        FileName = exePath,
                        UseShellExecute = true // Use the operating system shell to start the process
                    };

                    using (Process process = Process.Start(startInfo))
                    {
                        Console.WriteLine("");
                        Console.ForegroundColor = ConsoleColor.Green;
                        Console.WriteLine(" Process started successfully: " + exePath);
                        Console.ResetColor();
                        // Wait for the process to exit
                        process.WaitForExit();
                        Console.WriteLine("");
                        Console.ForegroundColor = ConsoleColor.Yellow;
                        Console.WriteLine(" Process exited with exitcode: " + process.ExitCode);
                        Console.ResetColor();
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("An error occurred while starting the process: " + ex.Message, "Error starting process", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            else
            {
                MessageBox.Show("The file: " + exePath + " does not exist.", "Error 404", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            Task.Delay(5000).Wait();
        }

        static void RunUnitConverter() // Start external program (Relative Path) [Unit-Converter.exe]
        {
            string Option;

            Console.Clear();
            Console.WriteLine("");
            Console.WriteLine(" Unit Converter");
            Console.WriteLine("");
            Console.WriteLine("");
            Console.WriteLine(" Opens a Windows Forms application to calculate different values like Size, Weight or volume values.");
            Console.WriteLine("");
            Console.WriteLine(" Choose Input and Output value type like Liters and Mililiters. Then specify the value (Number) and hit calculate.");
            Console.WriteLine("");
            Console.WriteLine("");
            Console.WriteLine(" Type 'help' to get more information about this function and see usage examples.");
            Console.WriteLine("");
            Console.WriteLine(" Type 'exit' to return to the main menu.");
            Console.WriteLine("");
            Console.WriteLine("");

            Console.Write(" Press any key or type 'exit' / 'help'... ");

            Option = Console.ReadLine().ToLower();

            string BaseDirectory = AppDomain.CurrentDomain.BaseDirectory;

            string ProjectDirectory = Path.GetFullPath(Path.Combine(BaseDirectory, @"..\..\..\ExternalPrograms"));

            string ProjectEXEFileName = "Unit-Converter.exe";

            string exePath = Path.Combine(ProjectDirectory, ProjectEXEFileName);

            if (Option == "exit")
            {
                return;
            }

            if (Option == "help")
            {
                DisplayHelp("UnitConverter");
                return;
            }

            if (File.Exists(exePath))
            {
                try
                {
                    // Start the process using the .lnk file
                    ProcessStartInfo startInfo = new ProcessStartInfo
                    {
                        FileName = exePath,
                        UseShellExecute = true // Use the operating system shell to start the process
                    };

                    using (Process process = Process.Start(startInfo))
                    {
                        Console.WriteLine("");
                        Console.ForegroundColor = ConsoleColor.Green;
                        Console.WriteLine(" Process started successfully: " + exePath);
                        Console.ResetColor();
                        // Wait for the process to exit
                        process.WaitForExit();
                        Console.WriteLine("");
                        Console.ForegroundColor = ConsoleColor.Yellow;
                        Console.WriteLine(" Process exited with exitcode: " + process.ExitCode);
                        Console.ResetColor();
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("An error occurred while starting the process: " + ex.Message, "Error starting process", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            else
            {
                MessageBox.Show("The file: " + exePath + " does not exist.", "Error 404", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            Task.Delay(5000).Wait();
        }

        static void RunSystemInformation() // Systeminformation Submenu (Main menu -> Systeminformation Submenu)
        {
            while (true)
            {
                Console.Clear();
                Console.WriteLine("");
                Console.ForegroundColor = ConsoleColor.DarkGray;
                Console.WriteLine("=============================");
                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.WriteLine("         Multitool");
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("     Systeminformation");
                Console.ForegroundColor = ConsoleColor.DarkGray;
                Console.WriteLine("=============================");
                Console.ResetColor();
                Console.WriteLine("");
                Console.WriteLine("");
                Console.WriteLine(" A collection of Tools for retrieving system Information");
                Console.WriteLine("");
                Console.WriteLine(" Possible Arguments: System Details, Hardware Monitoring, Energy Status");
                Console.WriteLine(" Enviroment Variables");
                Console.WriteLine("");
                Console.WriteLine("");
                Console.WriteLine(" Type 'exit' to return to the main menu.");
                Console.WriteLine("");
                Console.WriteLine("");

                Console.Write(" Option: ");

                String SystemInfoMenu = Console.ReadLine().ToLower();

                if (SystemInfoMenu == "exit")
                {
                    return;
                }

                switch (SystemInfoMenu)
                {
                    case System_Details:
                        {
                            RunSystemDetails();
                            break;
                        }

                    case Hardware_Monitoring:
                        {
                            RunHardwareMonitoring();
                            break;
                        }

                    case Energy_Status:
                        {
                            RunEnergyStatus();
                            break;
                        }

                    case Enviroment_Variables:
                        {
                            RunEnvironmentVariables();
                            break;
                        }

                    default:
                        Console.WriteLine("");
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine(" Invalid or empty argument!");
                        Console.WriteLine("");
                        Task.Delay(2000).Wait();
                        Console.ResetColor();
                        break;
                }
            }
        }

        public static void RunSystemDetails()
        {
            Console.Clear();
            Console.ForegroundColor = ConsoleColor.Cyan;  // Header color
            Console.WriteLine("");
            Console.WriteLine(" ===== System Information =====");
            Console.ResetColor();

            // Operating System
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("\n\n --Operating System-- \n");
            Console.ResetColor();
            Console.WriteLine($" OS Version: {(RuntimeInformation.OSDescription)}");
            Console.WriteLine($" Architecture: {RuntimeInformation.OSArchitecture}");
            DisplaySystemUptime();

            // System Manufacturer and Model
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("\n\n --System Model and Manufacturer-- \n");
            Console.ResetColor();
            DisplaySystemModel();

            // BIOS Information
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("\n\n --BIOS Information-- \n");
            Console.ResetColor();
            DisplayBIOSInfo();

            // Processor Info
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("\n\n --Processor Information-- \n");
            Console.ResetColor();
            DisplayCPUInfo();

            // RAM Info
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("\n\n --Memory Information--");
            Console.ResetColor();
            DisplayRAMInfo();

            // GPU Info
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("\n\n --Graphics Card Information-- \n");
            Console.ResetColor();
            DisplayGPUInfo();

            // Network Interfaces - MAC Addresses and IP Addresses and speed
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("\n --Network Interfaces (MAC Addresses and IP Addresses)--");
            Console.ResetColor();
            DisplayMACAddresses();

            // Disk Info
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("\n --Disk Information-- \n");
            Console.ResetColor();
            DisplayDiskInfo();

            // Peripheral Devices
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("\n --Peripheral Devices-- \n");
            Console.ResetColor();
            DisplayPeripheralDevices();

            Console.WriteLine("\n=============================================================");

            Console.ReadLine();
        }

        private static void DisplaySystemModel() // System Model
        {
            var searcher = new ManagementObjectSearcher("select * from Win32_ComputerSystem");
            foreach (var obj in searcher.Get())
            {
                Console.WriteLine($" Manufacturer: {obj["Manufacturer"]}");
                Console.WriteLine($" Model: {obj["Model"]}");
            }
        }

        private static void DisplayBIOSInfo() // Bios Version and Type
        {
            var searcher = new ManagementObjectSearcher("select * from Win32_BIOS");
            foreach (var obj in searcher.Get())
            {
                Console.WriteLine($" BIOS Version: {obj["SMBIOSBIOSVersion"]}");
                Console.WriteLine($" Release Date: {obj["ReleaseDate"]}");
                Console.WriteLine($" BIOS Manufacturer: {obj["Manufacturer"]}");
            }
        }

        private static void DisplayCPUInfo() // RTetrieve CPU information
        {
            var searcher = new ManagementObjectSearcher("select * from Win32_Processor");
            foreach (var obj in searcher.Get())
            {
                Console.WriteLine($" Name: {obj["Name"]}");
                Console.WriteLine($" Processor ID: {obj["ProcessorId"]}");
                Console.WriteLine($" Cores: {obj["NumberOfCores"]}");
                Console.WriteLine($" Logical Processors: {obj["NumberOfLogicalProcessors"]}");
            }
        }

        private static void DisplayRAMInfo()
        {
            var searcher = new ManagementObjectSearcher("select * from Win32_PhysicalMemory");
            ulong totalMemory = 0;
            int slotNumber = 1;

            foreach (var obj in searcher.Get())
            {
                // Retrieve and calculate capacity of each RAM stick
                ulong capacity = (ulong)obj["Capacity"];
                totalMemory += capacity;
                Console.WriteLine($"\n RAM Slot {slotNumber}:");
                Console.WriteLine($"   Size: {capacity / (1024 * 1024 * 1024)} GB");

                // Retrieve the speed of the RAM stick
                Console.WriteLine($"   Speed: {obj["Speed"] ?? "Unknown"} MHz");

                // Retrieve and interpret the memory type using MemoryType or SMBIOSMemoryType
                string memoryTypeStr = "Unknown Type";
                int memoryType = obj["MemoryType"] != null ? Convert.ToInt32(obj["MemoryType"]) : 0;

                if (memoryType == 0 && obj["SMBIOSMemoryType"] != null)
                {
                    memoryType = Convert.ToInt32(obj["SMBIOSMemoryType"]); // If the object searcher cant retrieve it from Win32_PhysicalMemory
                }

                switch (memoryType) // Type switch
                {
                    case 20: memoryTypeStr = "DDR"; break;
                    case 21: memoryTypeStr = "DDR-2"; break;
                    case 22: memoryTypeStr = "DDR-2 FB-DIMM"; break;
                    case 24: memoryTypeStr = "DDR-3"; break;
                    case 26: memoryTypeStr = "DDR-4"; break;
                    default: Console.WriteLine($"Unknown memory type code: {memoryType}"); break;
                }

                Console.WriteLine($"   Type: {memoryTypeStr}");

                slotNumber++;
            }

            Console.WriteLine($"\n Total RAM: {totalMemory / (1024 * 1024 * 1024)} GB"); // Calculate into GB
        }

        private static void DisplayMACAddresses() // Internet equipment (MAC, Controllers and Spped)
        {
            foreach (NetworkInterface adapter in NetworkInterface.GetAllNetworkInterfaces())
            {
                Console.WriteLine("");
                Console.WriteLine($" Adapter: {adapter.Description}");
                Console.WriteLine($" MAC Address: {adapter.GetPhysicalAddress()}");
                Console.WriteLine($" Status: {adapter.OperationalStatus}");
                foreach (var ip in adapter.GetIPProperties().UnicastAddresses)
                {
                    Console.WriteLine($" IP Address: {ip.Address}");
                }
                Console.WriteLine();
            }

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("\n --Network Interfaces (Speed)-- \n");
            Console.ResetColor();

            foreach (NetworkInterface adapter in NetworkInterface.GetAllNetworkInterfaces())
            {
                if (adapter.OperationalStatus == OperationalStatus.Up)
                {
                    Console.WriteLine($" Adapter: {adapter.Description}");
                    Console.WriteLine($" Speed: {adapter.Speed / 1_000_000} Mbps");
                    Console.WriteLine();
                }
            }
        }

        private static void DisplayDiskInfo() // Get all connected Disks (Size, Allocation and Type)
        {
            var searcher = new ManagementObjectSearcher("select * from Win32_DiskDrive");
            foreach (var obj in searcher.Get())
            {
                Console.WriteLine($" Disk Name: {obj["Model"]}");
                Console.WriteLine($" Serial Number: {obj["SerialNumber"]}");
                Console.WriteLine($" Interface Type: {obj["InterfaceType"]}");
                Console.WriteLine($" Media Type: {obj["MediaType"]}");
                Console.WriteLine($" Size: {((ulong)obj["Size"]) / (1024 * 1024 * 1024)} GB");
                Console.WriteLine();
            }
        }

        private static void DisplayGPUInfo() // Retrieve GPU information
        {
            var searcher = new ManagementObjectSearcher("select * from Win32_VideoController");
            foreach (var obj in searcher.Get())
            {
                Console.WriteLine($" Name: {obj["Name"] ?? "N/A"}");

                if (obj["AdapterRAM"] != null && ulong.TryParse(obj["AdapterRAM"].ToString(), out ulong adapterRam))
                {
                    Console.WriteLine($" Adapter RAM: {adapterRam / (1024 * 1024)} MB"); // Claculate VRAM
                }
                else
                {
                    Console.WriteLine("Adapter RAM: Not available");
                }

                Console.WriteLine($" Driver Version: {obj["DriverVersion"] ?? "N/A"}");
                Console.WriteLine($" Video Processor: {obj["VideoProcessor"] ?? "N/A"}");

                Console.WriteLine();
            }
        }

        private static void DisplaySystemUptime() // System uptime (How long is the device running)
        {
            var searcher = new ManagementObjectSearcher("SELECT LastBootUpTime FROM Win32_OperatingSystem");
            foreach (ManagementObject obj in searcher.Get().Cast<ManagementObject>())
            {
                DateTime lastBootUpTime = ManagementDateTimeConverter.ToDateTime(obj["LastBootUpTime"].ToString());
                TimeSpan uptime = DateTime.Now - lastBootUpTime;
                Console.WriteLine($" System Uptime: {uptime.Days}d {uptime.Hours}h {uptime.Minutes}m");
            }
        }

        private static void DisplayPeripheralDevices() // Info of connected devices
        {
            var searcher = new ManagementObjectSearcher("SELECT * FROM Win32_PnPEntity WHERE PNPClass = 'USB'");

            foreach (var obj in searcher.Get())
            {
                // Basic device information
                Console.WriteLine($" Device Name: {obj["Name"] ?? "Unknown"}");
                Console.WriteLine($" Device ID: {obj["DeviceID"] ?? "Unknown"}");

                // Device type (e.g., USB storage, mouse, keyboard, etc.)
                string deviceDescription = obj["Description"] != null ? obj["Description"].ToString() : "Unknown Type";
                Console.WriteLine($" Device Type: {deviceDescription}");

                // Check if the device is a USB storage device
                if (deviceDescription.Contains("Mass Storage"))
                {
                    var storageSearcher = new ManagementObjectSearcher("SELECT * FROM Win32_DiskDrive WHERE InterfaceType='USB'");
                    foreach (var storageObj in storageSearcher.Get())
                    {
                        Console.WriteLine($"   Storage Capacity: {((ulong)storageObj["Size"] / (1024 * 1024 * 1024))} GB");
                        Console.WriteLine($"   Media Type: {storageObj["MediaType"] ?? "Unknown"}");
                    }
                }

                // Optional: Check if the device supports power management and retrieve its status
                Console.WriteLine($" Status: {obj["Status"] ?? "Unknown"}");

                // USB Port Information (if available)
                string portNumber = obj["PNPDeviceID"]?.ToString();
                if (portNumber != null)
                {
                    if (portNumber.Contains("USB3"))
                    {
                        Console.WriteLine(" USB Version: USB 3.0");
                    }
                    else
                    {
                        Console.WriteLine(" USB Version: USB 2.0 or earlier");
                    }
                }
                Console.WriteLine();
            }
        }

        static void RunHardwareMonitoring() // Start external program (Relative Path) [Hardware-Monitor.exe]
        {
            string Option;

            Console.Clear();
            Console.WriteLine("");
            Console.WriteLine(" Hardware Monitoring");
            Console.WriteLine("");
            Console.WriteLine(" Opens a Windows Forms UI for monitoring the Hardware like CPU, RAM ...");
            Console.WriteLine("");
            Console.WriteLine("");
            Console.WriteLine(" Type 'exit' to return to the main menu.");
            Console.WriteLine("");
            Console.WriteLine("");

            Console.Write(" Press any key or type 'exit' / 'help'... ");

            Option = Console.ReadLine().ToLower();

            string BaseDirectory = AppDomain.CurrentDomain.BaseDirectory;

            string ProjectDirectory = Path.GetFullPath(Path.Combine(BaseDirectory, @"..\..\..\ExternalPrograms"));

            string ProjectEXEFileName = "Hardware-Monitor.exe";

            string exePath = Path.Combine(ProjectDirectory, ProjectEXEFileName);

            if (Option == "exit")
            {
                return;
            }

            if (File.Exists(exePath))
            {
                try
                {
                    // Start the process using the .lnk file
                    ProcessStartInfo startInfo = new ProcessStartInfo
                    {
                        FileName = exePath,
                        UseShellExecute = true // Use the operating system shell to start the process
                    };

                    using (Process process = Process.Start(startInfo))
                    {
                        Console.WriteLine("");
                        Console.ForegroundColor = ConsoleColor.Green;
                        Console.WriteLine(" Process started successfully: " + exePath);
                        Console.ResetColor();
                        // Wait for the process to exit
                        process.WaitForExit();
                        Console.WriteLine("");
                        Console.ForegroundColor = ConsoleColor.Yellow;
                        Console.WriteLine(" Process exited with exitcode: " + process.ExitCode);
                        Console.ResetColor();
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("An error occurred while starting the process: " + ex.Message, "Error starting process", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            else
            {
                MessageBox.Show("The file: " + exePath + " does not exist.", "Error 404", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            Task.Delay(5000).Wait();
        }

        static void RunEnergyStatus() // Retrieve Battery info (if available) and Powerconfig
        {
            Console.Clear();
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("");
            Console.WriteLine(" ===== Power and Battery Information =====");
            Console.ResetColor();

            DisplayPowerAndBatteryInfo();

            Console.WriteLine("\n=========================================");
        }

        private static void DisplayPowerAndBatteryInfo()
        {
            // Battery Information
            var powerStatus = SystemInformation.PowerStatus;
            if (powerStatus.PowerLineStatus == PowerLineStatus.Online)
            {
                Console.WriteLine("");
                Console.WriteLine("Power Source: AC Power");
                Console.WriteLine("No battery detected (This is likely a desktop PC).");
            }
            else
            {
                Console.WriteLine("\n--Battery Information--");
                Console.WriteLine("");
                Console.WriteLine("Power Source: Battery");
                Console.WriteLine($"Remaining Charge: {powerStatus.BatteryLifePercent * 100}%");
                Console.WriteLine($"Battery Charge Status: {powerStatus.BatteryChargeStatus}");
            }

            // Power Plan Information
           PowerPlanInfo();

           Console.ReadLine();
        }

        private static void PowerPlanInfo() // Powerplan of the device
        {
            Console.WriteLine("\n--Power Plan Information--");

            try
            {
                // Create a process to run the powercfg command
                var processStartInfo = new ProcessStartInfo
                {
                    FileName = @"C:\Windows\System32\powercfg.exe",
                    Arguments = "/getactivescheme",
                    RedirectStandardOutput = true,
                    UseShellExecute = false,
                    CreateNoWindow = true
                };

                using (var process = Process.Start(processStartInfo))
                {
                    using (var reader = process.StandardOutput)
                    {
                        string result = reader.ReadToEnd();
                        if (!string.IsNullOrEmpty(result))
                        {
                            Console.WriteLine(result);
                        }
                        else
                        {
                            Console.WriteLine("No active power plan found.");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error retrieving power plan information: " + ex.Message);
            }

            // Power source detection
            var powerSource = SystemInformation.PowerStatus.PowerLineStatus;
            Console.WriteLine($"Power Source: {(powerSource == PowerLineStatus.Online ? "AC Power" : "Battery")}");
        }

        static void RunEnvironmentVariables() // Display all enviromental variables
        {
            Console.Clear();
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("");
            Console.WriteLine("===== Environment Variables =====");
            Console.ResetColor();
            Console.WriteLine();

            // Get all environment variables
            var environmentVariables = Environment.GetEnvironmentVariables();

            int i = 0;

            // Display each environment variable
            foreach (DictionaryEntry variable in environmentVariables)
            {
                i++;

                Console.WriteLine();
                Console.WriteLine($"{i}: {variable.Key}: {variable.Value}");
            }

            Console.WriteLine("\n=======================================================");
            Console.ResetColor();

            Console.ReadLine();
        }

        static void AnimateText(string text, ConsoleColor color, int delay = 80) // Text animation for info part
        {
            Console.ForegroundColor = color;

            foreach (char c in text)
            {
                Console.Write(c);
                Thread.Sleep(delay); // Delay in milliseconds
            }

            Console.ResetColor(); // Reset color after the animation
        }

        static void RunTextEn_Decryption() // Text En/Decryption submenu (Main menu -> Text En/Decryption submenu)
        {
            while (true)
            {
                Console.Clear();
                Console.WriteLine("");
                Console.ForegroundColor = ConsoleColor.DarkGray;
                Console.WriteLine("=============================");
                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.WriteLine("         Multitool");
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("     Text En/Decryption");
                Console.ForegroundColor = ConsoleColor.DarkGray;
                Console.WriteLine("=============================");
                Console.ResetColor();
                Console.WriteLine("");
                Console.WriteLine("");
                Console.WriteLine(" A collection of different text en/decryption methods.");
                Console.WriteLine("");
                Console.WriteLine(" Possible arguments: Hashing, AES, BASE64, Caesar Cipher");
                Console.WriteLine("");
                Console.WriteLine("");
                Console.WriteLine(" Type 'exit' to return to the main menu.");
                Console.WriteLine("");
                Console.WriteLine("");

                Console.Write(" Option: ");

                string En_DecryptionMenu = Console.ReadLine().ToLower();

                if (En_DecryptionMenu == "exit")
                {
                    return;
                }

                switch (En_DecryptionMenu)
                {
                    case Hashing:
                        {
                            string HASHED;

                            Console.Clear();
                            Console.WriteLine("");
                            Console.WriteLine(" Hashing");
                            Console.WriteLine("");
                            Console.WriteLine("");
                            Console.WriteLine(" Possible encryption strengths: SHA160, SHA256, SHA384, SHA512");
                            Console.WriteLine("");
                            Console.WriteLine("");
                            Console.Write(" Strength: ");

                            string SHAVer = Console.ReadLine().ToLower(); // Variable for Strength of Hashing

                            Console.WriteLine("");
                            Console.WriteLine("");
                            Console.Write(" Enter text: ");
                            string SHA_Text = Console.ReadLine(); // Text to Hash

                            HASHED = RunTextHasher(SHAVer, SHA_Text);

                            Console.WriteLine("");
                            Console.WriteLine("");
                            Console.WriteLine(" The Hashed text is: " + HASHED);

                            Console.ReadLine();

                            break;
                        }

                    case AES_En_Decryption:
                        {
                            Console.Clear();
                            Console.WriteLine("");
                            Console.WriteLine(" AES En/Decryption");
                            Console.WriteLine("");
                            Console.WriteLine(" Please Choose one of the following options:");
                            Console.WriteLine("");
                            Console.WriteLine(" 1. Encrypt Text");
                            Console.WriteLine(" 2. Decrypt Text");
                            Console.WriteLine("");
                            Console.WriteLine("");
                            Console.WriteLine(" Type 'help' to get more information about this function and see usage examples.");
                            Console.WriteLine("");
                            Console.WriteLine(" Type 'exit' to return to the main menu.");
                            Console.WriteLine("");
                            Console.WriteLine("");
                            Console.Write(" Option: ");

                            string option = Console.ReadLine().ToLower();

                            if (option == "exit")
                            {
                                return;
                            }

                            switch (option)
                            {
                                case "1":
                                    RunAESEncrypt();
                                    break;

                                case "2":
                                    RunAESDecrypt();
                                    break;

                                case "help":
                                    DisplayHelp("AES");
                                    break;

                                default:
                                    Console.WriteLine("");
                                    Console.ForegroundColor = ConsoleColor.Red;
                                    Console.WriteLine(" Invalid or empty argument!");
                                    Task.Delay(2000).Wait();
                                    Console.ResetColor();
                                    break;
                            }
                            break;
                        }

                    case BASE64:
                        {
                            Console.Clear();
                            Console.WriteLine("");
                            Console.WriteLine(" BASE64 En/Decoding");
                            Console.WriteLine("");
                            Console.WriteLine(" Please Choose one of the following options:");
                            Console.WriteLine("");
                            Console.WriteLine(" 1. Encode Text");
                            Console.WriteLine(" 2. Decode Text");
                            Console.WriteLine("");
                            Console.WriteLine("");
                            Console.WriteLine(" Type 'help' to get more information about this function and see usage examples.");
                            Console.WriteLine("");
                            Console.WriteLine(" Type 'exit' to return to the main menu.");
                            Console.WriteLine("");
                            Console.WriteLine("");
                            Console.Write(" Option: ");

                            string option = Console.ReadLine().ToLower();

                            if (option == "exit")
                            {
                                return;
                            }

                            switch (option)
                            {
                                case "1":
                                    RunBASE64Encode();
                                    break;

                                case "2":
                                    RunBASE64Decode();
                                    break;

                                case "help":
                                    DisplayHelp("Base64");
                                    break;

                                default:
                                    Console.WriteLine("");
                                    Console.ForegroundColor = ConsoleColor.Red;
                                    Console.WriteLine(" Invalid or empty argument!");
                                    Task.Delay(2000).Wait();
                                    Console.ResetColor();
                                    break;
                            }
                            break;
                        }

                    case Caesar_Cipher:
                        {
                            RunCaesarCipher();
                            break;
                        }

                    default:
                        Console.WriteLine("");
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine(" Invalid or empty argument!");
                        Console.WriteLine("");
                        Task.Delay(2000).Wait();
                        Console.ResetColor();
                        break;
                }
            }
        }

        static string RunTextHasher(string SHAVer, string SHA_Text)
        {
            var algorithms = new Dictionary<string, Func<HashAlgorithm>> // Choose Algorythm
            {
            { "sha160", () => SHA1.Create() },
            { "sha256", () => SHA256.Create() },
            { "sha384", () => SHA384.Create() },
            { "sha512", () => SHA512.Create() }
            };

            if (!algorithms.TryGetValue(SHAVer, out var algorithmFactory))
            {
                Console.WriteLine("");
                return " Error! No valid SHA strength specified...";
            }

            using (var algorithm = algorithmFactory())
            {
                byte[] hashBytes = algorithm.ComputeHash(Encoding.UTF8.GetBytes(SHA_Text)); // Generate Hash
                return BitConverter.ToString(hashBytes).Replace("-", "").ToLower();
            }
        }

        static void RunAESEncrypt()
        {
            Console.Clear();
            Console.WriteLine("");
            Console.WriteLine(" AES Encryption");
            Console.WriteLine("");
            Console.Write(" Enter text to encrypt: ");

            string plainText = Console.ReadLine(); // Text to encrypt

            if (string.IsNullOrEmpty(plainText))
            {
                Console.WriteLine("");
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine(" String is empty. Please enter a text.");
                Task.Delay(2000).Wait();
                Console.ResetColor();
                return;
            }

            string key = GetKey(requiredLength: 16, isDecrypting: false); // Call GetKey Method for random Key generation (If user presses Insert)
            Console.WriteLine("");
            Console.WriteLine("");

            string encrypted = AESEncrypt(plainText, key); // Encrypt text with Encryption Method
            Console.WriteLine(" Encrypted text: " + encrypted);

            Console.ReadLine();
        }

        static void RunAESDecrypt()
        {
            Console.Clear();
            Console.WriteLine("");
            Console.WriteLine(" AES Decryption");
            Console.WriteLine("");
            Console.Write(" Enter text to decrypt: ");

            string cipherText = Console.ReadLine();

            if (string.IsNullOrEmpty(cipherText))
            {
                Console.WriteLine("");
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine(" String is empty. Please enter a text.");
                Task.Delay(2000).Wait();
                Console.ResetColor();
                return;
            }

            string key = GetKey(requiredLength: 16, isDecrypting: true); // Deactivate GetKey method since user must insert encryption Key

            string decrypted = AESDecrypt(cipherText, key); // Decrypt text with Decryption Method
            Console.WriteLine("\n \n Decrypted text: " + decrypted);

            Console.ReadLine();
        }

        static string GetKey(int requiredLength, bool isDecrypting)
        {
            Console.WriteLine("");
            Console.Write(isDecrypting
                ? $" Enter the key (must be {requiredLength} characters long): "
                : $" Enter a key (must be {requiredLength} characters long) or press 'Insert' to generate a key: ");

            string key = string.Empty;
            ConsoleKeyInfo keyInfo;

            while (key.Length < requiredLength)
            {
                keyInfo = Console.ReadKey(true);

                if (!isDecrypting && keyInfo.Key == ConsoleKey.Insert)
                {
                    key = GenerateRandomKey(requiredLength);
                    Console.WriteLine("");
                    Console.WriteLine($"\n Generated Key: {key}");
                    break;
                }
                else if (keyInfo.Key == ConsoleKey.Backspace && key.Length > 0)
                {
                    key = key.Substring(0, key.Length - 1);
                    Console.Write("\b \b");
                }
                else if (!char.IsControl(keyInfo.KeyChar))
                {
                    key += keyInfo.KeyChar;
                    Console.Write(keyInfo.KeyChar);
                }
            }

            if (key.Length != requiredLength)
            {
                Console.WriteLine($"\n Key must be exactly {requiredLength} characters long.");
                return GetKey(requiredLength, isDecrypting);
            }

            return key;
        }

        // Generate a random key of the specified length
        static string GenerateRandomKey(int length)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
            Random random = new Random();
            return new string(Enumerable.Repeat(chars, length).Select(s => s[random.Next(s.Length)]).ToArray());
        }

        static string AESEncrypt(string plainText, string key)
        {
            if (key.Length == 16)
            {

                try
                {
                    using (Aes aesAlg = Aes.Create())
                    {
                        aesAlg.Key = Encoding.UTF8.GetBytes(key); // Read Key to use it for encryption
                        aesAlg.GenerateIV(); // Generate random IV (Initialisation Vector [For enhanced security the encrypted text is always unique with this random IV] so patterns cannot be searched)
                        byte[] iv = aesAlg.IV;

                        ICryptoTransform encryptor = aesAlg.CreateEncryptor(aesAlg.Key, aesAlg.IV); // Initialize Encryptor

                        using (MemoryStream msEncrypt = new MemoryStream())
                        {
                            msEncrypt.Write(iv, 0, iv.Length); // Prepend IV
                            using (CryptoStream csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
                            {
                                using (StreamWriter swEncrypt = new StreamWriter(csEncrypt))
                                {
                                    swEncrypt.Write(plainText);
                                }
                            }
                            return Convert.ToBase64String(msEncrypt.ToArray());
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("");
                    return " An error occurred during AES encryption: " + ex.Message;
                }
            }

            else
            {
                Console.WriteLine("");
                throw new ArgumentException(" Key must be exactly 16 characters long.");
            }
        }

        static string AESDecrypt(string cipherText, string key)
        {
            if (key.Length == 16)
            {
                try
                {
                    byte[] fullCipher = Convert.FromBase64String(cipherText); // Read whole encrypted text
                    byte[] iv = new byte[16];                                 // Only read unique IV from encrypted text
                    byte[] cipher = new byte[fullCipher.Length - iv.Length];  // Only read text to decrypt

                    Array.Copy(fullCipher, 0, iv, 0, iv.Length);
                    Array.Copy(fullCipher, iv.Length, cipher, 0, cipher.Length);

                    using (Aes aesAlg = Aes.Create())
                    {
                        aesAlg.Key = Encoding.UTF8.GetBytes(key);
                        aesAlg.IV = iv;

                        ICryptoTransform decryptor = aesAlg.CreateDecryptor(aesAlg.Key, aesAlg.IV); // Initialize Decryptor

                        using (MemoryStream msDecrypt = new MemoryStream(cipher))
                        {
                            using (CryptoStream csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
                            {
                                using (StreamReader srDecrypt = new StreamReader(csDecrypt))
                                {
                                    return srDecrypt.ReadToEnd();
                                }
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("");
                    return " An error occurred during AES decryption: " + ex.Message;
                }
            }

            else
            {
                Console.WriteLine("");
                throw new ArgumentException(" Key must be exactly 16 characters long.");
            }
        }

        static void RunBASE64Encode()
        {
            try
            {
                Console.Clear();
                Console.WriteLine("");
                Console.WriteLine(" Base64 Encode");
                Console.WriteLine("");
                Console.Write(" Enter text to encode: ");

                string plainText = Console.ReadLine();
                string encodedText = Base64Encode(plainText);
                Console.WriteLine("");
                Console.WriteLine(" Encoded text: " + encodedText);

                Console.ReadLine();
            }
            catch (Exception ex)
            {
                Console.WriteLine("");
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("An error occurred during Base64 encoding: " + ex.Message);
                Console.ResetColor();
                Task.Delay(2000).Wait();
            }
        }

        static void RunBASE64Decode()
        {
            try
            {
                Console.Clear();
                Console.WriteLine("");
                Console.WriteLine(" Base64 Decode");
                Console.WriteLine("");
                Console.Write(" Enter text to decode: ");

                string encodedText = Console.ReadLine();
                string decodedText = Base64Decode(encodedText);
                Console.WriteLine("");
                Console.WriteLine(" Decoded text: " + decodedText);

                Console.ReadLine();
            }
            catch (Exception ex)
            {
                Console.WriteLine("");
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("An error occurred during Base64 decoding: " + ex.Message);
                Console.ResetColor();
                Task.Delay(2000).Wait();
            }
        }

        static string Base64Encode(string plainText)
        {
            var plainTextBytes = Encoding.UTF8.GetBytes(plainText); // Encode text
            return Convert.ToBase64String(plainTextBytes);
        }

        static string Base64Decode(string base64EncodedData)
        {
            var base64EncodedBytes = Convert.FromBase64String(base64EncodedData); // Decode text
            return Encoding.UTF8.GetString(base64EncodedBytes);
        }

        static void RunCaesarCipher() // Caesar Cipher submenu (Main menu -> Text En/Decryption submenu -> Caesar Cipher submenu)
        {
            Console.Clear();
            Console.WriteLine("");
            Console.WriteLine(" Caesar Cipher");
            Console.WriteLine("");
            Console.WriteLine(" Please Choose one of the following options:");
            Console.WriteLine("");
            Console.WriteLine(" 1. Encrypt Text");
            Console.WriteLine(" 2. Decrypt Text");
            Console.WriteLine("");
            Console.WriteLine("");
            Console.WriteLine(" Type 'help' to get more information about this function and see usage examples.");
            Console.WriteLine("");
            Console.WriteLine(" Type 'exit' to return to the main menu.");
            Console.WriteLine("");
            Console.WriteLine("");
            Console.Write(" Option: ");

            string option = Console.ReadLine().ToLower();

            if (option == "exit")
            {
                return;
            }

            switch (option)
            {
                case "1":
                    RunCaesarEncrypt();
                    break;

                case "2":
                    RunCaesarDecrypt();
                    break;

                case "help":
                    DisplayHelp("CaesarCipher");
                    break;

                default:
                    Console.WriteLine("");
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine(" Invalid or empty argument!");
                    Task.Delay(2000).Wait();
                    Console.ResetColor();
                    break;
            }
        }

        static void RunCaesarEncrypt() // Character shift encryption
        {
            Console.Clear();
            Console.WriteLine("");
            Console.WriteLine(" Caesar Cipher Encryption");
            Console.WriteLine("");
            Console.Write(" Enter text to encrypt: ");
            string plainText = Console.ReadLine();

            if (string.IsNullOrEmpty(plainText))
            {
                Console.WriteLine("");
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine(" String is empty. Please enter a text.");
                Task.Delay(2000).Wait();
                Console.ResetColor();
                return;
            }

            Console.WriteLine("");

            Console.Write(" Enter shift amount (integer): "); // Validate Integer input in right range
            if (!int.TryParse(Console.ReadLine(), out int shift))
            {
                Console.WriteLine("");
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine(" Shift should be a number between -25 and 25!");
                Task.Delay(2000).Wait();
                Console.ResetColor();
                return;
            }

            string encrypted = CaesarCipherEncrypt(plainText, shift); // Call Caesar Encrypt Method
            Console.WriteLine("");
            Console.WriteLine(" Encrypted text: " + encrypted);

            Console.ReadLine();
        }

        static string CaesarCipherEncrypt(string plainText, int shift)
        {
            try
            {
                // Ensure shift is within 0-25
                shift = (shift % 26 + 26) % 26;

                StringBuilder result = new StringBuilder(); // Initialize StringBuilder

                foreach (char c in plainText)
                {
                    if (char.IsLetter(c))
                    {
                        char offset = char.IsUpper(c) ? 'A' : 'a';
                        char enc = (char)(((c - offset + shift) % 26) + offset); // Encrypt
                        result.Append(enc);
                    }
                    else
                    {
                        result.Append(c); // Non-letter characters remain unchanged
                    }
                }

                return result.ToString();
            }
            catch (Exception ex)
            {
                Console.WriteLine("");
                return "An error occurred during Caesar cipher encryption: " + ex.Message;
            }
        }

        static void RunCaesarDecrypt()
        {
            Console.Clear();
            Console.WriteLine("");
            Console.WriteLine(" Caesar Cipher Decryption");
            Console.WriteLine("");
            Console.Write(" Enter text to decrypt: ");
            string cipherText = Console.ReadLine();

            if (string.IsNullOrEmpty(cipherText))
            {
                Console.WriteLine("");
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine(" String is empty. Please enter a text.");
                Task.Delay(2000).Wait();
                Console.ResetColor();
                return;
            }

            Console.WriteLine("");

            Console.Write(" Enter shift amount (integer): "); // Validate Integer input in right range
            if (!int.TryParse(Console.ReadLine(), out int shift))
            {
                Console.WriteLine("");
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine(" Shift should be a number between -25 and 25!");
                Task.Delay(2000).Wait();
                Console.ResetColor();
                return;
            }

            string decrypted = CaesarCipherDecrypt(cipherText, shift);
            Console.WriteLine("");
            Console.WriteLine(" Decrypted text: " + decrypted);

            Console.ReadLine();
        }

        static string CaesarCipherDecrypt(string cipherText, int shift)
        {
            try
            {
                // Ensure shift is within 0-25
                shift = (shift % 26 + 26) % 26;

                return CaesarCipherEncrypt(cipherText, 26 - shift); // Decrypting is just shifting backwards
            }
            catch (Exception ex)
            {
                Console.WriteLine("");
                return "An error occurred during Caesar cipher decryption: " + ex.Message;
            }
        }

        static void RunNetworkTools() // Network tools main menu (Main menu ->  Network tools Submenu)
        {
            while (true)
            {
                Console.SetWindowSize(120, 34);
                Console.Clear();
                Console.WriteLine("");
                Console.ForegroundColor = ConsoleColor.DarkGray;
                Console.WriteLine("=============================");
                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.WriteLine("         Multitool");
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("       Network Tools");
                Console.ForegroundColor = ConsoleColor.DarkGray;
                Console.WriteLine("=============================");
                Console.ResetColor();
                Console.WriteLine("");
                Console.WriteLine("");
                Console.WriteLine(" A collection of different network tools");
                Console.WriteLine("");
                Console.WriteLine(" Possible arguments: Ping, Traceroute, DNS Lookup, HTTP Requests, Port Scanner");
                Console.WriteLine("                     NII (Network Interface Information)");
                Console.WriteLine("");
                Console.WriteLine("");
                Console.WriteLine(" Type 'exit' to return to the main menu.");
                Console.WriteLine("");
                Console.WriteLine("");

                Console.Write(" Option: ");

                string Network_Tools_Menu = Console.ReadLine().ToLower();

                if (Network_Tools_Menu == "exit")
                {
                    return;
                }

                switch (Network_Tools_Menu)
                {
                    case Ping:
                        {
                            RunPing().Wait();
                            break;
                        }

                    case Traceroute:
                        {
                            RunTraceRoute();
                            break;
                        }

                    case DNS_Lookup:
                        {
                            RunDNSLookup();
                            break;
                        }

                    case HTTP_Requests:
                        {
                            RunHTTPRequests().Wait();
                            break;
                        }

                    case Port_Scanner:
                        {
                            RunPortScanner().Wait();
                            break;
                        }

                    case Network_Interface_Information:
                        {
                            RunNII();
                            break;
                        }

                    default:
                        Console.WriteLine("");
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine(" Invalid or empty argument!");
                        Task.Delay(2000).Wait();
                        Console.ResetColor();
                        break;
                }
            }
        }

        static async Task RunPing()
        {
            Console.Clear();
            Console.SetWindowSize(120, 50);
            Console.WriteLine("");
            Console.WriteLine(" Pinging tool");
            Console.WriteLine("");
            Console.WriteLine("");
            Console.WriteLine(" Type 'help' to get more information about this function and see usage examples.");
            Console.WriteLine("");
            Console.WriteLine(" Type 'exit' to return to the main menu.");
            Console.WriteLine("");
            Console.WriteLine("");
            Console.WriteLine(" Enter an IP address or hostname to ping.");
            Console.WriteLine("");
            Console.Write(" IP or Hostname: ");

            string target = Console.ReadLine();

            if (target == "exit")
            {
                return;
            }

            if (target == "help")
            {
                Console.SetWindowSize(120, 34);
                DisplayHelp("Ping");
                return;
            }

            if (string.IsNullOrWhiteSpace(target))
            {
                Console.WriteLine("");
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine(" The input is empty. Please enter a valid IP or hostname.");
                Console.ResetColor();
                Console.ReadLine();
                return;
            }

            Console.WriteLine("");
            Console.Write(" Number of pings to execute: ");

            if (!int.TryParse(Console.ReadLine(), out int numberOfPings) || numberOfPings <= 0 || numberOfPings > 100) // Validate Integer input in right range
            {
                Console.WriteLine("");
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine(" The number of pings must be a positive number between 1 and 100.");
                Task.Delay(2000).Wait();
                Console.ResetColor();
                return;
            }

            Console.WriteLine("");
            Console.Write(" Perform reverse DNS lookup? (y/n): ");
            bool reverseDns = Console.ReadLine().ToLower() == "y"; // To resolve to server names in addition to IP
            Console.WriteLine("");
            Console.WriteLine("");

            string resolvedTarget = target;

            if (reverseDns)
            {
                string hostName = GetHostName(target);
                if (!string.IsNullOrEmpty(hostName))
                {
                    resolvedTarget = hostName;
                }
            }

            Ping ping = new Ping();
            List<long> latencies = new List<long>(); // List for storing results

            Console.WriteLine("");
            Console.WriteLine($" Pinging '{resolvedTarget}'...");
            Console.WriteLine("");

            try
            {
                for (int i = 0; i < numberOfPings; i++) // Ping for specified times
                {
                    PingReply reply = ping.Send(target);
                    if (reply.Status == IPStatus.Success)
                    {
                        latencies.Add(reply.RoundtripTime);
                        // Set the color based on latency
                        if (reply.RoundtripTime < 50)
                        {
                            Console.ForegroundColor = ConsoleColor.Green; // Fast ping
                        }
                        else if (reply.RoundtripTime >= 50 && reply.RoundtripTime <= 150)
                        {
                            Console.ForegroundColor = ConsoleColor.Yellow; // Moderate ping
                        }
                        else
                        {
                            Console.ForegroundColor = ConsoleColor.Red; // Slow ping
                        }

                        Console.WriteLine($" Ping {i + 1}: {reply.RoundtripTime} ms");
                    }
                    else
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine($" Ping {i + 1}: Failed ({reply.Status})");
                        Console.ResetColor();
                    }

                    Console.ResetColor(); // Reset color after each ping
                    await Task.Delay(1000);
                }

                if (latencies.Count > 0) // Format and display final reults
                {
                    long minLatency = latencies.Min();
                    long maxLatency = latencies.Max();
                    double avgLatency = latencies.Average();

                    Console.WriteLine("");
                    Console.WriteLine(" Minimum Latency: " + minLatency + "ms");
                    Console.WriteLine(" Maximum Latency: " + maxLatency + "ms");
                    Console.WriteLine($" Average Latency: {avgLatency:F2} ms");

                    Console.WriteLine("");
                    Console.WriteLine($" Pinging of '{resolvedTarget}' completed...");
                }
                else
                {
                    Console.WriteLine("");
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine(" No successful pings were received.");
                    Console.ResetColor();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("");
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine(" An error occurred: " + ex.Message);
                Console.ResetColor();
            }

            Console.ReadLine();
            Console.SetWindowSize(120, 34);
        }

        static void RunTraceRoute()
        {
            Console.Clear();
            Console.SetWindowSize(120, 50);

            Console.WriteLine("");
            Console.WriteLine(" Trace Route Tool");
            Console.WriteLine("");
            Console.WriteLine("");
            Console.WriteLine(" Type 'help' to get more information about this function and see usage examples.");
            Console.WriteLine("");
            Console.WriteLine(" Type 'exit' to return to the main menu.");
            Console.WriteLine("");

            Console.Write(" Enter the IP address or hostname to trace: ");
            string target = Console.ReadLine();

            if (string.IsNullOrEmpty(target))
            {
                Console.WriteLine("");
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine(" Target cannot be empty.");
                Task.Delay(2000).Wait();
                Console.ResetColor();
                return;
            }

            if (target == "exit")
            {
                return;
            }

            if (target == "help")
            {
                Console.SetWindowSize(120, 34);
                DisplayHelp("TraceRoute");
                return;
            }

            Console.WriteLine("");
            Console.Write(" Perform reverse DNS lookup? (y/n): ");
            bool reverseDns = Console.ReadLine().ToLower() == "y"; // To resolve to server names in addition to IP

            int ttl;

            try
            {
                Ping ping = new Ping();
                PingOptions options = new PingOptions(1, true); // Start with TTL = 1
                string data = new string('a', 32);
                byte[] buffer = System.Text.Encoding.ASCII.GetBytes(data);

                Console.WriteLine("");
                Console.WriteLine("");
                Console.WriteLine($" Tracing route to '{target}'...");
                Console.WriteLine("");

                for (ttl = 1; ttl <= 30; ttl++) // Perform trace for Max 30 Hops
                {
                    options.Ttl = ttl;
                    PingReply reply = ping.Send(target, 5000, buffer, options); // Send data to trace

                    if (reply.Status == IPStatus.TtlExpired || reply.Status == IPStatus.Success)
                    {
                        string hopInfo = reply.Address.ToString();
                        if (reverseDns)
                        {
                            string hostName = GetHostName(reply.Address.ToString()); // Get Adress and Hostname
                            if (!string.IsNullOrEmpty(hostName))
                            {
                                hopInfo += $" | {hostName}";
                            }
                        }
                        Console.WriteLine($" {ttl} {hopInfo} {reply.RoundtripTime} ms"); // Format Output

                        if (reply.Status == IPStatus.Success)
                            break;
                    }
                    else if (reply.Status == IPStatus.TimedOut)
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine($" {ttl} Request timed out.");
                        Console.ResetColor();
                    }
                    else
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine($" {ttl} Error: {reply.Status}");
                        Console.ResetColor();
                    }
                    Task.Delay(1000).Wait();
                }

                Console.WriteLine("");
                Console.WriteLine(" Trace to target '" + target + "' completed."); // Format and Display final results
                Console.WriteLine("");
                Console.WriteLine(" Total hops: " + ttl);
            }
            catch (Exception ex)
            {
                Console.WriteLine("");
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($" An error occurred: {ex.Message}");
                Console.ResetColor();
            }

            Console.ReadLine();
            Console.SetWindowSize(120, 34);
        }

        static void RunDNSLookup()
        {
            Console.Clear();
            Console.WriteLine("");
            Console.WriteLine(" DNS Lookup Tool");
            Console.WriteLine("");

            Console.Write(" Enter the domain name: ");
            string domain = Console.ReadLine();

            if (string.IsNullOrWhiteSpace(domain))
            {
                Console.WriteLine("");
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine(" The input is empty. Please enter a valid domain name.");
                Console.ResetColor();
                Console.ReadLine();
                return;
            }

            if (domain == "exit")
            {
                return;
            }

            if (domain == "help")
            {
                DisplayHelp("DNSLookup");
            }

            try
            {
                IPHostEntry hostEntry = Dns.GetHostEntry(domain);

                Console.WriteLine("");
                Console.WriteLine($" Domain: {domain}");
                Console.WriteLine("");
                Console.WriteLine(" IP Addresses:");
                foreach (IPAddress ip in hostEntry.AddressList) // Search for entries and display them
                {
                    Console.WriteLine($" - {ip}");
                }

                if (hostEntry.Aliases.Length > 0) // Search for entries and display them if count is bigger than 0
                {
                    Console.WriteLine("");
                    Console.WriteLine(" Aliases:");
                    foreach (string alias in hostEntry.Aliases)
                    {
                        Console.WriteLine($" - {alias}");
                    }
                }
                else
                {
                    Console.WriteLine("");
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.WriteLine(" No aliases found.");
                    Console.ResetColor();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("");
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($" An error occurred: {ex.Message}");
                Console.ResetColor();
            }

            Console.ReadLine();
        }

        static string GetHostName(string ipAddress) // Method for resolving Ip to Hostname
        {
            if (dnsCache.ContainsKey(ipAddress)) // Search for same ip in chace
            {
                return dnsCache[ipAddress];
            }

            try
            {
                IPHostEntry hostEntry = Dns.GetHostEntry(ipAddress); // Convert to Hostname
                dnsCache[ipAddress] = hostEntry.HostName;
                return hostEntry.HostName;
            }
            catch (Exception)
            {
                dnsCache[ipAddress] = null;
                return null;
            }
        }

        static async Task RunHTTPRequests() // Network tools submenu (Main menu -> Network tools submenu -> HTTP Requests submenu)
        {
            while (true)
            {
                Console.SetWindowSize(120, 34);
                Console.Clear();
                Console.WriteLine("");
                Console.WriteLine(" HTTP Requests");
                Console.WriteLine("");
                Console.WriteLine("");
                Console.WriteLine(" Possible options: GET, POST, PUT, DELETE, help, exit");
                Console.WriteLine("");
                Console.Write(" Option: ");
                string option = Console.ReadLine().ToLower();

                if (option == "exit")
                {
                    break;
                }

                if (option == "help")
                {
                    DisplayHelp("HTTPRequests");
                }

                switch (option)
                {
                    case "get":
                        await RunGetRequest();
                        break;
                    case "post":
                        await RunPostRequest();
                        break;
                    case "put":
                        await RunPutRequest();
                        break;
                    case "delete":
                        await RunDeleteRequest();
                        break;
                    default:
                        Console.WriteLine("");
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine(" Invalid or empty option!");
                        Console.ResetColor();
                        break;
                }
            }
        }

        static async Task RunGetRequest() // Get data from server
        {
            Console.Clear();
            Console.WriteLine("");
            Console.WriteLine(" GET Request Tool");
            Console.WriteLine("");
            Console.Write(" Enter a URL: ");
            string url = Console.ReadLine();
            Console.WriteLine("");

            if (string.IsNullOrWhiteSpace(url))
            {
                Console.WriteLine("");
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine(" The URL is empty. Please enter a valid URL.");
                Console.ResetColor();
                return;
            }

            try
            {
                using (HttpClient client = new HttpClient())
                {
                    HttpResponseMessage response = await client.GetAsync(url);
                    string responseData = await response.Content.ReadAsStringAsync();

                    // Check if the response is valid JSON
                    if (IsJson(responseData))
                    {
                        // Parse and format the JSON
                        var parsedJson = JsonConvert.DeserializeObject(responseData);
                        string formattedJson = JsonConvert.SerializeObject(parsedJson, Formatting.Indented);

                        Console.WriteLine(" Response (formatted):");
                        Console.WriteLine(formattedJson);
                    }
                    else
                    {
                        Console.WriteLine(" Response:");
                        Console.WriteLine(responseData);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("");
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($" An error occurred: {ex.Message}");
                Console.ResetColor();
            }

            Console.WriteLine("");
            Console.WriteLine(" Press Enter to continue...");
            Console.ReadLine();
        }

        static async Task RunPostRequest() // Post data to server
        {
            Console.Clear();
            Console.WriteLine("");
            Console.WriteLine(" POST Request Tool");
            Console.WriteLine("");
            Console.Write(" Enter a URL: ");
            string url = Console.ReadLine();
            Console.WriteLine("");

            if (string.IsNullOrWhiteSpace(url))
            {
                Console.WriteLine("");
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine(" The URL is empty. Please enter a valid URL.");
                Console.ResetColor();
                return;
            }

            Console.Write(" Enter the JSON body: ");
            string jsonBody = Console.ReadLine();

            if (string.IsNullOrWhiteSpace(jsonBody))
            {
                Console.WriteLine("");
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine(" The JSON body is empty. Please enter valid JSON.");
                Console.ResetColor();
                return;
            }

            try
            {
                using (HttpClient client = new HttpClient())
                {
                    StringContent content = new StringContent(jsonBody, Encoding.UTF8, "application/json");
                    HttpResponseMessage response = await client.PostAsync(url, content);
                    string responseData = await response.Content.ReadAsStringAsync();

                    Console.WriteLine("");
                    Console.WriteLine(" Response:");
                    Console.WriteLine("");
                    Console.WriteLine(responseData);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("");
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($" An error occurred: {ex.Message}");
                Console.ResetColor();
            }

            Console.WriteLine("");
            Console.WriteLine(" Press Enter to continue...");
            Console.ReadLine();
        }

        static async Task RunPutRequest() // Change data on server
        {
            Console.Clear();
            Console.WriteLine("");
            Console.WriteLine(" PUT Request Tool");
            Console.WriteLine("");
            Console.Write(" Enter a URL: ");
            Console.WriteLine("");
            string url = Console.ReadLine();

            if (string.IsNullOrWhiteSpace(url))
            {
                Console.WriteLine("");
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine(" The URL is empty. Please enter a valid URL.");
                Console.ResetColor();
                return;
            }

            Console.Write(" Enter the JSON body: ");
            string jsonBody = Console.ReadLine();

            if (string.IsNullOrWhiteSpace(jsonBody))
            {
                Console.WriteLine("");
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine(" The JSON body is empty. Please enter valid JSON.");
                Console.ResetColor();
                return;
            }

            try
            {
                using (HttpClient client = new HttpClient())
                {
                    StringContent content = new StringContent(jsonBody, Encoding.UTF8, "application/json");
                    HttpResponseMessage response = await client.PutAsync(url, content);
                    string responseData = await response.Content.ReadAsStringAsync();

                    Console.WriteLine(" Response:");
                    Console.WriteLine(responseData);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("");
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($" An error occurred: {ex.Message}");
                Console.ResetColor();
            }

            Console.WriteLine("");
            Console.WriteLine(" Press Enter to continue...");
            Console.ReadLine();
        }

        static async Task RunDeleteRequest() // Delete data on server
        {
            Console.Clear();
            Console.WriteLine("");
            Console.WriteLine(" DELETE Request Tool");
            Console.WriteLine("");
            Console.Write(" Enter the URL: ");
            string url = Console.ReadLine();
            Console.WriteLine("");

            if (string.IsNullOrWhiteSpace(url))
            {
                Console.WriteLine("");
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine(" The URL is empty. Please enter a valid URL.");
                Console.ResetColor();
                return;
            }

            try
            {
                using (HttpClient client = new HttpClient())
                {
                    HttpResponseMessage response = await client.DeleteAsync(url);
                    string responseData = await response.Content.ReadAsStringAsync();

                    Console.WriteLine(" Response:");
                    Console.WriteLine(responseData);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("");
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($" An error occurred: {ex.Message}");
                Console.ResetColor();
            }
        }

        static bool IsJson(string input)
        {
            input = input.Trim();
            return (input.StartsWith("{") && input.EndsWith("}")) || // Object
                   (input.StartsWith("[") && input.EndsWith("]"));   // Array
        }

        static async Task RunPortScanner()
        {
            Console.SetWindowSize(120, 60);

            var commonPorts = new Dictionary<int, string> // Common services
            {
                { 80, " HTTP" },
                { 8080, " HTTP-Alt" },
                { 443, " HTTPS" },
                { 21, " FTP" },
                { 22, " SSH" },
                { 25, " SMTP" },
                { 110, " POP3" },
                { 143, " IMAP" },
                { 53, " DNS" },
                { 123, " NTP" },
                { 3306, " MySQL" },
                { 1433, " MSSQL" },
                { 1521, " Oracle SQL" },
                { 5900, " VNC" },
                { 3389, " RDP" },
                { 445, " SMB" },
            };

            Console.Clear();
            Console.WriteLine("");
            Console.WriteLine(" Port Scanner");
            Console.WriteLine("");
            Console.WriteLine("");
            Console.WriteLine(" Type 'help' to get more information about this function and see usage examples.");
            Console.WriteLine("");
            Console.WriteLine(" Type 'exit' to return to the main menu.");
            Console.WriteLine("");
            Console.WriteLine("");
            Console.WriteLine(" Supports IPv4 172.29.64.123 and IPv6 [2001:db8::1428:57]");
            Console.WriteLine("");
            Console.Write(" Enter the target IP address: ");
            string targetIP = Console.ReadLine();

            if (string.IsNullOrWhiteSpace(targetIP))
            {
                Console.WriteLine("");
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine(" The input is empty. Please enter a valid IP or hostname.");
                Console.ResetColor();
                return;
            }

            if (targetIP == "exit")
            {
                return;
            }

            if (targetIP == "help")
            {
                DisplayHelp("PortScanner");
                return;
            }

            Console.WriteLine("");
            Console.WriteLine(" Checking common ports (Critical Services)...");
            Console.WriteLine("");

            var tasks = commonPorts.Select(async kvp => // Port selection and formatting
            {
                var port = kvp.Key;
                var service = kvp.Value;
                string status = await CheckPortStatus(targetIP, port);
                return new { Port = port, Service = service, Status = status };
            });

            var results = await Task.WhenAll(tasks);

            int openPortsCount = 0;
            int closedPortsCount = 0;

            foreach (var result in results) // Feedback and color appliance
            {
                Console.ForegroundColor = result.Status == "open" ? ConsoleColor.Green : ConsoleColor.Red;
                Console.WriteLine($"{result.Service} (PORT {result.Port}) {result.Status}");
                Console.ResetColor();

                if (result.Status == "open")
                {
                    openPortsCount++;
                }
                else
                {
                    closedPortsCount++;
                }
            }

            Console.WriteLine("");
            Console.Write(" Do you want to scan a custom range of ports? (y/n): ");

            if (Console.ReadLine().ToLower() == "y")
            {
                Console.WriteLine("");
                Console.Write(" Enter the starting port (1-65535): ");
                if (!int.TryParse(Console.ReadLine(), out int startPort) || startPort < 1 || startPort > 65535) // Check for valid input
                {
                    Console.WriteLine("");
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("Invalid start port.");
                    Console.ResetColor();
                    return;
                }

                Console.WriteLine("");
                Console.Write(" Enter the ending port (1-65535): ");
                if (!int.TryParse(Console.ReadLine(), out int endPort) || endPort < startPort || endPort > 65535) // Check for valid input
                {
                    Console.WriteLine("");
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine(" Invalid end port.");
                    Console.ResetColor();
                    return;
                }

                Console.WriteLine("");
                Console.WriteLine($" Scanning ports from {startPort} to {endPort} on {targetIP}...");
                Console.WriteLine("");

                int totalPorts = endPort - startPort + 1;
                int processedPorts = 0;
                int customOpenPortsCount = 0;
                int customClosedPortsCount = 0;

                // Initialize progress bar
                Console.Write("[");
                ShowProgressBar(processedPorts, totalPorts);
                Console.Write("]");

                // Move cursor to the progress bar line
                Console.SetCursorPosition(0, Console.CursorTop - 1);

                var scanTasks = Enumerable.Range(startPort, endPort - startPort + 1).Select(async port =>
                {
                    string status = await CheckPortStatus(targetIP, port);
                    return new { Port = port, Status = status };
                });

                foreach (var task in scanTasks) // Count port status
                {
                    var result = await task;

                    if (result.Status == "open")
                    {
                        customOpenPortsCount++;
                    }
                    else
                    {
                        customClosedPortsCount++;
                    }

                    processedPorts++;
                    ShowProgressBar(processedPorts, totalPorts); // Update the progress bar

                    // Set color based on status
                    Console.ForegroundColor = result.Status == "open" ? ConsoleColor.Green : ConsoleColor.Red;
                    Console.WriteLine($" Port {result.Port} is {result.Status.ToLower()}."); // Print output
                    Console.ResetColor();
                }

                Console.WriteLine("");
                Console.WriteLine("");
                Console.WriteLine(" Port scanning completed.");
                Console.WriteLine("");
                Console.WriteLine($" Open Ports in custom range: {customOpenPortsCount}");
                Console.WriteLine($" Closed Ports in custom range: {customClosedPortsCount}");
                Console.WriteLine("");

                Console.ReadLine();
            }
        }

        static void ShowProgressBar(int current, int total, int width = 50) // Formatting for progress bar
        {
            Console.CursorLeft = 0; // Move cursor to the start of the line
            double progress = (double)current / total;
            int progressWidth = (int)(progress * width);
            string progressBar = new string('#', progressWidth) + new string('-', width - progressWidth);
            Console.Write($"[{progressBar}] {current}/{total} ({progress * 100:F2}%)");
        }

        static async Task<string> CheckPortStatus(string ipAddress, int port)
        {
            try
            {
                using (var client = new TcpClient())
                {
                    var task = client.ConnectAsync(ipAddress, port); // Connecting to target to get information back
                    var completedTask = await Task.WhenAny(task, Task.Delay(500)); // 1/2-second timeout

                    if (completedTask == task)
                    {
                        return "open";
                    }
                    else
                    {
                        return "closed";
                    }
                }
            }
            catch (Exception)
            {
                return "closed";
            }
        }

        static void RunNII()
        {
            Console.Clear();
            Console.WriteLine("");
            Console.WriteLine(" Network Interface Information");
            Console.WriteLine("");

            foreach (NetworkInterface networkInterface in NetworkInterface.GetAllNetworkInterfaces()) // Retrieve all network interfaces
            {
                // Filter out virtual adapters, loopbacks, and other unwanted interfaces
                if (networkInterface.NetworkInterfaceType == NetworkInterfaceType.Loopback ||
                    networkInterface.Description.Contains("Virtual") ||
                    networkInterface.Description.Contains("Hyper-V"))
                {
                    continue;
                }

                Console.WriteLine("");
                Console.Write(" Type 'help' if you want to get an explaination for this function: ");
                string input = Console.ReadLine().ToLower();

                if (input == "help")
                {
                    DisplayHelp("NII");
                    break;
                }

                else
                {
                    Console.Clear();
                    Console.WriteLine("");
                    Console.WriteLine(" Network Interface Information");
                    Console.WriteLine("");
                }

                // Check if the network interface is up and running
                if (networkInterface.OperationalStatus == OperationalStatus.Up)
                {
                    // Determine the type of network interface
                    switch (networkInterface.NetworkInterfaceType)
                    {
                        case NetworkInterfaceType.Ethernet:
                            Console.WriteLine("");
                            Console.WriteLine(" Connected to a wired network (Ethernet).");
                            Console.WriteLine("");
                            ShowEthernetDetails(networkInterface);
                            break;

                        case NetworkInterfaceType.Wireless80211:
                            Console.WriteLine("");
                            Console.WriteLine(" Connected to a wireless network (Wi-Fi).");
                            Console.WriteLine("");
                            ShowWifiDetails(networkInterface);
                            break;

                        default:
                            Console.WriteLine("");
                            Console.WriteLine($" Connected via {networkInterface.NetworkInterfaceType}.");
                            ShowGenericNetworkDetails(networkInterface);
                            break;
                    }
                }
            }

            Console.WriteLine("");
            Console.WriteLine(" Press Enter to continue...");
            Console.ReadLine();
        }

        private static void ShowEthernetDetails(NetworkInterface ethernetInterface) // If Cable (Ethernet)
        {
            Console.WriteLine("");
            Console.WriteLine(" Ethernet Interface Details:");
            ShowCommonDetails(ethernetInterface);
        }

        private static void ShowWifiDetails(NetworkInterface wifiInterface) // If Wireless (WIFI)
        {
            Console.WriteLine("");
            Console.WriteLine(" Wi-Fi Interface Details:");
            ShowCommonDetails(wifiInterface);
        }

        private static void ShowGenericNetworkDetails(NetworkInterface networkInterface) // Default
        {
            Console.WriteLine("");
            Console.WriteLine($"{networkInterface.NetworkInterfaceType} Interface Details:");
            ShowCommonDetails(networkInterface);
        }

        private static void ShowCommonDetails(NetworkInterface networkInterface) // print and format information
        {
            Console.WriteLine();
            SetConsoleColor(ConsoleColor.Cyan);
            Console.WriteLine($"  Name: {networkInterface.Name}");
            SetConsoleColor(ConsoleColor.Green);
            Console.WriteLine($"  Description: {networkInterface.Description}");
            SetConsoleColor(ConsoleColor.Yellow);
            Console.WriteLine($"  Status: {networkInterface.OperationalStatus}");
            SetConsoleColor(ConsoleColor.Magenta);
            Console.WriteLine($"  Speed: {networkInterface.Speed / 1_000_000} Mbps");
            SetConsoleColor(ConsoleColor.DarkYellow);
            Console.WriteLine($"  MAC Address: {networkInterface.GetPhysicalAddress()}");
            SetConsoleColor(ConsoleColor.Cyan);
            Console.WriteLine($"  IPv4 Address: {GetIPv4Address(networkInterface)}");
            SetConsoleColor(ConsoleColor.Gray);
            Console.WriteLine($"  Subnet Mask: {GetSubnetMask(networkInterface)}");
            SetConsoleColor(ConsoleColor.Cyan);
            Console.WriteLine($"  IPv6 Address: {GetIPv6Address(networkInterface)}");
            SetConsoleColor(ConsoleColor.DarkCyan);
            Console.WriteLine($"  DNS Suffix: {networkInterface.GetIPProperties().DnsSuffix}");
            SetConsoleColor(ConsoleColor.Blue);
            Console.WriteLine($"  DHCP Enabled: {networkInterface.GetIPProperties().GetIPv4Properties()?.IsDhcpEnabled}");
            SetConsoleColor(ConsoleColor.DarkGreen);
            Console.WriteLine($"  Gateway Address(es): {string.Join(", ", GetGatewayAddresses(networkInterface))}");
            SetConsoleColor(ConsoleColor.DarkMagenta);
            Console.WriteLine($"  DNS Servers: {string.Join(", ", GetDnsAddresses(networkInterface))}");

            Console.ResetColor();
            Console.WriteLine();
        }

        private static string GetIPv4Address(NetworkInterface networkInterface)
        {
            foreach (UnicastIPAddressInformation ip in networkInterface.GetIPProperties().UnicastAddresses)
            {
                if (ip.Address.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
                {
                    return ip.Address.ToString();
                }
            }
            return " No IPv4 address assigned.";
        }

        private static string GetIPv6Address(NetworkInterface networkInterface)
        {
            foreach (UnicastIPAddressInformation ip in networkInterface.GetIPProperties().UnicastAddresses)
            {
                if (ip.Address.AddressFamily == System.Net.Sockets.AddressFamily.InterNetworkV6)
                {
                    return ip.Address.ToString();
                }
            }
            return " No IPv6 address assigned.";
        }

        private static string[] GetGatewayAddresses(NetworkInterface networkInterface)
        {
            return networkInterface.GetIPProperties().GatewayAddresses
                .Select(gateway => gateway.Address.ToString())
                .ToArray();
        }

        private static string[] GetDnsAddresses(NetworkInterface networkInterface)
        {
            return networkInterface.GetIPProperties().DnsAddresses
                .Where(addr => addr.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
                .Select(addr => addr.ToString())
                .ToArray();
        }

        private static string GetSubnetMask(NetworkInterface networkInterface)
        {
            foreach (UnicastIPAddressInformation ip in networkInterface.GetIPProperties().UnicastAddresses)
            {
                if (ip.Address.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
                {
                    return ip.IPv4Mask?.ToString() ?? " No subnet mask assigned.";
                }
            }
            return " No subnet mask assigned.";
        }

        static void RunExternalPrograms() // External programs submenu (Main menu -> External programs)
        {
            while (true)
            {
                Console.Clear();
                Console.WriteLine("");
                Console.ForegroundColor = ConsoleColor.DarkGray;
                Console.WriteLine("=============================");
                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.WriteLine("         Multitool");
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("     External Programs");
                Console.ForegroundColor = ConsoleColor.DarkGray;
                Console.WriteLine("=============================");
                Console.ResetColor();
                Console.WriteLine("");
                Console.WriteLine("");
                Console.WriteLine(" A collection of external programs integrated into this program");
                Console.WriteLine("");
                Console.WriteLine(" Possible arguments: Bit/Byte Calculator, Password Generator, Unit Converter");
                Console.WriteLine("");
                Console.WriteLine("");
                Console.WriteLine(" Type 'exit' to return to the main menu.");
                Console.WriteLine("");
                Console.WriteLine("");

                Console.Write(" Option: ");

                string Option = Console.ReadLine().ToLower();

                if (Option == "exit")
                {
                    return;
                }

                switch (Option)
                {
                    case Digital_Storage_Measurement_System:
                        {
                            RunDigitalStorageMeasurementSystem();
                            break;
                        }
                    case Dynamic_Password_Generator:
                        {
                            RunDynamicPasswordGenerator();
                            break;
                        }
                    case Unit_Converter:
                        {
                            RunUnitConverter();
                            break;
                        }
                    default:
                        Console.WriteLine("");
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine(" Invalid or empty argument!");
                        Task.Delay(2000).Wait();
                        Console.ResetColor();
                        break;
                }
            }
        }

        static void RunAnimation() // Animation triggered by typing info in the main menu
        {
            Console.Clear();

            // Text
            Console.WriteLine("");
            AnimateText(" Author:", ConsoleColor.Green);
            AnimateText(" Dennis Plischke", ConsoleColor.Cyan);

            Console.WriteLine("");
            Console.WriteLine("");

            AnimateText(" Project name:", ConsoleColor.Green);
            AnimateText(" Multitool", ConsoleColor.Cyan);

            Console.WriteLine("");
            Console.WriteLine("");

            AnimateText(" Date of creation:", ConsoleColor.Green);
            AnimateText(" Sunday, 12th March 2024, 11:09:37", ConsoleColor.Cyan);

            Console.WriteLine("");
            Console.WriteLine("");

            AnimateText(" Last update:", ConsoleColor.Green);
            AnimateText(" Friday, 22th Novemberber 2024, 21:20:44", ConsoleColor.Cyan);

            Console.WriteLine("");
            Console.WriteLine("");

            AnimateText(" Total development time:", ConsoleColor.Green);
            AnimateText(" (Current, Approx.)", ConsoleColor.Cyan);
            AnimateText(" 152", ConsoleColor.Blue);
            AnimateText(" Days (", ConsoleColor.Cyan);
            AnimateText("178", ConsoleColor.Blue);
            AnimateText(" Days including four other related side projects)", ConsoleColor.Cyan);

            Console.WriteLine("");
            Console.WriteLine("");

            AnimateText(" Total lines of code", ConsoleColor.Green);
            AnimateText(" (Current): 4677", ConsoleColor.Cyan);

            Console.WriteLine("");
            Console.WriteLine("");

            AnimateText(" Resources used:", ConsoleColor.Green);
            AnimateText(" GitHub, Microsoft Learn, ChatGPT, Stack Overflow, W3Schools (...)", ConsoleColor.Cyan);

            Console.WriteLine("");
            Console.WriteLine("");
            Console.WriteLine("");
            Console.WriteLine("");

            AnimateText(" The resources listed above were used for researching functionality, use cases, and implementation examples.", ConsoleColor.White);

            Console.WriteLine("");
            Console.WriteLine("");

            AnimateText(" For challenging parts like the calculator logic and specific problems, ChatGPT was used.", ConsoleColor.White);

            Console.WriteLine("");
            Console.WriteLine("");
            Console.WriteLine("");
            Console.WriteLine("");

            AnimateText(" UI design, code structure, comments, error messages (if custom), help descriptions and information", ConsoleColor.Green);

            Console.WriteLine("");
            Console.WriteLine("");

            AnimateText(" as well as ideas for functionalities and overall style", ConsoleColor.Green);

            Console.WriteLine("");
            Console.WriteLine("");

            AnimateText(" Were self-made and written by Dennis Plischke", ConsoleColor.Cyan);

            Console.WriteLine("");
            Console.WriteLine("");

            Console.ReadLine();
        }

        static void DisplayHelp(string functionName) // Called from each Method by typing 'help'
        {
            switch (functionName.ToLower())
            {
                case "calculator":
                    {
                        Console.Clear();
                        Console.WriteLine("");
                        Console.WriteLine(" This is an advanced string calculator that supports a variety of mathematical operations.");
                        Console.WriteLine("");
                        Console.WriteLine(" Features include:");
                        Console.WriteLine("");
                        Console.WriteLine(" - Basic arithmetic operations: addition (+), subtraction (-), multiplication (*), division (/).");
                        Console.WriteLine("");
                        Console.WriteLine(" - Exponentiation using the caret symbol (^), e.g., '2^3'.");
                        Console.WriteLine("");
                        Console.WriteLine(" - Square roots with the 'sqrt()' function.");
                        Console.WriteLine("");
                        Console.WriteLine(" - Trigonometric functions: sine 'sin()', cosine 'cos()', and tangent 'tan()' (in degrees).");
                        Console.WriteLine("");
                        Console.WriteLine(" - Natural logarithm using 'ln()' and base-10 logarithm with 'log()'.");
                        Console.WriteLine("");
                        Console.WriteLine(" - The exponential function 'exp()' to calculate e raised to a given power.");
                        Console.WriteLine("");
                        Console.WriteLine(" - Constants: Use 'pi' for (3.1415...) and 'e' for Euler's number.");
                        Console.WriteLine("");
                        Console.WriteLine(" - Support for parentheses to control operator precedence.");
                        Console.WriteLine("");
                        Console.WriteLine(" - Percentage calculations by appending % to numbers.");
                        Console.WriteLine("");
                        Console.WriteLine(" You can enter expressions like 'sqrt(16)', 'log(100)', 200 * 50% or '2^3 + (5 * 2) * (12 - 4)'.");
                        Console.WriteLine("");

                        break;
                    }

                case "dsms":
                    {
                        Console.Clear();
                        Console.WriteLine("");
                        Console.WriteLine(" Welcome to the Digital Storage Units Calculator.");
                        Console.WriteLine("");
                        Console.WriteLine(" This tool provides an interface with multiple textboxes, each corresponding to a specific data size unit.");
                        Console.WriteLine("");
                        Console.WriteLine(" You can enter a value into any textbox, and it will calculate the equivalent values in the other units.");
                        Console.WriteLine("");
                        Console.WriteLine(" Simply choose a textbox for the data size you want to convert, enter a number, and click 'Calculate' to see the conversions.");
                        Console.WriteLine("");
                        Console.WriteLine(" Note: The percentage difference feature is still under development.");
                        Console.WriteLine("");

                        Console.ReadLine();
                        break;
                    }

                case "fileextensionsarchive":
                    {
                        Console.Clear();
                        Console.WriteLine("");
                        Console.WriteLine(" Welcome to the File Extensions Archive.");
                        Console.WriteLine("");
                        Console.WriteLine(" Here, you can get a brief explanation of what a specific file extension represents.");
                        Console.WriteLine("");
                        Console.WriteLine(" Simply type an extension, such as .exe, and press Enter.");
                        Console.WriteLine("");
                        Console.WriteLine(" The tool will display the following information:");
                        Console.WriteLine("");
                        Console.WriteLine(" - Full name of the file type.");
                        Console.WriteLine("");
                        Console.WriteLine(" - Common use cases and purposes.");
                        Console.WriteLine("");
                        Console.WriteLine(" - Associated file type categories.");
                        Console.WriteLine("");

                        Console.ReadLine();
                        break;
                    }

                case "passwordgenerator":
                    {
                        Console.Clear();
                        Console.WriteLine("");
                        Console.WriteLine(" This is the Dynamic password generator.");
                        Console.WriteLine("");
                        Console.WriteLine(" Its a UI where you can choose the password building pools using checkboxes.");
                        Console.WriteLine("");
                        Console.WriteLine(" Available pools: Letters (Lowercase), Letters (Capitalized), Numbers and Special characters.");
                        Console.WriteLine("");
                        Console.WriteLine(" You can choose the password length by click the up / down buttons or type in a number.");
                        Console.WriteLine("");
                        Console.WriteLine(" You can also use a custom pool when unchecking all checkboxes and clicking on 'Use Custom Pool'.");
                        Console.WriteLine("");
                        Console.WriteLine(" In the textbox which appeared, you can type in the characters you want to use as you password building pool.");
                        Console.WriteLine("");
                        Console.WriteLine(" After you specified your password pool, hit 'Use Custom Pool' again to confirm.");
                        Console.WriteLine("");
                        Console.WriteLine(" If you have checked any Pool or if you are using your custom pool and you specified the password length, hit generate.");
                        Console.WriteLine("");
                        Console.WriteLine(" Hit generate as many times as you want. It will just regenerate antoher password.");
                        Console.WriteLine("");

                        Console.ReadLine();
                        break;
                    }

                case "aes":
                    {
                        Console.Clear();
                        Console.WriteLine("");
                        Console.WriteLine(" This is the AES en / decryption tool");
                        Console.WriteLine("");
                        Console.WriteLine(" You can choose to 1: Encrypt the entered text with a key or 2: Decrypt the encrypted text with the key.");
                        Console.WriteLine("");
                        Console.WriteLine(" To encrypt text, just type the text you want to enrypt and hit enter.");
                        Console.WriteLine("");
                        Console.WriteLine(" Then, type down a key or let the program generate one by pressing the 'Insert' button on your keyboard.");
                        Console.WriteLine("");
                        Console.WriteLine(" The key or the generated key will be presented along with the encrypted text.");
                        Console.WriteLine("");
                        Console.WriteLine(" Note that in order to decrypt the encrypted text, you will need that key you entered or the generated one.");
                        Console.WriteLine("");
                        Console.WriteLine(" To decrypt the text just paste the text in, hit enter and then paste the key and hit enter again.");
                        Console.WriteLine("");
                        Console.WriteLine(" The decrypted text will be shown down below.");
                        Console.WriteLine("");

                        Console.ReadLine();
                        break;
                    }

                case "base64":
                    {
                        Console.Clear();
                        Console.WriteLine("");
                        Console.WriteLine(" Similar to 'AES', you can choose between two options: 1) Encode text or 2) Decode text.");
                        Console.WriteLine("");
                        Console.WriteLine(" To encode text, simply type or paste the text you want to encode and press Enter.");
                        Console.WriteLine("");
                        Console.WriteLine(" To decode encoded text, paste the Base64 encoded string and press Enter.");
                        Console.WriteLine("");
                        Console.WriteLine(" Note: This is ENCODING, not ENCRYPTING. The difference is that encoding changes the format of the text, ");
                        Console.WriteLine("");
                        Console.WriteLine(" making it readable in different systems. Encoding can be easily reversed.");
                        Console.WriteLine("");
                        Console.WriteLine(" On the other hand, encryption secures data, making it unreadable without the proper decryption key.");
                        Console.WriteLine("");

                        Console.ReadLine();
                        break;
                    }

                case "caesarcipher":
                    {
                        Console.Clear();
                        Console.WriteLine("");
                        Console.WriteLine(" The Caesar Cipher is an ancient encryption method used around 100 BC.");
                        Console.WriteLine("");
                        Console.WriteLine(" It’s a very simple form of encryption, where the letters in the alphabet are shifted by a specific number.");
                        Console.WriteLine("");
                        Console.WriteLine(" To encrypt text, choose option 1, enter your text, then specify the shift amount (between -25 and 25) and press Enter.");
                        Console.WriteLine("");
                        Console.WriteLine(" To decrypt, paste the encrypted text, input the shift number used for encryption, and press Enter.");
                        Console.WriteLine("");

                        Console.ReadLine();
                        break;
                    }

                case "ping":
                    {
                        Console.Clear();
                        Console.WriteLine("");
                        Console.WriteLine(" The Ping tool is used to measure the latency of an internet connection by sending data packets to a specified server.");
                        Console.WriteLine("");
                        Console.WriteLine(" The time it takes for packets to travel from your computer to the server and back is recorded for each attempt.");
                        Console.WriteLine("");
                        Console.WriteLine(" You must first specify the server or service by entering its IP address or website name (Hostname). (Eg. google.com)");
                        Console.WriteLine("");
                        Console.WriteLine(" Then, enter the number of pings to execute. After providing all the necessary information, press Enter to begin.");
                        Console.WriteLine("");
                        Console.WriteLine(" You can also choose to run a 'reverse DNS lookup,' which converts the IP address into the hostname.");
                        Console.WriteLine("");
                        Console.WriteLine(" The program will display each ping (ping number) and its latency in milliseconds.");
                        Console.WriteLine("");
                        Console.WriteLine(" Successful pings will be highlighted in green, while unsuccessful ones will appear in red.");
                        Console.WriteLine("");
                        Console.WriteLine(" Once all pings are completed, a summary will be displayed, showing the average, minimum, and maximum latency.");
                        Console.WriteLine("");

                        Console.ReadLine();
                        break;
                    }

                case "traceroute":
                    {
                        Console.Clear();
                        Console.WriteLine("");
                        Console.WriteLine(" The Traceroute tool is used to trace the path your internet traffic takes to reach a specific server or website.");
                        Console.WriteLine("");
                        Console.WriteLine(" It sends data to the specified target and records the servers that your connection passes through.");
                        Console.WriteLine("");
                        Console.WriteLine(" You must first specify the server or service by entering its IP address or website name (Hostname). (Eg. google.com)");
                        Console.WriteLine("");
                        Console.WriteLine(" You can also choose to run a 'reverse DNS lookup' to display the hostnames of the servers instead of IP addresses.");
                        Console.WriteLine("");
                        Console.WriteLine(" The program shows the latency at each server your connection passes through.");
                        Console.WriteLine("");
                        Console.WriteLine(" At the end, it will display the total number of hops your connection took from your PC to the target.");
                        Console.WriteLine("");

                        Console.ReadLine();
                        break;
                    }

                case "dnslookup":
                    {
                        Console.Clear();
                        Console.WriteLine("");
                        Console.WriteLine(" The DNS lookup tool is used to retrieve the IP address(es) of websites.");
                        Console.WriteLine("");
                        Console.WriteLine(" Normally, when you type a URL like www.google.com, your PC queries a DNS server to provide the IP address of one of Google's servers.");
                        Console.WriteLine("");
                        Console.WriteLine(" This allows your browser to connect to the server. With this tool, you can manually request the IP address(es) for various websites.");
                        Console.WriteLine("");
                        Console.WriteLine(" Simply enter the website you want to get the IP address for and press Enter.");
                        Console.WriteLine("");
                        Console.WriteLine(" The tool will then display a list of IP addresses (if more than one exists) along with any aliases (if applicable).");
                        Console.WriteLine("");

                        Console.ReadLine();
                        break;
                    }

                case "httprequests":
                    {
                        Console.Clear();
                        Console.WriteLine("");
                        Console.WriteLine(" The HTTP Requests tool allows you to interact with web APIs using different HTTP methods.");
                        Console.WriteLine("");
                        Console.WriteLine(" These methods represent the most common types of HTTP operations:");
                        Console.WriteLine("");
                        Console.WriteLine(" - GET: Retrieves data from a specified URL. This is often used to fetch resources or data from a server or API.");
                        Console.WriteLine("");
                        Console.WriteLine(" (Example: https://catfact.ninja/fact)");
                        Console.WriteLine("");
                        Console.WriteLine(" - POST: Sends data to a specified URL to create or update a resource. Commonly used to submit forms or upload data.");
                        Console.WriteLine("");
                        Console.WriteLine(" - PUT: Updates an existing resource at a specified URL. You provide the updated data in the request.");
                        Console.WriteLine("");
                        Console.WriteLine(" - DELETE: Removes a specified resource from the server.");
                        Console.WriteLine("");
                        Console.WriteLine(" NOTE: For POST requests, ensure that the data is sent in proper JSON format.");
                        Console.WriteLine("");

                        Console.ReadLine();
                        break;
                    }

                case "portscanner":
                    {
                        Console.Clear();
                        Console.WriteLine("");
                        Console.WriteLine(" The port scanner is a tool used to test the connectivity and configuration of servers by checking their ports.");
                        Console.WriteLine("");
                        Console.WriteLine(" It scans the ports of the target server and displays their status as either open (green) or closed (red).");
                        Console.WriteLine("");
                        Console.WriteLine(" You can enter an IP address as the target. For domain names like google.com, ");
                        Console.WriteLine("");
                        Console.WriteLine(" use the 'DNS Lookup' tool to get the IP address.");
                        Console.WriteLine("");
                        Console.WriteLine(" The tool checks common protocols such as: HTTP, HTTPS, FTP, SSH, SMTP, POP3, and IMAP.");
                        Console.WriteLine("");
                        Console.WriteLine(" Additionally, you can perform a custom scan with a range of ports from 1 to 56,000.");
                        Console.WriteLine("");
                        Console.WriteLine(" A progress bar will be displayed to show the scan progress, for example:");
                        Console.WriteLine("");
                        Console.WriteLine(" [--------------------------------------------------] 1/100 (1.00%) Port 1 is closed. (red)");
                        Console.WriteLine(" [#-------------------------------------------------] 10/100 (10.00%) Port 10 is open. (green)");
                        Console.WriteLine("");

                        Console.ReadLine();
                        break;
                    }

                case "nii":
                    {
                        Console.Clear();
                        Console.WriteLine("");
                        Console.WriteLine(" The Network Interface Information tool provides detailed information about the network adapter.");
                        Console.WriteLine("");
                        Console.WriteLine(" It includes the following details:");
                        Console.WriteLine("");
                        Console.WriteLine(" - Name: The name of the network interface.");
                        Console.WriteLine(" - Description: A brief description of the network interface.");
                        Console.WriteLine(" - Status: The operational status of the interface (e.g., up, down).");
                        Console.WriteLine(" - Speed: The speed of the network interface in Mbps.");
                        Console.WriteLine(" - MAC Address: The hardware address of the network interface.");
                        Console.WriteLine(" - IPv4 Address: The IPv4 address assigned to the interface.");
                        Console.WriteLine(" - Subnet Mask: The subnet mask for the IPv4 address.");
                        Console.WriteLine(" - IPv6 Address: The IPv6 address assigned to the interface.");
                        Console.WriteLine(" - DNS Suffix: The DNS suffix associated with the interface.");
                        Console.WriteLine(" - DHCP Enabled: Indicates whether DHCP is enabled for the interface.");
                        Console.WriteLine(" - Gateway Address(es): The gateway addresses associated with the interface.");
                        Console.WriteLine(" - DNS Servers: The DNS servers used by the interface.");
                        Console.WriteLine("");
                        Console.WriteLine(" Each detail is color-coded for easy identification.");
                        Console.WriteLine("");

                        break;
                    }

            }
        }

        private static void SetConsoleColor(ConsoleColor color)
        {
            Console.ForegroundColor = color;
        }
    }
}