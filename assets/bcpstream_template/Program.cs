using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;
using System.Reflection;
using System.ComponentModel.DataAnnotations.Schema;

using BulkWriter;

// https://stackoverflow.com/questions/64133771/net-5-not-compiling-to-single-file-executables

namespace bcpstream
{
    class Program
    {
        static void Main(string[] args)
        {
            string connStr = "{{connection_string}}";

            if (args.Length == 1 && args[0] == "selftest")
            {
                Helpers.SelfTest();
                return;
            }

            string prefix = args[0];
            long totalRows = long.Parse(args[1]);

            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
            Console.InputEncoding = Encoding.GetEncoding("Unicode");

            using var bulkWriter = new BulkWriter<DataRow>(connStr)
            {
                BulkCopyTimeout = 0,
                BatchSize = 1000
            };
            bulkWriter.WriteToDatabase(ReadStdin(prefix, totalRows));
        }

        public static IEnumerable<DataRow> ReadStdin(string prefix, long totalRows = -1)
        {
            string line;

            Stopwatch stopwatch = new();
            long count = 0;
            long prevCount = 0;
            TimeSpan prevTimestamp = new();

            stopwatch.Start();

            while ((line = Console.ReadLine()) != null)
            {
                DataRow row;

                try
                {
                    row = new DataRow(line);
                }
                catch (FormatException)
                {
                    PrintError(line);
                    throw;
                }

                yield return row;
                count++;

                if (count % 1000 == 0)
                {
                    TimeSpan timestamp = stopwatch.Elapsed;
                    double timeDelta = (timestamp - prevTimestamp).TotalSeconds;
                    double curSpeed = (count - prevCount) / timeDelta;

                    if (timeDelta >= 1.0)
                    {
                        Console.Write("[{0}] [{1}] ", GetFormatTableName(), prefix);
                        Console.Write("sent: {0:N0} rows; speed: {1:N0} rps", count, (long)curSpeed);

                        if (totalRows > 0)
                        {
                            double avgSpeed = count / timestamp.TotalSeconds;
                            double remain = (totalRows - count) / avgSpeed;
                            double remainForPrint;
                            string remainUnit;

                            if (remain <= 0)
                            {
                                remainForPrint = -1;
                                remainUnit = "?";
                            }
                            else if (remain <= 60)
                            {
                                remainForPrint = remain;
                                remainUnit = "sec";
                            }
                            else if (remain <= 60 * 60)
                            {
                                remainForPrint = remain / 60;
                                remainUnit = "min";
                            }
                            else
                            {
                                remainForPrint = remain / 60 / 60;
                                remainUnit = "hrs";
                            }

                            Console.Write("; progress: {0:N0}%; remain: {1:N1} {2}",
                                          100 * count / totalRows, remainForPrint, remainUnit);
                        }

                        Console.WriteLine();
                        Console.Out.Flush();

                        prevCount = count;
                        prevTimestamp = timestamp;
                    }
                }
            }

            stopwatch.Stop();
            PrintStats(prefix, count, stopwatch.Elapsed);
        }

        public static void PrintError(string line)
        {
            Console.Error.WriteLine("bcpstream format error");
            Console.Error.WriteLine("table: {0}", GetFormatTableName());
            Console.Error.WriteLine("error on line:");
            Console.Error.WriteLine(line.Replace("\t", "\\t"));
            Console.Error.Flush();
        }

        public static void PrintStats(string prefix, long count, TimeSpan ts)
        {
            double avgSpeed = count / ts.TotalSeconds;

            Console.WriteLine("[{0}] [{1}] sent: {2:N0} rows; time: {3:c}; speed: {4:N0} rps",
                              GetFormatTableName(), prefix, count, ts, (long)avgSpeed);
            Console.Out.Flush();
        }

        public static string GetFormatTableName()
        {
            return typeof(DataRow).GetTypeInfo().GetCustomAttribute<TableAttribute>()?.Name
                    ?? typeof(DataRow).Name;
        }
    }
}
