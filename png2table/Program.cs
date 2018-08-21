using System;
using System.Drawing;
using System.IO;

namespace png2table {

    internal class Program {

        private static void Main(string[] args) {
            if (args.Length == 0) {
                PrintHelp();
                return;
            }

            var inputFile = string.Empty;
            var outputFile = string.Empty;

            var noColSpan = false;
            var noAa = false;
            var st = false;
            var bgColor = Color.White.Name;

            for (int i = 0; i < args.Length; i++) {
                switch (args[i].Trim().ToLower()) {
                    case "-nocolspan":
                        noColSpan = true;
                        break;

                    case "-noaa":
                        noAa = true;
                        break;

                    case "-st":
                        st = true;
                        break;

                    case "-bg":
                        if (args.Length >= i + 1) {
                            bgColor = args[i + 1];
                            i++;
                        }
                        break;

                    default:
                        if (inputFile == string.Empty) {
                            inputFile = args[i];
                        } else {
                            outputFile = args[i];
                        }
                        break;
                }
            }

            Console.WriteLine($"Converting {Path.GetFileName(inputFile)} to {Path.GetFileName(outputFile)}...");
            Console.WriteLine("");
            Console.WriteLine("Options:");
            Console.WriteLine($"Use colspan attribue: {(noColSpan ? "no" : "yes")}");
            Console.WriteLine($"Use antialiasing: {(noAa ? "no" : "yes")}");
            if (noAa) Console.WriteLine($"Background color: {bgColor}");
            Console.WriteLine($"Use style tag: {(st ? "yes" : "no")}");
            Console.WriteLine("");

            try {
                var p2t = new Png2Table();
                var html = p2t.Convert(inputFile, !noColSpan, st, !noAa, bgColor);
                File.WriteAllText(outputFile, html);

                Console.WriteLine($"Input file size: {(new FileInfo(inputFile).Length / 1000)} kb");
                Console.WriteLine($"Output file size: {(new FileInfo(outputFile).Length / 1000)} kb");
                Console.WriteLine("");

                Console.WriteLine("Conversion completed!");
            } catch (Exception ex) {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"Exception:\n{ex}\n");
                Console.WriteLine("Conversion failed!");
            } finally {
                Console.ResetColor();
            }
        }

        public static void PrintHelp() {
            Console.WriteLine("Usage: png2table [-nocolspan] [-st] [-noaa] [-bg html_color] image_file html_output_file");
            Console.WriteLine("");
            Console.WriteLine("Options:");
            Console.WriteLine("\t -nocolspan \t\t Disables use of the colspan attribute. May increase file size substantially depending on input image.");
            Console.WriteLine("\t -noaa \t\t\t Disables antialiasing, allows for transparent pixels.");
            Console.WriteLine("\t -bg html_color \t Background color to use when using antialiasing. Default: white.");
            Console.WriteLine("\t -st \t\t\t Outputs a style tag for additional styling. May improve compatibility in some browsers.");
        }
    }
}