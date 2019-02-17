using System;
using System.Diagnostics;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Driver;

namespace SpeedTest
{
    class Program
    {

        class NDocument{
            public string Name { get; set; }
        }

        static void Main(string[] args)
        {
            var stopwatch = new Stopwatch();

            var client = new MongoClient();
            var database = client.GetDatabase("foo");
            var collection = database.GetCollection<NDocument>("bar");
            var session = client.StartSession();
            stopwatch.Start();
            session.StartTransaction(new TransactionOptions(
                readConcern: ReadConcern.Snapshot,
                writeConcern: WriteConcern.WMajority));

            for (int i = 0; i < 10000; i++)
            {
                collection.InsertOne(session, new NDocument(){ Name = "index"+i });
            }
            session.CommitTransaction();
            stopwatch.Stop();
            Console.WriteLine($"Time taken:{stopwatch.ElapsedMilliseconds}");
        }
    }
}
