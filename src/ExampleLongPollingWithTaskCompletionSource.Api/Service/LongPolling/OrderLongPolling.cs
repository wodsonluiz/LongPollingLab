﻿using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace ExampleLongPollingWithTaskCompletionSource.Api.Service.LongPolling
{
    public class OrderLongPolling
    {
        public string? Id { get; set; }
        public string? Description { get; set; }
        public string? SerialNumber { get; set; }
        public string? Status { get; set; }

        private static List<OrderLongPolling> orderLongPollings = new List<OrderLongPolling>();
        private readonly TaskCompletionSource<bool> tcs;

        public OrderLongPolling(string id, string description, string serialNumber, string status)
        {
            Id = id;
            SerialNumber = serialNumber;
            Description = description;

            tcs = new TaskCompletionSource<bool>();

            lock (orderLongPollings!)
            {
                orderLongPollings.Add(this);
            }
        }

        public void Notify()
        {
            lock (orderLongPollings!)
            {
                tcs.SetResult(true);
                orderLongPollings.Remove(this);
            }
        }

        public static IEnumerable<OrderLongPolling> GetOrdersLongPollings() => orderLongPollings;
    }
}
