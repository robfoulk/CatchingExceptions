using System;
using System.Collections.Generic;

//Source found at ...
//https://github.com/robfoulk/CatchingExceptions/

namespace CatchingExceptions {
    class Program {
        static void Main(string[] args) {
            Console.WriteLine();
            Console.WriteLine("Reliable Customer Order History Application");
            Console.WriteLine();

            DataComponent data = new DataComponent();
            IEnumerable<Order> orderHistory;

            Console.Write("Enter CustomerId (####): ");
            string customerEntry = Console.ReadLine();
            int customerId = int.Parse(customerEntry);
 
            orderHistory = data.GetHistory(customerId);

            Console.WriteLine();
            Console.WriteLine($"Order history for customer Id {customerId}");
            foreach (var item in orderHistory) {
                Console.WriteLine(item);
            }

        }
    }
}


#region DemoInfrastructure

namespace CatchingExceptions {
    using System.Data.SqlClient;
    using System.Runtime.Serialization;
    using System.Linq;
    using System.Collections.Generic;

    class DataComponent {
        public bool UseCache { get; set; }
        /// <summary>
        /// Retrieves the customer's order history from customer DB or local cache
        /// </summary>
        ///<param name="customerId">the id of the customer to lookup</param>
        /// <returns>a collection of Orders</returns>
        /// <exception cref="System.Data.SqlClient.SqlException">SqlException</exception>
        /// <exception cref="CustomerNotFoundException">CustomerNotFoundException</exception>
        public IEnumerable<Order> GetHistory(int customerId) {
            if (customerId < 1000 || customerId > 3999) {
                throw new CustomerNotFoundException($"CustomerID '{customerId}' was not found.");
            }
            if (UseCache) {
                var rnd = new Random(customerId);

                var orders = new List<Order>() {
                    CreateFakeOrder(rnd),
                    CreateFakeOrder(rnd),
                    CreateFakeOrder(rnd),
                    CreateFakeOrder(rnd),
                    CreateFakeOrder(rnd),
                };
                return orders.OrderBy(p => p.OrderDate);
            } else {
                throw new SqlException("Cannot open database \"customer_db678\" requested by the login.");
            }
        }

        private static Order CreateFakeOrder(Random rnd) {
            var order = new Order {
                Total = (decimal)(rnd.Next(1000, 10000) / 10),
                OrderDate = DateTimeOffset.Now.AddDays(-rnd.Next(365))
            };

            return order;
        }
    }

    class Order {
        public DateTimeOffset OrderDate { get; set; }
        public Decimal Total { get; set; }

        public override string ToString() {
            var date = OrderDate.Date.ToString("d").PadLeft(10);
            return $"{date} - {Total:c}";
        }
    }

    class CustomerNotFoundException : Exception {
        public CustomerNotFoundException() {
        }

        public CustomerNotFoundException(string message) : base(message) {
        }

        public CustomerNotFoundException(string message, Exception innerException) : base(message, innerException) {
        }

        protected CustomerNotFoundException(SerializationInfo info, StreamingContext context) : base(info, context) {
        }
    }
}
//This is a fake for SQLException
//This removes the need for a SQL Server to run this example
namespace System.Data.SqlClient {
    using System.Runtime.Serialization;
    class SqlException : Exception {
        public SqlException() {
        }

        public SqlException(string message) : base(message) {
        }

        public SqlException(string message, Exception innerException) : base(message, innerException) {
        }

        protected SqlException(SerializationInfo info, StreamingContext context) : base(info, context) {
        }
    }
}
#endregion