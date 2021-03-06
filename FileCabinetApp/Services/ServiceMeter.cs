using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Reflection;
using FileCabinetApp.CommandHandlers;

namespace FileCabinetApp.Services
{
    /// <summary>
    /// Service that displays methods execution time to the console.
    /// </summary>
    public class ServiceMeter : IFileCabinetService
    {
        private readonly IFileCabinetService service;

        /// <summary>
        /// Initializes a new instance of the <see cref="ServiceMeter"/> class.
        /// </summary>
        /// <param name="service">Service to be used.</param>
        public ServiceMeter(IFileCabinetService service)
        {
            this.service = service;
        }

        /// <inheritdoc/>
        public int CreateRecord(RecordParameters parameters)
        {
            Stopwatch creationTime = Stopwatch.StartNew();

            var result = this.service.CreateRecord(parameters);

            creationTime.Stop();
            DisplayDuration(nameof(this.service.CreateRecord), creationTime.ElapsedTicks);

            return result;
        }

        /// <inheritdoc/>
        public void EditRecord(int id, RecordParameters parameters)
        {
            Stopwatch editingTime = Stopwatch.StartNew();

            this.service.EditRecord(id, parameters);

            editingTime.Stop();
            DisplayDuration(nameof(this.service.EditRecord), editingTime.ElapsedTicks);
        }

        /// <inheritdoc/>
        public (int total, int deleted) GetStat()
        {
            Stopwatch statTime = Stopwatch.StartNew();

            var result = this.service.GetStat();

            statTime.Stop();
            DisplayDuration(nameof(this.service.GetStat), statTime.ElapsedTicks);

            return result;
        }

        /// <inheritdoc/>
        public FileCabinetServiceSnapshot MakeSnapShot()
        {
            Stopwatch makingSnapshotTime = Stopwatch.StartNew();

            var snapshot = this.service.MakeSnapShot();

            makingSnapshotTime.Stop();
            DisplayDuration(nameof(this.service.MakeSnapShot), makingSnapshotTime.ElapsedTicks);

            return snapshot;
        }

        /// <inheritdoc/>
        public int Purge()
        {
            Stopwatch purgeTime = Stopwatch.StartNew();

            var result = this.service.Purge();

            purgeTime.Stop();
            DisplayDuration(nameof(this.service.Purge), purgeTime.ElapsedTicks);

            return result;
        }

        /// <inheritdoc/>
        public List<int> Delete(PropertyInfo recordProperty, object propertyValue)
        {
            Stopwatch deletingTime = Stopwatch.StartNew();

            var result = this.service.Delete(recordProperty, propertyValue);

            deletingTime.Stop();
            DisplayDuration(nameof(this.Delete), deletingTime.ElapsedTicks);

            return result;
        }

        /// <inheritdoc/>
        public string Restore(FileCabinetServiceSnapshot snapshot)
        {
            Stopwatch restoringTime = Stopwatch.StartNew();

            var result = this.service.Restore(snapshot);

            restoringTime.Stop();
            DisplayDuration(nameof(this.service.Restore), restoringTime.ElapsedTicks);

            return result;
        }

        /// <inheritdoc/>
        public void Insert(FileCabinetRecord recordToInsert)
        {
            Stopwatch insertingTime = Stopwatch.StartNew();

            this.service.Insert(recordToInsert);

            insertingTime.Stop();
            DisplayDuration(nameof(this.service.Insert), insertingTime.ElapsedTicks);
        }

        /// <inheritdoc/>
        public ReadOnlyCollection<FileCabinetRecord> FindRecords(Dictionary<PropertyInfo, object> propertiesWithValues, OperationType operation)
        {
            Stopwatch findingTime = Stopwatch.StartNew();

            var result = this.service.FindRecords(propertiesWithValues, operation);

            findingTime.Stop();
            DisplayDuration(nameof(this.FindRecords), findingTime.ElapsedTicks);

            return result;
        }

        private static void DisplayDuration(string methodName, long ticks)
        {
            Console.WriteLine($"{methodName} method execution duration is {ticks} ticks.");
        }
    }
}
