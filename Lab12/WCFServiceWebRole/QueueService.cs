using Azure.Storage.Queues;
using Azure.Storage.Queues.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace WCFServiceWebRole
{
	public class QueueService
	{
        private static string _connectionString = "UseDevelopmentStorage=true";

        private static QueueClient GetQueueClient(string queueName)
        {
            var client = new QueueClient(_connectionString, queueName);
            client.CreateIfNotExists();
            return client;
        }

        public static void AddMessage(string queueName, string message)
        {
            var client = GetQueueClient(queueName);
            client.SendMessage(message);
        }

        public static QueueMessage GetMessage(string queueName)
        {
            var client = GetQueueClient(queueName);
            var response = client.ReceiveMessage();
            if (response.Value is null)
            {
                return null;
            }
            return response.Value;
        }

        public static void DeleteMessage(string queueName, string messageId, string popReceipt = "empty")
        {
            var client = GetQueueClient(queueName);
            client.DeleteMessage(messageId, popReceipt);
        }
    }
}