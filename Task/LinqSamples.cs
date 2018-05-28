// Copyright © Microsoft Corporation.  All Rights Reserved.
// This code released under the terms of the 
// Microsoft Public License (MS-PL, http://opensource.org/licenses/ms-pl.html.)
//
//Copyright (C) Microsoft Corporation.  All rights reserved.

using System.Linq;
using SampleSupport;
using Task.Data;

// Version Mad01

namespace SampleQueries
{
    [Title("LINQ Module")]
    [Prefix("Linq")]
    public class LinqSamples : SampleHarness
    {

        private readonly DataSource dataSource = new DataSource();


        [Title("LINQ - Task 1-1")]
        [Description("1. Выдайте список всех клиентов, чей суммарный оборот (сумма всех заказов) превосходит величину 100000.")]
        public void Linq1_1()
        {
            var customers = dataSource.Customers
                .Select(c => new { c.CustomerID, c.CompanyName, OrdersSum = c.Orders.Sum(o => o.Total) })
                .Where(c => c.OrdersSum > 100000);

            ObjectDumper.Write(customers);
        }

        [Title("LINQ - Task 1-2")]
        [Description("1. Выдайте список всех клиентов, чей суммарный оборот (сумма всех заказов) превосходит величину 40000.")]
        public void Linq1_2()
        {
            var customers = dataSource.Customers
                .Select(c => new { c.CustomerID, c.CompanyName, OrdersSum = c.Orders.Sum(o => o.Total) })
                .Where(c => c.OrdersSum > 40000);


            ObjectDumper.Write(customers);

        }

        [Title("LINQ - Task 2-1")]
        [Description("2. Для каждого клиента составьте список поставщиков, находящихся в той же стране и том же городе. Сделайте задание без использования группировки")]
        public void Linq2_1()
        {
            var customerWithSuppliersCollection = dataSource.Customers
                .Select(c => new
                {
                    c.CompanyName,
                    c.Country,
                    c.City,
                    suppliers = dataSource.Suppliers.Where(s => s.City == c.City && s.Country == c.Country)
                });

            ObjectDumper.Write(customerWithSuppliersCollection, 2);
        }

        [Title("LINQ - Task 2-2")]
        [Description("2. Для каждого клиента составьте список поставщиков, находящихся в той же стране и том же городе. Сделайте задание с использованием группировки")]
        public void Linq2_2()
        {
            var customerWithSuppliersCollection = dataSource.Customers
                .Select(c => new
                {
                    c.CompanyName,
                    c.City,
                    c.Country,
                    suppliers = dataSource.Suppliers.GroupBy(s => new { s.City, s.Country })
                        .Where(s => s.Key.Equals(new { c.City, c.Country }))
                });


            ObjectDumper.Write(customerWithSuppliersCollection, 2);
        }

        [Title("LINQ - Task 3")]
        [Description("3. Найдите всех клиентов, у которых были заказы, превосходящие по сумме величину X")]
        public void Linq3()
        {
            var customers = dataSource.Customers
                .Where(c => c.Orders.Any(o => o.Total > 15000));
            ObjectDumper.Write(customers, 1);
        }

        [Title("LINQ - Task 4")]
        [Description("4. Выдайте список клиентов с указанием, начиная с какого месяца какого года они стали клиентами (принять за таковые месяц и год самого первого заказа)")]
        public void Linq4()
        {
            var customers = dataSource.Customers
                .Where(c => c.Orders.Length > 0)
                .Select(c => new
                {
                    c.CustomerID,
                    c.CompanyName,
                    StartDate = c.Orders.OrderBy(o => o.OrderDate).First().OrderDate,
                    c.Orders
                });

            ObjectDumper.Write(customers, 1);

        }

        [Title("LINQ - Task 5")]
        [Description("5. Сделайте предыдущее задание, но выдайте список отсортированным по году, месяцу, оборотам клиента (от максимального к минимальному) и имени клиента")]
        public void Linq5()
        {
            var customers = dataSource.Customers
                .Where(c => c.Orders.Length > 0)
                .Select(c => new
                {
                    c.CustomerID,
                    c.CompanyName,
                    StartDate = c.Orders.OrderBy(o => o.OrderDate).FirstOrDefault()?.OrderDate,
                    TotalSum = c.Orders.Sum(o => o.Total),
                    c.Orders
                })
                .OrderBy(c => c.StartDate.Value.Year)
                .ThenBy(c => c.StartDate.Value.Month)
                .ThenByDescending(c => c.Orders.Sum(o => o.Total))
                .ThenBy(c => c.CompanyName);


            ObjectDumper.Write(customers, 1);
        }


        [Title("LINQ - Task 6")]
        [Description("6. Укажите всех клиентов, у которых указан нецифровой почтовый код или не заполнен регион или в телефоне не указан код оператора (для простоты считаем, что это равнозначно «нет круглых скобочек в начале»).")]
        public void Linq6()
        {
            var customers = dataSource.Customers
                .Where(c => string.IsNullOrEmpty(c.PostalCode) || c.PostalCode.Any(char.IsLetter) || string.IsNullOrEmpty(c.Region) || !(c.Phone.StartsWith("(") && c.Phone.Contains(")")));
            ObjectDumper.Write(customers);
        }

        [Title("LINQ - Task 7")]
        [Description("7. Сгруппируйте все продукты по категориям, внутри – по наличию на складе, внутри последней группы отсортируйте по стоимости ")]
        public void Linq7()
        {
            var productGroups = dataSource.Products
                .GroupBy(prod => prod.Category)
                .Select(prodgroup => new
                {
                    Category = prodgroup.Key,
                    StockGroup = prodgroup.GroupBy(product => product.UnitsInStock).Select(stockGroup => new
                    {
                        AmountInStock = stockGroup.Key,
                        Products = stockGroup.OrderBy(product => product.UnitPrice)
                            .Select(product => new { product.ProductName, product.UnitPrice })
                    })
                });
            ObjectDumper.Write(productGroups, 2);
        }

        [Title("LINQ - Task 8")]
        [Description("8. Сгруппируйте товары по группам «дешевые», «средняя цена», «дорогие». Границы каждой группы задайте сами")]
        public void Linq8()
        {
            var productGroupd = dataSource.Products.GroupBy(product =>
            {
                if (product.UnitPrice < 20)
                {
                    return "Cheap";
                }

                if (product.UnitPrice < 50)
                {
                    return "Medium price";
                }

                return "Expensive";
            }).Select(priceGroup => new
            {
                Price = priceGroup.Key,
                Products = priceGroup
            });
            ObjectDumper.Write(productGroupd, 1);
        }

        [Title("LINQ - Task 9")]
        [Description("9. Рассчитайте среднюю прибыльность каждого города (среднюю сумму заказа по всем клиентам из данного города)" +
                     " и среднюю интенсивность (среднее количество заказов, приходящееся на клиента из каждого города)")]
        public void Linq9()
        {
            var cityInfo = dataSource.Customers
                .GroupBy(customer => customer.City).Select(cityGroup => new
                {
                    City = cityGroup.Key,
                    AverageEarnings = cityGroup.Average(customer => customer.Orders.Length == 0 ? 0 : customer.Orders.Average(o => o.Total)),
                    AverageIntencity = cityGroup.Average(customer => customer.Orders.Length)
                });
            ObjectDumper.Write(cityInfo, 1);
        }

        [Title("LINQ - Task 10")]
        [Description("10. Сделайте среднегодовую статистику активности (количество заказов) клиентов по месяцам (без учета года), " +
                     "статистику по годам, по годам и месяцам (т.е. когда один месяц в разные годы имеет своё значение).")]
        public void Linq10()
        {
            var customersInfo = dataSource.Customers.Select(customer => new
            {
                Company = customer.CompanyName,
                MonthStats = customer.Orders
                    .GroupBy(order => order.OrderDate.Month)
                    .Select(monthStats => new
                    {
                        Month = monthStats.Key,
                        AmountOfOrders = monthStats.Count()
                    }).OrderBy(s => s.Month),
                YearStats = customer.Orders
                    .GroupBy(order => order.OrderDate.Year)
                    .Select(yearStats => new
                    {
                        Year = yearStats.Key,
                        AmountOfOrders = yearStats.Count()
                    }).OrderBy(s => s.Year),
                OveralStats = customer.Orders
                    .GroupBy(order => (order.OrderDate.Month < 10 ? "0" : "") + $"{order.OrderDate.Month}/{order.OrderDate.Year}")
                    .Select(overalStats => new
                    {
                        Date = overalStats.Key,
                        AmountOfOrders = overalStats.Count()
                    }).OrderBy(s => s.Date),
            });
            ObjectDumper.Write(customersInfo, 2);
        }
    }
}
